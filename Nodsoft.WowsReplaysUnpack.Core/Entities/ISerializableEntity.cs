namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

public interface ISerializableEntity
{
	void SetProperty(string name, object? value, int[] indexes);
}