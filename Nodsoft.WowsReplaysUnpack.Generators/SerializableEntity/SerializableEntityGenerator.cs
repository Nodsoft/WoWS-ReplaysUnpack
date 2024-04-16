using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nodsoft.WowsReplaysUnpack.Generators.SerializableEntity;

[Generator]
public class SerializableEntityGenerator : IIncrementalGenerator
{
	private const string SerializableEntityAttribute =
		"Nodsoft.WowsReplaysUnpack.Generators.SerializableEntityAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"SerializableEntityAttribute.g.cs",
			SourceText.From(SourceGenerationHelper.SerializableEntityAttribute, Encoding.UTF8)));

		IncrementalValuesProvider<EntityToGenerate?> provider = context.SyntaxProvider
			.ForAttributeWithMetadataName(SerializableEntityAttribute,
				predicate: (node, _) => node is ClassDeclarationSyntax,
				transform: Transform)
			.WithTrackingName(TrackingNames.InitialExtraction)
			.Where(static c => c is not null)
			.WithTrackingName(TrackingNames.RemovingNulls);

		//context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
		context.RegisterSourceOutput(provider, (ctx, entityInput) =>
		{
			if (!entityInput.HasValue)
			{
				return;
			}


			ctx.AddSource(entityInput.Value.ClassName + ".g.cs",
				SerializableEntitySourceWriter.Generate(entityInput.Value));
		});
	}

	private static EntityToGenerate? Transform(GeneratorAttributeSyntaxContext context,
		CancellationToken cancellationToken)
	{
		if (context.TargetSymbol is not INamedTypeSymbol symbol ||
		    context.TargetNode is not ClassDeclarationSyntax node || !node.Modifiers.Any(t => t.Text is "partial"))
		{
			return null;
		}

		PropertyData[] properties = GetPropertiesRecursive(symbol, 0).ToArray();
		return new EntityToGenerate(symbol.Name, symbol.ContainingNamespace.Name, properties);
	}

	private static IEnumerable<PropertyData> GetPropertiesRecursive(INamedTypeSymbol symbol,
		int depth,
		string? parentPath = null,
		string? parentMappedPath = null,
		int? listLevel = null,
		bool requiresIndex = false)
	{
		if (depth > 15)
		{
			yield break;
		}

		foreach (ISymbol? member in symbol.GetMembers())
		{
			if (member is not IPropertySymbol propertySymbol)
			{
				continue;
			}

			string mappedPropertyPath =
				GetMappedPropertyPath(parentMappedPath, listLevel, requiresIndex, propertySymbol);
			string propertyPath = GetPropertyPath(parentPath, propertySymbol);

			if (propertySymbol.Type is INamedTypeSymbol { IsReferenceType: true } propertyTypeSymbol)
			{
				if (propertyTypeSymbol is { IsGenericType: true, Name: "List" })
				{
					yield return new PropertyData(propertySymbol.Name, propertyPath, mappedPropertyPath, true,
						propertyTypeSymbol.Name, listLevel);

					if (propertyTypeSymbol.TypeArguments[0] is INamedTypeSymbol { IsReferenceType: true } itemSymbol)
					{
						yield return new PropertyData(propertySymbol.Name, propertyPath + ".#Add", mappedPropertyPath,
							true,
							itemSymbol.Name, listLevel, true);

						foreach (PropertyData child in GetPropertiesRecursive(itemSymbol, depth++, propertyPath,
							         mappedPropertyPath,
							         (listLevel ?? 0) + 1, true))
						{
							yield return child;
						}
					}
					else
					{
						yield return new PropertyData(propertySymbol.Name, propertyPath + ".#Add", mappedPropertyPath,
							false,
							propertyTypeSymbol.TypeArguments[0].GetTypeName(), listLevel, true);
					}
				}
				else
				{
					yield return new PropertyData(propertySymbol.Name, propertyPath, mappedPropertyPath, true,
						propertyTypeSymbol.Name, listLevel);

					foreach (PropertyData child in GetPropertiesRecursive(propertyTypeSymbol, depth++, propertyPath,
						         mappedPropertyPath))
					{
						yield return child;
					}
				}
			}
			else
			{
				yield return new PropertyData(propertySymbol.Name, propertyPath, mappedPropertyPath, false,
					propertySymbol.Type.GetTypeName(), listLevel);
			}
		}
	}

	private static string GetPropertyPath(string? parentPath, ISymbol propertySymbol)
	{
		string propertyPath = string.IsNullOrEmpty(parentPath)
			? propertySymbol.Name
			: parentPath + "." + propertySymbol.Name;
		return propertyPath;
	}

	private static string GetMappedPropertyPath(string? parentMappedPath, int? listLevel, bool requiresIndex,
		ISymbol propertySymbol)
	{
		string mappedPropertyPath;
		if (requiresIndex)
		{
			mappedPropertyPath = parentMappedPath + $"[indexes[{listLevel - 1}]]." + propertySymbol.Name;
		}
		else
		{
			mappedPropertyPath = string.IsNullOrEmpty(parentMappedPath)
				? propertySymbol.Name
				: parentMappedPath + "." + propertySymbol.Name;
		}

		return mappedPropertyPath;
	}
}