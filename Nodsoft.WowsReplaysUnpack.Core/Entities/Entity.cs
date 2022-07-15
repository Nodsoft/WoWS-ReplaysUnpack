﻿using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System.Numerics;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.Core.Entities;
public enum EntityType
{
	Client = 1,
	Cell = 2,
	Base = 4
}
public class Entity
{
	protected ILogger<Entity> Logger { get; }

	protected EntityDefinition EntityDefinition { get; }

	protected Dictionary<string, MethodInfo> MethodSubscriptions { get; }
	protected Dictionary<string, MethodInfo> PropertyChangedSubscriptions { get; }

	protected PropertyDefinition[] ClientPropertyDefinitions { get; }
	protected PropertyDefinition[] InternalClientPropertyDefinitions { get; }
	protected PropertyDefinition[] CellPropertyDefinitions { get; }
	protected PropertyDefinition[] BasePropertyDefinitions { get; }

	public int Id { get; }
	public string Name { get; }

	/// <summary>
	/// Is in Area of Influence (visible area)
	/// </summary>
	public bool IsInAoI { get; set; }

	public Dictionary<string, object?> ClientProperties { get; } = new();
	public Dictionary<string, object?> CellProperties { get; } = new();
	public Dictionary<string, object?> BaseProperties { get; } = new();
	public Dictionary<string, object> VolatileProperties { get; } = new();
	public List<EntityMethodDefinition> MethodDefinitions => EntityDefinition.ClientMethods;


	public Vector3 VPosition
	{
		get => VolatileProperties.ContainsKey("position") ? (Vector3)VolatileProperties["position"] : new Vector3();
		set => VolatileProperties["position"] = value;
	}

	public float Yaw
	{
		get => VolatileProperties.ContainsKey("yaw") ? (float)VolatileProperties["yaw"] : 0f;
		set => VolatileProperties["yaw"] = value;
	}

	public float Pitch
	{
		get => VolatileProperties.ContainsKey("pitch") ? (float)VolatileProperties["pitch"] : 0f;
		set => VolatileProperties["pitch"] = value;
	}

	public float Roll
	{
		get => VolatileProperties.ContainsKey("roll") ? (float)VolatileProperties["roll"] : 0f;
		set => VolatileProperties["roll"] = value;
	}

	public Entity(int id, string name, EntityDefinition entityDefinition,
		Dictionary<string, MethodInfo> methodSubscriptions,
		Dictionary<string, MethodInfo> propertyChangedSubscriptions,
		ILogger<Entity> logger)
	{
		Id = id;
		Name = name;
		EntityDefinition = entityDefinition;
		MethodSubscriptions = methodSubscriptions;
		PropertyChangedSubscriptions = propertyChangedSubscriptions;
		Logger = logger;
		VolatileProperties = EntityDefinition.VolatileProperties.ToDictionary(kv => kv.Key, kv => kv.Value);

		ClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS | EntityFlag.BASE_AND_CLIENT | EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT | EntityFlag.CELL_PUBLIC_AND_OWN, true);

		InternalClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS | EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT | EntityFlag.CELL_PUBLIC_AND_OWN);

		CellPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.CELL_PUBLIC_AND_OWN | EntityFlag.CELL_PUBLIC);

		BasePropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.BASE_AND_CLIENT);
	}

	public string GetClientPropertyNameForIndex(int index)
		=> ClientPropertyDefinitions[index].Name;

	public virtual void CallClientMethod(int index, BinaryReader reader, object? subscriptionTarget)
	{
		if (subscriptionTarget is null)
			return;

		EntityMethodDefinition? methodDefinition = MethodDefinitions.ElementAtOrDefault(index);
		if (methodDefinition is null)
		{
			Logger.LogError("Method with index {index} was not found on entity with name {Name} ({Id})", index, Name, Id);
			return;
		}
		string hash = $"{Name}_{methodDefinition.Name}";
		if (MethodSubscriptions.TryGetValue(hash, out MethodInfo? methodInfo))
		{
			ParameterInfo[] methodParameters = methodInfo.GetParameters();
			if (methodDefinition.Arguments.Count != methodParameters.Length - 1
				|| methodParameters[0].ParameterType != typeof(Entity)
				|| !methodDefinition.Arguments.Select(a => a.DataType.ClrType).SequenceEqual(methodParameters.Skip(1).Select(m => m.ParameterType))
			)
			{
				Logger.LogError("Arguments of method definition and method subscription does not match");
				return;
			}
			try
			{
				object?[] methodArgumentValues = methodDefinition.Arguments.Select(a => a.GetValue(reader))
					.Prepend(this).ToArray();
				Logger.LogDebug("Calling method subscription with hash {hash}", hash);
				methodInfo.Invoke(subscriptionTarget, methodArgumentValues);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Error when calling method subscription with hash {hash}", hash);
			}
		}
	}

	public virtual void SetClientProperty(int exposedIndex, BinaryReader reader, object? subscriptionTarget)
	{
		Logger.LogDebug("Setting client property with index {index} on entity {Name} ({id})", exposedIndex, Name, Id);
		PropertyDefinition propertyDefinition = ClientPropertyDefinitions[exposedIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;

		if (subscriptionTarget is null)
			return;

		string hash = $"{Name}_{propertyDefinition.Name}";
		if (PropertyChangedSubscriptions.TryGetValue(hash, out MethodInfo? methodInfo))
		{
			ParameterInfo[] methodParameters = methodInfo.GetParameters();
			if (methodParameters.Length != 2
					|| methodParameters[0].ParameterType != typeof(Entity)
					|| methodParameters[1].ParameterType != propertyDefinition.DataType.ClrType
			)
			{
				Logger.LogError("Arguments of property definition and property changed subscription does not match");
				return;
			}
			try
			{
				Logger.LogDebug("Calling property changed subscription with hash {hash}", hash);
				methodInfo.Invoke(subscriptionTarget, new[] { this, propertyValue });
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Error when calling property changed subscription with hash {hash}", hash);
			}
		}
	}

	public virtual void SetInternalClientProperty(int internalIndex, BinaryReader reader)
	{
		Logger.LogDebug("Setting internal client property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		PropertyDefinition propertyDefinition = InternalClientPropertyDefinitions[internalIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;
	}

	public virtual void SetCellProperty(int internalIndex, BinaryReader reader)
	{
		Logger.LogDebug("Setting cell property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		PropertyDefinition propertyDefinition = CellPropertyDefinitions[internalIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		CellProperties[propertyDefinition.Name] = propertyValue;
	}

	public virtual void SetBaseProperty(int internalIndex, BinaryReader reader)
	{
		Logger.LogDebug("Setting base property with index {index} on entity {Name} ({id})", internalIndex, Name, Id);
		PropertyDefinition propertyDefinition = BasePropertyDefinitions[internalIndex];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		BaseProperties[propertyDefinition.Name] = propertyValue;
	}

	public virtual void SetBaseProperties(BinaryReader reader)
	{
		for (int i = 0; i < BasePropertyDefinitions.Length; i++)
			SetBaseProperty(i, reader);
	}

	public virtual void SetInternalClientProperties(BinaryReader reader)
	{
		for (int i = 0; i < InternalClientPropertyDefinitions.Length; i++)
			SetInternalClientProperty(i, reader);
	}

	public void SetPosition(PositionContainer position)
	{
		VPosition = position.Position;
		Yaw = position.Yaw;
		Pitch = position.Pitch;
		Roll = position.Roll;
	}

	public PositionContainer GetPosition()
		=> new(VPosition, Yaw, Pitch, Roll);

	public override string ToString() => $"{Name} <{Id}>";
}
