using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Macios.Generator;
using Microsoft.Macios.Transformer.Generator.Attributes;


namespace Microsoft.Macios.Transformer.Generator;

/// <summary>
/// Generator that will write the needed code to parse the old Xamarin API in the transformer. The
/// idea is that there is a lot of code that is needed by the tansformer that can be generated.
/// </summary>
[Generator]
public class XamarinBindingAPIGenerator : IIncrementalGenerator {
	const string Namespace = "Microsoft.Macios.Transformer.Generator";

	public void Initialize (IncrementalGeneratorInitializationContext context)
	{
		// Add the marker attribute to the compilation.
		context.RegisterPostInitializationOutput (ctx => ctx.AddSource (
			"BindingFlagAttribute.g.cs",
			SourceText.From (BindingFlagData.Source, Encoding.UTF8)));

		context.RegisterPostInitializationOutput (ctx => ctx.AddSource (
			"BindingAttributeAttribute.g.cs",
			SourceText.From (BindingAttributeData.Source, Encoding.UTF8)));

		// Filter the [BindingFlagAttribute] annotate fields.
		var provider = context.SyntaxProvider
			.CreateSyntaxProvider (
				(s, _) => s is FieldDeclarationSyntax,
				(ctx, _) => GetClassDeclarationForSourceGen (ctx))
			.Where (t => t.attrType != AttributeType.None);

		// Generate the source code.
		context.RegisterSourceOutput (context.CompilationProvider.Combine (provider.Collect ()),
			((ctx, t) => GenerateCode (ctx, t.Left, t.Right)));
	}

