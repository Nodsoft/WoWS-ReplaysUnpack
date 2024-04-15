using System.Collections;
using System.Collections.Generic;

namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

public record struct MethodSubscriptionData
{
	public readonly string CallName;
	public readonly string EntityName;
	public readonly string MethodName;
	public readonly bool ParamsAsDictionary;
	public readonly bool IncludeEntity;
	public readonly bool IncludePacketTime;
	public readonly int? Priority;
	public EquatableArray<string> ParameterTypes;

	public MethodSubscriptionData(string callName, string entityName, string methodName, bool paramsAsDictionary,
		bool includeEntity, bool includePacketTime, int? priority, string[] parameterTypes)
	{
		CallName = callName;
		EntityName = entityName;
		MethodName = methodName;
		ParamsAsDictionary = paramsAsDictionary;
		IncludeEntity = includeEntity;
		IncludePacketTime = includePacketTime;
		Priority = priority;
		ParameterTypes = new EquatableArray<string>(parameterTypes);
	}
}