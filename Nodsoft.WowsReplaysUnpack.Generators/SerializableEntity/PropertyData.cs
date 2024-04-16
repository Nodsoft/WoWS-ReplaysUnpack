namespace Nodsoft.WowsReplaysUnpack.Generators.SerializableEntity;

public record struct PropertyData
{
	public readonly string Name;
	public readonly string Path;
	public readonly string MappedPath;
	public readonly bool IsClass;
	public readonly bool IsAdd;
	public readonly string Type;
	public readonly int? ListLevel;

	public PropertyData(string name, string path, string mappedPath, bool isClass, string type, int? listLevel, bool isAdd = false)
	{
		Name = name;
		Path = path;
		MappedPath = mappedPath;
		Path = path;
		IsClass = isClass;
		Type = type;
		ListLevel = listLevel;
		IsAdd = isAdd;
	}
}