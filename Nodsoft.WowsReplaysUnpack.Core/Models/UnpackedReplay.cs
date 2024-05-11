using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

/// <summary>
/// Represents an unpacked replay file.
/// </summary>
[PublicAPI]
public class UnpackedReplay
{
	/// <summary>
	/// Game client version.
	/// </summary>
	public Version ClientVersion { get; init; } = null!;

	/// <summary>
	/// Arena info associated to the replay.
	/// Contains useful information about the battle that took place.
	/// </summary>
	public ArenaInfo ArenaInfo { get; init; } = null!;
	
	/// <summary>
	/// Additional info about the replay.
	/// </summary>
	public List<JsonElement> ExtraJsonData { get; } = new();
	
	/// <summary>
	/// Entities present in the replay.
	/// </summary>
	public Dictionary<uint, Entity> Entities { get; } = new();
	
	/// <summary>
	/// ID of the entity related to the current replay player.
	/// </summary>
	public uint? PlayerEntityId { get; set; }
	
	/// <summary>
	/// Name of the map the replay was played on.
	/// </summary>
	public string? MapName { get; set; }
}
