using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Nodsoft.WowsReplaysUnpack.Generators;

public static class Extensions
{
	public static bool GetBoolValue(this ImmutableDictionary<string, TypedConstant> dictionary, string name)
	{
		if (dictionary.TryGetValue(name, out TypedConstant constant) && constant.Value is bool value)
		{
			return value;
		}

		return false;
	}
	
	public static int? GetIntValue(this ImmutableDictionary<string, TypedConstant> dictionary, string name)
	{
		if (dictionary.TryGetValue(name, out TypedConstant constant) && constant.Value is int value)
		{
			return value;
		}

		return null;
	}
	
	public static string? GetStringValue(this ImmutableDictionary<string, TypedConstant> dictionary, string name)
	{
		if (dictionary.TryGetValue(name, out TypedConstant constant) && constant.Value is string value)
		{
			return value;
		}

		return null;
	}

	public static string GetTypeName(this IParameterSymbol parameterSymbol) => parameterSymbol.Type.GetTypeName();

	public static string GetTypeName(this ITypeSymbol typeSymbol)
	{
		if (typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "Nullable" } namedTypeSymbol)
		{
			return namedTypeSymbol.TypeArguments[0].Name + "?";
		}

		return typeSymbol.Name;
	}
}