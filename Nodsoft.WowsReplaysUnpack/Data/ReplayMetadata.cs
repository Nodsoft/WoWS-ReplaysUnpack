using System;

namespace Nodsoft.WowsReplaysUnpack.Data;

/// <summary>
/// A DTO containing metadata of a replay file.
/// </summary>
public sealed record ReplayMetadata
{
	internal ReplayMetadata()
	{
	}
	
	/// <summary>
	/// Gets the <see cref="Nodsoft.WowsReplaysUnpack.Data.ArenaInfo"/> of the replay file.
	/// </summary>
	public ArenaInfo ArenaInfo { get; internal init; } = default!;
	
	internal byte[] BReplaySignature { get; init; } = Array.Empty<byte>();
	internal int ReplayBlockCount { get; init; }
	internal int ReplayBlockSize { get; init; }
}