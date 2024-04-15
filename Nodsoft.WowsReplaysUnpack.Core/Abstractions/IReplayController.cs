using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Core.Abstractions;

/// <summary>
/// Specifies a replay controller, responsible for handling the replay data extraction.
/// </summary>
public interface IReplayController
{
	/// <summary>
	/// Handles a network packet, based on its type and properties.
	/// </summary>
	/// <param name="networkPacket">Network packet to handle.</param>
	/// <param name="options">Options to use when handling the packet.</param>
	void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options);

	/// <summary>
	/// Handles calls to method or property subscriptions
	/// </summary>
	/// <param name="hash">The has of the subscription</param>
	/// <param name="entity">The entity the method is called for</param>
	/// <param name="packetTime">The timestamp of the network packet</param>
	/// <param name="arguments">The arguments for the subscription</param>
	void CallSubscription(string hash, Entity entity, float packetTime,
		Dictionary<string, object?> arguments);
}

public interface IReplayController<out T> : IReplayController
	where T : UnpackedReplay
{
	/// <summary>
	/// Creates an <inheritdoc cref="UnpackedReplay" /> out of an existing <see cref="ArenaInfo" />.
	/// </summary>
	/// <param name="arenaInfo">The arena info.</param>
	/// <returns>The unpacked replay.</returns>
	T CreateUnpackedReplay(ArenaInfo arenaInfo);
}