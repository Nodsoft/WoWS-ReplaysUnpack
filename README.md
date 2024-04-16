
<img align="right" src="logo.png" alt="logo" width="200"/>

# WoWS-ReplaysUnpack
A C# file unpacking library for World of Warships Replays, inspired by Monstrofil's [replays_unpack](https://github.com/Monstrofil/replays_unpack/).

## Information before using the library
The library supports only World of Warships replays starting with version 0.10.10. 
Trying to use an older replay can result in unexpected errors when processing the replay.

## How to install
Install  [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget)

Then, install from the package manager console:

```
PM> Install-Package Nodsoft.WowsReplaysUnpack
```
Or from the .NET CLI as:
```
dotnet add package Nodsoft.WowsReplaysUnpack
```

## How to use
Add the service to an `IServiceCollection`
```csharp
services.AddWowsReplayUnpacker();
```
Get the factory with DI, get the `IReplayUnpackerService` from the factory and call the `Unpack` method with either a `Stream` or `byte[]`
```csharp
ReplayUnpackerFactory replayUnpackerFactory = serviceProvider.GetService<IReplayUnpackerService>();
UnpackedReplay unpackedReplay = replayUnpackerFactory
	.GetUnpacker()
	.Unpack(File.OpenRead("my-replay.wowsreplay"));
```
## Custom Implementations
You can provide custom implementations of certain services.
```csharp
services.Snippet.AddWowsReplayUnpacker(builder =>
{
	builder.AddReplayController<MyCustomReplayController, MyCustomReplay>();
	builder.WithReplayDataParser<MyCustomReplayDataParser>();
	builder.WithDefinitionLoader<MyCustomDefinitionLoader>();
	builder.WithDefinitionStore<MyCustomDefinitionStore>();
})
```
### DefinitionStore
Responsible for managing, giving access and caching the `.def` files (used for type and property mapping).
Uses the `IDefinitionLoader` for resolving non-cached files once.

Your custom definition store has to implement `IDefinitionStore` or extend `DefaultDefinitionStore`

### DefinitionLoader
Responsible for loading the actual definition files.

Your custom definition store has to implement `IDefinitionLoader`.
The default loader is the [AssemblyDefinitionLoader](Nodsoft.WowsReplaysUnpack.Core/Definitions/AssemblyDefinitionLoader.cs).

You can optionally use the [FileSystemDefinitionLoader](Nodsoft.WowsReplaysUnpack.FileStore/Definitions/FileSystemDefinitionLoader.cs) 
by installing the `Nodsoft.WowsReplaysUnpack.FileStore` nuget package.

### ReplayDataParser
Responsible for parsing the binary packets to the specific [network packets](Nodsoft.WowsReplaysUnpack.Core/Network/Packets).

Your custom replay data parser has to implement `IReplayDataParser` or extend `DefaultReplayParser`

### ReplayController
Responsible for handling parsed network packets and filling the UnpackedReplay with information.

Your custom replay controller has to implement `IReplayController` but it is strongly suggested 
to use `ReplayControllerBase<T>` where T is your custom replay class.
An example of this is the [ExtendedDataController](Nodsoft.WowsReplaysUnpack.ExtendedData/ExtendedDataController.cs).

To use your custom controller add the replay type to the `GetUnpacker()` method.
```csharp
UnpackedReplay unpackedReplay = replayUnpackerFactory
	.GetUnpacker<MyCustomReplay>()
	.Unpack(File.OpenRead("my-replay.wowsreplay"));
```
**CVE Check Only Implementation**

In the library you get a custom implementation ready to use for when you only want to check the CVE .
`CveCheckOnlyController`

It skips all network packets except the affected ones.

You can add it with the `AddCveCheckController()` method and get the unpacker with `GetCveCheckUnpacker()`

**Extend the replay data**

When implementing your own controller and extending `ReplayControllerBase<T>`;
The replay class has to extend `UnpackedReplay`. 
That way you can add extra properties.

