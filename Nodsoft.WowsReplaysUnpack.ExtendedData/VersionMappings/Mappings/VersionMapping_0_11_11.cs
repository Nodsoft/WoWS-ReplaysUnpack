using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings.Mappings;

// ReSharper disable once InconsistentNaming
public class VersionMapping_0_11_11 : DefaultVersionMapping
{
	public override Version? Version { get; } = new(0, 11, 11);

	public override Dictionary<int, string> ReplayPlayerPropertyMappings => new()
	{
		{ 0, nameof(ReplayPlayer.AccountId) },
		{ 1, nameof(ReplayPlayer.AntiAbuseEnabled) },
		{ 2, nameof(ReplayPlayer.AvatarId) },
		{ 3, nameof(ReplayPlayer.CamouflageInfo) },
		{ 4, nameof(ReplayPlayer.ClanColor) },
		{ 5, nameof(ReplayPlayer.ClanId) },
		{ 6, nameof(ReplayPlayer.ClanTag) },
		{ 7, nameof(ReplayPlayer.CrewParams) },
		{ 8, nameof(ReplayPlayer.DogTag) },
		{ 9, nameof(ReplayPlayer.FragsCount) },
		{ 10, nameof(ReplayPlayer.FriendlyFireEnabled) },
		{ 11, nameof(ReplayPlayer.Id) },
		{ 12, nameof(ReplayPlayer.InvitationsEnabled) },
		{ 13, nameof(ReplayPlayer.IsAbuser) },
		{ 14, nameof(ReplayPlayer.IsAlive) },
		{ 15, nameof(ReplayPlayer.IsBot) },
		{ 16, nameof(ReplayPlayer.IsClientLoaded) },
		{ 17, nameof(ReplayPlayer.IsConnected) },
		{ 18, nameof(ReplayPlayer.IsHidden) },
		{ 19, nameof(ReplayPlayer.IsLeaver) },
		{ 20, nameof(ReplayPlayer.IsPreBattleOwner) },
		{ 21, nameof(ReplayPlayer.IsTShooter) },
		{ 22, nameof(ReplayPlayer.KilledBuildingsCount) },
		{ 23, nameof(ReplayPlayer.IsCookie) },
		{ 24, nameof(ReplayPlayer.MaxHealth) },
		{ 25, nameof(ReplayPlayer.Name) },
		{ 26, nameof(ReplayPlayer.PlayerMode) },
		{ 27, nameof(ReplayPlayer.PreBattleIdOnStart) },
		{ 28, nameof(ReplayPlayer.PreBattleSign) },
		{ 29, nameof(ReplayPlayer.PreBattleId) },
		{ 30, nameof(ReplayPlayer.Realm) },
		{ 31, nameof(ReplayPlayer.ShipComponents) },
		{ 32, nameof(ReplayPlayer.ShipConfigDump) },
		{ 33, nameof(ReplayPlayer.ShipId) },
		{ 34, nameof(ReplayPlayer.ShipParamsId) },
		{ 35, nameof(ReplayPlayer.SkinId) },
		{ 36, nameof(ReplayPlayer.TeamId) },
		{ 37, nameof(ReplayPlayer.TtkStatus) }
	};
}