// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Syntax factory for the Dlfcn calls.
/// </summary>
static partial class BindingSyntaxFactory {
	readonly static string Dlfcn = "Dlfcn";

	/// <summary>
	/// Get the syntax needed to access a library handle.
	/// </summary>
	/// <param name="libraryName">The library name whose handle we want to retrieve.</param>
	/// <returns>An argument that points to a library name handle.</returns>
	static ArgumentSyntax GetLibraryArgument (string libraryName)
	{
		return Argument (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				MemberAccessExpression (
					SyntaxKind.SimpleMemberAccessExpression,
					IdentifierName ("Libraries"),
					IdentifierName (libraryName)),
				IdentifierName ("Handle")));
	}


	internal static StatementSyntax CachePointer (string libraryName, string fieldName, string storageVariableName)
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName), Token (SyntaxKind.CommaToken),
			Argument (IdentifierName (storageVariableName))
		};

		return StaticInvocationExpression (Dlfcn, "CachePointer", arguments);
	}

	/// <summary>
	/// Generic method that returns the syntax for the Get* methods.
	/// </summary>
	/// <param name="libraryName">The name of the library that contains the field.</param>
	/// <param name="fieldName">The field name literal.</param>
	/// <param name="suppressNullableWarning">If the ! operator should be used.</param>
	/// <param name="methodName">The method name for the invocation.</param>
	/// <returns>The syntax needed to get a constant.</returns>
	static StatementSyntax GetConstant (string libraryName, string fieldName, bool suppressNullableWarning = false,
		[CallerMemberName] string methodName = "")
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
		};
		return StaticInvocationExpression (Dlfcn, methodName, arguments, suppressNullableWarning: suppressNullableWarning);
	}

	/// <summary>
	/// Generic method that returns the syntax for the Set methods.
	/// </summary>
	/// <param name="libraryName">The name of the library that contains the field.</param>
	/// <param name="fieldName">The field name literal.</param>
	/// <param name="variableName">The name of the variable to use for the value. This is NOT a literal value.</param>
	/// <param name="methodName">The method name for the invocation.</param>
	/// <param name="castTarget">An optional type to cast to.</param>
	/// <returns>The sytax needed to set a constant.</returns>
	static StatementSyntax SetConstant (string libraryName, string fieldName,
		string variableName, [CallerMemberName] string methodName = "", string? castTarget = null)
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName),
			Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
			Token (SyntaxKind.CommaToken),
			Argument (castTarget is null
				? IdentifierName (variableName)
				: CastExpression (IdentifierName (castTarget), IdentifierName (variableName))),
		};
		return StaticInvocationExpression (Dlfcn, methodName, arguments);
	}

	static StatementSyntax GetGenericConstant (string methodName, string genericName, string libraryName,
		string fieldName)
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
		};
		var argumentList = ArgumentList (SeparatedList<ArgumentSyntax> (arguments)).NormalizeWhitespace ();
		return ExpressionStatement (StaticInvocationGenericExpression (Dlfcn, methodName, genericName, argumentList));
	}

	/// <summary>
	/// Generates a call for "Dlfcn.GetStringConstant (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetStringConstant (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName, suppressNullableWarning: true);


	/// <summary>
	/// Generates a call for "Dlfcn.GetIndirect (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetIndirect (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetStruct;lt&type;gt& (libraryName, fieldName);"];
	/// </summary>
	/// <param name="type">The name of the type of the structure to return.</param>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetStruct (string type, string libraryName, string fieldName)
		=> GetGenericConstant ("GetStruct", type, libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetSByte (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetSByte (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetSByte (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetSByte (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetSByte (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetSByte (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetByte (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetByte (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetByte (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetByte (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetByte (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetByte (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetInt16 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetInt16 (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetInt16 (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetInt16 (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetInt16 (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetInt16 (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetUInt16 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetUInt16 (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt16 (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUInt16 (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt16 (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUInt16 (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetInt32 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetInt32 (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetInt32 (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetInt32 (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetInt32 (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetInt32 (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetUInt32 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetUInt32 (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt32 (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUInt32 (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt32 (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUInt32 (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetInt64 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetInt64 (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt32 (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetInt64 (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt32 (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetInt64 (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.GetUInt64 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetUInt64 (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt64 (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUInt64 (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUInt64 (libraryName, fieldName, (cast)value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <param name="castTarget">An optional type to cast too.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUInt64 (string libraryName, string fieldName, string variableName, string? castTarget)
		=> SetConstant (libraryName, fieldName, variableName, castTarget: castTarget);

	/// <summary>
	/// Generates a call for "Dlfcn.SetString (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetString (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetArray (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetArray (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetObject (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetObject (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetNFloat (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetNFloat (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetNFloat (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetNFloat (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetIntPtr (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetIntPtr (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetUIntPtr (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetUIntPtr (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetUIntPtr (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetUIntPtr (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetIntPtr (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetIntPtr (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetCGRect (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetCGRect (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetCGSize (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetCGSize (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetCGSize (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetCGSize (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetDouble (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetDouble (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetDouble (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetDouble (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetFloat (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetFloat (string libraryName, string fieldName)
		=> GetConstant (libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.SetFloat (libraryName, fieldName, value);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="variableName">The name of the variable to get the value from.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax SetFloat (string libraryName, string fieldName, string variableName)
		=> SetConstant (libraryName, fieldName, variableName);

	/// <summary>
	/// Generates a call for "Runtime.GetNSObject&lt;nsobjectType&gt; (Dlfcn.GetIndirect (libraryName, fieldName))!;"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static StatementSyntax GetNSObjectField (string nsObjectType, string libraryName, string fieldName)
	{
		var getIndirectArguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
		};

		var getIndirectInvocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (Dlfcn),
				IdentifierName ("GetIndirect").WithTrailingTrivia (Space)
			)
		).WithArgumentList (ArgumentList (SeparatedList<ArgumentSyntax> (getIndirectArguments)).NormalizeWhitespace ());

		return ExpressionStatement (GetNSObject (nsObjectType, [Argument (getIndirectInvocation)], suppressNullableWarning: true));
	}

	public static StatementSyntax GetBlittableField (string blittableType, string libraryName, string fieldName)
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
		};

		// Dlfcn.dlsym (FOO, BAR))
		var dlsymInvocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (Dlfcn),
				IdentifierName ("dlsym").WithTrailingTrivia (Space)
			)
		).WithArgumentList (ArgumentList (SeparatedList<ArgumentSyntax> (arguments)).NormalizeWhitespace ());

		// *((TYPE *) dlsymCall)
		var castExpression = PrefixUnaryExpression (
			SyntaxKind.PointerIndirectionExpression,
			ParenthesizedExpression (
				CastExpression (
					PointerType (
						IdentifierName (blittableType)),
					dlsymInvocation.WithLeadingTrivia (Space)
				)));

		return ExpressionStatement (castExpression);
	}

	/// <summary>
	/// Returns a tuple with the getter and setter for a given field property.
	/// </summary>
	/// <param name="property">The property whose getter and setter we want to generate.</param>
	/// <returns>A tuple with the syntax factory methods for the getters and setters.</returns>
	/// <exception cref="NotSupportedException">When the property is not a field property.</exception>
	/// <exception cref="NotImplementedException">When the return type of the property is not supported.</exception>
	public static (Func<string, string, StatementSyntax> Getter,
		Func<string, string, string, StatementSyntax> Setter) FieldConstantGetterSetter (
			in Property property)
	{
		// check if this is a field, if it is not, we have an issue with the code generator
		if (!property.IsField)
			throw new NotSupportedException ("Cannot retrieve getter for non field property.");

		var fieldType = property.ReturnType.FullyQualifiedName;
		var underlyingEnumType = property.ReturnType.EnumUnderlyingType.GetKeyword ();

		Func<string, string, StatementSyntax> WrapGenericCall (string genericType,
			Func<string, string, string, StatementSyntax> genericCall)
		{
			return (libraryName, fieldName) => genericCall (genericType, libraryName, fieldName);
		}

		Func<string, string, string, StatementSyntax> WrapThrow ()
		{
			return (_, _, _) => ThrowNotSupportedException ($"Setting fields of type '{fieldType}' is not supported.");
		}

		Func<string, string, string, StatementSyntax> WithCast (Func<string, string, string, string?, StatementSyntax> setterCall)
		{
			return (libraryName, fieldName, variableName) =>
				setterCall (libraryName, fieldName, variableName, underlyingEnumType);
		}

		// keep the formatting to make it more readable
#pragma warning disable format
		if (property.ReturnType.IsNSObject) {
			return property.ReturnType switch { 
				{ FullyQualifiedName: "Foundation.NSString" } => (Getter: GetStringConstant, Setter: SetString), 
				{ FullyQualifiedName: "Foundation.NSArray" } => (
					Getter: WrapGenericCall (property.ReturnType.FullyQualifiedName, GetNSObjectField),
					Setter: SetArray),
				_ => (
					Getter: WrapGenericCall (property.ReturnType.FullyQualifiedName, GetNSObjectField),
					Setter: SetObject)
			};
		}

		// use the return type and the special type of the property to decide what getter we are going to us
		return property.ReturnType switch {
			// special types
			{ FullyQualifiedName: "CoreGraphics.CGSize" } => (Getter: GetCGSize, Setter: SetCGSize),
			{ FullyQualifiedName: "CoreMedia.CMTag" } => (
				Getter: WrapGenericCall ("CoreMedia.CMTag", GetStruct),
				Setter: WrapThrow ()),
			{ FullyQualifiedName: "nfloat" } => (Getter: GetNFloat, Setter: SetNFloat),

			// Blittable types 
			{ FullyQualifiedName: "CoreMedia.CMTime" or "AVFoundation.AVCaptureWhiteBalanceGains" }
				=> (Getter: WrapGenericCall (property.ReturnType.FullyQualifiedName, GetBlittableField),
					Setter: WrapThrow ()),

			// enum types, decide based on its enum backing field, smart enums have to be done in the binding
			// manually
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_SByte } => (Getter: GetSByte, Setter: WithCast (SetSByte)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Byte } => (Getter: GetByte, Setter: WithCast (SetByte)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int16 } => (Getter: GetInt16, Setter: WithCast (SetInt16)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt16 } => (Getter: GetUInt16, Setter: WithCast (SetUInt16)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int32 } => (Getter: GetInt32, Setter: WithCast (SetInt32)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt32 } => (Getter: GetUInt32, Setter: WithCast (SetUInt32)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int64 } => (Getter: GetInt64, Setter: WithCast (SetInt64)),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt64 } => (Getter: GetUInt64, Setter: WithCast (SetUInt64)),

			// General special types
			{ SpecialType: SpecialType.System_SByte } => (Getter: GetSByte, Setter: SetSByte),
			{ SpecialType: SpecialType.System_Byte } => (Getter: GetByte, Setter: SetByte),
			{ SpecialType: SpecialType.System_Int16 } => (Getter: GetInt16, Setter: SetInt16),
			{ SpecialType: SpecialType.System_UInt16 } => (Getter: GetUInt16, Setter: SetUInt16),
			{ SpecialType: SpecialType.System_IntPtr } => (Getter: GetIntPtr, Setter: SetIntPtr),
			{ SpecialType: SpecialType.System_Int32 } => (Getter: GetInt32, Setter: SetInt32),
			{ SpecialType: SpecialType.System_UIntPtr } => (Getter: GetUIntPtr, Setter: SetUIntPtr),
			{ SpecialType: SpecialType.System_UInt32 } => (Getter: GetUInt32, Setter: SetUInt32),
			{ SpecialType: SpecialType.System_Int64 } => (Getter: GetInt64, Setter: SetInt64),
			{ SpecialType: SpecialType.System_UInt64 } => (Getter: GetUInt64, Setter: SetUInt64),
			{ SpecialType: SpecialType.System_Double } => (Getter: GetDouble, Setter: SetDouble),
			{ SpecialType: SpecialType.System_Single } => (Getter: GetFloat, Setter: SetFloat),

			// We do not support the property
			_ => throw new NotImplementedException ($"Return type {property.ReturnType} is not implemented."),
		};
#pragma warning restore format
	}

	/// <summary>
	/// Returns the getter needed to access the native value exposed by a field property. The method will return the
	/// Dflcn calls needed.
	/// </summary>
	/// <param name="property">A field property under code generation.</param>
	/// <returns>The appropriate Dlfcn call to retrieve the native value of a field property.</returns>
	/// <exception cref="NotSupportedException">When the caller tries to generate the call for a no field property.</exception>
	/// <exception cref="NotImplementedException">When the property type is not supported for a field property.</exception>
	public static StatementSyntax FieldConstantGetter (in Property property)
	{
		// check if this is a field, if it is not, we have an issue with the code generator
		if (!property.IsField)
			throw new NotSupportedException ("Cannot retrieve getter for non field property.");

		// retrieve all the necessary data from the info field of the property
		var libraryName = property.ExportFieldData.Value.LibraryName;
		var symbolName = property.ExportFieldData.Value.FieldData.SymbolName;
		return FieldConstantGetterSetter (property).Getter (libraryName, symbolName);
	}

	/// <summary>
	/// Returns the setter needed to access the native value exposed by a field property. The method will return the
	/// Dflcn calls needed.
	/// </summary>
	/// <param name="property">A field property under code generation.</param>
	/// <param name="variableName">The name of the variable that contains the value to set.</param>
	/// <returns>The appropriate Dlfcn call to retrieve the native value of a field property.</returns>
	/// <exception cref="NotSupportedException">When the caller tries to generate the call for a no field property.</exception>
	/// <exception cref="NotImplementedException">When the property type is not supported for a field property.</exception>
	public static StatementSyntax FieldConstantSetter (in Property property, string variableName)
	{
		if (variableName is null) throw new ArgumentNullException (nameof (variableName));
		// check if this is a field, if it is not, we have an issue with the code generator
		if (!property.IsField)
			throw new NotSupportedException ("Cannot retrieve getter for non field property.");

		// retrieve all the necessary data from the info field of the property
		var libraryName = property.ExportFieldData.Value.LibraryName;
		var symbolName = property.ExportFieldData.Value.FieldData.SymbolName;
		return FieldConstantGetterSetter (property).Setter (libraryName, symbolName, variableName);
	}
}
