using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;

public interface IReplayMessageTypes
{
	public abstract byte OnChatMessage { get; }
	public abstract byte OnArenaStatesReceived { get; }
}

public interface IPlayerMessageMapping
{
	public Dictionary<int, string> PropertyMapping { get; }
}

public interface IShipConfigMapping
{
	public byte ShipId { get; }
	public byte TotalValueCount { get; }
	public byte ShipModules { get; }
	public byte ShipUpgrades { get; }
	public byte ExteriorSlots { get; }
	public byte AutoSupplyState { get; }
	public byte ColorScheme { get; }
	public byte ConsumableSlots { get; }
	public byte Flags { get; }
}
