namespace Nodsoft.WowsReplaysUnpack.Generators.SerializableEntity;

public record struct EntityToGenerate
{
	public readonly string ClassName;
	public readonly string Namespace;
	public readonly EquatableArray<PropertyData> Properties;

	public EntityToGenerate(string className, string ns, PropertyData[] properties)
	{
		ClassName = className;
		Namespace = ns;
		Properties = new EquatableArray<PropertyData>(properties);
	}
}