// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xunit;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class BindingSyntaxFactoryObjCRuntimeTests {
	class TestDataCastToNativeTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// not enum parameter
			var boolParam = new Parameter (
				position: 0,
				type: ReturnTypeForBool (),
				name: "myParam");
			yield return [boolParam, null!];

			// not smart enum parameter
			var enumParam = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false),
				name: "myParam");

			yield return [enumParam, null!];

			// int64
			var byteEnum = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: true, underlyingType: SpecialType.System_Int64),
				name: "myParam");
			yield return [byteEnum, "(IntPtr) (long) myParam"];

			// uint64
			var int64Enum = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: true, underlyingType: SpecialType.System_UInt64),
				name: "myParam");
			yield return [int64Enum, "(UIntPtr) (ulong) myParam"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataCastToNativeTests))]
	void CastToNativeTests (Parameter parameter, string? expectedCast)
	{
		var expression = CastToNative (parameter);
		if (expectedCast is null) {
			Assert.Null (expression);
		} else {
			Assert.NotNull (expression);
			Assert.Equal (expectedCast, expression?.ToString ());
		}
	}

	class TestDataCastToPrimitive : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// not enum parameter
			var boolParam = new Parameter (
				position: 0,
				type: ReturnTypeForBool (),
				name: "myParam");
			yield return [boolParam, null!];

			var enumParam = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false),
				name: "myParam");

			yield return [enumParam, "(int) myParam"];

			var byteParam = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false, underlyingType: SpecialType.System_Byte),
				name: "myParam");

			yield return [byteParam, "(byte) myParam"];


			var longParam = new Parameter (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", isNativeEnum: false, underlyingType: SpecialType.System_Int64),
				name: "myParam");

			yield return [longParam, "(long) myParam"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataCastToPrimitive))]
	void CastToPrimitiveTests (Parameter parameter, string? expectedCast)
	{
		var expression = CastToPrimitive (parameter);
		if (expectedCast is null) {
			Assert.Null (expression);
		} else {
			Assert.NotNull (expression);
			Assert.Equal (expectedCast, expression?.ToString ());
		}
	}

	class TestDataByteToBoolTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				InvocationExpression (
					MemberAccessExpression (
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName ("NSObject"),
						IdentifierName ("TestFunction").WithTrailingTrivia (Space))),
				"NSObject.TestFunction () != 0"
			];

			yield return [
				InvocationExpression (
						MemberAccessExpression (
							SyntaxKind.SimpleMemberAccessExpression,
							IdentifierName ("NSObject"),
							IdentifierName ("TestFunction").WithTrailingTrivia (Space)))
					.WithArgumentList (
						ArgumentList (
							SingletonSeparatedList<ArgumentSyntax> (
								Argument (
									IdentifierName ("arg1"))))),
				"NSObject.TestFunction (arg1) != 0"
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataByteToBoolTests))]
	void ByteToBoolTests (InvocationExpressionSyntax invocationExpressionSyntax, string expectedDeclaration)
	{
		var declaration = ByteToBool (invocationExpressionSyntax);
		Assert.Equal (expectedDeclaration, declaration.ToString ());
	}

	[Fact]
	void CastToByteTests ()
	{
		var boolParameter = new Parameter (0, ReturnTypeForBool (), "myParameter");
		var conditionalExpr = CastToByte (boolParameter);
		Assert.NotNull (conditionalExpr);
		Assert.Equal ("myParameter ? (byte) 1 : (byte) 0", conditionalExpr.ToString ());

		var intParameter = new Parameter (1, ReturnTypeForInt (), "myParameter");
		conditionalExpr = CastToByte (intParameter);
		Assert.Null (conditionalExpr);
	}

	class TestDataGetNSArrayAuxVariableTest : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// not array 

			yield return [
				new Parameter (0, ReturnTypeForInt (isNullable: false), "myParam"),
				null!,
				false
			];

			// not nullable string[]
			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: false), "myParam"),
				"var nsa_myParam = NSArray.FromStrings (myParam);",
				false
			];

			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: false), "myParam"),
				"using var nsa_myParam = NSArray.FromStrings (myParam);",
				true
			];

			// nullable string []
			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: true), "myParam"),
				"var nsa_myParam = myParam is null ? null : NSArray.FromStrings (myParam);",
				false
			];

			yield return [
				new Parameter (0, ReturnTypeForArray ("string", isNullable: true), "myParam"),
				"using var nsa_myParam = myParam is null ? null : NSArray.FromStrings (myParam);",
				true
			];

			// nsstrings

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: false), "myParam"),
				"var nsa_myParam = NSArray.FromNSObjects (myParam);",
				false
			];

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: false), "myParam"),
				"using var nsa_myParam = NSArray.FromNSObjects (myParam);",
				true
			];

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: true), "myParam"),
				"var nsa_myParam = myParam is null ? null : NSArray.FromNSObjects (myParam);",
				false
			];

			yield return [
				new Parameter (0, ReturnTypeForArray ("NSString", isNullable: true), "myParam"),
				"using var nsa_myParam = myParam is null ? null : NSArray.FromNSObjects (myParam);",
				true
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetNSArrayAuxVariableTest))]
	void GetNSArrayAuxVariableTests (in Parameter parameter, string? expectedDeclaration, bool withUsing)
	{
		var declaration = GetNSArrayAuxVariable (in parameter);
		if (withUsing && expectedDeclaration is not null)
			declaration = Using (GetNSArrayAuxVariable (in parameter)!);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetHandleAuxVariableTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// nsobject type
			yield return [
				new Parameter (0, ReturnTypeForNSObject ("MyNSObject"), "myParam"),
				"var myParam__handle__ = myParam.GetHandle ();",
				false
			];

			yield return [
				new Parameter (0, ReturnTypeForNSObject ("MyNSObject"), "myParam"),
				"var myParam__handle__ = myParam!.GetNonNullHandle ( nameof (myParam));",
				true
			];

			// interface type
			yield return [
				new Parameter (0, ReturnTypeForINativeObject ("MyNativeObject"), "myParam"),
				"var myParam__handle__ = myParam.GetHandle ();",
				false
			];

			yield return [
				new Parameter (0, ReturnTypeForINativeObject ("MyNativeObject"), "myParam"),
				"var myParam__handle__ = myParam!.GetNonNullHandle ( nameof (myParam));",
				true
			];

			// value type
			yield return [
				new Parameter (0, ReturnTypeForBool (), "myParam"),
				null!,
				false
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetHandleAuxVariableTests))]
	void GetHandleAuxVariableTests (in Parameter parameter, string? expectedDeclaration, bool withNullAllowed)
	{
		var declaration = GetHandleAuxVariable (parameter, withNullAllowed: withNullAllowed);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetStringAuxVariableTest : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new Parameter (0, ReturnTypeForString (), "myParam"),
				"var nsmyParam = CFString.CreateNative (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForBool (), "myParam"),
				null!,
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetStringAuxVariableTest))]
	void GetStringAuxVariableTests (in Parameter parameter, string? expectedDeclaration)
	{
		var declaration = GetStringAuxVariable (parameter);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetNSNumberAuxVariableTest : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new Parameter (0, ReturnTypeForInt (), "myParam"),
				"var nsb_myParam = NSNumber.FromInt32 (myParam);"
			];

			yield return [
				new Parameter (0, ReturnTypeForInt (isUnsigned: true), "myParam"),
				"var nsb_myParam = NSNumber.FromUInt32 (myParam);"
			];

			yield return [
				new Parameter (0, ReturnTypeForBool (), "myParam"),
				"var nsb_myParam = NSNumber.FromBoolean (myParam);"
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum"), "myParam"),
				"var nsb_myParam = NSNumber.FromInt32 ((int) myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_Byte), "myParam"),
				"var nsb_myParam = NSNumber.FromByte ((byte) myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_SByte), "myParam"),
				"var nsb_myParam = NSNumber.FromSByte ((sbyte) myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_Int16), "myParam"),
				"var nsb_myParam = NSNumber.FromInt16 ((short) myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_UInt16), "myParam"),
				"var nsb_myParam = NSNumber.FromUInt16 ((ushort) myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_Int64), "myParam"),
				"var nsb_myParam = NSNumber.FromInt64 ((long) myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_UInt64), "myParam"),
				"var nsb_myParam = NSNumber.FromUInt64 ((ulong) myParam);",
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetNSNumberAuxVariableTest))]
	void GetNSNumberAuxVariableTests (in Parameter parameter, string? expectedDeclaration)
	{
		var declaration = GetNSNumberAuxVariable (parameter);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetNSValueAuxVariableTest : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreGraphics.CGAffineTransform"), "myParam"),
				"var nsb_myParam = NSValue.FromCGAffineTransform (myParam);"
			];
			yield return [
				new Parameter (0, ReturnTypeForStruct ("Foundation.NSRange"), "myParam"),
				"var nsb_myParam = NSValue.FromRange (myParam);"
			];
			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreGraphics.CGVector"), "myParam"),
				"var nsb_myParam = NSValue.FromCGVector (myParam);"
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("SceneKit.SCNMatrix4"), "myParam"),
				"var nsb_myParam = NSValue.FromSCNMatrix4 (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreLocation.CLLocationCoordinate2D"), "myParam"),
				"var nsb_myParam = NSValue.FromMKCoordinate (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("SceneKit.SCNVector3"), "myParam"),
				"var nsb_myParam = NSValue.FromVector (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("SceneKit.SCNVector4"), "myParam"),
				"var nsb_myParam = NSValue.FromVector (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreGraphics.CGPoint"), "myParam"),
				"var nsb_myParam = NSValue.FromCGPoint (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreGraphics.CGRect"), "myParam"),
				"var nsb_myParam = NSValue.FromCGRect (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreGraphics.CGSize"), "myParam"),
				"var nsb_myParam = NSValue.FromCGSize (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("UIKit.UIEdgeInsets"), "myParam"),
				"var nsb_myParam = NSValue.FromUIEdgeInsets (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("UIKit.UIOffset"), "myParam"),
				"var nsb_myParam = NSValue.FromUIOffset (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("MapKit.MKCoordinateSpan"), "myParam"),
				"var nsb_myParam = NSValue.FromMKCoordinateSpan (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreMedia.CMTimeRange"), "myParam"),
				"var nsb_myParam = NSValue.FromCMTimeRange (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreMedia.CMTime"), "myParam"),
				"var nsb_myParam = NSValue.FromCMTime (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreMedia.CMTimeMapping"), "myParam"),
				"var nsb_myParam = NSValue.FromCMTimeMapping (myParam);",
			];

			yield return [
				new Parameter (0, ReturnTypeForStruct ("CoreAnimation.CATransform3D"), "myParam"),
				"var nsb_myParam = NSValue.FromCATransform3D (myParam);",
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetNSValueAuxVariableTest))]
	void GetNSValueAuxVariableTests (in Parameter parameter, string? expectedDeclaration)
	{
		var declaration = GetNSValueAuxVariable (parameter);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetNSStringSmartEnumAuxVariableTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				new Parameter (0, ReturnTypeForEnum ("CoreAnimation.CATransform3D", isSmartEnum: true), "myParam"),
				"var nsb_myParam = myParam.GetConstant ();",
			];

			yield return [
				new Parameter (0, ReturnTypeForEnum ("CoreAnimation.CATransform3D", isSmartEnum: false), "myParam"),
				null!
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetNSStringSmartEnumAuxVariableTests))]
	void GetNSStringSmartEnumAuxVariableTests (in Parameter parameter, string? expectedDeclaration)
	{
		var declaration = GetNSStringSmartEnumAuxVariable (parameter);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetNSArrayBindFromAuxVariableTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// nsnumber
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("nint"),
					name: "myParam") { BindAs = new ("Foundation.NSNumber"), },
				"var nsb_myParam = NSArray.FromNSObjects (obj => new NSNumber (obj), myParam);"
			];

			// nsvalue
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("CoreGraphics.CGAffineTransform", isStruct: true),
					name: "myParam") { BindAs = new ("Foundation.NSValue"), },
				"var nsb_myParam = NSArray.FromNSObjects (obj => new NSValue (obj), myParam);"
			];

			// smart enum
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("MySmartEnum", isEnum: true, isSmartEnum: true),
					name: "myParam") { BindAs = new ("Foundation.NSString"), },
				"var nsb_myParam = NSArray.FromNSObjects (obj => obj.GetConstant(), myParam);"
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetNSArrayBindFromAuxVariableTests))]
	void GetNSArrayBindFromAuxVariableTests (in Parameter parameter, string? expectedDeclaration)
	{
		var declaration = GetNSArrayBindFromAuxVariable (parameter);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	class TestDataGetBindFromAuxVariableTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// nsnumber
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_UInt64),
					name: "myParam") { BindAs = new ("Foundation.NSNumber"), },
				"var nsb_myParam = NSNumber.FromUInt64 ((ulong) myParam);",
			];

			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("nint"),
					name: "myParam") { BindAs = new ("Foundation.NSNumber"), },
				"var nsb_myParam = NSArray.FromNSObjects (obj => new NSNumber (obj), myParam);"
			];

			// nsvalue	
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForStruct ("CoreAnimation.CATransform3D"),
					name: "myParam") { BindAs = new ("Foundation.NSValue"), },
				"var nsb_myParam = NSValue.FromCATransform3D (myParam);",
			];

			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("CoreGraphics.CGAffineTransform", isStruct: true),
					name: "myParam") { BindAs = new ("Foundation.NSValue"), },
				"var nsb_myParam = NSArray.FromNSObjects (obj => new NSValue (obj), myParam);"
			];

			// smart enum
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForEnum ("CoreAnimation.CATransform3D", isSmartEnum: true),
					name: "myParam") { BindAs = new ("Foundation.NSString"), },
				"var nsb_myParam = myParam.GetConstant ();",
			];

			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForArray ("MySmartEnum", isEnum: true, isSmartEnum: true),
					name: "myParam") { BindAs = new ("Foundation.NSString"), },
				"var nsb_myParam = NSArray.FromNSObjects (obj => obj.GetConstant(), myParam);"
			];

			//missing attr
			yield return [
				new Parameter (
					position: 0,
					type: ReturnTypeForEnum ("CoreAnimation.CATransform3D", isSmartEnum: true),
					name: "myParam"),
				null!
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetBindFromAuxVariableTests))]
	void GetBindFromAuxVariableTests (in Parameter parameter, string? expectedDeclaration)
	{
		var declaration = GetBindFromAuxVariable (parameter);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	[Fact]
	void GetAutoreleasePoolVariableTests ()
	{
		const string expected = "var autorelease_pool = new NSAutoreleasePool ();";
		var declaration = GetAutoreleasePoolVariable ();
		Assert.NotNull (declaration);
		Assert.Equal (expected, declaration.ToString ());
	}

	[Theory]
	[InlineData (PlatformName.iOS, "UIKit.UIApplication.EnsureUIThread ();")]
	[InlineData (PlatformName.TvOS, "UIKit.UIApplication.EnsureUIThread ();")]
	[InlineData (PlatformName.MacCatalyst, "UIKit.UIApplication.EnsureUIThread ();")]
	[InlineData (PlatformName.MacOSX, "AppKit.NSApplication.EnsureUIThread ();")]
	[InlineData (PlatformName.WatchOS, null)]
	[InlineData (PlatformName.None, null)]
	void EnsureUiThreadTests (PlatformName platform, string? expectedDeclaration)
	{
		var declaration = EnsureUiThread (platform);
		if (expectedDeclaration is null) {
			Assert.Null (declaration);
		} else {
			Assert.NotNull (declaration);
			Assert.Equal (expectedDeclaration, declaration.ToString ());
		}
	}

	[Fact]
	void GetExceptionHandleAuxVariableTests ()
	{
		var expected = "IntPtr exception_gchandle = IntPtr.Zero;";
		var declaration = GetExceptionHandleAuxVariable ();
		Assert.Equal (expected, declaration.ToString ());
	}

	class TestDataUsingTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				GetAutoreleasePoolVariable (),
				"using var autorelease_pool = new NSAutoreleasePool ();",
			];

			Parameter parameter = new (
				position: 0,
				type: ReturnTypeForEnum ("MyEnum", underlyingType: SpecialType.System_UInt64),
				name: "myParam") { BindAs = new ("Foundation.NSNumber") };

			yield return [
				GetBindFromAuxVariable (parameter)!,
				"using var nsb_myParam = NSNumber.FromUInt64 ((ulong) myParam);",
			];

			parameter = new (
				position: 0,
				type: ReturnTypeForArray ("nint"),
				name: "myParam") { BindAs = new ("Foundation.NSNumber") };

			yield return [
				GetBindFromAuxVariable (parameter)!,
				"using var nsb_myParam = NSArray.FromNSObjects (obj => new NSNumber (obj), myParam);",
			];

			parameter = new (
				position: 0,
				type: ReturnTypeForStruct ("CoreAnimation.CATransform3D"),
				name: "myParam") { BindAs = new ("Foundation.NSValue") };

			yield return [
				GetBindFromAuxVariable (parameter)!,
				"using var nsb_myParam = NSValue.FromCATransform3D (myParam);",
			];

			parameter = new (
				position: 0,
				type: ReturnTypeForArray ("CoreGraphics.CGAffineTransform", isStruct: true),
				name: "myParam") { BindAs = new ("Foundation.NSValue") };

			yield return [
				GetBindFromAuxVariable (parameter)!,
				"using var nsb_myParam = NSArray.FromNSObjects (obj => new NSValue (obj), myParam);",
			];

			parameter = new (
				position: 0,
				type: ReturnTypeForEnum ("CoreAnimation.CATransform3D", isSmartEnum: true),
				name: "myParam") { BindAs = new ("Foundation.NSString"), };

			yield return [
				GetBindFromAuxVariable (parameter)!,
				"using var nsb_myParam = myParam.GetConstant ();",
			];

			parameter = new Parameter (
				position: 0,
				type: ReturnTypeForArray ("MySmartEnum", isEnum: true, isSmartEnum: true),
				name: "myParam") { BindAs = new ("Foundation.NSString"), };

			yield return [
				GetBindFromAuxVariable (parameter)!,
				"using var nsb_myParam = NSArray.FromNSObjects (obj => obj.GetConstant(), myParam);",
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataUsingTests))]
	void UsingTests (LocalDeclarationStatementSyntax declaration, string expectedDeclaration)
		=> Assert.Equal (expectedDeclaration, Using (declaration).ToString ());

	class TestDataGetReturnValueAuxVariable : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				ReturnTypeForBool (),
				"ret",
				"bool ret;"
			];

			yield return [
				ReturnTypeForString (),
				"ret",
				"string ret;"
			];

			yield return [
				ReturnTypeForString (isNullable: true),
				"ret",
				"string? ret;"
			];

			yield return [
				ReturnTypeForNSObject ("NSLocale"),
				"ret",
				"NSLocale ret;"
			];

			yield return [
				ReturnTypeForNSObject ("NSLocale", isNullable: true),
				"ret",
				"NSLocale? ret;"
			];

			yield return [
				ReturnTypeForStruct ("MyStruct"),
				"ret",
				"MyStruct ret;"
			];

			yield return [
				ReturnTypeForArray ("int"),
				"ret",
				"int[] ret;"
			];

			yield return [
				ReturnTypeForArray ("int", isNullable: true),
				"ret",
				"int[]? ret;"
			];

			yield return [
				ReturnTypeForArray ("int?", isNullable: true),
				"ret",
				"int?[]? ret;"
			];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetReturnValueAuxVariable))]
	void GetReturnValueAuxVariableTests (TypeInfo typeInfo, string expectedVariable, string expectedDeclaration)
	{
		var (name, declaration) = GetReturnValueAuxVariable (typeInfo);
		Assert.Equal (expectedVariable, name);
		Assert.Equal (expectedDeclaration, declaration.ToString ());
	}

	class TestDataGetSelectorStringField : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// selection of example selectors
			yield return [
				"RTFDFileWrapperFromRange:documentAttributes:",
				"selRTFDFileWrapperFromRange_DocumentAttributes_XHandle",
				"const string selRTFDFileWrapperFromRange_DocumentAttributes_X = \"RTFDFileWrapperFromRange:documentAttributes:\";"
			];

			yield return [
				"RTFDFromRange:documentAttributes:",
				"selRTFDFromRange_DocumentAttributes_XHandle",
				"const string selRTFDFromRange_DocumentAttributes_X = \"RTFDFromRange:documentAttributes:\";"
			];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetSelectorStringField))]
	void GetSelectorStringFieldTest (string selector, string selectorName, string expectedDeclaration)
		=> Assert.Equal (expectedDeclaration, GetSelectorStringField (selector, selectorName).ToString ());

	class TestDataGetSelectorHandleField : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// selection of example selectors
			yield return [
				"RTFDFileWrapperFromRange:documentAttributes:",
				"selRTFDFileWrapperFromRange_DocumentAttributes_XHandle",
				"static readonly NativeHandle selRTFDFileWrapperFromRange_DocumentAttributes_XHandle = Selector.GetHandle (\"RTFDFileWrapperFromRange:documentAttributes:\");"
			];

			yield return [
				"RTFDFromRange:documentAttributes:",
				"selRTFDFromRange_DocumentAttributes_XHandle",
				"static readonly NativeHandle selRTFDFromRange_DocumentAttributes_XHandle = Selector.GetHandle (\"RTFDFromRange:documentAttributes:\");"
			];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetSelectorHandleField))]
	void GetSelectorHandleFieldTest (string selector, string selectorName, string expectedDeclaration)
		=> Assert.Equal (expectedDeclaration, GetSelectorHandleField (selector, selectorName).ToString ());
}
