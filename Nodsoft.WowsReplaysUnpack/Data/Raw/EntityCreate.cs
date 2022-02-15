using System;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Data.Raw;

public sealed record EntityCreate
{
	public EntityCreate(Stream stream)
	{
		byte[] bEntityId = new byte[4];
		byte[] bType = new byte[2];
		byte[] bVehicleId = new byte[4];
		byte[] bSpaceId = new byte[4];

		stream.Read(bEntityId);
		stream.Read(bType);
		stream.Read(bVehicleId);
		stream.Read(bSpaceId);

		EntityId = BitConverter.ToUInt32(bEntityId);
		Type = BitConverter.ToInt16(bType);
		VehicleId = BitConverter.ToInt32(bVehicleId);
		SpaceId = BitConverter.ToInt32(bSpaceId);

		Position = new(stream);
		Direction = new(stream);
		State = new(stream);
	}

	public long EntityId { get; }

	public short Type { get; }

	public int VehicleId { get; }

	public int SpaceId { get; }

	public Vector3Raw Position { get; }

	public Vector3Raw Direction { get; }

	public BinaryStream State { get; }
}