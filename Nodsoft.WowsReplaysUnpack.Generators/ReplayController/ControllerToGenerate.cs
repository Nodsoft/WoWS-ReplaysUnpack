﻿namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

public record struct ControllerToGenerate
{
	public readonly string Name;
	public readonly string ClassName;
	public readonly string FullyQualifiedName;
	public readonly string Namespace;

	public readonly EquatableArray<MethodSubscriptionData> Methods;
	public readonly EquatableArray<PropertyChangedSubscriptionData> Properties;

	public ControllerToGenerate(string name, string className, string fullyQualifiedName, string @namespace,
		MethodSubscriptionData[] methods,
		PropertyChangedSubscriptionData[] properties)
	{
		Name = name;
		ClassName = className;
		FullyQualifiedName = fullyQualifiedName;
		Namespace = @namespace;
		Methods = new EquatableArray<MethodSubscriptionData>(methods);
		Properties = new EquatableArray<PropertyChangedSubscriptionData>(properties);
	}
}