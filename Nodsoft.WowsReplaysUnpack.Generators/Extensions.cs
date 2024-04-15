using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Nodsoft.WowsReplaysUnpack.Generators;

public static class Extensions
{
	public static bool GetBoolValue(this ImmutableDictionary<string, TypedConstant> dictionary, string name)
	{
		if (dictionary.TryGetValue(name, out var constant) && constant.Value is bool value)
			return value;

		return false;
	}
	
	public static int? GetIntValue(this ImmutableDictionary<string, TypedConstant> dictionary, string name)
	{
		if (dictionary.TryGetValue(name, out var constant) && constant.Value is int value)
			return value;

		return null;
	}

	public static string GetTypeName(this IParameterSymbol parameterSymbol)
	{
		if (parameterSymbol.Type is INamedTypeSymbol { IsGenericType: true, Name: "Nullable" } namedTypeSymbol)
		{
			return namedTypeSymbol.TypeArguments[0].Name + "?";
		}

		return parameterSymbol.Type.Name;
	}
}