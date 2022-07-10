﻿using Mapster;
using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Data.Raw;
using Razorvine.Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;

/// <summary>
/// The abstract base class for replay parser implementations.
/// </summary>
public abstract class ReplayParserBase : IReplayParser
{
	protected static readonly PropertyInfo[] _replayPlayerProperties = typeof(ReplayPlayer).GetProperties();

	public virtual ReplayRaw ParseReplay(MemoryStream memStream, ReplayMetadata replayMetadata)
	{
		byte[] byteBlowfishKey = GlobalConstants.BlowfishKey.Select(Convert.ToByte).ToArray();
		Blowfish blowfish = new(byteBlowfishKey);
		long prev = 0;

		using MemoryStream compressedData = new();
		memStream.Seek(8, SeekOrigin.Begin);

		foreach (byte[] chunk in Utilities.ChunkData(memStream))
		{
			try
			{
				long decryptedBlock = BitConverter.ToInt64(blowfish.Decrypt_ECB(chunk));

				if (prev is not 0)
				{
					decryptedBlock ^= prev;
				}

				prev = decryptedBlock;
				compressedData.Write(BitConverter.GetBytes(decryptedBlock));
			}
			catch (ArgumentOutOfRangeException) { }
		}

		compressedData.Seek(2, SeekOrigin.Begin); //DeflateStream doesn't strip the header so we strip it manually.
		MemoryStream decompressedData = new();

		using (DeflateStream df = new(compressedData, CompressionMode.Decompress))
		{
			df.CopyTo(decompressedData);
		}

		decompressedData.Seek(0, SeekOrigin.Begin);

		ReplayRaw replay = new(replayMetadata);
		while (decompressedData.Position != decompressedData.Length)
		{
			NetPacket np = new(decompressedData);

			if (np.Type is GlobalConstants.ReplayPacketTypes.OnEntityMethod)
			{
				EntityMethod em = new(np.RawData);

				if (em.MessageId == MessageTypes.OnArenaStatesReceived) // 10.10=124, OnArenaStatesReceived
				{
					replay.ReplayPlayers.AddRange(ParsePlayersPacket(em));
				}
				else if (em.MessageId == MessageTypes.OnChatMessage) // 10.10=122, OnChatMessage
				{
					replay.ChatMessages.Add(ParseChatMessagePacket(em));
				}
			}
		}

		return replay;
	}
	
	/// <inheritdoc />
	/// <remarks>
	/// This method checks and accounts for CVE-2022-31265 exploit,
	/// and will throw a <see cref="SecurityException"/> if an injection is detected.
	/// </remarks>
	public virtual ReplayPlayer ParseReplayPlayer(ArrayList playerInfo)
	{
		Dictionary<string, object> data = new();

		for (int i = 0; i < playerInfo.Count; i++)
		{
			if (PlayerMessageMapping.PropertyMapping.GetValueOrDefault(i) is string key && playerInfo[i] is object[] values)
			{
				data.Add(key, values[1]);
			}
		}

		ReplayPlayer player = new(ShipConfigMapping) { Properties = data };

		foreach (KeyValuePair<string, object> value in player.Properties)
		{
			PropertyInfo? propertyInfo = _replayPlayerProperties.FirstOrDefault(p => p.Name == value.Key);
			Type sourceType = value.Value.GetType();

			if (propertyInfo is not null)
			{
				if (propertyInfo.PropertyType == sourceType)
				{
					propertyInfo.SetValue(player, value.Value);
				}
				else
				{
					propertyInfo.SetValue(player, value.Value.Adapt(value.Value.GetType(), propertyInfo.PropertyType));
				}
			}
		}

		return player;
	}

