using Mapster;
using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure;
using Razorvine.Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;


namespace Nodsoft.WowsReplaysUnpack;


public class ReplayUnpacker
{
	private static readonly PropertyInfo[] _replayPlayerProperties = typeof(ReplayPlayer).GetProperties();


	public ReplayRaw UnpackReplay(Stream stream)
	{


		byte[] bReplaySignature = new byte[4];
		byte[] bReplayBlockCount = new byte[4];
		byte[] bReplayBlockSize = new byte[4];

		stream.Read(bReplaySignature, 0, 4);
		stream.Read(bReplayBlockCount, 0, 4);
		stream.Read(bReplayBlockSize, 0, 4);

		int jsonDataSize = BitConverter.ToInt32(bReplayBlockSize, 0);

		byte[] bReplayJsonData = new byte[jsonDataSize];
		stream.Read(bReplayJsonData, 0, jsonDataSize);

		ReplayRaw replay = new() { ArenaInfoJson = Encoding.UTF8.GetString(bReplayJsonData) };

		using MemoryStream memStream = new();
		stream.CopyTo(memStream);

		const string stringBlowfishKey = "\x29\xB7\xC9\x09\x38\x3F\x84\x88\xFA\x98\xEC\x4E\x13\x19\x79\xFB";
		byte[] byteBlowfishKey = stringBlowfishKey.Select(Convert.ToByte).ToArray();
		Blowfish blowfish = new(byteBlowfishKey);
		long prev = 0;

		using MemoryStream compressedData = new();

		foreach ((int, byte[]) chunk in ChunkData(memStream.ToArray()[8..]))
		{
			try
			{
				long decryptedBlock = BitConverter.ToInt64(blowfish.Decrypt_ECB(chunk.Item2));

				if (prev is not 0)
				{
					decryptedBlock ^= prev;
				}

				prev = decryptedBlock;
				compressedData.Write(BitConverter.GetBytes(decryptedBlock));
			}
			catch (ArgumentOutOfRangeException)	{ }
		}

		compressedData.Seek(2, SeekOrigin.Begin); //DeflateStream doesn't strip the header so we strip it manually.
		MemoryStream decompressedData = new();

		using (DeflateStream df = new(compressedData, CompressionMode.Decompress))
		{
			df.CopyTo(decompressedData);
		}

		decompressedData.Seek(0, SeekOrigin.Begin);

		while (decompressedData.Position != decompressedData.Length)
		{
			NetPacket np = new(decompressedData);

			if (np.Type is 0x08)
			{
				EntityMethod em = new(np.RawData);

				if (em.MessageId is Constants.ReplayMessageTypes.OnArenaStatesReceived) // 10.10=124, OnArenaStatesReceived
				{
					byte[]? arenaId = new byte[8];
					em.Data.Value.Read(arenaId);

					byte[]? teamBuildTypeId = new byte[1];
					em.Data.Value.Read(teamBuildTypeId);

					byte[]? blobPreBattlesInfo = new ReplayBlob(em.Data.Value).Data; // useless
					byte[]? blobPlayerStates = new ReplayBlob(em.Data.Value).Data; // what we need
					byte[]? blobObserversStates = new ReplayBlob(em.Data.Value).Data; // useless
					byte[]? blobBuildingsInfo = new ReplayBlob(em.Data.Value).Data; // useless

					Unpickler.registerConstructor("CamouflageInfo", "CamouflageInfo", new CamouflageInfo());
					ArrayList players = new Unpickler().load(new MemoryStream(blobPlayerStates)) as ArrayList ?? new ArrayList();

					foreach (ArrayList player in players)
					{
						replay.ReplayPlayers.Add(ParseReplayPlayer(player));
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
				else if (em.MessageId is Constants.ReplayMessageTypes.OnChatMessage) // 10.10=122, OnChatMessage
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

					replay.ChatMessages.Add(new(entityId, messageGroup, messageContent, replay));
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
			}
		}

		return replay;
	}

	internal static ReplayPlayer ParseReplayPlayer(ArrayList playerInfo)
	{
		Dictionary<string, object> data = new();

		for (int i = 0; i < playerInfo.Count; i++)
		{
			if (Constants.PropertyMapping.GetValueOrDefault(i) is string key && playerInfo[i] is object[] values)
			{
				data.Add(key, values[1]);
			}

		}

		ReplayPlayer player = new() { Properties = data };


		foreach (KeyValuePair<string, object> value in player.Properties)
		{
			PropertyInfo? propertyInfo = _replayPlayerProperties.FirstOrDefault(p => p.Name.Equals(value.Key, StringComparison.OrdinalIgnoreCase));
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

	private static IEnumerable<(int, byte[])> ChunkData(byte[] data, int len = 8)
	{
		int idx = 0;

		for (int s = 0; s <= data.Length; s += len)
		{
			byte[] g;

			try
			{
				g = data[s..(s + len)];
			}
			catch (ArgumentOutOfRangeException)
			{
				g = data[s..];
			}

			idx++;
			yield return (idx, g);
		}
	}
}
