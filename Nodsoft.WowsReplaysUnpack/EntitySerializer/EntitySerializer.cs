using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.EntitySerializer;

public static class EntitySerializer
{
	public static T Deserialize<T>(Entity entity) where T : class, ISerializableEntity, new()
	{
		T obj = new();
		DeserializeDictionaryProperties(entity.ClientProperties, obj);
		return obj;
	}

	public static T[] Deserialize<T>(IEnumerable<Entity> entities) where T : class, ISerializableEntity, new()
	{
		List<T> result = new();
		foreach (Entity entity in entities)
		{
			result.Add(Deserialize<T>(entity));
		}

		return result.ToArray();
	}

	private static void DeserializeDictionaryProperties<T>(Dictionary<string, object?> entityProperties, T obj)
		where T : class, ISerializableEntity
	{
		Dictionary<string, object?> invariantDictionary =
			entityProperties.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
		
		foreach (KeyValuePair<string, object?> property in invariantDictionary)
		{
			SetProperty(obj, property.Key, property.Value, []);
		}
	}

	private static void SetProperty<T>(T instance, string propertyName, object? propertyValue, int[] indexes)
		where T : class, ISerializableEntity
	{
		if (propertyValue is null)
		{
			return;
		}


		if (propertyValue is FixedDictionary dict)
		{
			instance.SetProperty(propertyName, null, []);
			foreach (KeyValuePair<string, object?> dictProperty in dict)
			{
				SetProperty(instance, propertyName + "." + dictProperty.Key, dictProperty.Value, indexes);
			}
		}
		else if (propertyValue is FixedList { Count: > 0 } list)
		{
			instance.SetProperty(propertyName + ".#Add", null, indexes);

			for (int i = 0; i < list.Count; i++)
			{
				instance.SetProperty(propertyName, null, indexes);
				SetProperty(instance, propertyName, list[i], [..indexes, i]);
			}
		}
		else
		{
			instance.SetProperty(propertyName, propertyValue, indexes);
		}
	}
}