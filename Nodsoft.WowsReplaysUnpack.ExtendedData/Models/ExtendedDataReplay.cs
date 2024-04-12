using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

public record ExtendedDataReplay : UnpackedReplay
{
	public ExtendedDataReplay() { }

	public List<ReplayPlayer> ReplayPlayers { get; } = new();
	public List<ChatMessage> ChatMessages { get; } = new();
}