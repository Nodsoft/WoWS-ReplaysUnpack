using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nodsoft.WowsReplaysUnpack.EntitySerializer;
public static class EntitySerializer
{
	public static T Deserialize<T>(Entity entity) where T : class
	{
		Dictionary<string, object?> clientProperties = entity.ClientProperties;
		PropertyInfo[] properties = typeof(T).GetProperties();
		T obj = Activator.CreateInstance<T>();
		DeserializeDictionaryProperties(clientProperties, properties, obj);
		return obj;
	}

	public static T[] Deserialize<T>(IEnumerable<Entity> entities) where T : class
	{
	var result = new List<T>();
		foreach(var entity in entities)
			result.Add(Deserialize<T>(entity));
		return result.ToArray();
	}

	private static void DeserializeDictionaryProperties(Dictionary<string, object?> entityProperties, PropertyInfo[] propertyInfos, object obj)
	{
		Dictionary<string, object?> invariantDictionary = entityProperties.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

		foreach (PropertyInfo? propertyInfo in propertyInfos)
		{
			string propertyName = propertyInfo.Name;
			DataMemberAttribute? dataMemberAttribute = propertyInfo.GetCustomAttribute<DataMemberAttribute>();
			if (dataMemberAttribute is { Name.Length: > 0 })
			{
				propertyName = dataMemberAttribute.Name;
			}
			if (invariantDictionary.TryGetValue(propertyName, out object? value))
			{
				DeserializeProperty(value, propertyInfo, obj);
			}
		}
	}

	private static void DeserializeProperty(object? entityPropertyValue, PropertyInfo propertyInfo, object obj)
	{
		if (entityPropertyValue is null)
		{
			return;
		}
		else if (entityPropertyValue is FixedDictionary dict)
		{
			propertyInfo.SetValue(obj, DeserializeFixedDictionary(dict, propertyInfo.PropertyType));
		}
		else if (entityPropertyValue is FixedList list)
		{
			propertyInfo.SetValue(obj, DeserializeFixedList(list, propertyInfo.PropertyType.GenericTypeArguments[0]));
		}
		else
		{
			propertyInfo.SetValue(obj, entityPropertyValue);
		}
	}

	private static object? DeserializeFixedDictionary(FixedDictionary dict, Type propertyType)
	{
		object propertyObj = Activator.CreateInstance(propertyType)!;
		DeserializeDictionaryProperties(dict, propertyType.GetProperties(), propertyObj);
		return propertyObj;
	}

	private static object? DeserializeFixedList(FixedList list, Type elementType)
	{
		Type listType = typeof(List<>).MakeGenericType(elementType);
		MethodInfo addMethod = listType.GetMethod("Add")!;
		object values = Activator.CreateInstance(listType)!;
		foreach (object? item in list)
		{
			if (item is FixedDictionary itemDict)
			{
				object itemObj = Activator.CreateInstance(elementType)!;
				addMethod.Invoke(values, new[] { DeserializeFixedDictionary(itemDict, elementType) });
			}
			else if (item is FixedList itemList)
			{
				throw new NotSupportedException("List in list not supported");
			}
			else
			{
				addMethod.Invoke(values, new[] { item });
			}
		}
		return values;
	}
}
