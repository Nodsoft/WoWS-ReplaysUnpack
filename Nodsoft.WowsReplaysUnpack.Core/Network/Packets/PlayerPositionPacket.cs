﻿using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class PlayerPositionPacket : INetworkPacket
{
	public int EntityId1 { get; } = 0;
	public int EntityId2 { get; } = 0;
	public PositionContainer Position { get; }

	public PlayerPositionPacket(BinaryReader binaryReader)
	{
		EntityId1 = binaryReader.ReadInt32();
		EntityId2 = binaryReader.ReadInt32();

		var position = binaryReader.ReadVector3();
		var yaw = binaryReader.ReadSingle();
		var pitch = binaryReader.ReadSingle();
		var roll = binaryReader.ReadSingle();

		Position = new(position, yaw, pitch, roll);
	}
}