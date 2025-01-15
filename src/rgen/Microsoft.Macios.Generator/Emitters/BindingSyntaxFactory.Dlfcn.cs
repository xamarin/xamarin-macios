// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Syntaxt factory for the Dlfcn calls.
/// </summary>
static partial class BindingSyntaxFactory {
	readonly static string Dlfcn = "Dlfcn";

	/// <summary>
	/// Get the syntax needed to access a library handle.
	/// </summary>
	/// <param name="libraryName">The library name whose handle we want to retrienve.</param>
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


	public static CompilationUnitSyntax CachePointer (string libraryName, string fieldName, string storageVariableName)
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
	/// <param name="methodName">The method name for the invocation.</param>
	/// <param name="libraryName">The name of the library that contains the field.</param>
	/// <param name="fieldName">The field name literal.</param>
	/// <returns></returns>
	static CompilationUnitSyntax GetConstant (string methodName, string libraryName, string fieldName)
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
		};
		return StaticInvocationExpression (Dlfcn, methodName, arguments);
	}

	static CompilationUnitSyntax GetGenericConstant (string methodName, string genericName, string libraryName,
		string fieldName)
	{
		var arguments = new SyntaxNodeOrToken [] {
			GetLibraryArgument (libraryName), Token (SyntaxKind.CommaToken),
			GetLiteralExpressionArgument (SyntaxKind.StringLiteralExpression, fieldName),
		};
		var argumentList = ArgumentList (SeparatedList<ArgumentSyntax> (arguments)).NormalizeWhitespace ();
		return StaticInvocationGenericExpression (Dlfcn, methodName, genericName, argumentList);
	}

	/// <summary>
	/// Generates a call for "Dlfcn.GetStringConstant (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetStringConstant (string libraryName, string fieldName)
		=> GetConstant ("GetStringConstant", libraryName, fieldName);


	/// <summary>
	/// Generates a call for "Dlfcn.GetIndirect (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetIndirect (string libraryName, string fieldName)
		=> GetConstant ("GetIndirect", libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetStruct;lt&type;gt& (libraryName, fieldName);"];
	/// </summary>
	/// <param name="type">The name of the type of the structure to return.</param>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetStruct (string type, string libraryName, string fieldName)
		=> GetGenericConstant ("GetStruct", type, libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetSByte (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetSByte (string libraryName, string fieldName)
		=> GetConstant ("GetSByte", libraryName, fieldName);

	public static CompilationUnitSyntax SetSByte (string libraryName, string symbol, sbyte value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetByte (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetByte (string libraryName, string fieldName)
		=> GetConstant ("GetByte", libraryName, fieldName);


	public static CompilationUnitSyntax SetByte (string libraryName, string symbol, byte value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetInt16 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetInt16 (string libraryName, string fieldName)
		=> GetConstant ("GetInt16", libraryName, fieldName);

	public static CompilationUnitSyntax SetInt16 (string libraryName, string symbol, short value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetUInt16 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetUInt16 (string libraryName, string fieldName)
		=> GetConstant ("GetUInt16", libraryName, fieldName);

	public static CompilationUnitSyntax SetUInt16 (IntPtr handle, string symbol, ushort value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetInt32 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetInt32 (string libraryName, string fieldName)
		=> GetConstant ("GetInt32", libraryName, fieldName);

	public static CompilationUnitSyntax SetInt32 (IntPtr handle, string symbol, int value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetUInt32 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetUInt32 (string libraryName, string fieldName)
		=> GetConstant ("GetUInt32", libraryName, fieldName);

	public static CompilationUnitSyntax SetUInt32 (IntPtr handle, string symbol, uint value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetInt64 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetInt64 (string libraryName, string fieldName)
		=> GetConstant ("GetInt64", libraryName, fieldName);

	public static CompilationUnitSyntax SetInt64 (IntPtr handle, string symbol, long value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetUInt64 (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetUInt64 (string libraryName, string fieldName)
		=> GetConstant ("GetUInt64", libraryName, fieldName);

	public static CompilationUnitSyntax SetUInt64 (string libraryName, string symbol, ulong value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax SetString (string libraryName, string symbol, string? value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax SetArray (string libraryName, string symbol, /*NSArray?*/ string array)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax SetObject (string libraryName, string symbol, /*NSObject?*/ string value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax GetNInt (string libraryName, string symbol)
		=> GetConstant ("GetNInt", libraryName, symbol);

	public static CompilationUnitSyntax SetNInt (IntPtr handle, string symbol, nint value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax GetNUInt (string libraryName, string symbol)
		=> GetConstant ("GetNInt", libraryName, symbol);

	public static CompilationUnitSyntax SetNUInt (string libraryName, string symbol, nuint value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetNFloat (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetNFloat (string libraryName, string fieldName)
		=> GetConstant ("GetNFloat", libraryName, fieldName);

	public static CompilationUnitSyntax SetNFloat (string libraryName, string symbol, /*nfloat*/ string value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetIntPtr (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetIntPtr (string libraryName, string fieldName)
		=> GetConstant ("GetIntPtr", libraryName, fieldName);

	/// <summary>
	/// Generates a call for "Dlfcn.GetUIntPtr (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetUIntPtr (string libraryName, string fieldName)
		=> GetConstant ("GetUIntPtr", libraryName, fieldName);

	public static CompilationUnitSyntax SetUIntPtr (string libraryName, string symbol, UIntPtr value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax SetIntPtr (string libraryName, string symbol, IntPtr value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax GetCGRect (string libraryName, string symbol)
		=> GetConstant ("GetCGRect", libraryName, symbol);

	/// <summary>
	/// Generates a call for "Dlfcn.GetCGSize (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetCGSize (string libraryName, string fieldName)
		=> GetConstant ("GetCGSize", libraryName, fieldName);

	public static CompilationUnitSyntax SetCGSize (string libraryName, string symbol, /*CGSize*/ string value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetDouble (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetDouble (string libraryName, string fieldName)
		=> GetConstant ("GetDouble", libraryName, fieldName);

	public static CompilationUnitSyntax SetDouble (string libraryName, string symbol, double value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetFloat (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetFloat (string libraryName, string fieldName)
		=> GetConstant ("GetFloat", libraryName, fieldName);

	public static CompilationUnitSyntax SetFloat (IntPtr handle, string symbol, float value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Dlfcn.GetSizeF (libraryName, fieldName);"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetSizeF (string libraryName, string fieldName)
		=> GetConstant ("GetSizeF", libraryName, fieldName);

	public static CompilationUnitSyntax GetSizeF (IntPtr handle, string fieldName, float value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Generates a call for "Runtime.GetNSObject;lt&Foundation.NSArray;gt& (Dlfcn.GetIndirect (libraryName, fieldName))!;"];
	/// </summary>
	/// <param name="libraryName">The library from where the field will be loaded.</param>
	/// <param name="fieldName">The field name.</param>
	/// <returns>A compilation unit with the desired Dlfcn call.</returns>
	public static CompilationUnitSyntax GetNSObjectField (string nsObjectType, string libraryName, string fieldName)
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

		var getNSObjectArguments =
			ArgumentList (SingletonSeparatedList (Argument (getIndirectInvocation)));
		return GetNSObject (nsObjectType, getNSObjectArguments, suppressNullableWarning: true);
	}

	public static CompilationUnitSyntax SetNSObject (string nsObjectType, string libraryName, string symbol,
		string value)
		=> throw new NotImplementedException ();

	public static CompilationUnitSyntax GetBlittableField (string blittableType, string libraryName, string fieldName)
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

		var compilationUnit = CompilationUnit ().WithMembers (
			SingletonList<MemberDeclarationSyntax> (
				GlobalStatement (
					ExpressionStatement (castExpression
					))));
		return compilationUnit;
	}


	public static CompilationUnitSyntax SetBlittableField (string nsObjectType, string libraryName, string fieldName,
		string value)
		=> throw new NotImplementedException ();

	/// <summary>
	/// Returns the getter needed to access the native value exposed by a field property. The method will return the
	/// Dflcn calls needed.
	/// </summary>
	/// <param name="property">A field property under code generation.</param>
	/// <returns>The appropriate Dlfcn call to retrieve the native value of a field property.</returns>
	/// <exception cref="NotSupportedException">When the caller tries to generate the call for a no field property.</exception>
	/// <exception cref="NotImplementedException">When the property type is not supported for a field property.</exception>
	public static CompilationUnitSyntax FieldConstantGetter (in Property property)
	{
		// check if this is a field, if it is not, we have an issue with the code generator
		if (!property.IsField)
			throw new NotSupportedException ("Cannot retrieve getter for non field property.");

		// retrieve all the necessary data from the info field of the property
		var libraryName = property.ExportFieldData.Value.LibraryName;
		var symbolName = property.ExportFieldData.Value.FieldData.SymbolName;

		if (property.ReturnType.IsNSObject) {
			return property.ReturnType.Name == "Foundation.NSString"
				? GetStringConstant (libraryName, symbolName)
				// all nsobjects are retrieved using the same generic getter
				: GetNSObjectField (property.ReturnType.Name, libraryName, symbolName);
		}

		// keep the formatting to make it more readable
#pragma warning disable format
		// use the return type and the special type of the property to decide what getter we are going to us
		return property.ReturnType switch {
			// special types
			{ Name: "CoreGraphics.CGSize" } => GetCGSize (libraryName, symbolName),
			{ Name: "CoreMedia.CMTag" } => GetStruct ("CoreMedia.CMTag", libraryName, symbolName),
			{ Name: "nfloat" } => GetNFloat (libraryName, symbolName),
			{ Name: "System.Drawing.SizeF" } => GetSizeF (libraryName, symbolName),

			// Billable types 
			{ Name: "CoreMedia.CMTime" or "AVFoundation.AVCaptureWhiteBalanceGains" }
				=> GetBlittableField (property.ReturnType.Name, libraryName, symbolName),

			// enum types, decide based on its enum backing field, smart enums have to be done in the binding
			// manually
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_SByte } => GetSByte (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Byte } => GetByte (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int16 } => GetInt16 (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt16 } => GetUInt16 (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int32 } => GetInt32 (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt32 } => GetUInt32 (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int64 } => GetInt64 (libraryName, symbolName),
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt64 } => GetUInt64 (libraryName, symbolName),

			// General special types
			{ SpecialType: SpecialType.System_SByte } => GetSByte (libraryName, symbolName),
			{ SpecialType: SpecialType.System_Byte } => GetByte (libraryName, symbolName),
			{ SpecialType: SpecialType.System_Int16 } => GetInt16 (libraryName, symbolName),
			{ SpecialType: SpecialType.System_UInt16 } => GetUInt16 (libraryName, symbolName),
			{ SpecialType: SpecialType.System_IntPtr } => GetIntPtr (libraryName, symbolName),
			{ SpecialType: SpecialType.System_Int32 } => GetInt32 (libraryName, symbolName),
			{ SpecialType: SpecialType.System_UIntPtr } => GetUIntPtr (libraryName, symbolName),
			{ SpecialType: SpecialType.System_UInt32 } => GetUInt32 (libraryName, symbolName),
			{ SpecialType: SpecialType.System_Int64 } => GetInt64 (libraryName, symbolName),
			{ SpecialType: SpecialType.System_UInt64 } => GetUInt64 (libraryName, symbolName),
			{ SpecialType: SpecialType.System_Double } => GetDouble (libraryName, symbolName),
			{ SpecialType: SpecialType.System_Single } => GetFloat (libraryName, symbolName),

			// We do not support the property
			_ => throw new NotImplementedException ($"Return type {property.ReturnType} is not implemented."),
		};
#pragma warning restore format
	}
}
