// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class TypeSymbolExtensionsInheritanceTests : BaseGeneratorTestClass {

	readonly CollectionComparer<string> comparer = new CollectionComparer<string> ();

	class TestDataInheritanceClasses : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string simpleClass = @"
using System;

namespace NS;

public class TestClass {}
";
			yield return [simpleClass, false, false, new [] { "object" }, new string [] { }];

			const string genericClass = @"
using System;

namespace NS;

public class TestClass<T> where T Enum {}
";
			yield return [genericClass, false, false, new [] { "object" }, new string [] { }];

			const string singleParent = @"
using System;

namespace NS;

public class Parent {}
public class TestClass : Parent {}
";

			yield return [singleParent, false, false, new [] { "NS.Parent", "object" }, new string [] { }];

			const string genericParent = @"
using System;

namespace NS;

public class Parent<T> where T object {}
public class TestClass : Parent<string> {}
";

			yield return [genericParent, false, false, new [] { "NS.Parent<string>", "object" }, new string [] { }];

			const string multiParent = @"
using System;

namespace NS;

public class Parent0 {}
public class Parent1 : Parent0 {}
public class TestClass : Parent1 {}
";

			yield return [multiParent, false, false, new [] { "NS.Parent1", "NS.Parent0", "object" }, new string [] { }];

			const string singleInterface = @"
using System;

namespace NS;

public interface IInterface {}
public class TestClass : IInterface {}
";

			yield return [singleInterface, false, false, new [] { "object" }, new [] { "NS.IInterface" }];

			const string genericInterface = @"
using System;

namespace NS;

public interface IInterface<T> where T : object {}
public class TestClass : IInterface<string> {}
";

			yield return [genericInterface, false, false, new [] { "object" }, new [] { "NS.IInterface<string>" }];

			const string severalInterfaces = @"
using System;

namespace NS;

public interface IInterface1 {}
public interface IInterface2 {}
public class TestClass : IInterface1, IInterface2  {}
";

			yield return [severalInterfaces, false, false, new [] { "object" }, new [] { "NS.IInterface1", "NS.IInterface2" }];

			const string severalGenericInterfaces = @"
using System;

namespace NS;

public interface IInterface1<T> where T : object {}
public class TestClass : IInterface1<string>, IInterface1<int>  {}
";

			yield return [severalGenericInterfaces, false, false, new [] { "object" }, new [] { "NS.IInterface1<string>", "NS.IInterface1<int>" }];

			const string parentSingleInterface = @"
using System;

namespace NS;

public interface IInterface {}
public class Parent : IInterface {}
public class TestClass : Parent {}
";

			yield return [parentSingleInterface, false, false, new [] { "NS.Parent", "object" }, new [] { "NS.IInterface" }];

			const string nsObjectChild = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;
public partial class AVCaptureDataOutputSynchronizer : NSObject
{
	public override NativeHandle ClassHandle { get { throw new PlatformNotSupportedException (); } }

	protected AVCaptureDataOutputSynchronizer (NSObjectFlag t) : base (t)
	{
		throw new PlatformNotSupportedException ();
	}
}	
";

			yield return [nsObjectChild,
				true,
				true,
				new [] { "Foundation.NSObject", "object" },
				new [] {
				"ObjCRuntime.INativeObject",
				"System.IEquatable<Foundation.NSObject>",
				"System.IDisposable",
				"Foundation.INSObjectFactory",
				"Foundation.INSObjectProtocol",
			}];

			const string nsObjectNestedChild = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;
public partial class AVCaptureDataOutputSynchronizer : NSObject
{
	public override NativeHandle ClassHandle { get { throw new PlatformNotSupportedException (); } }

	protected AVCaptureDataOutputSynchronizer (NSObjectFlag t) : base (t)
	{
		throw new PlatformNotSupportedException ();
	}
}	

public partial class Child : AVCaptureDataOutputSynchronizer {}
";

			yield return [nsObjectNestedChild,
				true,
				true,
				new [] { "NS.AVCaptureDataOutputSynchronizer", "Foundation.NSObject", "object" },
				new [] {
				"ObjCRuntime.INativeObject",
				"System.IEquatable<Foundation.NSObject>",
				"System.IDisposable",
				"Foundation.INSObjectFactory",
				"Foundation.INSObjectProtocol",
			}];

			const string nativeObjectInterface = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;
public partial class AVCaptureDataOutputSynchronizer : INativeObject
{
	public override NativeHandle ClassHandle { get { throw new PlatformNotSupportedException (); } }

	protected AVCaptureDataOutputSynchronizer (NSObjectFlag t) : base (t)
	{
		throw new PlatformNotSupportedException ();
	}
}	
";

			yield return [nativeObjectInterface,
				false,
				true,
				new [] { "object" },
				new [] {
				"ObjCRuntime.INativeObject",
			}];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataInheritanceClasses>]
	void GetInheritance (ApplePlatform platform, string inputText,
		bool expectedIsNSObject, bool expectedIsNativeObject, string [] expectedParents, string [] expectedInterfaces)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault (); // always grab the last one, you might get into failures if you are not careful with this
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		symbol.GetInheritance (
			isNSObject: out var isNsObject,
			isNativeObject: out var isNativeObject,
			parents: out var parents,
			interfaces: out var interfaces);
		Assert.Equal (expectedIsNSObject, isNsObject);
		Assert.Equal (expectedIsNativeObject, isNativeObject);
		Assert.Equal (expectedParents, parents);
		Assert.Equal (expectedInterfaces, interfaces);
	}
}
