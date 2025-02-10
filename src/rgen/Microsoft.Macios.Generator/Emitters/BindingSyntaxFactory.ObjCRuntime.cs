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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;
using Parameter = Microsoft.Macios.Generator.DataModel.Parameter;

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
			LiteralExpression (SyntaxKind.NumericLiteralExpression, Literal (1)).WithLeadingTrivia (Space)
				.WithTrailingTrivia (Space)
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

	/// <summary>
	/// Returns the aux nsarray variable for an array object. This method will do the following:
	/// 1. Check if the object is nullable or not.
	/// 2. Use the correct NSArray method depending on the content of the array. 
	/// </summary>
	/// <param name="parameter">The parameter whose aux variable we want to generate.</param>
	/// <param name="withUsing">If the using clause should be added to the declaration.</param>
	/// <returns>The variable declaration for the NSArray aux variable of the parameter.</returns>
	internal static LocalDeclarationStatementSyntax? GetNSArrayAuxVariable (in Parameter parameter,
		bool withUsing = false)
	{
		if (!parameter.Type.IsArray)
			return null;
		var nsArrayFactoryMethod = parameter.Type.Name switch {
			"string" => "FromStrings",
			_ => "FromNSObjects" // the general assumption is that we are working with nsobjects unless we have a bind form
		};
		// syntax that calls the NSArray factory method using the parameter: NSArray.FromNSObjects (targetTensors);
		var factoryInvocation = InvocationExpression (MemberAccessExpression (SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName ("NSArray"), IdentifierName (nsArrayFactoryMethod).WithTrailingTrivia (Space)))
			.WithArgumentList (
				ArgumentList (SingletonSeparatedList<ArgumentSyntax> (
					Argument (IdentifierName (parameter.Name)))));

		// variable name
		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.NSArray);
		if (variableName is null)
			return null;
		var declarator = VariableDeclarator (Identifier (variableName));
		// we have the basic constructs, now depending on if the variable is nullable or not, we need to write the initializer 	
		if (parameter.Type.IsNullable) {
			// writes the param ? null : NSArray expression
			var nullCheck = ConditionalExpression (
				IsPatternExpression (
					IdentifierName (parameter.Name).WithLeadingTrivia (Space).WithTrailingTrivia (Space),
					ConstantPattern (LiteralExpression (SyntaxKind.NullLiteralExpression).WithLeadingTrivia (Space)
						.WithTrailingTrivia (Space))),
				LiteralExpression (
					SyntaxKind.NullLiteralExpression).WithLeadingTrivia (Space).WithTrailingTrivia (Space),
				factoryInvocation.WithLeadingTrivia (Space));

			// translates to = x ? null : NSArray.FromNSObject (parameterName), notice we added the '=' here
			declarator = declarator.WithInitializer (EqualsValueClause (nullCheck).WithLeadingTrivia (Space));
		} else {
			// translates to = NSArray.FromNSObject (parameterName);
			declarator = declarator.WithInitializer (EqualsValueClause (factoryInvocation.WithLeadingTrivia (Space))
				.WithLeadingTrivia (Space));
		}

		// complicated way to write 'var auxVariableName = '
		var variableDeclaration = VariableDeclaration (IdentifierName (
				Identifier (TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithTrailingTrivia (Space)
			.WithVariables (SingletonSeparatedList (declarator));
		var statement = LocalDeclarationStatement (variableDeclaration);
		// add using if requested
		return withUsing
			? statement.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space))
			: statement;
	}

	/// <summary>
	/// Returns the aux variable for a handle object. This method will do the following:
	/// 1. Check if the object is nullable or not.
	/// 2. Use the correct GetHandle method depending on the content of the object.
	/// </summary>
	internal static LocalDeclarationStatementSyntax? GetHandleAuxVariable (in Parameter parameter,
		bool withNullAllowed = false)
	{
		if (!parameter.Type.IsNSObject && !parameter.Type.IsINativeObject)
			return null;

		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.Handle);
		if (variableName is null)
			return null;
		// decide about the factory based on the need of a null check 
		InvocationExpressionSyntax factoryInvocation;
		if (withNullAllowed) {
			// generates: zone!.GetNonNullHandle (nameof (zone));
			factoryInvocation = InvocationExpression (
					MemberAccessExpression (SyntaxKind.SimpleMemberAccessExpression,
						PostfixUnaryExpression (
							SyntaxKind.SuppressNullableWarningExpression,
							IdentifierName (parameter.Name)),
						IdentifierName ("GetNonNullHandle").WithTrailingTrivia (Space)))
				.WithArgumentList (ArgumentList (
					SingletonSeparatedList (Argument (
						InvocationExpression (
								IdentifierName (Identifier (TriviaList (Space), SyntaxKind.NameOfKeyword, "nameof",
									"nameof",
									TriviaList (Space))))
							.WithArgumentList (ArgumentList (
								SingletonSeparatedList<ArgumentSyntax> (
									Argument (IdentifierName (parameter.Name)))))))));
		} else {
			// generates: zone.GetHandle ();
			factoryInvocation = InvocationExpression (
				MemberAccessExpression (SyntaxKind.SimpleMemberAccessExpression,
					IdentifierName (parameter.Name),
					IdentifierName ("GetHandle").WithTrailingTrivia (Space)));
		}

		// generates: variable = {FactoryCall}
		var declarator = VariableDeclarator (Identifier (variableName))
			.WithInitializer (EqualsValueClause (factoryInvocation.WithLeadingTrivia (Space))
				.WithLeadingTrivia (Space));
		// generates the final statement: 
		// var x = zone.GetHandle ();
		// or 
		// var x = zone!.GetNonNullHandle (nameof (constantValues));
		return LocalDeclarationStatement (
			VariableDeclaration (
				IdentifierName (
					Identifier (
						TriviaList (),
						SyntaxKind.VarKeyword,
						"var",
						"var",
						TriviaList ()))).WithVariables (
				SingletonSeparatedList (declarator.WithLeadingTrivia (Space))
			));
	}

	internal static LocalDeclarationStatementSyntax? GetStringAuxVariable (in Parameter parameter)
	{
		if (parameter.Type.Name != "string")
			return null;

		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.NSString);
		if (variableName is null)
			return null;

		// generates: CFString.CreateNative ({parameter.Name});
		var cfstringFactoryInvocation = InvocationExpression (
				MemberAccessExpression (
					SyntaxKind.SimpleMemberAccessExpression,
					IdentifierName ("CFString"),
					IdentifierName ("CreateNative").WithTrailingTrivia (Space))
			)
			.WithArgumentList (ArgumentList (SingletonSeparatedList (
				Argument (IdentifierName (parameter.Name)))));

		// generates {var} = CFString.CreateNative ({parameter.Name});
		var declarator =
			VariableDeclarator (Identifier (variableName).WithLeadingTrivia (Space).WithTrailingTrivia (Space))
				.WithInitializer (EqualsValueClause (cfstringFactoryInvocation.WithLeadingTrivia (Space)));


		// put everythign together
		var declaration = VariableDeclaration (IdentifierName (Identifier (
				TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithVariables (SingletonSeparatedList (declarator));

		return LocalDeclarationStatement (declaration);
	}

	internal static LocalDeclarationStatementSyntax? GetNSNumberAuxVariable (in Parameter parameter)
	{
		// the BindFrom attribute with a nsnumber supports the following types:
		// - bool
		// - byte
		// - double
		// - float
		// - short
		// - int
		// - long
		// - sbyte
		// - ushort
		// - uint
		// - ulong
		// - nfloat
		// - nint
		// - nuint
		// - Enums: this are simply casted to their backing representation
		// if we do not match the expected type, return null

		// make sure that the parameter type is valid and return the required method for the nsnumber variable
#pragma warning disable format
		var factoryMethod = parameter.Type switch {
			{ Name: "nint" } => "FromNInt",
			{ Name: "nuint" } => "FromNUInt",
			{ Name: "nfloat" or "NFloat" } => "FromNFloat",
			{ IsEnum: true, IsSmartEnum: true } => null, // we do not support smart enums, there is a special case for them 
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_SByte } => "FromSByte",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Byte } => "FromByte",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int16 } => "FromInt16",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt16 } => "FromUInt16",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int32 } => "FromInt32",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt32 } => "FromUInt32",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int64 } => "FromInt64",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt64 } => "FromUInt64",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_IntPtr } => "FromNint",
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UIntPtr } => "FromNUint",
			{ SpecialType: SpecialType.System_Boolean } => "FromBoolean",
			{ SpecialType: SpecialType.System_Byte } => "FromByte",
			{ SpecialType: SpecialType.System_Double } => "FromDouble",
			{ SpecialType: SpecialType.System_Single } => "FromFloat",
			{ SpecialType: SpecialType.System_Int16 } => "FromInt16",
			{ SpecialType: SpecialType.System_Int32 } => "FromInt32",
			{ SpecialType: SpecialType.System_Int64 } => "FromInt64",
			{ SpecialType: SpecialType.System_SByte } => "FromSByte",
			{ SpecialType: SpecialType.System_UInt16 } => "FromUInt16",
			{ SpecialType: SpecialType.System_UInt32 } => "FromUInt32",
			{ SpecialType: SpecialType.System_UInt64 } => "FromUInt64",
			{ SpecialType: SpecialType.System_IntPtr } => "FromNint",
			{ SpecialType: SpecialType.System_UIntPtr } => "FromNUint",
			_ => null,
		};