You can see this in action [here](Nodsoft.WowsReplaysUnpack.ExtendedData/ExtendedDataController.cs)

### Method/Property Subscriptions

When implementing your own controller and extending `ReplayControllerBase<T>` you can subscribe to `EntityMethods` and `EntityProperty` calls by adding a method with an attribute.

You will have to install the `Nodsoft.WowsReplaysUnpack.Generators` nuget package and add the `[ReplayController]` attribute to your controller class.
This will generate the required logic to make the dynamic subscriptions work.

#### Methods
`MethodSubscription("EntityName", "MethodName")`

You have a few extra properties on the attribute to configure how the method will be called:
`bool IncludeEntity` => When `true` it will include the `Entity entity` parameter

`bool IncludePacketTime` => When `true` it will include the `float packetTime` parameter

`bool ParamsAsDictionary` => When `true` the last paremeter will be `Dictionary<string, object?> arguments` / When `false` the parameters have to match the actual packet parameters in order and type exactly. When they don't match you will get an exception telling you the required parameters.

`bool Priority` => Defines the order in which methods are called when you have multiple subscriptions on the same method. Smaller = Earlier. Don't use -1.

Example:
```csharp
[MethodSubscription("Avatar", "onArenaStateReceived")]
public void OnArenaStateReceived(Entity entity, float packetTime, ...)
{
}
```

#### Properties
`PropertySubscription("EntityName", "PropertyName")`

There are no extra properties available and the `Entity entity` parameter is always there.

Example:

```csharp
[PropertySubscription("Avatar", "selectedWeapon")]
public void SelectedWeaponChanged(Entity entity, uint selectedWeaponId)
{
}
```

## ExtendedData Library
You can install the `Nodsoft.WowsReplaysUnpack.ExtendedData` package from nuget to get a ready to use implementation that fills the replay with more information than the default controller.

Currently included in the `ExtendedDataReplay`:

 - Player Information
 - Chat Messages

### How to use
```csharp
services.AddWowsReplayUnpacker(builder =>
{
	builder.AddExtendedData();
});

ExtendedDataReplay unpackedReplay = replayUnpackerFactory
	.GetExtendedDataUnpacker()
	.Unpack(File.OpenRead("my-replay.wowsreplay"));
```

## Additional Entities
The replay contains a multitude of other entities. If you want to retreive those you have two convenient options we provide.

### Option 1 - Extension Methods

```csharp
// Step 1 - Retreive the entity properties you're interested in
var battleLogicProperties = replay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.Properties;
// Step 2 - Use extension methods to cast the properties to their actual type
battleLogicProperties.GetAsDict("propertyName");
battleLogicProperties.GetAsArr("propertyName");
battleLogicProperties.GetAsValue<short>("propertyName");
```

An example of this can be seen [here](Nodsoft.WowsReplaysUnpack.Console/Samples/EntitySerializer/EntitySerializerSample.cs) in the `ManualExtensions` method.

### Option 2 - Strong Type Serializing

Requires the `Nodsoft.WowsReplaysUnpack.Generators` nuget package.

```csharp
// Step 1 - Create a class representing the entity annotated with the SerializableEntity attribute
[SerializableEntity]
public class BattleLogic {
  [DataMember(Name = "state")]
  public State State { get; set; } = null!;
    
    ...
}

// Step 2 - Use extension method on the replay to deserialize the entity
var battleLogic = replay.DeserializeEntity<BattleLogic>("BattleLogic");
```

For collections only `List<T>` is currently supported. 

The property mapping is case-sensitive. So you either have to name your properties exactly like they are in the entities properties dictionary or use the `DataMember` attribute.

An example of this can be seen [here](Nodsoft.WowsReplaysUnpack.Console/Samples/EntitySerializer/EntitySerializerSample.cs) in the `Serializer` method.