using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

[Generator]
public class ReplayControllerGenerator : IIncrementalGenerator
{
	private const string ReplayControllerAttribute =
		"Nodsoft.WowsReplaysUnpack.Generators.ReplayControllerAttribute";

	private const string MethodSubscriptionAttribute =
		"Nodsoft.WowsReplaysUnpack.Core.Entities.MethodSubscriptionAttribute";
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"ReplayControllerAttribute.g.cs",
			SourceText.From(SourceGenerationHelper.ReplayControllerAttribute, Encoding.UTF8)));

		IncrementalValuesProvider<ControllerToGenerate?> provider = context.SyntaxProvider
			.ForAttributeWithMetadataName(ReplayControllerAttribute,
				predicate: (node, _) => node is ClassDeclarationSyntax,
				transform: Transform)
			.WithTrackingName(TrackingNames.InitialExtraction)
			.Where(static c => c is not null)
			.WithTrackingName(TrackingNames.RemovingNulls);

		//context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
		context.RegisterSourceOutput(provider, (ctx, controllerInput) =>
		{
			if (!controllerInput.HasValue)
			{
				return;
			}

			ctx.AddSource(controllerInput.Value.Name + ".g.cs",
				ReplayControllerSourceWriter.Generate(controllerInput.Value));
		});
	}

	private static ControllerToGenerate? Transform(GeneratorAttributeSyntaxContext context,
		CancellationToken cancellationToken)
	{
		if (context.TargetSymbol is not INamedTypeSymbol symbol ||
		    context.TargetNode is not ClassDeclarationSyntax node || !node.Modifiers.Any(t => t.Text is "partial"))
		{
			return null;
		}

		List<MethodSubscriptionData> methodSubscriptions = ExtractMethodSubscriptions(symbol);
		List<PropertyChangedSubscriptionData> propertyChangedSubscriptions = ExtractPropertyChangedSubscriptions(symbol);

		if (methodSubscriptions.Count is 0 && propertyChangedSubscriptions.Count is 0)
		{
			return null;
		}

		string? fullName = symbol.ToString();
		return new ControllerToGenerate(symbol.Name, fullName.Substring(fullName.LastIndexOf('.') + 1), fullName,
			symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToString(),
			[..methodSubscriptions], [..propertyChangedSubscriptions]);
	}

	private static List<MethodSubscriptionData> ExtractMethodSubscriptions(INamedTypeSymbol symbol)
	{
		List<MethodSubscriptionData> subscriptions = new();
		foreach (ISymbol? member in symbol.GetMembers())
		{
			if (member is not IMethodSymbol method)
			{
				continue;
			}

			ImmutableArray<AttributeData> attributes = method.GetAttributes();
			foreach (AttributeData? attribute in attributes)
			{
				if (attribute.AttributeClass?.ToString() is not MethodSubscriptionAttribute)
				{
					continue;
				}

				string entityName = (string)attribute.ConstructorArguments[0].Value!;
				string methodName = (string)attribute.ConstructorArguments[1].Value!;

				ImmutableDictionary<string, TypedConstant> arguments = attribute.NamedArguments.ToImmutableDictionary();
				bool paramsAsDict = arguments.GetBoolValue("ParamsAsDictionary");
				bool includeEntity = arguments.GetBoolValue("IncludeEntity");
				bool includePacketTime = arguments.GetBoolValue("IncludePacketTime");
				int? priority = arguments.GetIntValue("Priority");

				int skipParams = includeEntity && includePacketTime ? 2 : includeEntity | includePacketTime ? 1 : 0;
				string[] parameters = method.Parameters
					.Skip(skipParams).Select(Extensions.GetTypeName)
					.ToArray();

				subscriptions.Add(new(method.Name, entityName, methodName, paramsAsDict, includeEntity,
					includePacketTime,
					priority, parameters));
			}
		}

		return subscriptions;
	}

	private static List<PropertyChangedSubscriptionData> ExtractPropertyChangedSubscriptions(INamedTypeSymbol symbol)
	{
		List<PropertyChangedSubscriptionData> subscriptions = new();
		foreach (ISymbol? member in symbol.GetMembers())
		{
			if (member is not IMethodSymbol method)
			{
				continue;
			}

			ImmutableArray<AttributeData> attributes = method.GetAttributes();
			foreach (AttributeData? attribute in attributes)
			{
				string fullName = attribute.AttributeClass!.ContainingNamespace + "." + attribute.AttributeClass.Name;
				if (fullName is not "Nodsoft.WowsReplaysUnpack.Core.Entities.PropertyChangedSubscriptionAttribute")
				{
					continue;
				}

				string entityName = (string)attribute.ConstructorArguments[0].Value!;
				string propertyName = (string)attribute.ConstructorArguments[1].Value!;
				subscriptions.Add(new(method.Name, entityName, propertyName, method.Parameters[1].GetTypeName()));
			}
		}

		return subscriptions;
	}
}