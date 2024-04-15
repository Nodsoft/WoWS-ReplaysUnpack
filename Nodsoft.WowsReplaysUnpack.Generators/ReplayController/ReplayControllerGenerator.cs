using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

[Generator]
public class ReplayControllerGenerator : IIncrementalGenerator
{
	private const string ReplayControllerAttribute =
		"Nodsoft.WowsReplaysUnpack.Generators.ReplayControllerAttribute";

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
				return;

			ctx.AddSource(controllerInput.Value.Name + ".g.cs",
				ReplayControllerSourceWriter.NewMethod(controllerInput.Value));
		});
	}

	static ControllerToGenerate? Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.TargetSymbol is not INamedTypeSymbol symbol ||
		    context.TargetNode is not ClassDeclarationSyntax node || !node.Modifiers.Any(t => t.Text is "partial"))
			return null;

		List<MethodSubscriptionData> subscriptions = new();
		foreach (MemberDeclarationSyntax member in node.Members)
		{
			if (member is not MethodDeclarationSyntax { AttributeLists.Count: > 0 } method)
				continue;


			foreach (AttributeSyntax attribute in method.AttributeLists[0].Attributes)
			{
				if (attribute.Name.ToString() is not "MethodSubscription")
					continue;
				
				string entityName = attribute.ArgumentList!.Arguments[0].ToString().StripQuotes();
				string methodName = attribute.ArgumentList!.Arguments[1].ToString().StripQuotes();

				bool paramsAsDict = attribute.ArgumentList.Arguments
					.Any(a => a is { Expression.RawKind: ExpressionKinds.TrueLiteral, NameEquals: not null } &&
					          a.NameEquals.Name.ToString() is "ParamsAsDictionary");

				bool includeEntity = attribute.ArgumentList.Arguments
					.Any(a => a is { Expression.RawKind: ExpressionKinds.TrueLiteral, NameEquals: not null } &&
					          a.NameEquals.Name.ToString() is "IncludeEntity");

				bool includePacketTime = attribute.ArgumentList.Arguments
					.Any(a => a is { Expression.RawKind: ExpressionKinds.TrueLiteral, NameEquals: not null } &&
					          a.NameEquals.Name.ToString() is "IncludePacketTime");

				int? priority = null;
				if (attribute.ArgumentList.Arguments
						    .FirstOrDefault(a =>
							    a is
							    {
								    Expression.RawKind: ExpressionKinds.Unary or ExpressionKinds.MinusUnary,
								    NameEquals: not null
							    } &&
							    a.NameEquals.Name.ToString() is "Priority") is
					    { } priorityArgument && int.TryParse(priorityArgument.Expression.ToString(), out int priorityRaw))
				{
					priority = priorityRaw;
				}

				int skipParams = includeEntity && includePacketTime ? 2 : includeEntity | includePacketTime ? 1 : 0;
				string[] parameters = method.ParameterList.Parameters
					.Where(p => p.Type is not null).Skip(skipParams).Select(p => p.Type!.ToString())
					.ToArray();
				
				subscriptions.Add(new(method.Identifier.Text, entityName, methodName, paramsAsDict, includeEntity,
					includePacketTime,
					priority, parameters));
			}
		}

		if (subscriptions.Count <= 0)
			return null;

		string? fullName = symbol.ToString();
		return new ControllerToGenerate(symbol.Name, fullName.Substring(fullName.LastIndexOf('.') + 1), fullName,
			symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToString(),
			[..subscriptions]);
	}
}