using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Specifies a parser for replay data.
/// </summary>
public interface IReplayDataParser : IDisposable
{
	/// <summary>
	/// Reads a data stream and parses it into network packets.
	/// </summary>
	/// <param name="replayDataStream">The data stream to parse.</param>
	/// <param name="options">The options to use when parsing.</param>
	/// <param name="gameVersion">The game version to use</param>
	/// <returns>The parsed packets.</returns>
	IEnumerable<NetworkPacketBase> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options, Version gameVersion);
}