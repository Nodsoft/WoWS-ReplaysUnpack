using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Represents the default implementation for a data parser.
/// </summary>
[UsedImplicitly]
public class DefaultReplayDataParser : IReplayDataParser
{
	private readonly ILogger<DefaultReplayDataParser> _logger;
	private readonly MemoryStream _packetBuffer = new();
	private readonly BinaryReader _packetBufferReader;

	public DefaultReplayDataParser(ILogger<DefaultReplayDataParser> logger) =>
		(_logger, _packetBufferReader) = (logger, new(_packetBuffer));

	/// <inheritdoc />
	public virtual IEnumerable<NetworkPacketBase> ParseNetworkPackets(MemoryStream replayDataStream,
		ReplayUnpackerOptions options, Version gameVersion)
	{
		int packetIndex = 0;
		using BinaryReader binaryReader = new(replayDataStream);
		while (replayDataStream.Position != replayDataStream.Length)
		{
			uint packetSize = binaryReader.ReadUInt32();
			uint packetType = binaryReader.ReadUInt32();
			float packetTime = binaryReader.ReadSingle(); // Time in seconds from battle start

			_logger.LogDebug(
				"Packet parsed of type '{PacketType}' with size '{PacketSize}' and timestamp '{PacketTime}'",
				NetworkPacketTypes.GetTypeName(packetType, gameVersion), packetSize, packetTime);

			byte[] packetData = binaryReader.ReadBytes((int)packetSize);

			// Reset packet buffer, write current data and set position to start
			_packetBuffer.SetLength(packetSize);
			_packetBuffer.Seek(0, SeekOrigin.Begin);
			_packetBuffer.Write(packetData);
			_packetBuffer.Seek(0, SeekOrigin.Begin);

			yield return PacketTypeMap.TryGetValue(NetworkPacketTypes.GetTypeName(packetType, gameVersion),
				out var packetTypeFunc)
				? packetTypeFunc(packetIndex, packetTime, _packetBufferReader)
				: new UnknownPacket(packetIndex, _packetBufferReader);

			packetIndex++;
		}
	}

	private static readonly Dictionary<string, Func<int, float, BinaryReader, NetworkPacketBase>> PacketTypeMap = new()
	{
		{ "", static (index, _, reader) => new UnknownPacket(index, reader) },
		{ "BasePlayerCreate", static (index, _, reader) => new BasePlayerCreatePacket(index, reader) },
		{ "CellPlayerCreate", static (index, _, reader) => new CellPlayerCreatePacket(index, reader) },
		{ "EntityControl", static (index, _, reader) => new EntityControlPacket(index, reader) },
		{ "EntityEnter", static (index, _, reader) => new EntityEnterPacket(index, reader) },
		{ "EntityLeave", static (index, _, reader) => new EntityLeavePacket(index, reader) },
		{ "EntityCreate", static (index, _, reader) => new EntityCreatePacket(index, reader) },
		{ "EntityProperty", static (index, _, reader) => new EntityPropertyPacket(index, reader) },
		{ "EntityMethod", static (index, time, reader) => new EntityMethodPacket(index, time, reader) },
		{ "Map", static (index, _, reader) => new MapPacket(index, reader) },
		{ "NestedProperty", static (index, _, reader) => new NestedPropertyPacket(index, reader) },
		{ "Position", static (index, _, reader) => new PositionPacket(index, reader) },
		{ "PlayerPosition", static (index, _, reader) => new PlayerPositionPacket(index, reader) }
	};

	/// <summary>
	/// Disposes the data parser.
	/// </summary>
	public void Dispose()
	{
		_packetBufferReader.Dispose();
		GC.SuppressFinalize(this);
	}
}