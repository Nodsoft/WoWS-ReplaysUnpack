using Razorvine.Pickle.Objects;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData.Models;

// TODO: Source Gen
public record ReplayPlayer
{
	// // Source Gen
	// static void SetProperty(ReplayPlayer player, string name, object value)
	// {
	// 	switch (name)
	// 	{
	// 		case "AccountId" when value is uint v1:
	// 			player.AccountId = v1;
	// 			break;
	// 	}
	// }
	
	public static IEnumerable<PropertyInfo> PropertyInfos { get; } = typeof(ReplayPlayer).GetProperties();

	public uint AccountId { get; set; }

	public bool AntiAbuseEnabled { get; set; }

	public uint AvatarId { get; set; }

	public string? CamouflageInfo { get; set; }

	public uint ClanColor { get; set; }

	public uint ClanId { get; set; }

	public string? ClanTag { get; set; }

	public ArrayList? CrewParams { get; set; }

	public ArrayList? DogTag { get; set; }

	public short FragsCount { get; set; }

	public bool FriendlyFireEnabled { get; set; }

	public uint Id { get; set; }

	public bool InvitationsEnabled { get; set; }

	public bool IsAbuser { get; set; }

	public bool IsAlive { get; set; }

	public bool IsBot { get; set; }

	public bool IsClientLoaded { get; set; }

	public bool IsConnected { get; set; }

	public bool IsHidden { get; set; }

	public bool IsLeaver { get; set; }

	public bool IsPreBattleOwner { get; set; }

	public bool IsTShooter { get; set; }

	public short KilledBuildingsCount { get; set; }

	public uint MaxHealth { get; set; }

	public string Name { get; set; } = string.Empty;

	public ClassDict? PlayerMode { get; set; }

	public uint PreBattleIdOnStart { get; set; }

	public object? PreBattleSign { get; set; }

	public uint PreBattleId { get; set; }

	public string Realm { get; set; } = string.Empty;

	public Hashtable? ShipComponents { get; set; }

	public string ShipConfigDump { get; set; } = string.Empty;

	public uint ShipId { get; set; }

	public uint ShipParamsId { get; set; }

	public uint SkinId { get; set; }

	public uint TeamId { get; set; }

	public bool TtkStatus { get; set; }

	public Lazy<ShipData?> ShipData { get; }
	
	
	/*
	 * FIXME: This is a hack to get replays to parse correctly post 0.11.11+.
	 * No idea what it is for now, my assumption was on the new Key Target / Cookie feature.
	 */
	public object? IsCookie { get; set; }

	public ReplayPlayer(ShipConfigMapping shipConfigMapping)
	{
		ShipData = new(() =>
			{
				if (string.IsNullOrEmpty(ShipConfigDump))
				{
					return null;
				}

				byte[] byteArray = Encoding.Latin1.GetBytes(ShipConfigDump);
				using MemoryStream memoryStream = new(byteArray);
				using BinaryReader binaryReader = new(memoryStream);

				List<uint> configDumpList = new();

				while (memoryStream.Position != memoryStream.Length) configDumpList.Add(binaryReader.ReadUInt32());

				return new(ProcessShipConfigDump(configDumpList), shipConfigMapping);
			}
		);
	}

	private static IReadOnlyList<uint[]> ProcessShipConfigDump(IReadOnlyList<uint> rawConfigDump)
	{
		// First value is useless, then it's shipId, then total values
		// After that it's the same format until the end
		// Length Indicator + Items (As many as the length indicator defines) 
		uint[] tempList = rawConfigDump.Skip(3).ToArray();
		List<uint[]> resultList = new()
		{
			new[] { rawConfigDump[1] }, // ShipId
			new[] { rawConfigDump[2] }  // TotalValues
		};

		while (tempList.Length > 0)
		{
			int length = (int)tempList[0];
			tempList = tempList.Skip(1).ToArray();
			resultList.Add(tempList.Take(length).ToArray());
			tempList = tempList.Skip(length).ToArray();
		}

		return resultList;
	}
}