namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

public record struct PropertyChangedSubscriptionData
{
	public readonly string CallName;
	public readonly string EntityName;
	public readonly string PropertyName;
	public string ParameterType;

	public PropertyChangedSubscriptionData(string callName, string entityName, string propertyName, string parameterType)
	{
		CallName = callName;
		EntityName = entityName;
		PropertyName = propertyName;
		ParameterType = parameterType;
	}
}