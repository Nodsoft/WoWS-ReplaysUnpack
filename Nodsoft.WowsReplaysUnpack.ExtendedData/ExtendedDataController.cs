using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings;
using Nodsoft.WowsReplaysUnpack.Generators;
using Razorvine.Pickle;
using System.Collections;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData;

[PublicAPI]
public class ExtendedDataController : ExtendedDataController<ExtendedDataReplay>
{
	public ExtendedDataController(VersionMappingFactory versionMappingFactory,
		IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(versionMappingFactory,
		definitionStore, entityLogger)
	{
	}
}

[ReplayController]
[PublicAPI]
public partial class ExtendedDataController<TReplay> : ReplayControllerBase<TReplay>
	where TReplay : ExtendedDataReplay, new()
{
	private readonly VersionMappingFactory _versionMappingFactory;

	static ExtendedDataController() =>
		Unpickler.registerConstructor("CamouflageInfo", "CamouflageInfo", new CamouflageInfo());

	public ExtendedDataController(VersionMappingFactory versionMappingFactory, IDefinitionStore definitionStore,
		ILogger<Entity> entityLogger)
		: base(definitionStore, entityLogger)
		=> _versionMappingFactory = versionMappingFactory;

	/// <summary>
	/// Triggered when a chat message is parsed from the replay.
	/// </summary>
	/// <param name="packetTime">The time the packet was received.</param>
	/// <param name="entityId">The entity ID of the player who sent the message.</param>
	/// <param name="messageGroup">The message group of the message (All/).</param>
	/// <param name="messageContent">The content of the message.</param>
	/// <param name="reserved1">Parameter unused</param>
	[MethodSubscription("Avatar", "onChatMessage", IncludePacketTime = true)]
	private void AvatarOnChatMessage(float packetTime, int entityId, string messageGroup, string messageContent,
		string reserved1)
	{
		Replay.ChatMessages.Add(new((uint)entityId, packetTime, messageGroup, messageContent));
	}

	/// <summary>
	/// Triggered when arena data is parsed from the replay.
	/// </summary>
	/// <param name="arguments">The arguments of the event.</param>
	[MethodSubscription("Avatar", "onArenaStateReceived", ParamsAsDictionary = true)]
	public void OnArenaStateReceived(Dictionary<string, object?> arguments)
	{
		byte[]? playerStatesData = (byte[]?)arguments["playersStates"];

		if (playerStatesData is null)
		{
			return;
		}

		using Unpickler unpickler = new();
		using MemoryStream memoryStream = new(playerStatesData);
		ArrayList players = unpickler.load(memoryStream) as ArrayList ?? new();

		foreach (ArrayList player in players)
		{
			AddPlayerToReplay(player);
		}
	}

	private void AddPlayerToReplay(ArrayList properties)
	{
		IVersionMapping mapping = _versionMappingFactory.GetMappings(Replay.ClientVersion);
		ReplayPlayer replayPlayer = new(mapping.ShipConfigMapping);

		foreach (object[] propertyArray in properties)
		{
			string? propertyName = mapping.ReplayPlayerPropertyMappings.GetValueOrDefault((int)propertyArray[0]);

			if (string.IsNullOrEmpty(propertyName))
			{
				continue;
			}

			PropertyInfo? propertyInfo = ReplayPlayer.PropertyInfos.SingleOrDefault(x => x.Name == propertyName);
			propertyInfo?.SetValue(replayPlayer, Convert.ChangeType(propertyArray[1], propertyInfo.PropertyType),
				null);
		}

		Replay.ReplayPlayers.Add(replayPlayer);
	}
}