using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Abstractions;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System.Numerics;

namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

/// <summary>
/// Represents an entity in the game.
/// </summary>
[PublicAPI]
public class Entity
{
	/// <summary>
	/// Logger instance for this class.
	/// </summary>
	protected ILogger<Entity> Logger { get; }

	/// <summary>
	/// Definition of the entity.
	/// </summary>
	protected EntityDefinition EntityDefinition { get; }

	/// <summary>
	/// Definitions of the entity's properties, as scoped for public use from the client.
	/// </summary>
	protected PropertyDefinition[] ClientPropertyDefinitions { get; }

	/// <summary>
	/// Definitions of the entity's properties, as scoped for internal use from the client.
	/// </summary>
	protected PropertyDefinition[] InternalClientPropertyDefinitions { get; }

	/// <summary>
	/// Definitions of the entity's properties, as scoped for the cell.
	/// </summary>
	protected PropertyDefinition[] CellPropertyDefinitions { get; }

	/// <summary>
	/// Definitions of the entity's base properties.
	/// </summary>
	protected PropertyDefinition[] BasePropertyDefinitions { get; }

	/// <summary>
	/// ID of the entity.
	/// </summary>
	public uint Id { get; }

	/// <summary>
	/// Name of the entity.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Is in Area of Influence (visible area)
	/// </summary>
	public bool IsInAoI { get; set; }

	/// <summary>
	/// Entity properties, as scoped for public use from the client.
	/// </summary>
	public Dictionary<string, object?> ClientProperties { get; } = new();

	/// <summary>
	/// Entity properties, as scoped for internal use from the client.
	/// </summary>
	public Dictionary<string, object?> CellProperties { get; } = new();

	/// <summary>
	/// Entity base properties.
	/// </summary>
	public Dictionary<string, object?> BaseProperties { get; } = new();

	/// <summary>
	/// Volatile properties of the entity.
	/// </summary>
	public Dictionary<string, object> VolatileProperties { get; }

	/// <summary>
	/// Public method definitions exposed for this entity.
	/// </summary>
	/// <remarks>
	/// Reflects the <see cref="Definitions.EntityDefinition.ClientMethods"/> of this entity's <see cref="EntityDefinition"/>.
	/// </remarks>
	public IEnumerable<EntityMethodDefinition> MethodDefinitions => EntityDefinition.ClientMethods;

	/// <summary>
	/// 3D position of the entity.
	/// </summary>
	public Vector3 VPosition
	{
		get => VolatileProperties.TryGetValue("position", out object? value) ? (Vector3)value : new();
		set => VolatileProperties["position"] = value;
	}

	/// <summary>
	/// Yaw of the entity.
	/// </summary>
	public float Yaw
	{
		get => VolatileProperties.TryGetValue("yaw", out object? value) ? (float)value : 0f;
		set => VolatileProperties["yaw"] = value;
	}

	/// <summary>
	/// Pitch of the entity.
	/// </summary>
	public float Pitch
	{
		get => VolatileProperties.TryGetValue("pitch", out object? value) ? (float)value : 0f;
		set => VolatileProperties["pitch"] = value;
	}

	/// <summary>
	///	Roll of the entity.
	/// </summary>
	public float Roll
	{
		get => VolatileProperties.TryGetValue("roll", out object? value) ? (float)value : 0f;
		set => VolatileProperties["roll"] = value;
	}

	public Entity(uint id, string name, EntityDefinition entityDefinition,
		ILogger<Entity> logger)
	{
		Id = id;
		Name = name;
		EntityDefinition = entityDefinition;
		Logger = logger;
		VolatileProperties = EntityDefinition.VolatileProperties.ToDictionary(kv => kv.Key, kv => kv.Value);

		ClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS
		                                                                  | EntityFlag.BASE_AND_CLIENT
		                                                                  | EntityFlag.OTHER_CLIENTS
		                                                                  | EntityFlag.OWN_CLIENT
		                                                                  | EntityFlag.CELL_PUBLIC_AND_OWN, true
		);

		InternalClientPropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.ALL_CLIENTS
			| EntityFlag.OTHER_CLIENTS
			| EntityFlag.OWN_CLIENT
			| EntityFlag.CELL_PUBLIC_AND_OWN
		);

		CellPropertyDefinitions =
			EntityDefinition.GetPropertiesByFlags(EntityFlag.CELL_PUBLIC_AND_OWN | EntityFlag.CELL_PUBLIC);

		BasePropertyDefinitions = EntityDefinition.GetPropertiesByFlags(EntityFlag.BASE_AND_CLIENT);
	}

	/// <summary>
	/// Gets the name of a client property by its index.
	/// </summary>
	/// <param name="index">Index of the client property.</param>
	/// <returns>Name of the client property.</returns>
	public string? GetClientPropertyName(int index) => ClientPropertyDefinitions.ElementAtOrDefault(index)?.Name;

	/// <summary>
	/// Gets the name of a client method by its index.
	/// </summary>
	/// <param name="index">Index of the client method.</param>
	/// <returns>Name of the client method.</returns>
	public string? GetClientMethodName(uint index) => MethodDefinitions.ElementAtOrDefault((int)index)?.Name;

	/// <summary>
	/// Calls a client method.
	/// </summary>
	/// <param name="index">Index of the client method.</param>
	/// <param name="packetTime">Time of the packet.</param>
	/// <param name="reader">Packet binary reader.</param>
	/// <param name="subscriptionTarget">Subscription target (Shouldn't be null).</param>
	public virtual void CallClientMethod(uint index, float packetTime, BinaryReader reader,
		IReplayController subscriptionTarget)
	{
		EntityMethodDefinition? methodDefinition = MethodDefinitions.ElementAtOrDefault((int)index);

		if (methodDefinition is null)
		{
			Logger.LogError("Method with index {Index} was not found on entity with name {Name} ({Id})", index, Name,
				Id);

			return;
		}

		string hash = $"{Name}_{methodDefinition.Name}";


		Dictionary<string, object?> arguments =
			methodDefinition.Arguments.ToDictionary(a => a.Name, a => a.GetValue(reader));

		try
		{
			subscriptionTarget.CallSubscription(hash, this, packetTime, arguments);
		}
		catch (Exception exception)
		{
			if (exception is CveSecurityException)
			{
				throw;
			}
			
			if (exception is ArgumentOutOfRangeException or InvalidCastException)
			{
				var expectedParameters = methodDefinition.Arguments.Select(a => new { Type = a.DataType.ClrType, a.Name })
					.ToArray();

				Logger.LogError(exception, """
				                           Arguments of method definition and method subscription do not match
				                           Entity Name: {entityName}
				                           Method Name: {methodName}
				                           Expected Parameters: {expectedParameters}
				                           """,
					Name,
					methodDefinition.Name,
					string.Join(", ", expectedParameters.Select(t => $"{t.Type.Name} {t.Name}")));
			}
			else
			{
				Logger.LogError(exception, "Error when calling method subscription with hash {Hash}", hash);
			}
		}
	}

	/// <summary>
	/// Sets a client property's value by its exposed index.
	/// </summary>
	/// <param name="index">Exposed index of the property.</param>
	/// <param name="reader">Binary reader to read the value from.</param>
	/// <param name="subscriptionTarget">Target object to set the property on.</param>
	public virtual void SetClientProperty(uint index, BinaryReader reader, IReplayController subscriptionTarget)
	{
		Logger.LogDebug("Setting client property with index {Index} on entity {Name} ({Id})", index, Name, Id);
		PropertyDefinition propertyDefinition = ClientPropertyDefinitions[index];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;

		string hash = $"{Name}_{propertyDefinition.Name}";
		try
		{
			subscriptionTarget.PropertyChanged(hash, this, propertyValue);
		}
		catch (Exception exception)
		{
			if (exception is CveSecurityException)
			{
				throw;
			}

			if (exception is InvalidCastException)
			{
				Logger.LogError(exception, """
				                           Parameter type of property changed subscription does not match
				                           Entity Name: {entityName}
				                           Method Name: {methodName}
				                           Expected Parameter: {expectedParameter}
				                           """,
					Name,
					propertyDefinition.Name, propertyDefinition.DataType.ClrType);
			}
			else
			{
				Logger.LogError(exception, "Error when calling property subscription with hash {Hash}", hash);
			}
		}
	}

	/// <summary>
	/// Sets an internal client property's value by its internal index.
	/// </summary>
	/// <param name="index">Internal index of the property.</param>
	/// <param name="reader">Binary reader to read the value from.</param>
	public virtual void SetInternalClientProperty(int index, BinaryReader reader)
	{
		Logger.LogDebug("Setting internal client property with index {Index} on entity {Name} ({Id})", index, Name, Id);
		PropertyDefinition propertyDefinition = InternalClientPropertyDefinitions[index];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		ClientProperties[propertyDefinition.Name] = propertyValue;
	}

	/// <summary>
	/// Sets a cell property's value by its internal index.
	/// </summary>
	/// <param name="index">Internal index of the property.</param>
	/// <param name="reader">Binary reader to read the value from.</param>
	public virtual void SetCellProperty(int index, BinaryReader reader)
	{
		Logger.LogDebug("Setting cell property with index {Index} on entity {Name} ({Id})", index, Name, Id);
		PropertyDefinition propertyDefinition = CellPropertyDefinitions[index];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		CellProperties[propertyDefinition.Name] = propertyValue;
	}

	/// <summary>
	/// Sets a base property's value by its internal index.
	/// </summary>
	/// <param name="index">Internal index of the property.</param>
	/// <param name="reader">Binary reader to read the value from.</param>
	public virtual void SetBaseProperty(int index, BinaryReader reader)
	{
		Logger.LogDebug("Setting base property with index {Index} on entity {Name} ({Id})", index, Name, Id);
		PropertyDefinition propertyDefinition = BasePropertyDefinitions[index];
		object? propertyValue = propertyDefinition.GetValue(reader, propertyDefinition.XmlNode);
		BaseProperties[propertyDefinition.Name] = propertyValue;
	}

	/// <summary>
	/// Sets multiple base properties from a binary reader.
	/// </summary>
	/// <remarks>
	///	This method expects the binary reader to contain the base properties
	/// and their values in the same order as the base property definitions.
	/// </remarks>
	/// <param name="reader">Binary reader to read the values from.</param>
	public virtual void SetBaseProperties(BinaryReader reader)
	{
		for (int i = 0; i < BasePropertyDefinitions.Length; i++)
		{
			SetBaseProperty(i, reader);
		}
	}

	/// <summary>
	/// Sets multiple cell properties from a binary reader.
	/// </summary>
	/// <remarks>
	/// This method expects the binary reader to contain the internal client properties
	/// and their values in the same order as the internal client property definitions.
	/// </remarks>
	/// <param name="reader">Binary reader to read the values from.</param>
	public virtual void SetInternalClientProperties(BinaryReader reader)
	{
		for (int i = 0; i < InternalClientPropertyDefinitions.Length; i++)
		{
			SetInternalClientProperty(i, reader);
		}
	}

	/// <summary>
	/// Sets the position (Coordinates and PYR) of the entity.
	/// </summary>
	/// <param name="position">Position to set.</param>
	public void SetPosition(PositionContainer position)
	{
		VPosition = position.Position;

		(Pitch, Yaw, Roll) = (position.Pitch, position.Yaw, position.Roll);
	}

	/// <summary>
	/// Gets the Position (Coordinates and PYR) of the entity.
	/// </summary>
	/// <returns>Position of the entity, as a <see cref="PositionContainer"/> object.</returns>
	public PositionContainer GetPosition() => new(VPosition, Yaw, Pitch, Roll);

	/// <summary>
	/// Gets the string representation of the entity.
	/// </summary>
	/// <returns>String representation of the entity.</returns>
	public override string ToString() => $"{Name} <{Id}>";
}