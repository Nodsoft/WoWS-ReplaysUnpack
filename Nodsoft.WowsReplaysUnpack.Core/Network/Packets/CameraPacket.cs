﻿using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

public class CameraPacket : INetworkPacket
{
	public float Fov { get; }

	public CameraPacket(BinaryReader binaryReader)
	{
		// 7 x float (4 bytes) unknown values
		_ = binaryReader.ReadBytes(7 * 4);

		Fov = binaryReader.ReadSingle(); // fov;
		_ = binaryReader.ReadVector3(); // position (Vector3, 3xfloat)
		_ = binaryReader.ReadVector3(); // direction (Vector3, 3xfloat)
	}
}
