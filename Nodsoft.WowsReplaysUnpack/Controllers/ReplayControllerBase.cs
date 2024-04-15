using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Abstractions;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Security;
using Nodsoft.WowsReplaysUnpack.Generators;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

// ReSharper disable VirtualMemberNeverOverridden.Global
/// <summary>
/// Defines a base class for a replay controller.
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
[ReplayController]
public abstract partial class ReplayControllerBase<T> : IReplayController<T> where T : UnpackedReplay, new()
{
	/// <summary>
	/// Definition store used by the controller.
	/// </summary>
	protected IDefinitionStore DefinitionStore { get; }

	/// <summary>
	/// Logger used to log <see cref="Entity"/> processing related events.
	/// </summary>
	protected ILogger<Entity> EntityLogger { get; }

	/// <summary>
	/// Unpacked replay being processed.
	/// </summary>
	public T Replay { get; protected set; } = null!;

	protected ReplayControllerBase(IDefinitionStore definitionStore, ILogger<Entity> entityLogger)
		=> (DefinitionStore, EntityLogger) = (definitionStore, entityLogger);

	/// <inheritdoc />
	public T CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new() { ArenaInfo = arenaInfo };
		return Replay;
	}

	#region Packet Handling

	/// <inheritdoc />
	public virtual void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options)
	{
		Action? action = networkPacket switch
		{
			MapPacket p => () => OnMap(p),
			BasePlayerCreatePacket p => () => OnBasePlayerCreate(p),
			CellPlayerCreatePacket p => () => OnCellPlayerCreate(p),
			EntityEnterPacket p when Replay.Entities.ContainsKey(p.EntityId) => () =>
				Replay.Entities[p.EntityId].IsInAoI = true,
			EntityLeavePacket p when Replay.Entities.ContainsKey(p.EntityId) => () =>
				Replay.Entities[p.EntityId].IsInAoI = false,
			EntityCreatePacket p => () => OnEntityCreate(p),
			PositionPacket p => () => OnPosition(p),
			PlayerPositionPacket p => () => OnPlayerPosition(p),
			EntityMethodPacket p => () => OnEntityMethod(p),
			EntityPropertyPacket p => () => OnEntityProperty(p),
			NestedPropertyPacket p => () => OnNestedProperty(p),
			_ => null
		};

		action?.Invoke();
	}

	/// <summary>
	/// Triggered when a <see cref="MapPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnMap(MapPacket packet) => Replay.MapName = packet.Name;

	/// <summary>
	/// Triggered when a <see cref="BasePlayerCreatePacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnBasePlayerCreate(BasePlayerCreatePacket packet)
	{
		Replay.Entities.GetOrAddValue(packet.EntityId, out Entity? entity,
			() => CreateEntity(packet.EntityId, "Avatar"));

		if (entity is null)
		{
			return;
		}

		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		entity.SetBaseProperties(binaryReader);
		Replay.PlayerEntityId = packet.EntityId;
	}

	/// <summary>
	/// Triggered when a <see cref="CellPlayerCreatePacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnCellPlayerCreate(CellPlayerCreatePacket packet)
	{
		Replay.Entities.GetOrAddValue(packet.EntityId, out Entity? entity,
			() => CreateEntity(packet.EntityId, "Avatar"));

		if (entity is null)
		{
			return;
		}

		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		entity.SetInternalClientProperties(binaryReader);
	}

	/// <summary>
	/// Triggered when a <see cref="EntityEnterPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnEntityCreate(EntityCreatePacket packet)
	{
		Entity entity = CreateEntity(packet.EntityId, packet.Type);

		Replay.Entities[packet.EntityId] = entity;
		using BinaryReader binaryReader = packet.Data.GetBinaryReader();
		byte valuesCount = binaryReader.ReadByte();

		for (int i = 0; i < valuesCount; i++)
		{
			byte propertyIndex = binaryReader.ReadByte();
			entity.SetClientProperty(propertyIndex, binaryReader, this);
		}
	}

	/// <summary>
	/// Triggered when a <see cref="PositionPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnPosition(PositionPacket packet)
	{
		if (!Replay.Entities.TryGetValue(packet.EntityId, out Entity? entity))
		{
			return;
		}

		entity.SetPosition(packet.Position);
	}

	/// <summary>
	/// Triggered when a <see cref="PlayerPositionPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnPlayerPosition(PlayerPositionPacket packet)
	{
		/*
		 Entity at ID 1 is the primary one's position being updated
		 Avatar-only packets have no position until death, and
		 are linked to a vehicle. After death they have no ID for a
		 Vehicle anymore, and a position instead.
		 Before death only "Vehicle in ID 1" packets have a position.
		*/
		if (packet.EntityId2 is not 0 && Replay.Entities.TryGetValue(packet.EntityId1, out Entity? slaveEntity) &&
		    Replay.Entities.TryGetValue(packet.EntityId2, out Entity? masterEntity))
		{
			/*
				This serves to link the positions of the two entities
				where the position of entity 1 is set by wherever entity 2
				is, rather than by the position field.
				e.g. Assigning the Avatar the position of the Vehicle
			 */

			slaveEntity.SetPosition(masterEntity.GetPosition());
		}
		else if (packet is { EntityId1: not 0, EntityId2: 0 } &&
		         Replay.Entities.TryGetValue(packet.EntityId1, out Entity? entity))
		{
			// This is a regular update for entity 1, without entity 2
			entity.SetPosition(packet.Position);
		}
	}

	/// <summary>
	/// Triggered when a <see cref="EntityMethodPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnEntityMethod(EntityMethodPacket packet)
	{
		if (!Replay.Entities.TryGetValue(packet.EntityId, out Entity? entity))
		{
			return;
		}

		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, packet.PacketTime, methodDataReader, this);
	}

	/// <summary>
	/// Triggered when a <see cref="EntityPropertyPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnEntityProperty(EntityPropertyPacket packet)
	{
		if (!Replay.Entities.TryGetValue(packet.EntityId, out Entity? entity))
		{
			return;
		}

		using BinaryReader propertyData = packet.Data.GetBinaryReader();
		entity.SetClientProperty(packet.MessageId, propertyData, this);
	}

	/// <summary>
	/// Triggered when a <see cref="EntityPropertyPacket" /> is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="packet">The packet.</param>
	protected virtual void OnNestedProperty(NestedPropertyPacket packet)
	{
		if (!Replay.Entities.TryGetValue(packet.EntityId, out Entity? entity))
		{
			return;
		}

		packet.Apply(entity);
	}

	/// <summary>
	/// Creates an entity object for the given ID and name.
	/// </summary>
	/// <param name="id">The ID of the entity.</param>
	/// <param name="name">The name of the entity.</param>
	/// <returns>The entity object.</returns>
	protected virtual Entity CreateEntity(uint id, string name)
		=> new(id, name, DefinitionStore.GetEntityDefinition(Replay.ClientVersion, name),
			// _methodSubscriptions,
			// _propertyChangedSubscriptions, 
			EntityLogger);

	/// <summary>
	/// Creates an entity object for the given ID and index.
	/// </summary>
	/// <param name="id">The ID of the entity.</param>
	/// <param name="index">The index of the entity.</param>
	/// <returns>The entity object.</returns>
	protected virtual Entity CreateEntity(uint id, int index)
	{
		EntityDefinition definition = DefinitionStore.GetEntityDefinition(Replay.ClientVersion, index - 1);
		return new(id, definition.Name, definition,
			// _methodSubscriptions, 
			// _propertyChangedSubscriptions, 
			EntityLogger);
	}

	#endregion

	#region Subscriptions

	/// <summary>
	/// Triggered when a CVE is handled by <see cref="HandleNetworkPacket"/>.
	/// </summary>
	/// <param name="arguments">The arguments.</param>
	[MethodSubscription("Avatar", "onArenaStateReceived", ParamsAsDictionary = true)]
	public void OnArenaStateReceivedCVECheck(Dictionary<string, object?> arguments)
	{
		var x = arguments.Values.ElementAt(0);
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["preBattlesInfo"]!,
			"Avatar_onArenaStateReceived_preBattlesInfo");
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["playersStates"]!,
			"Avatar_onArenaStateReceived_playersStates");
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["observersState"]!,
			"Avatar_onArenaStateReceived_observersState");
		CveChecks.ScanForCVE_2022_31265((byte[])arguments["buildingsInfo"]!,
			"Avatar_onArenaStateReceived_buildingsInfo");
	}

	#endregion
}