#pragma warning restore format

		if (factoryMethod is null)
			return null;

		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.BindFrom);
		if (variableName is null)
			return null;

		// generates: NSNumber.FromDouble
		var factoryInvocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName ("NSNumber"),
				IdentifierName (factoryMethod).WithTrailingTrivia (Space))
		);

		// the arguments of the factory information depends on if we are dealing with a enum, in which case we cast
		// or not, in which case we just add the arguments
		if (parameter.Type.IsEnum) {
			// generates: NSNumber.FromDouble ((int)value);
			factoryInvocation = factoryInvocation
				.WithArgumentList (ArgumentList (SingletonSeparatedList (Argument (
					CastExpression (
						IdentifierName (parameter.Type.EnumUnderlyingType.GetKeyword () ?? ""),
						IdentifierName (parameter.Name).WithLeadingTrivia (Space))))));
		} else {
			// generates: NSNumber.FromDouble (value);
			factoryInvocation = factoryInvocation
				.WithArgumentList (ArgumentList (SingletonSeparatedList (
					Argument (IdentifierName (parameter.Name)))));
		}

		var declarator =
			VariableDeclarator (Identifier (variableName).WithLeadingTrivia (Space).WithTrailingTrivia (Space))
				.WithInitializer (EqualsValueClause (factoryInvocation.WithLeadingTrivia (Space)));

		// generats: var nba_variable = NSNumber.FromDouble(value);
		var declaration = VariableDeclaration (IdentifierName (Identifier (
				TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithVariables (SingletonSeparatedList (declarator));

		return LocalDeclarationStatement (declaration);
	}

	internal static LocalDeclarationStatementSyntax? GetNSValueAuxVariable (in Parameter parameter)
	{

		// the BindFrom attribute with a nsvalue supports the following types:
		// - CGAffineTransform
		// - NSRange
		// - CGVector
		// - SCNMatrix4
		// - CLLocationCoordinate2D
		// - SCNVector3
		// - SCNVector4
		// - CGPoint / PointF
		// - CGRect / RectangleF
		// - CGSize / SizeF
		// - UIEdgeInsets
		// - UIOffset
		// - MKCoordinateSpan
		// - CMTimeRange
		// - CMTime
		// - CMTimeMapping
		// - CATransform3D
		var t = parameter.Type.Name;

#pragma warning disable format
		// get the factory method based on the parameter type, if it is not found, return null
		var factoryMethod = parameter.Type switch { 
			{ FullyQualifiedName: "CoreGraphics.CGAffineTransform" } => "FromCGAffineTransform", 
			{ FullyQualifiedName: "Foundation.NSRange" } => "FromRange", 
			{ FullyQualifiedName: "CoreGraphics.CGVector" } => "FromCGVector", 
			{ FullyQualifiedName: "SceneKit.SCNMatrix4" } => "FromSCNMatrix4", 
			{ FullyQualifiedName: "CoreLocation.CLLocationCoordinate2D" } => "FromMKCoordinate", 
			{ FullyQualifiedName: "SceneKit.SCNVector3" } => "FromVector", 
			{ FullyQualifiedName: "SceneKit.SCNVector4" } => "FromVector", 
			{ FullyQualifiedName: "CoreGraphics.CGPoint" } => "FromCGPoint", 
			{ FullyQualifiedName: "CoreGraphics.CGRect" } => "FromCGRect", 
			{ FullyQualifiedName: "CoreGraphics.CGSize" } => "FromCGSize", 
			{ FullyQualifiedName: "UIKit.UIEdgeInsets" } => "FromUIEdgeInsets", 
			{ FullyQualifiedName: "UIKit.UIOffset" } => "FromUIOffset", 
			{ FullyQualifiedName: "MapKit.MKCoordinateSpan" } => "FromMKCoordinateSpan", 
			{ FullyQualifiedName: "CoreMedia.CMTimeRange" } => "FromCMTimeRange", 
			{ FullyQualifiedName: "CoreMedia.CMTime" } => "FromCMTime", 
			{ FullyQualifiedName: "CoreMedia.CMTimeMapping" } => "FromCMTimeMapping", 
			{ FullyQualifiedName: "CoreAnimation.CATransform3D" } => "FromCATransform3D",
			_ => null,
		};
#pragma warning restore format

		if (factoryMethod is null)
			return null;

		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.BindFrom);
		if (variableName is null)
			return null;

		// generates: NSValue.FromCMTime 
		var factoryInvocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName ("NSValue"),
				IdentifierName (factoryMethod).WithTrailingTrivia (Space))
		).WithArgumentList (ArgumentList (SingletonSeparatedList (
			Argument (IdentifierName (parameter.Name)))));

		var declarator =
			VariableDeclarator (Identifier (variableName).WithLeadingTrivia (Space).WithTrailingTrivia (Space))
				.WithInitializer (EqualsValueClause (factoryInvocation.WithLeadingTrivia (Space)));

		// generats: var nba_variable = NSNumber.FromDouble(value);
		var declaration = VariableDeclaration (IdentifierName (Identifier (
				TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithVariables (SingletonSeparatedList (declarator));

		return LocalDeclarationStatement (declaration);
	}

	internal static LocalDeclarationStatementSyntax? GetNSStringSmartEnumAuxVariable (in Parameter parameter)
	{
		if (!parameter.Type.IsSmartEnum)
			return null;

		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.BindFrom);
		if (variableName is null)
			return null;

		// smart enums are very simple to do, we need to call the GetConstant that was generated as an extension
		// method
		var factoryInvocation = InvocationExpression (
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (parameter.Name),
				IdentifierName ("GetConstant").WithTrailingTrivia (Space))
		);

		var declarator =
			VariableDeclarator (Identifier (variableName).WithLeadingTrivia (Space).WithTrailingTrivia (Space))
				.WithInitializer (EqualsValueClause (factoryInvocation.WithLeadingTrivia (Space)));

		// generats: var nba_variable = NSNumber.FromDouble(value);
		var declaration = VariableDeclaration (IdentifierName (Identifier (
				TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithVariables (SingletonSeparatedList (declarator));

		return LocalDeclarationStatement (declaration);
	}

	internal static LocalDeclarationStatementSyntax? GetNSArrayBindFromAuxVariable (in Parameter parameter)
	{
		// we can only work with parameters that are an array
		if (!parameter.Type.IsArray)
			return null;

		var variableName = parameter.GetNameForVariableType (Parameter.VariableType.BindFrom);
		if (variableName is null)
			return null;

		// use a switch to decide which of the constructors we are going to use to build the array.
		var lambdaFunctionVariable = "obj";
		var nsNumberExpr = ObjectCreationExpression (
				IdentifierName ("NSNumber").WithLeadingTrivia (Space).WithTrailingTrivia (Space))
			.WithArgumentList (ArgumentList (SingletonSeparatedList (
				Argument (IdentifierName (lambdaFunctionVariable)))));
		var nsValueExpr = ObjectCreationExpression (
				IdentifierName ("NSValue").WithLeadingTrivia (Space).WithTrailingTrivia (Space))
			.WithArgumentList (ArgumentList (SingletonSeparatedList (
				Argument (IdentifierName (lambdaFunctionVariable)))));
		var smartEnumExpr = InvocationExpression (MemberAccessExpression (
			SyntaxKind.SimpleMemberAccessExpression,
			IdentifierName (lambdaFunctionVariable).WithLeadingTrivia (Space),
			IdentifierName ("GetConstant")));
			
#pragma warning disable format
		ExpressionSyntax? constructor = parameter.Type switch {
			// smart enums
			{ IsEnum: true, IsSmartEnum: true } => smartEnumExpr,
			// NSNumber types:
			{ Name: "nint" } => nsNumberExpr,
			{ Name: "nuint" } => nsNumberExpr,
			{ Name: "nfloat" or "NFloat" } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_SByte } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Byte } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int16 } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt16 } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int32 } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt32 } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_Int64 } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UInt64 } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_IntPtr } => nsNumberExpr,
			{ IsEnum: true, EnumUnderlyingType: SpecialType.System_UIntPtr } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Boolean } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Byte } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Double } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Single } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Int16 } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Int32 } => nsNumberExpr,
			{ SpecialType: SpecialType.System_Int64 } => nsNumberExpr,
			{ SpecialType: SpecialType.System_SByte } => nsNumberExpr,
			{ SpecialType: SpecialType.System_UInt16 } => nsNumberExpr,
			{ SpecialType: SpecialType.System_UInt32 } => nsNumberExpr,
			{ SpecialType: SpecialType.System_UInt64 } => nsNumberExpr,
			{ SpecialType: SpecialType.System_IntPtr } => nsNumberExpr,
			{ SpecialType: SpecialType.System_UIntPtr } => nsNumberExpr,
			// NSValue related types:
			{ FullyQualifiedName: "CoreGraphics.CGAffineTransform" } => nsValueExpr, 
			{ FullyQualifiedName: "Foundation.NSRange" } =>  nsValueExpr, 
			{ FullyQualifiedName: "CoreGraphics.CGVector" } =>  nsValueExpr, 
			{ FullyQualifiedName: "SceneKit.SCNMatrix4" } => nsValueExpr, 
			{ FullyQualifiedName: "CoreLocation.CLLocationCoordinate2D" } => nsValueExpr, 
			{ FullyQualifiedName: "SceneKit.SCNVector3" } => nsValueExpr,
			{ FullyQualifiedName: "SceneKit.SCNVector4" } => nsValueExpr,
			{ FullyQualifiedName: "CoreGraphics.CGPoint" } => nsValueExpr,
			{ FullyQualifiedName: "CoreGraphics.CGRect" } => nsValueExpr,
			{ FullyQualifiedName: "CoreGraphics.CGSize" } => nsValueExpr,
			{ FullyQualifiedName: "UIKit.UIEdgeInsets" } => nsValueExpr,
			{ FullyQualifiedName: "UIKit.UIOffset" } => nsValueExpr,
			{ FullyQualifiedName: "MapKit.MKCoordinateSpan" } => nsNumberExpr,
			{ FullyQualifiedName: "CoreMedia.CMTimeRange" } => nsNumberExpr,
			{ FullyQualifiedName: "CoreMedia.CMTime" } => nsValueExpr, 
			{ FullyQualifiedName: "CoreMedia.CMTimeMapping" } => nsValueExpr, 
			{ FullyQualifiedName: "CoreAnimation.CATransform3D" } => nsValueExpr, 
			_ => null
		};
