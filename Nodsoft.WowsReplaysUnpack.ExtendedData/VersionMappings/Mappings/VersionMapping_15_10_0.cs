using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings.Mappings;

// ReSharper disable once InconsistentNaming
public class VersionMapping_15_10_0 : DefaultVersionMapping
{
	public override Version? Version { get; } = new(15, 10, 0);

	public override Dictionary<int, string> ReplayPlayerPropertyMappings => new()
	{
		{ 0, nameof(ReplayPlayer.AccountId) },
		{ 1, nameof(ReplayPlayer.AntiAbuseEnabled) },
		{ 2, nameof(ReplayPlayer.AvatarId) },
		{ 3, nameof(ReplayPlayer.CamouflageInfo) },
		{ 4, nameof(ReplayPlayer.ClanColor) },
		{ 5, nameof(ReplayPlayer.ClanId) },
		{ 6, nameof(ReplayPlayer.ClanTag) },
		{ 7, nameof(ReplayPlayer.UnknwonField1) },
		{ 8, nameof(ReplayPlayer.CrewParams) },
		{ 9, nameof(ReplayPlayer.DogTag) },
		{ 10, nameof(ReplayPlayer.FragsCount) },
		{ 11, nameof(ReplayPlayer.FriendlyFireEnabled) },
		{ 12, nameof(ReplayPlayer.Id) },
		{ 13, nameof(ReplayPlayer.InvitationsEnabled) },
		{ 14, nameof(ReplayPlayer.IsAbuser) },
		{ 15, nameof(ReplayPlayer.IsAlive) },
		{ 16, nameof(ReplayPlayer.IsBot) },
		{ 17, nameof(ReplayPlayer.IsClientLoaded) },
		{ 18, nameof(ReplayPlayer.IsConnected) },
		{ 19, nameof(ReplayPlayer.IsHidden) },
		{ 20, nameof(ReplayPlayer.IsLeaver) },
		{ 21, nameof(ReplayPlayer.IsPreBattleOwner) },
		{ 22, nameof(ReplayPlayer.IsTShooter) },
		{ 23, nameof(ReplayPlayer.KilledBuildingsCount) },
		{ 24, nameof(ReplayPlayer.IsCookie) },
		{ 25, nameof(ReplayPlayer.MaxHealth) },
		{ 26, nameof(ReplayPlayer.Name) },
		{ 27, nameof(ReplayPlayer.PlayerMode) },
		{ 28, nameof(ReplayPlayer.PreBattleIdOnStart) },
		{ 29, nameof(ReplayPlayer.PreBattleSign) },
		{ 30, nameof(ReplayPlayer.PreBattleId) },
		{ 31, nameof(ReplayPlayer.Realm) },
		{ 32, nameof(ReplayPlayer.ShipComponents) },
		{ 33, nameof(ReplayPlayer.ShipConfigDump) },
		{ 34, nameof(ReplayPlayer.ShipFrags) },
		{ 35, nameof(ReplayPlayer.ShipId) },
		{ 36, nameof(ReplayPlayer.ShipParamsId) },
		{ 37, nameof(ReplayPlayer.SkinId) },
		{ 38, nameof(ReplayPlayer.TeamId) },
		{ 39, nameof(ReplayPlayer.TtkStatus) }
	};
}