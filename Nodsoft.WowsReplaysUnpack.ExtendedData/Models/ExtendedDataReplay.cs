using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

[PublicAPI]
public class ExtendedDataReplay : UnpackedReplay
{
	public List<ReplayPlayer> ReplayPlayers { get; } = new();
	public List<ChatMessage> ChatMessages { get; } = new();
}