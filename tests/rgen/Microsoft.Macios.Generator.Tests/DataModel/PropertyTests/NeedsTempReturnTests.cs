// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel.PropertyTests;

public class NeedsTempReturnTests : BaseGeneratorTestClass {
	
	class TestDataUseTempReturn : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string nsobjectProperty = @"
using System;
using ObjCBindings;
using Foundation;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual NSUrl AppStoreReceiptUrl { get; }			
}
";
			yield return [
				nsobjectProperty,
				true	
			];

			const string stringProperty = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual string BuiltinPluginsPath { get; }
";
			yield return [
				stringProperty, 
				false 
			];
			
			const string boolProperty = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual bool BuiltinPluginsPath { get; }
";
			yield return [
				boolProperty, 
				true
			];
			
			const string charProperty = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual char BuiltinPluginsPath { get; }
";
			yield return [
				charProperty, 
				true
			];
			
			const string nfloatProperty = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual nfloat BuiltinPluginsPath { get; }
";
			yield return [
				nfloatProperty, 
				false 
			];
			
			const string stretType = @"
using System;
using ObjCBindings;
using CoreGraphics;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual NMatrix4 BuiltinPluginsPath { get; }
";
			yield return [
				stretType, 
				true	
			];
			
			const string nativeEnumType = @"
using System;
using AVFoundation;
using ObjCBindings;
using CoreGraphics;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual AVAudioApplicationMicrophoneInjectionPermission BuiltinPluginsPath { get; }
";
			yield return [
				nativeEnumType, 
				true	
			];
			
			const string delegateProperty = @"
using System;
using AVFoundation;
using ObjCBindings;
using CoreGraphics;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"")]
	public virtual Action<string> BuiltinPluginsPath { get; }
";
			yield return [
				delegateProperty, 
				true	
			];

			const string marshalNativeExceptions = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Property>(""appStoreReceiptURL"", Flags = Property.MarshalNativeExceptions)]
	public virtual string BuiltinPluginsPath { get; }
";
			yield return [
				marshalNativeExceptions, 
				true	
			];

		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}
	
	[Theory]
	[AllSupportedPlatformsClassData<TestDataUseTempReturn>]
	void UseTempReturnTests (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Property.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes.Value.UseTempReturn);
	}
}
