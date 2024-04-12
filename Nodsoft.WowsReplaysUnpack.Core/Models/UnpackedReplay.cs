using Nodsoft.WowsReplaysUnpack.Core.Entities;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

/// <summary>
/// Represents an unpacked replay file.
/// </summary>
public record UnpackedReplay
{
	/// <summary>
	/// Game client version.
	/// </summary>
	public Version ClientVersion { get; init; }
	
	/// <summary>
	/// Arena info associated to the replay.
	/// Contains useful information about the battle that took place.
	/// </summary>
	public ArenaInfo ArenaInfo { get; init; }
	
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

	
	public UnpackedReplay()
	{
		
	}
	// public UnpackedReplay(ArenaInfo arenaInfo)
	// {
	// 	ArenaInfo = arenaInfo;
	// 	ClientVersion = Version.Parse(string.Join('.', ArenaInfo.ClientVersionFromExe.Split(',')[..3]));
	// }
}