	static (FieldDeclarationSyntax, AttributeType attrType) GetClassDeclarationForSourceGen (
		GeneratorSyntaxContext context)
	{
		var declarationSyntax = Unsafe.As<FieldDeclarationSyntax> (context.Node);

		// Go through all attributes of the field
		foreach (AttributeListSyntax attributeListSyntax in declarationSyntax.AttributeLists) {
			foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes) {
				if (context.SemanticModel.GetSymbolInfo (attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
					continue; // if we can't get the symbol, ignore it

				string attributeName = attributeSymbol.ContainingType.ToDisplayString ();

				if (attributeName == $"{Namespace}.{BindingFlagData.Name}")
					return (declarationSyntax, AttributeType.Flag);
				if (attributeName == $"{Namespace}.{BindingAttributeData.Name}")
					return (declarationSyntax, AttributeType.Data);
			}
		}

		return (declarationSyntax, AttributeType.None);
	}

	static string [] GetFlagsForTarget (Dictionary<string, (string AttributeFullName, AttributeTargets Targets)> flags,
		AttributeTargets[] targets)
		=> flags.Where (kv => targets.Any (t => kv.Value.Targets.HasFlag (t)))
			.Select (kv => kv.Key)
			.ToArray ();

	static (string AttributeFullName, string AttributeName, BindingAttributeData Data) [] GetAttributesForTarget (
		Dictionary<string, (string AttributeFullName, string AttributeName, BindingAttributeData Data)> dataAttribute,
		AttributeTargets [] targets)
		// return all the attributes that have at least one of the targets
		=> dataAttribute.Where (kv => targets.Any (t => kv.Value.Data.Target.HasFlag (t)))
			.Select (kv => kv.Value)
			.ToArray ();
	

	static void WriteFlagProperty (TabbedStringBuilder sb, string flagName)
	{
		// write the backing field
		sb.AppendLine ($"readonly bool _{flagName} = false;");
		using (var flagPropertyBlock = sb.CreateBlock ($"public bool {flagName}", block: true)) {
			flagPropertyBlock.AppendLine ($"get => _{flagName};");
			flagPropertyBlock.AppendLine ($"private init => _{flagName} = value;");
		}
	}

	static void WriteAttributeProperty (TabbedStringBuilder sb,
		(string AttributeFullName, string AttributeName, BindingAttributeData Data) attrData)
	{
		// add a property that will state if we have the attr, this will help with nullability
		sb.AppendLine($"readonly bool _has{attrData.AttributeName} = false;");
		sb.AppendLine ($"[MemberNotNullWhen (true, nameof ({attrData.AttributeName}))]");
		using (var flagPropertyBlock = sb.CreateBlock ($"public bool Has{attrData.AttributeName}", block: true)) {
			flagPropertyBlock.AppendLine ($"get => _has{attrData.AttributeName};");
			flagPropertyBlock.AppendLine ($"private init => _has{attrData.AttributeName} = value;");
		}
		sb.AppendLine ();	
		sb.AppendLine($"readonly {attrData.Data.DataModelType}? _{attrData.AttributeName} = null;");	
		// decorate to help with nullability
		using (var attributePropertyBlock = sb.CreateBlock ($"public {attrData.Data.DataModelType}? {attrData.AttributeName}", block: true)) {
			attributePropertyBlock.AppendLine ($"get => _{attrData.AttributeName};");
			attributePropertyBlock.AppendLine ($"private init => _{attrData.AttributeName} = value;");
		}
	}

	static void WriteDataModelExtension (TabbedStringBuilder sb, string dataModel, string [] flags, 
		(string AttributeFullName, string AttributeName, BindingAttributeData Data)[] attributes)
	{
		sb.Clear ();
		sb.AppendLine ("// <auto-generated/>");
		sb.AppendLine ("#nullable enable");
		sb.AppendLine ("");
		sb.AppendLine ("using System;");
		sb.AppendLine ("using System.Diagnostics.CodeAnalysis;");
		sb.AppendLine ("using Microsoft.CodeAnalysis;");
		sb.AppendLine ("using Microsoft.Macios.Transformer.Extensions;");
		sb.AppendLine ();
		sb.AppendLine ("namespace Microsoft.Macios.Generator.DataModel;");
		sb.AppendLine ();
		using (var modelBlock = sb.CreateBlock ($"readonly partial struct {dataModel}", block: true)) {
			// add all the properties that will allow us to get the flags make them public getter and private init
			// so that they can be set when we add the attr dict.
			foreach (var flag in flags) {
				modelBlock.AppendLine ();
				WriteFlagProperty (modelBlock, flag);
			}

			foreach (var attrData in attributes) {
				modelBlock.AppendLine ();
				WriteAttributeProperty (modelBlock, attrData);
			}

			// property to store the dictionary
			modelBlock.AppendLine ();
			modelBlock.AppendLine ("readonly Dictionary<string, List<AttributeData>>? _attributesDictionary = null;");
			using (var dictionaryPropertyBlock =
			       modelBlock.CreateBlock ("public Dictionary<string, List<AttributeData>>? AttributesDictionary",
				       block: true)) {
				dictionaryPropertyBlock.AppendLine ("get => _attributesDictionary;");
				using (var initBlock = dictionaryPropertyBlock.CreateBlock ("private init", block: true)) {
					initBlock.AppendLine ("_attributesDictionary = value;");
					using (var ifBlock = initBlock.CreateBlock ("if (_attributesDictionary is not null)", block: true)) {
						foreach (var flag in flags) {
							ifBlock.AppendLine ($"{flag} = _attributesDictionary.{flag} ();");
						}

						foreach (var attributeData in attributes) {
							// check if the attribute is present, if it is, set the value
							ifBlock.AppendLine($"Has{attributeData.AttributeName} = _attributesDictionary.Has{attributeData.AttributeName} ();");
							using (var attrIfBlock = ifBlock.CreateBlock ($"if (Has{attributeData.AttributeName})", block: true)) {
								attrIfBlock.AppendLine ($"{attributeData.AttributeName} = _attributesDictionary.Get{attributeData.AttributeName} ();");
							}
						}
					}
				}
			}
		}
	}

	static void GenerateModelExtension (TabbedStringBuilder sb, string dataModel,
		Dictionary<string, (string AttributeFullName, AttributeTargets Targets)> flags, 
		Dictionary<string, (string AttributeFullName, string AttributeName, BindingAttributeData Data)> attributes, 
		AttributeTargets[] targets,
		SourceProductionContext context)
	{
		var methodFlags = GetFlagsForTarget (flags, targets);
		var methodAttributes = GetAttributesForTarget (attributes, targets);
		WriteDataModelExtension (sb, dataModel, methodFlags, methodAttributes);
		context.AddSource ($"{dataModel}.Transformer.g.cs",
			SourceText.From (sb.ToString (), Encoding.UTF8));
	}

	static AttributeTargets GetTarget (ISymbol symbol)
	{
		var attrData = symbol.GetAttributes ();
		// loop over attrs, if we find the BindingFlagAttribute, return the target
		foreach (var attr in attrData) {
			if (attr.AttributeClass?.Name == BindingFlagData.Name
			    && BindingFlagData.TryParse (attr, out var data)) {
				return data.Value.Target;
			}
		}

		return AttributeTargets.All;
	}

	static BindingAttributeData? GetAttributeData (ISymbol symbol)
	{
		var attrData = symbol.GetAttributes ();
		// loop over attrs, if we find the BindingFlagAttribute, return the target
		foreach (var attr in attrData) {
			if (attr.AttributeClass?.Name == BindingAttributeData.Name
			    && BindingAttributeData.TryParse (attr, out var data)) {
				return data;
			}
		}

		return null;
	}

	void GenerateCode (SourceProductionContext context, Compilation compilation,
		ImmutableArray<(FieldDeclarationSyntax Field, AttributeType Type)> declarations)
	{
		// loop over the fields and create two dictionaries, one for flags and one for data
		var flags = new Dictionary<string, (string AttributeFullName, AttributeTargets Targets)> ();
		var dataAttributes =
			new Dictionary<string, (string AttributeFullName, string AttributeName, BindingAttributeData Data)> ();

		foreach (var (fieldDeclarationSyntax, attrType) in declarations) {
			// We need to get semantic model of the class to retrieve metadata.
			var semanticModel = compilation.GetSemanticModel (fieldDeclarationSyntax.SyntaxTree);

			switch (attrType) {
			// the attr type will let use know what data can be retrieved from the field
			case AttributeType.Flag:
				GetFlagsFromField (fieldDeclarationSyntax, semanticModel, flags);
				break;
			case AttributeType.Data:
				GetAttributesFromField (fieldDeclarationSyntax, semanticModel, dataAttributes);
				break;
			}
		}

		// all flags are collected, generate the code
		var sb = new TabbedStringBuilder (new());
		GenerateDictionaryExtension (sb, flags, dataAttributes);

		// Add the source code to the compilation.
		context.AddSource ("AttributeDataDictionaryExtensions.g.cs",
			SourceText.From (sb.ToString (), Encoding.UTF8));

#pragma warning disable format
		// generate the extra methods for the data model, group the fields by the model type based on the target
		var models = new (string Model, AttributeTargets[] Targets) [] {
			("EnumMember", [AttributeTargets.Field]), 
			("Parameter", [AttributeTargets.Parameter]),
			("Property", [AttributeTargets.Property]),
			("Method", [AttributeTargets.Method]),
			("Binding", [AttributeTargets.Interface, AttributeTargets.Class, AttributeTargets.Enum, AttributeTargets.Struct]),
			("TypeInfo", [AttributeTargets.Parameter])
		};
#pragma warning restore format 
		
		foreach (var (model, target) in models) {
			GenerateModelExtension (sb, model, flags, dataAttributes, target, context);
		}
	}

	static void GenerateDictionaryExtension (TabbedStringBuilder sb,
		Dictionary<string, (string AttributeFullName, AttributeTargets Targets)> flags,
		Dictionary<string, (string AttributeFullName, string AttributeName, BindingAttributeData Data)> dataAttributes)
	{
		sb.AppendLine ("// <auto-generated/>");
		sb.AppendLine ("#nullable enable");
		sb.AppendLine ("using System;");
		sb.AppendLine ("using System.Diagnostics.CodeAnalysis;");
		sb.AppendLine ("using Microsoft.CodeAnalysis;");
		sb.AppendLine ();
		sb.AppendLine ("namespace Microsoft.Macios.Transformer.Extensions;");
		sb.AppendLine ();
		using (var classBlock = sb.CreateBlock ("static class AttributeDataDictionaryExtension", block: true)) {
			// loop over the flags and generate a helper static method to retrieve it from a attribute data dict
			foreach (var (methodName, attributeName) in flags) {
				using (var methodBlock = classBlock.CreateBlock (
					       $"public static bool {methodName} (this Dictionary<string, List<AttributeData>> self)",
					       block: true)) {
					methodBlock.AppendLine ($"return self.ContainsKey ({attributeName.AttributeFullName});");
				}

				classBlock.AppendLine ();
			}

			// same operation over the data attributes
			foreach (var (methodName, attributeInfo) in dataAttributes) {
				// property to check if the attribute is present
				using (var methodBlock = classBlock.CreateBlock (
					       $"public static bool {methodName} (this Dictionary<string, List<AttributeData>> self)",
					       block: true)) {
					methodBlock.AppendLine ($"return self.ContainsKey ({attributeInfo.AttributeFullName});");
				}

				classBlock.AppendLine ();
				// property to access the attribute
				using (var methodBlock = classBlock.CreateBlock (
					       $"public static {attributeInfo.Data.DataModelType}? Get{attributeInfo.AttributeName} (this Dictionary<string, List<AttributeData>> self)",
					       block: true)) {
					methodBlock.AppendRaw (
$@"if (self.{methodName} ()) {{
	var data = self.GetAttribute<{attributeInfo.Data.DataModelType}> ({attributeInfo.AttributeFullName}, {attributeInfo.Data.DataModelType}.TryParse);
	return data;
}} else {{
	return null;
}}
");
				}

				classBlock.AppendLine ();
			}

			// add a generic method that will allow use to retrieve an attribute type
			classBlock.AppendRaw (
@"public delegate bool TryParseDelegate<T> (AttributeData attributeData, [NotNullWhen (true)] out T? data) where T : struct;

public static T? GetAttribute<T> (this Dictionary<string, List<AttributeData>> self, string attrName, TryParseDelegate<T> tryParse) where T : struct 
{
	if (!self.TryGetValue (attrName, out var attrs))
		return null;

	foreach (var attr in attrs) {
		if (tryParse (attr, out var data))
			return data.Value;
	}

	return null;
}
");
		}
	}

	static void GetAttributesFromField (FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel semanticModel,
		Dictionary<string, (string AttributeFullName, string AttributeName, BindingAttributeData Data)> dataAttributes)
	{
		foreach (var variableSyntax in fieldDeclarationSyntax.Declaration.Variables) {
			// get the symbol to retrieve the data
			if (semanticModel.GetDeclaredSymbol (variableSyntax) is not IFieldSymbol symbol)
				continue;
			var flagName = $"Has{symbol.Name}";
			var attributeFullName = symbol.ToDisplayString ().Trim ();
			var attrData = GetAttributeData (symbol);
			if (attrData is not null) {
				dataAttributes [flagName] = (attributeFullName, symbol.Name, attrData.Value);
			}
		}
	}

	static void GetFlagsFromField (FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel semanticModel,
		Dictionary<string, (string AttributeFullName, AttributeTargets Targets)> flags)
	{
		foreach (var variableSyntax in fieldDeclarationSyntax.Declaration.Variables) {
			// get the symbol to retrieve the data
			if (semanticModel.GetDeclaredSymbol (variableSyntax) is not IFieldSymbol symbol)
				continue;
			var flagName = $"Has{symbol.Name.Replace ("Attribute", "Flag")}";
			var attrName = symbol.ToDisplayString ().Trim ();
			var target = GetTarget (symbol);
			flags [flagName] = (attrName, target);
		}
	}
}
