// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel.MethodTests;

public class NeedsTempReturnTests : BaseGeneratorTestClass {
	
	class TestDataUseTempReturn : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string nsobjectMethod = @"
using System;
using ObjCBindings;
using Foundation;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual NSUrl GetAppStoreReceiptUrl ();			
}
";
			yield return [
				nsobjectMethod,
				true
			];
			
			const string stringMethod = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual string GetBuiltinPluginsPath ();
";
			yield return [
				stringMethod, 
				false 
			];
			
			const string boolMethod = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual bool GetBuiltinPluginsPath ()
";
			yield return [
				boolMethod, 
				true
			];
			
			const string charMethod = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual char GetBuiltinPluginsPath ();
";
			yield return [
				charMethod, 
				true
			];
			
			const string nfloatMethod = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual nfloat GetBuiltinPluginsPath ();
";
			yield return [
				nfloatMethod, 
				false 
			];
			
			const string stretType = @"
using System;
using ObjCBindings;
using CoreGraphics;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual NMatrix4 GetBuiltinPluginsPath ();
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
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual AVAudioApplicationMicrophoneInjectionPermission GetBuiltinPluginsPath ();
";
			yield return [
				nativeEnumType, 
				true	
			];
			
			const string delegateMethod = @"
using System;
using AVFoundation;
using ObjCBindings;
using CoreGraphics;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual Action<string> GetBuiltinPluginsPath ();
";
			yield return [
				delegateMethod, 
				true	
			];

			const string marshalNativeExceptions = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"", Flags = Method.MarshalNativeExceptions)]
	public virtual string GetBuiltinPluginsPath ();
";
			yield return [
				marshalNativeExceptions, 
				true	
			];
			
			const string factoryMethod = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"", Flags = Method.Factory)]
	public virtual string GetBuiltinPluginsPath ();
";
			yield return [
				factoryMethod, 
				true	
			];
			
			const string byRefParameters = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {
	[Export<Method>(""appStoreReceiptURL"")]
	public virtual string GetBuiltinPluginsPath (out NSError error);
";
			yield return [
				byRefParameters, 
				true	
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}
		
	[Theory]
	[AllSupportedPlatformsClassData<TestDataUseTempReturn>]
	void FromMethodDeclaration (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Method.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes.Value.UseTempReturn);
	}
}
