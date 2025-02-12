// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.Extensions;
using ObjCRuntime;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Method {

	/// <summary>
	/// The data of the export attribute used to mark the value as a property binding. 
	/// </summary>
	public ExportData<ObjCBindings.Method> ExportMethodData { get; }

	/// <summary>
	/// Returns the bind from data if present in the binding.
	/// </summary>
	public BindFromData? BindAs { get; init; }

	/// <summary>
	/// Returns if the method was marked as thread safe.
	/// </summary>
	public bool IsThreadSafe => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.IsThreadSafe);

	/// <summary>
	/// Return if the method invocation should be wrapped by a NSAutoReleasePool.
	/// </summary>
	public bool AutoRelease => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.AutoRelease);

	/// <summary>
	/// True if the method was exported with the MarshalNativeExceptions flag allowing it to support native exceptions.
	/// </summary>
	public bool MarshalNativeExceptions => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.MarshalNativeExceptions);

	/// <summary>
	/// True if the generator should not use a NSString for marshalling.
	/// </summary>
	public bool UsePlainString
		=> ExportMethodData.Flags.HasFlag (ObjCBindings.Method.PlainString);

	/// <summary>
	/// True if the generated code should retain the return value.
	/// </summary>
	public bool RetainReturnValue => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.RetainReturnValue);

	/// <summary>
	/// True if the generated code should release the return value.
	/// </summary>
	public bool ReleaseReturnValue => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.ReleaseReturnValue);

	/// <summary>
	/// True if the method was marked as a factory method.
	/// </summary>
	public bool IsFactory => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.Factory);

	/// <summary>
	/// True if the return type of the method was returned as a proxy object.
	/// </summary>
	public bool IsProxy => ExportMethodData.Flags.HasFlag (ObjCBindings.Method.Proxy);

	/// <summary>
	/// True if the generated method should use a temp return variable.
	/// </summary>
	public bool UseTempReturn {
		get {
			var byRefParameterCount = Parameters.Count (p => p.ReferenceKind != ReferenceKind.None);

			// based on the configuration flags of the method and the return type we can decide if we need a
			// temp return type
#pragma warning disable format
			return (Method: this, ByRefParameterCount: byRefParameterCount) switch {
				// focus first on the flags, since those are manually added and have more precedence
				{ ByRefParameterCount: > 0 } => true, 
				{ Method.ReleaseReturnValue: true } => true, 
				{ Method.IsFactory: true } => true, 
				{ Method.IsProxy: true } => true, 
				{ Method.MarshalNativeExceptions: true, Method.ReturnType.IsVoid: false } => true,

				// focus on the return type
				{ Method.ReturnType: { IsVoid: false, NeedsStret: true } } => true, 
				{ Method.ReturnType: { IsVoid: false, IsWrapped: true } } => true, 
				{ Method.ReturnType.IsNativeEnum: true } => true, 
				{ Method.ReturnType.SpecialType: SpecialType.System_Boolean 
					or SpecialType.System_Char or SpecialType.System_Delegate } => true, 
				{ Method.ReturnType.IsDelegate: true } => true,
				// default will be false
				_ => false
			};
#pragma warning restore format
		}
	}

	public Method (string type, string name, TypeInfo returnType,
		SymbolAvailability symbolAvailability,
		ExportData<ObjCBindings.Method> exportMethodData,
		ImmutableArray<AttributeCodeChange> attributes,
		ImmutableArray<SyntaxToken> modifiers,
		ImmutableArray<Parameter> parameters)
	{
		Type = type;
		Name = name;
		ReturnType = returnType;
		SymbolAvailability = symbolAvailability;
		ExportMethodData = exportMethodData;
		Attributes = attributes;
		Modifiers = modifiers;
		Parameters = parameters;
	}

	public static bool TryCreate (MethodDeclarationSyntax declaration, RootContext context,
		[NotNullWhen (true)] out Method? change)
	{
		if (context.SemanticModel.GetDeclaredSymbol (declaration) is not IMethodSymbol method) {
			change = null;
			return false;
		}

		var attributes = declaration.GetAttributeCodeChanges (context.SemanticModel);
		var parametersBucket = ImmutableArray.CreateBuilder<Parameter> ();
		// loop over the parameters of the construct since changes on those implies a change in the generated code
		foreach (var parameter in method.Parameters) {
			var parameterDeclaration = declaration.ParameterList.Parameters [parameter.Ordinal];
			if (!Parameter.TryCreate (parameter, parameterDeclaration, context, out var parameterChange))
				continue;
			parametersBucket.Add (parameterChange.Value);
		}

		// DO NOT USE default if null, the reason is that it will set the ArgumentSemantics to be value 0, when
		// none is value 1. The reason for that is that the default of an enum is 0, that was a mistake 
		// in the old binding code.
		var exportData = method.GetExportData<ObjCBindings.Method> ()
						 ?? new (null, ArgumentSemantic.None, ObjCBindings.Method.Default);

		change = new (
			type: method.ContainingSymbol.ToDisplayString ().Trim (), // we want the full name
			name: method.Name,
			returnType: new TypeInfo (method.ReturnType, context.Compilation),
			symbolAvailability: method.GetSupportedPlatforms (),
			exportMethodData: exportData,
			attributes: attributes,
			modifiers: [.. declaration.Modifiers],
			parameters: parametersBucket.ToImmutableArray ()) {
			BindAs = method.GetBindFromData (),
		};

		return true;
	}
}
