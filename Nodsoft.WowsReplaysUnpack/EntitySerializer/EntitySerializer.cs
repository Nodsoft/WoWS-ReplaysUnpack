using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.EntitySerializer;

public interface ISpecificEntity
{
	void SetProperty(string name, object? value = null, int? index = null);
}

// Source Gen
public static class EntitySerializer
{
	public static T Deserialize<T>(Entity entity) where T : class, ISpecificEntity, new()
	{
		var obj = new T();
		DeserializeDictionaryProperties(entity.ClientProperties, obj);
		return obj;
	}

	public static T[] Deserialize<T>(IEnumerable<Entity> entities) where T : class, ISpecificEntity, new()
	{
		var result = new List<T>();
		foreach (var entity in entities)
			result.Add(Deserialize<T>(entity));
		return result.ToArray();
	}

	private static void DeserializeDictionaryProperties<T>(Dictionary<string, object?> entityProperties, T obj)
		where T : class, ISpecificEntity
	{
		Dictionary<string, object?> invariantDictionary =
			entityProperties.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

		foreach (var property in invariantDictionary)
		{
			SetProperty(obj, property.Key, property.Value);
			// string propertyName = propertyInfo.Name;
			// DataMemberAttribute? dataMemberAttribute = propertyInfo.GetCustomAttribute<DataMemberAttribute>();
			// if (dataMemberAttribute is { Name.Length: > 0 })
			// {
			// 	propertyName = dataMemberAttribute.Name;
			// }
			// if (invariantDictionary.TryGetValue(propertyName, out object? value))
			// {
			// 	DeserializeProperty(value, propertyInfo, obj);
			// }
		}
	}

	private static void SetProperty<T>(T instance, string propertyName, object? propertyValue, int? index = null)
		where T : class, ISpecificEntity
	{
		if (propertyValue is null)
		{
			return;
		}


		if (propertyValue is FixedDictionary dict)
		{
			instance.SetProperty(propertyName);
			foreach (var dictProperty in dict)
			{
				SetProperty(instance, propertyName + "." + dictProperty.Key, dictProperty.Value);
			}
		}
		else if (propertyValue is FixedList { Count: > 0 } list)
		{
			instance.SetProperty(propertyName);

			for (int i = 0; i < list.Count; i++)
			{
				instance.SetProperty(propertyName);
				SetProperty(instance, propertyName, list[i], i);
			}
		}
		else
		{
			instance.SetProperty(propertyName, propertyValue, index);
		}
	}

	// private static object? DeserializeFixedDictionary(FixedDictionary dict, Type propertyType)
	// {
	// 	object propertyObj = Activator.CreateInstance(propertyType)!;
	// 	DeserializeDictionaryProperties(dict, propertyType.GetProperties(), propertyObj);
	// 	return propertyObj;
	// }
	//
	// private static object? DeserializeFixedList(FixedList list, Type elementType)
	// {
	// 	Type listType = typeof(List<>).MakeGenericType(elementType);
	// 	MethodInfo addMethod = listType.GetMethod("Add")!;
	// 	object values = Activator.CreateInstance(listType)!;
	// 	foreach (object? item in list)
	// 	{
	// 		if (item is FixedDictionary itemDict)
	// 		{
	// 			object itemObj = Activator.CreateInstance(elementType)!;
	// 			addMethod.Invoke(values, new[] { DeserializeFixedDictionary(itemDict, elementType) });
	// 		}
	// 		else if (item is FixedList itemList)
	// 		{
	// 			throw new NotSupportedException("List in list not supported");
	// 		}
	// 		else
	// 		{
	// 			addMethod.Invoke(values, new[] { item });
	// 		}
	// 	}
	//
	// 	return values;
	// }
}