using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

[JsonSerializable(typeof(ArenaInfo))]
[JsonSerializable(typeof(JsonElement?))]
public partial class UnpackerJsonSerializerContext : JsonSerializerContext
{
}