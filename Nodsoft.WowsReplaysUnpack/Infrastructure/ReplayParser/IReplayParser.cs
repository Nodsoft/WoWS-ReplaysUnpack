using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Data.Raw;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;

/// <summary>
/// Interface that defines a replay parser.
/// A replay parser can be used to process a replay file in order to retrieve its data.
/// <br/>
/// <b>It is recommended to not use this interface directly but to use the <see cref="ReplayParserBase"/> instead when implementing a custom replay reader.</b> 
/// </summary>
public interface IReplayParser
{
	/// <summary>
	/// Parses a replay file from a memory stream.
	/// </summary>
	/// <param name="decompressedData">The <see cref="MemoryStream"/> containing the decrypted and decompressed replay file content.</param>
	/// <param name="replayMetadata">An object containing metadata for the replay.</param>
	/// <returns>The populated <see cref="ReplayRaw"/> object.</returns>
	public ReplayRaw ParseReplay(MemoryStream decompressedData, ReplayMetadata replayMetadata);

	public ReplayPlayer ParseReplayPlayer(ArrayList playerInfo);

	public IEnumerable<ReplayPlayer> ParsePlayersPacket(EntityMethod em);
	public ReplayMessage ParseChatMessagePacket(EntityMethod em);
}
