using Mapster;
using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Data.Raw;
using Razorvine.Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;

/// <summary>
/// The abstract base class for replay parser implementations.
/// </summary>
public abstract class ReplayParserBase : IReplayParser
{
	protected static readonly PropertyInfo[] _replayPlayerProperties = typeof(ReplayPlayer).GetProperties();

	public virtual ReplayRaw ParseReplay(MemoryStream decompressedData, ReplayMetadata replayMetadata)
	{
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
	
	protected abstract IReplayMessageTypes MessageTypes { get; }
	
	protected abstract IShipConfigMapping ShipConfigMapping { get; }
	
	protected abstract IPlayerMessageMapping PlayerMessageMapping { get; }
}