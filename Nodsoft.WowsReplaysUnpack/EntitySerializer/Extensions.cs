using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.EntitySerializer;
public static class Extensions
{
	public static T SerializeEntity<T>(this UnpackedReplay replay, string entityName) where T : class
	{
		if (!replay.Entities.Any(e => e.Value.Name == entityName))
		{
			throw new InvalidOperationException("No entity found with name " + entityName);
		}
		return EntitySerializer.Deserialize<T>(replay.Entities.Single(e => e.Value.Name == entityName).Value);
	}

	public static T[] SerializeEntities<T>(this UnpackedReplay replay, string entityName) where T : class
	{
		if (!replay.Entities.Any(e => e.Value.Name == entityName))
		{
			throw new InvalidOperationException("No entity found with name " + entityName);
		}
		return EntitySerializer.Deserialize<T>(replay.Entities.Where(e => e.Value.Name == entityName).Select(e => e.Value));
	}


	public static T SerializeEntity<T>(this UnpackedReplay replay, uint entityId) where T : class
	{
		if (!replay.Entities.TryGetValue(entityId, out Entity? entity))
		{
			throw new InvalidOperationException("No entity found with id " + entityId);
		}
		return EntitySerializer.Deserialize<T>(entity);
	}
}
