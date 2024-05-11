using JetBrains.Annotations;

namespace Nodsoft.WowsReplaysUnpack.Core.Network;

/// <summary>
/// Type definitions and utilities for working with network packets.
/// </summary>
[PublicAPI]
public static class NetworkPacketTypes
{
	public static IReadOnlyDictionary<uint, string> BasePackets { get; } = new Dictionary<uint, string>
	{
		{ 0x0, "BasePlayerCreate"},
		{ 0x1, "CellPlayerCreate"},
		{ 0x2, "EntityControl"},
		{ 0x3, "EntityEnter"},
		{ 0x4, "EntityLeave"},
		{ 0x5, "EntityCreate"},
		{ 0x7, "EntityProperty"},
		{ 0x8, "EntityMethod"},
		{ 0x27, "Map"},
		{ 0x22, "NestedProperty"},
		{ 0x0a, "Position"},
		{ 0x2b, "PlayerPosition"}
	};

	// Amended Packet IDs for 0.12.6
	public static IReadOnlyDictionary<uint, string> AmendedPackets126 { get; } = new Dictionary<uint, string>
	{
		{ 0x0, "BasePlayerCreate"},
		{ 0x1, "CellPlayerCreate"},
		{ 0x2, "EntityControl"},
		{ 0x3, "EntityEnter"},
		{ 0x4, "EntityLeave"},
		{ 0x5, "EntityCreate"},
		{ 0x7, "EntityProperty"},
		{ 0x8, "EntityMethod"},
		{ 0x28, "Map" },
		{ 0x23, "NestedProperty" },
		{ 0x0a, "Position"},
		{ 0x2b, "PlayerPosition"}
			
	};

	/// <summary>
	/// Gets the name of a network packet type by its ID.
	/// </summary>
	/// <param name="id">The ID of the packet type.</param>
	/// <param name="version">The game version</param>
	/// <returns>The name of the packet type.</returns>
	public static string GetTypeName(uint id, Version version) =>(version switch
	{
		{ Major: > 12 } or { Major: 12, Minor: >= 6 } => AmendedPackets126,
		_ => BasePackets
	}).TryGetValue(id, out string? name) ? name : $"Unknown Packet ({id})";
}
