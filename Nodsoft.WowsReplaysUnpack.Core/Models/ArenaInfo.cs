using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Core.Json;
using System.Text.Json.Serialization;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

/// <summary>
/// The structure if the ArenaInfo property of a replay.
/// </summary>
[PublicAPI]
public sealed class ArenaInfo
{
	public ArenaInfo(string matchGroup,
		uint gameMode,
		string clientVersionFromExe,
		uint mapId,
		string clientVersionFromXml,
		Dictionary<int, List<string>> weatherParams,
		int playersPerTeam,
		int duration,
		string? gameLogic,
		string name,
		string scenario,
		List<VehicleDetails> vehicles,
		string gameType,
		DateTime dateTime,
		string playerName,
		int scenarioConfigId,
		int teamsCount,
		string? logic,
		string playerVehicle,
		int battleDuration,
		object[]? disabledShipClasses,
		object? mapBorder)
	{
		MatchGroup = matchGroup;
		GameMode = gameMode;
		ClientVersionFromExe = clientVersionFromExe;
		MapId = mapId;
		ClientVersionFromXml = clientVersionFromXml;
		WeatherParams = weatherParams;
		PlayersPerTeam = playersPerTeam;
		Duration = duration;
		GameLogic = gameLogic;
		Name = name;
		Scenario = scenario;
		Vehicles = vehicles;
		GameType = gameType;
		DateTime = dateTime;
		PlayerName = playerName;
		ScenarioConfigId = scenarioConfigId;
		TeamsCount = teamsCount;
		Logic = logic;
		PlayerVehicle = playerVehicle;
		BattleDuration = battleDuration;
		DisabledShipClasses = disabledShipClasses;
		MapBorder = mapBorder;

		ClientVersion = Version.Parse(string.Join('.', ClientVersionFromExe.Split(',')[..3]));
	}

	/// <summary>The match group of the replay, for example pvp.</summary>
	public string MatchGroup { get; }

	/// <summary>The numeric identifier for the game mode of the replay.</summary>
	public uint GameMode { get; }

	/// <summary>The client version from the game executable.</summary>
	public string ClientVersionFromExe { get; }

	/// <summary>The numeric id of the current map.</summary>
	public uint MapId { get; }

	/// <summary>The client version from the xml specification.</summary>
	public string ClientVersionFromXml { get; }

	/// <summary>Weather parameters affecting the current game.</summary>
	public Dictionary<int, List<string>> WeatherParams { get; }

	/// <summary>The number of players per team.</summary>
	public int PlayersPerTeam { get; }

	/// <summary>The maximum duration of the game in seconds.</summary>
	public int Duration { get; }

	/// <summary>The subtype of the game mode, for example Domination.</summary>
	public string? GameLogic { get; }

	/// <summary>The name of the game, for example 12x12.</summary>
	public string Name { get; }

	/// <summary>The specific game scenario, like Domination_3point.</summary>
	public string Scenario { get; }

	/// <summary>A list of all ships and players that are part of the replay.</summary>
	public List<VehicleDetails> Vehicles { get; }

	/// <summary>A string representing the game mode, for example RandomBattle.</summary>
	public string GameType { get; }

	/// <summary>The date when the battle started. In format 'dd.MM.yyyy HH:mm:ss'.</summary>
	public DateTime DateTime { get; }

	/// <summary>The name of the player.</summary>
	public string PlayerName { get; }

	/// <summary>The ID of the game scenario.</summary>
	public int ScenarioConfigId { get; }

	/// <summary>The number of teams in the game. This considers the actual teams, not divisions.</summary>
	public int TeamsCount { get; }

	/// <summary>The type of the game mode, usually the same as <see cref="GameLogic"/>.</summary>
	public string? Logic { get; }

	/// <summary>The internal name of the ship, underscores of the internal name are replaced by hyphens.</summary>
	public string PlayerVehicle { get; }

	/// <summary>The maximum duration of the game in seconds, usually the same as <see cref="Duration"/>.</summary>
	public int BattleDuration { get; }

	public object[]? DisabledShipClasses { get; }
	public object? MapBorder { get; }

	/// <summary>The parsed client version</summary>
	public Version ClientVersion { get; }
}

/// <summary>
/// Represents a vehicle in the replay.
/// </summary>
[PublicAPI]
public sealed class VehicleDetails
{
	public VehicleDetails(uint shipId, uint relation, uint id, string name)
	{
		ShipId = shipId;
		Relation = relation;
		Id = id;
		Name = name;
	}

	/// <summary>The numeric ID of the ship.</summary>
	public uint ShipId { get; }

	/// <summary>The relation of the ship to the player.</summary>
	public uint Relation { get; }

	/// <summary>The numeric ID of the player.</summary>
	public uint Id { get; }

	/// <summary>The name of the player.</summary>
	public string Name { get; }
}