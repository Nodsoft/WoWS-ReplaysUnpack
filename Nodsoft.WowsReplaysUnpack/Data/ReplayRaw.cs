using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Nodsoft.WowsReplaysUnpack.Data;


/// <summary>
/// Low-level DTO for Replay file info ingest
/// </summary>
public sealed record ReplayRaw
{
	public string? ArenaInfoJson { get; init; }

	public List<ReplayMessage> ChatMessages { get; private set; } = new();
	public List<ReplayPlayer> ReplayPlayers { get; init; } = new();


	internal byte[] BReplaySignature { get; init; } = Array.Empty<byte>();
	internal byte[] BReplayBlockCount { get; init; } = Array.Empty<byte>();
	internal byte[] BReplayBlockSize { get; init; } = Array.Empty<byte>();


	public void RefreshRelatedEntities()
	{
		ChatMessages = ChatMessages.Select(m => new ReplayMessage(m.EntityId, m.MessageGroup, m.MessageContent, this)).ToList();
	}
}