	public virtual IEnumerable<ReplayPlayer> ParsePlayersPacket(EntityMethod em)
	{
		byte[] arenaId = new byte[8];
		em.Data.Value.Read(arenaId);

		byte[] teamBuildTypeId = new byte[1];
		em.Data.Value.Read(teamBuildTypeId);

		byte[] blobPreBattlesInfo = new ReplayBlob(em.Data.Value).Data; // useless
		byte[] blobPlayerStates = new ReplayBlob(em.Data.Value).Data; // what we need
		byte[] blobObserversStates = new ReplayBlob(em.Data.Value).Data; // useless
		byte[] blobBuildingsInfo = new ReplayBlob(em.Data.Value).Data; // useless

		// Scan all blobs for CVE-2022-31265 exploit
		ScanBlobForRceInjection(blobPreBattlesInfo, nameof(blobPreBattlesInfo));
		ScanBlobForRceInjection(blobPlayerStates, nameof(blobPlayerStates));
		ScanBlobForRceInjection(blobObserversStates, nameof(blobObserversStates));
		ScanBlobForRceInjection(blobBuildingsInfo, nameof(blobBuildingsInfo));
		
		// All goood, business as usual. Parse the blobs.
		Unpickler.registerConstructor("CamouflageInfo", "CamouflageInfo", new CamouflageInfo());
		ArrayList players = new Unpickler().load(new MemoryStream(blobPlayerStates)) as ArrayList ?? new ArrayList();

		foreach (ArrayList player in players)
		{
			yield return ParseReplayPlayer(player);
		}

		/*
			...
			accountDBID          : 2016494874
			avatarId             : 919187
			camouflageInfo       : 4205768624, 0
			clanColor            : 13427940
			clanID               : 2000008825
			clanTag              : TF44
			crewParams           : System.Collections.ArrayList
			dogTag               : System.Collections.ArrayList
			fragsCount           : 0
			friendlyFireEnabled  : False
			id                   : 537149649
			invitationsEnabled   : True
			isAbuser             : False
			isAlive              : True
			isBot                : False
			isClientLoaded       : False
			isConnected          : True
			isHidden             : False
			isLeaver             : False
			isPreBattleOwner     : False
			killedBuildingsCount : 0
			maxHealth            : 27500
			name                 : notyourfather
			playerMode           : Razorvine.Pickle.Objects.ClassDict
			preBattleIdOnStart   : 537256655
			preBattleSign        : 0
			prebattleId          : 537256655
			realm                : ASIA
			shipComponents       : System.Collections.Hashtable
			shipId               : 919188
			shipParamsId         : 4288591856
			skinId               : 4288591856
			teamId               : 0
			ttkStatus            : False
			...
		 */
	}

	public virtual ReplayMessage ParseChatMessagePacket(EntityMethod em)
	{
		byte[] bEntityId = new byte[4];
		em.Data.Value.Read(bEntityId);
		uint entityId = BitConverter.ToUInt32(bEntityId);

		byte[] bMessageGroupSize = new byte[1];
		em.Data.Value.Read(bMessageGroupSize);
		byte[] bMessageGroup = new byte[bMessageGroupSize[0]];
		em.Data.Value.Read(bMessageGroup);
		string messageGroup = Encoding.UTF8.GetString(bMessageGroup);

		byte[] bMessageContentSize = new byte[1];
		em.Data.Value.Read(bMessageContentSize);
		byte[] bMessageContent = new byte[bMessageContentSize[0]];
		em.Data.Value.Read(bMessageContent);
		string messageContent = Encoding.UTF8.GetString(bMessageContent);

		return new(entityId, messageGroup, messageContent);
		/*
			615476 : battle_team : cv run
			615474 : battle_common : nb
			615488 : battle_team : lol
			615452 : battle_team : lol
			615480 : battle_team : ???????bug?
			615480 : battle_team : ?????????
			615480 : battle_team : ??
			615474 : battle_common : ??????
			615452 : battle_team : ???????
			615480 : battle_team : ???
			615480 : battle_team : ??
			615480 : battle_team : ????`
			615480 : battle_team : ?TM???
			615452 : battle_team : ??
			615480 : battle_team : ????????
			615480 : battle_team : ????????
			615480 : battle_team : ??
			615452 : battle_team : ? ???????
		 */
	}

	
	/// <summary>
	/// Provides detection for CVE-2022-31265 RCE injections. <br />
	/// See : https://www.cve.org/CVERecord?id=CVE-2022-31265
	/// </summary>
	/// <param name="blob">Python pickle byte array to scan.</param>
	/// <param name="blobName">Name of byte array (used for exception source reporting)</param>
	/// <exception cref="SecurityException">Raised when a CVE-2022-31265 RCE Injection attack is detected within a blob.</exception>
	protected virtual void ScanBlobForRceInjection(byte[] blob, string? blobName = null)
	{
		/*
		 * Scan the UTF-8 blobs for any RCE injections.
		 *
		 * These can be picked up on Windows (98% of all users),
		 * using the special moniker "cnt system", which defines an entry point for the RCE.
		 * This moniker consists of detecting the "c" pickle opcode, followed by the "nt system" value.
		 */

		if (RceInjectionRegex.IsMatch(Encoding.UTF8.GetString(blob)))
		{
			throw new SecurityException("CVE-2022-31265 RCE injection detected within replay blob.")
			{
				Source = blobName,
				Data = { { "blob", blob }, { "exploit", "CVE-2022-31265" } }
			};
		}
	}
	
	/// <summary>
	/// Looks for any matching CVE-2022-31265 RCE injections monikers in a Python pickle.
	/// <br />
	/// We're looking for the following monikers:
	/// "cnt system", "commands"
	/// </summary>
	protected static readonly Regex RceInjectionRegex = new(
		@"cnt\ssystem|commands", 
		RegexOptions.Compiled | RegexOptions.ExplicitCapture);
	
	protected abstract IReplayMessageTypes MessageTypes { get; }
	
	protected abstract IShipConfigMapping ShipConfigMapping { get; }
	
	protected abstract IPlayerMessageMapping PlayerMessageMapping { get; }
}