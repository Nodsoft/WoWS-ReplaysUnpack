using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

/// <summary>
/// Specifies a replay controller, responsible for handling the replay data extraction.
/// </summary>
public interface IReplayController
{
	/// <summary>
	/// Creates a <see cref="UnpackedReplay" /> instance from the specified arenaInfo.
	/// </summary>
	/// <param name="arenaInfo">Arena info to create the replay from.</param>
	/// <returns>A <see cref="UnpackedReplay" /> instance.</returns>
	UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo);
	
	/// <summary>
	/// Handles a network packet, based on its type and properties.
	/// </summary>
	/// <param name="networkPacket">Network packet to handle.</param>
	/// <param name="options">Options to use when handling the packet.</param>
	void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options);
}

public interface IReplayController<out T> 
	where T : UnpackedReplay
{
	T CreateUnpackedReplay(ArenaInfo arenaInfo);
	void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options);
}
