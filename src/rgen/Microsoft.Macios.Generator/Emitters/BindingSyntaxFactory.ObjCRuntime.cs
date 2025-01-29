// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Extensions;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Microsoft.Macios.Generator.Emitters;

static partial class BindingSyntaxFactory {
	readonly static string objc_msgSend = "objc_msgSend";
	readonly static string objc_msgSendSuper = "objc_msgSendSuper";

	/// <summary>
	/// Returns the expression needed to cast a parameter to its native type.
	/// </summary>
	/// <param name="parameter">The parameter whose casting we need to generate. The type info has to be
	/// and enum and be marked as native. If it is not, the method returns null</param>
	/// <returns>The cast C# expression.</returns>
	internal static CastExpressionSyntax? CastToNative (in Parameter parameter)
	{
		// not an enum and not a native value. we cannot calculate the casting expression.
		if (!parameter.Type.IsEnum || !parameter.Type.IsNativeEnum)
			return null;

		// build a casting expression based on the marshall type of the typeinfo
		var marshalType = parameter.Type.ToMarshallType ();
		if (marshalType is null)
			// cannot calculate the marshal, return null
			return null;

		var enumBackingValue = parameter.Type.EnumUnderlyingType.Value.GetKeyword ();
		var castExpression = CastExpression (IdentifierName (marshalType), // (IntPtr/UIntPtr) cast
			CastExpression (
				IdentifierName (enumBackingValue),
				IdentifierName (parameter.Name)
					.WithLeadingTrivia (Space))
				.WithLeadingTrivia (Space)); // (backingfield) (variable) cast
		return castExpression;
	}

	/// <summary>
	/// Returns the expression needed to cast an enum parameter to its primitive type to be used in marshaling.
	/// </summary>
	/// <param name="parameter">The parameter for which we need to generate the casting. The type info has to be
	/// an enumerator. If it is not, the method returns null.</param>
	/// <returns>The cast C# expression.</returns>
	internal static CastExpressionSyntax? CastToPrimitive (in Parameter parameter)
	{
		if (!parameter.Type.IsEnum) {
			return null;
		}

		if (parameter.Type.IsNativeEnum) {
			// return the native casting
			return CastToNative (parameter);
		}

		// returns the enum primitive to be used
		var marshalType = parameter.Type.ToMarshallType ();
		if (marshalType is null)
			return null;

		// (byte) parameter
		var castExpression = CastExpression (
			type: IdentifierName (marshalType),
			expression: IdentifierName (parameter.Name).WithLeadingTrivia (Space));
		return castExpression;
	}

	/// <summary>
	/// Returns the expression needed to cast a bool to a byte to be used in a native call. 
	/// </summary>
	/// <param name="parameter">The parameter to cast.</param>
	/// <returns>A conditional expression that casts a bool to a byte.</returns>
	internal static ConditionalExpressionSyntax? CastToByte (in Parameter parameter)
	{
		if (parameter.Type.SpecialType != SpecialType.System_Boolean)
			return null;
		// (byte) 1
		var castOne = CastExpression (
				PredefinedType (Token (SyntaxKind.ByteKeyword)),
				LiteralExpression (SyntaxKind.NumericLiteralExpression, Literal (1)).WithLeadingTrivia (Space).WithTrailingTrivia (Space)
				);
		// (byte) 0
		var castZero = CastExpression (
				PredefinedType (Token (SyntaxKind.ByteKeyword)),
				LiteralExpression (SyntaxKind.NumericLiteralExpression, Literal (0)).WithLeadingTrivia (Space)
				).WithLeadingTrivia (Space);

		// with this exact space count
		// foo ? (byte) 1 : (byte) 0
		return ConditionalExpression (
			condition: IdentifierName (parameter.Name).WithTrailingTrivia (Space),
			whenTrue: castOne.WithLeadingTrivia (Space),
			whenFalse: castZero);
	}

	static string? GetObjCMessageSendMethodName<T> (ExportData<T> exportData,
		TypeInfo returnType, ImmutableArray<Parameter> parameters, bool isSuper = false, bool isStret = false) where T : Enum
	{
		var flags = exportData.Flags;
		if (flags is null)
			// flags are not set, should be a bug, but we will return null
			return null;

		// the name of the objcSend method is calculated in the following way	
		// {CustomMarshallPrefix}_{MarshallTypeOfReturnType}_{objcSendMsg}{stret?_stret}_{string.Join('_', MarshallTypeArgs)}{nativeException?_exception}{CustomMarsahllPostfix}
		// we will use a sb to make things easy to follow
		var sb = new StringBuilder ();

		// first, decide if the user created a custom marshalling by checking the flags of the export data
		CustomMarshalDirective? customMarshalDirective = null;
		if (flags.HasCustomMarshalDirective ()) {
			customMarshalDirective = exportData.ToCustomMarshalDirective ();
		}

		if (customMarshalDirective?.NativePrefix is not null) {
			sb.Append (customMarshalDirective.NativePrefix);
		} else if (flags.HasMarshalNativeExceptions ()) {
			sb.Append ("xamarin_");
		}

		// return types do not have a reference kind
		sb.Append (returnType.ToMarshallType ());
		sb.Append ('_');
		// append the msg method based if it is for super or not, do not append '_' intimidatingly, since if we do
		// not have parameters, we are done
		sb.Append (isSuper ? objc_msgSendSuper : objc_msgSend);
		if (isStret) {
			sb.Append ("_stret");
		}
		// loop over params and get their native handler name
		if (parameters.Length > 0) {
			sb.Append ('_');
			sb.AppendJoin ('_', parameters.Select (p => p.Type.ToMarshallType ()));
		}

		// check if we do have a custom marshall exception set for the export

		// check any possible custom postfix naming
		if (customMarshalDirective?.NativeSuffix is not null) {
			sb.Append (customMarshalDirective.NativeSuffix);
		} else if (flags.HasMarshalNativeExceptions ()) {
			sb.Append ("_exception");
		}
		return sb.ToString ();
	}

	public static (string? Getter, string? Setter) GetObjCMessageSendMethods (in Property property, bool isSuper = false, bool isStret = false)
	{
		if (property.IsProperty) {
			// the getter and the setter depend of the accessors that have been set for the property, we do not want
			// to calculate things that we won't use. The export data used will also depend if the getter/setter has a
			// export attribute attached
			var getter = property.GetAccessor (AccessorKind.Getter);
			string? getterMsgSend = null;
			if (getter is not null) {
				var getterExportData = getter.Value.ExportPropertyData ?? property.ExportPropertyData;
				if (getterExportData is not null) {
					getterMsgSend = GetObjCMessageSendMethodName (getterExportData.Value, property.ReturnType, [],
						isSuper, isStret);
				}
			}

			var setter = property.GetAccessor (AccessorKind.Setter);
			string? setterMsgSend = null;
			if (setter is not null) {
				var setterExportData = setter.Value.ExportPropertyData ?? property.ExportPropertyData;
				if (setterExportData is not null) {
					setterMsgSend = GetObjCMessageSendMethodName (setterExportData.Value, TypeInfo.Void,
						[property.ValueParameter], isSuper, isStret);
				}
			}
			return (Getter: getterMsgSend, Setter: setterMsgSend);
		}

		return default;
	}

	public static string? GetObjCMessageSendMethod (in Method method, bool isSuper = false, bool isStret = false)
		=> GetObjCMessageSendMethodName (method.ExportMethodData, method.ReturnType, method.Parameters, isSuper, isStret);

}