#pragma warning restore format
		
		if (constructor is null)
			return null;
		// generates:
		// obj => new NSNumber (obj);
		// obj => new NSValue (obj);
		// obj => obj.GetValue ();
		var lambdaExpression = SimpleLambdaExpression (Parameter (Identifier (lambdaFunctionVariable).WithTrailingTrivia (Space)))
			.WithExpressionBody (constructor.WithLeadingTrivia (Space));

		// generate: NSArray.FromNSObjects (o => new NSNumber (o), shape);
		var factoryInvocation = InvocationExpression (MemberAccessExpression (
			SyntaxKind.SimpleMemberAccessExpression,
			IdentifierName ("NSArray"),
			IdentifierName ("FromNSObjects").WithTrailingTrivia (Space))).WithArgumentList (
			ArgumentList (
				SeparatedList<ArgumentSyntax> (
					new SyntaxNodeOrToken [] {
						Argument (lambdaExpression),
						Token (SyntaxKind.CommaToken),
						Argument (IdentifierName (parameter.Name).WithLeadingTrivia (Space))
					})));

		var declarator =
			VariableDeclarator (Identifier (variableName).WithLeadingTrivia (Space).WithTrailingTrivia (Space))
				.WithInitializer (EqualsValueClause (factoryInvocation.WithLeadingTrivia (Space)));

		var declaration = VariableDeclaration (IdentifierName (Identifier (
				TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithVariables (SingletonSeparatedList (declarator));

		return LocalDeclarationStatement (declaration);
	}

	/// <summary>
	/// Returns the aux variable declaration needed when a parameter has the BindFrom attribute.
	/// </summary>
	/// <param name="parameter">The parameter whose aux variable we want to declare.</param>
	/// <returns>The syntax declaration of the aux variable or null if it could not be generated.</returns>
	internal static LocalDeclarationStatementSyntax? GetBindFromAuxVariable (in Parameter parameter)
	{
		if (parameter.BindAs is null)
			return null;

#pragma warning disable format
		// based on the bindas type call one of the helper factory methods
		return (Type: parameter.BindAs.Value.Type, IsArray: parameter.Type.IsArray) switch {
			{ IsArray: true } => GetNSArrayBindFromAuxVariable (parameter),
			{ Type: "Foundation.NSNumber" } => GetNSNumberAuxVariable (parameter),
			{ Type: "Foundation.NSValue" } => GetNSValueAuxVariable (parameter),
			{ Type: "Foundation.NSString" } => GetNSStringSmartEnumAuxVariable(parameter),
			_ => null,
		};
#pragma warning restore format
	}

	/// <summary>
	/// Generate the definition for an autorelease pool to wrap a method/property call.
	/// </summary>
	/// <returns>The variable declaration for a auto release pool.</returns>
	internal static LocalDeclarationStatementSyntax GetAutoreleasePoolVariable ()
	{
		const string poolVariableName = "autorelease_pool";
		//  object creation
		var create =
			ObjectCreationExpression (
					IdentifierName ("NSAutoreleasePool").WithLeadingTrivia (Space).WithTrailingTrivia (Space))
				.WithArgumentList (ArgumentList ());

		// return the autorelease pool definition 
		var declarator = VariableDeclarator (Identifier (poolVariableName));
		declarator = declarator.WithInitializer (EqualsValueClause (create.WithLeadingTrivia (Space))
				.WithLeadingTrivia (Space));

		var variableDeclaration = VariableDeclaration (IdentifierName (
				Identifier (TriviaList (), SyntaxKind.VarKeyword, "var", "var", TriviaList ())))
			.WithTrailingTrivia (Space)
			.WithVariables (SingletonSeparatedList (declarator));

		return LocalDeclarationStatement (variableDeclaration);
	}

	/// <summary>
	/// Generate the method that will check the ui thread based on the platform.
	/// </summary>
	/// <param name="platform">The platform targeted in the compilation.</param>
	/// <returns>The correct expression to check if the code is executing in the UI thread.</returns>
	internal static ExpressionStatementSyntax? EnsureUiThread (PlatformName platform)
	{
		(string Namespace, string Classname)? caller = platform switch {
			PlatformName.iOS => ("UIKit", "UIApplication"),
			PlatformName.TvOS => ("UIKit", "UIApplication"),
			PlatformName.MacCatalyst => ("UIKit", "UIApplication"),
			PlatformName.MacOSX => ("AppKit", "NSApplication"),
			_ => null
		};
		if (caller is null)
			return null;

		return ExpressionStatement (InvocationExpression (MemberAccessExpression (
			SyntaxKind.SimpleMemberAccessExpression,
			MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (caller.Value.Namespace),
				IdentifierName (caller.Value.Classname)),
			IdentifierName ("EnsureUIThread").WithTrailingTrivia (Space))));
	}

	/// <summary>
	/// Generate the declaration needed for the exception marhsaling.
	/// </summary>
	/// <returns>The local declaration needed for the exception marshaling.</returns>
	internal static LocalDeclarationStatementSyntax GetExceptionHandleAuxVariable ()
	{
		const string handleVariableName = "exception_gchandle";
		const string intPtr = "IntPtr";
		//  object creation
		var zeroPtr = MemberAccessExpression (
				SyntaxKind.SimpleMemberAccessExpression,
				IdentifierName (intPtr),
				IdentifierName ("Zero"));

		var declarator = VariableDeclarator (Identifier (handleVariableName))
				.WithInitializer (EqualsValueClause (zeroPtr));

		return LocalDeclarationStatement (
			VariableDeclaration (IdentifierName (intPtr))
				.WithVariables (SingletonSeparatedList (declarator)))
			.NormalizeWhitespace (); // no special mono style
	}

	static string? GetObjCMessageSendMethodName<T> (ExportData<T> exportData,
		TypeInfo returnType, ImmutableArray<Parameter> parameters, bool isSuper = false, bool isStret = false)
		where T : Enum
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

	public static (string? Getter, string? Setter) GetObjCMessageSendMethods (in Property property,
		bool isSuper = false, bool isStret = false)
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
		=> GetObjCMessageSendMethodName (method.ExportMethodData, method.ReturnType, method.Parameters, isSuper,
			isStret);
}
