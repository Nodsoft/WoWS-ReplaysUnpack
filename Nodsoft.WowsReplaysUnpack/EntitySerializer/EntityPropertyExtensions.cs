using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.EntitySerializer;

[PublicAPI]
public static class EntityPropertyExtensions
{
	public static FixedDictionary? GetAsDict(this Dictionary<string, object?> dict, string key) =>
		dict.TryGetValue(key, out object? value) && value is FixedDictionary fixedDict ? fixedDict : null;

	public static FixedList? GetAsArr(this Dictionary<string, object?> dict, string key) =>
		dict.TryGetValue(key, out object? value) && value is FixedList fixedList ? fixedList : null;

	public static FixedDictionary? GetAsDict(this FixedList list, int index) =>
		list.Length > index && list[index] is FixedDictionary fixedDict ? fixedDict : null;

	public static T? GetAsValue<T>(this FixedDictionary dict, string key) =>
		dict.TryGetValue(key, out object? rawValue) && rawValue is T value ? value : default;
}