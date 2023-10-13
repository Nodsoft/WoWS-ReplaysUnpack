namespace Nodsoft.WowsReplaysUnpack.Core.Models;

/// <summary>
/// The structure if the ArenaInfo property of a replay.
/// </summary>
/// <param name="MatchGroup">The match group of the replay, for example pvp.</param>
/// <param name="GameMode">The numeric identifier for the game mode of the replay.</param>
/// <param name="ClientVersionFromExe">The client version from the game executable.</param>
/// <param name="MapId">The numeric id of the current map.</param>
/// <param name="ClientVersionFromXml">The client version from the xml specification.</param>
/// <param name="WeatherParams">Weather parameters affecting the current game.</param>
/// <param name="PlayersPerTeam">The number of players per team.</param>
/// <param name="Duration">The maximum duration of the game in seconds.</param>
/// <param name="GameLogic">The subtype of the game mode, for example Domination.</param>
/// <param name="Name">The name of the game, for example 12x12.</param>
/// <param name="Scenario">The specific game scenario, like Domination_3point.</param>
/// <param name="Vehicles">A list of all ships and players that are part of the replay.</param>
/// <param name="GameType">A string representing the game mode, for example RandomBattle.</param>
/// <param name="DateTime">The date when the battle started. In format 'dd.MM.yyyy HH:mm:ss'.</param>
/// <param name="PlayerName">The name of the player.</param>
/// <param name="ScenarioConfigId">The ID of the game scenario.</param>
/// <param name="TeamsCount">The number of teams in the game. This considers the actual teams, not divisions.</param>
/// <param name="Logic">The type of the game mode, usually the same as <see cref="GameLogic"/>.</param>
/// <param name="PlayerVehicle">The internal name of the ship, underscores of the internal name are replaced by hyphens.</param>
/// <param name="BattleDuration">The maximum duration of the game in seconds, usually the same as <see cref="Duration"/>.</param>
public sealed record ArenaInfo(
	string MatchGroup,
	uint GameMode,
	string ClientVersionFromExe,
	uint MapId,
	string ClientVersionFromXml,
	Dictionary<int, List<string>> WeatherParams,
	int PlayersPerTeam,
	int Duration,
	string? GameLogic,
	string Name,
	string Scenario,
	List<VehicleDetails> Vehicles,
	string GameType,
	DateTime DateTime,
	string PlayerName,
	int ScenarioConfigId,
	int TeamsCount,
	string? Logic,
	string PlayerVehicle,
	int BattleDuration,
	object[]? DisabledShipClasses,
	object? MapBorder
);

/// <summary>
/// Represents a vehicle in the replay.
/// </summary>
/// <param name="ShipId">The numeric ID of the ship.</param>
/// <param name="Relation">The relation of the ship to the player.</param>
/// <param name="Id">The numeric ID of the player.</param>
/// <param name="Name">The name of the player.</param>
public sealed record VehicleDetails(uint ShipId, uint Relation, uint Id, string Name);