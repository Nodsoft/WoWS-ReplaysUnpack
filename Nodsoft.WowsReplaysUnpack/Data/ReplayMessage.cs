namespace Nodsoft.WowsReplaysUnpack.Data;

public sealed record ReplayMessage
{
	public uint EntityId { get; init; }
	public ReplayPlayer? Player { get; init; }

	public string MessageGroup { get; init; } = string.Empty;

	public string MessageContent { get; init; } = string.Empty;

	public ReplayMessage() { }

	public ReplayMessage(uint entityId, string messageGroup, string messageContent, ReplayRaw? parentReplay = null)
	{
		EntityId = entityId;
		MessageGroup = messageGroup;
		MessageContent = messageContent;

		if (parentReplay is not null)
		{
			Player = parentReplay.ReplayPlayers.Find(p => p.AvatarId == entityId);
		}
	}
}
