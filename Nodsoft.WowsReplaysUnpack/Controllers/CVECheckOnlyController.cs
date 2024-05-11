using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

/// <summary>
/// Lightweight implementation of a replay controller, designed to analyse a replay for vulnerabilities. <br />
/// Currently scans for signs of <a href="https://www.cve.org/CVERecord?id=CVE-2022-31265">CVE-2022-31265</a>.
/// </summary>
[PublicAPI]
public class CveCheckOnlyController : ReplayControllerBase<CveCheckOnlyController.CveCheckReplay>
{
	public class CveCheckReplay : UnpackedReplay;
	
	// ReSharper disable once ContextualLoggerProblem
	public CveCheckOnlyController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger) { }

	/// <inheritdoc />
	public override void HandleNetworkPacket(NetworkPacketBase networkPacket, ReplayUnpackerOptions options)
	{
		if (networkPacket is BasePlayerCreatePacket bpPacker)
		{
			OnBasePlayerCreate(bpPacker);
		}

		if (networkPacket is CellPlayerCreatePacket cpPacket)
		{
			OnCellPlayerCreate(cpPacket);
		}

		if (networkPacket is EntityMethodPacket entityMethod)
		{
			OnEntityMethod(entityMethod);
		}
	}

	/// <inheritdoc />
	protected override void OnEntityMethod(EntityMethodPacket packet)
	{
		if (!Replay.Entities.ContainsKey(packet.EntityId))
		{
			return;
		}

		Entity entity = Replay.Entities[packet.EntityId];

		if (entity.Name is not "Avatar" && entity.GetClientMethodName(packet.MessageId) is not "onArenaStateReceived")
		{
			return;
		}

		using BinaryReader methodDataReader = packet.Data.GetBinaryReader();
		entity.CallClientMethod(packet.MessageId, packet.PacketTime, methodDataReader, this);
	}
}