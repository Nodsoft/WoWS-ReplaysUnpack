using Nodsoft.WowsReplaysUnpack.Generators;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nodsoft.WowsReplaysUnpack.Console.Samples.EntitySerializer;

[SerializableEntity]
public partial class BattleLogic
{
	[DataMember(Name = "state")]
	public State State { get; set; } = null!;
}

public class State
{
	[DataMember(Name = "missions")]
	public Missions Missions { get; set; } = null!;
}

public class Missions
{
	[DataMember(Name = "teamsScore")]
	public List<TeamsScore> TeamsScore { get; set; } = null!;
}

public class TeamsScore
{
	[DataMember(Name = "score")]
	public ushort Score { get; set; }
}