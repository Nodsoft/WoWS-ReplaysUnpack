using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.EntitySerializer;

[PublicAPI]
public static class UnpackedReplayExtensions
{
	public static T DeserializeEntity<T>(this UnpackedReplay replay, string entityName)
		where T : class, ISerializableEntity, new()
	{
		if (replay.Entities.All(e => e.Value.Name != entityName))
		{
			throw new InvalidOperationException("No entity found with name " + entityName);
		}

		return EntitySerializer.Deserialize<T>(replay.Entities.Single(e => e.Value.Name == entityName).Value);
	}

	public static T[] SerializeEntities<T>(this UnpackedReplay replay, string entityName)
		where T : class, ISerializableEntity, new()
	{
		if (replay.Entities.All(e => e.Value.Name != entityName))
		{
			throw new InvalidOperationException("No entity found with name " + entityName);
		}

		return EntitySerializer.Deserialize<T>(replay.Entities.Where(e => e.Value.Name == entityName)
			.Select(e => e.Value));
	}


	public static T DeserializeEntity<T>(this UnpackedReplay replay, uint entityId)
		where T : class, ISerializableEntity, new()
	{
		if (!replay.Entities.TryGetValue(entityId, out Entity? entity))
		{
			throw new InvalidOperationException("No entity found with id " + entityId);
		}

		return EntitySerializer.Deserialize<T>(entity);
	}
}