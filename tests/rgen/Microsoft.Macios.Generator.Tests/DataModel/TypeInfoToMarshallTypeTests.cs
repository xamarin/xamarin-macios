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

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class TypeInfoToMarshallTypeTests : BaseGeneratorTestClass {

	class TestDataToMarshallType : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string nfloatProperty = @"
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace NS;

public class Example {
	public nfloat Texturing { get; set; }
}
";
			yield return [nfloatProperty, "nfloat"];

			const string otherNfloatProperty = @"
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace NS;

public class Example {
	public NFloat Texturing { get; set; }
}
";
			yield return [otherNfloatProperty, "nfloat"];

			const string systemString = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;

public class Example {
	public string Texturing { get; set; }
}
";

			yield return [systemString, "NativeHandle"];

			const string nsstring = @"
using System;
using Foundation;
using ObjCRuntime;

namespace NS;

public class Example {
	public NSString Texturing { get; set; }
}
";

			yield return [nsstring, "NativeHandle"];

			//const string nsobject 

			const string nativeEnumInt64 = @"
using System;
using ObjCRuntime;

namespace NS;

[Native]
public enum AREnvironmentTexturing : long {
	None,
	Manual,
	Automatic,
}

public class Example {
	public AREnvironmentTexturing Texturing { get; set; }
}
";
			yield return [nativeEnumInt64, "IntPtr"];

			const string nativeEnumUInt64 = @"
using System;
using ObjCRuntime;

namespace NS;

[Native]
public enum AREnvironmentTexturing : ulong {
	None,
	Manual,
	Automatic,
}

public class Example {
	public AREnvironmentTexturing Texturing { get; set; }
}
";
			yield return [nativeEnumUInt64, "UIntPtr"];

			const string smartEnum = @"
using System;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType]
public enum AREnvironmentTexturing : ulong {
	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	None,
}

public class Example {
	public AREnvironmentTexturing Texturing { get; set; }
}
";
			yield return [smartEnum, "NativeHandle"];

			const string normalEnum = @"
using System;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

public enum AREnvironmentTexturing : ulong {
	None,
}

public class Example {
	public AREnvironmentTexturing Texturing { get; set; }
}
";

			yield return [normalEnum, "ulong"];

			const string boolProperty = @"
using System;

namespace NS;

public class Example {
	public bool Texturing { get; set; }
}
";
			yield return [boolProperty, "bool"];

			const string intProperty = @"
using System;

namespace NS;

public class Example {
	public int Texturing { get; set; }
}
";
			yield return [intProperty, "int"];

			const string structureProperty = @"
using System;

namespace NS;

public struct Point {
	int x;
	int y;
}

public class Example {
	public Point Centre { get; set; }
}
";
			yield return [structureProperty, "Point"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataToMarshallType>]
	void ToMarshallType (ApplePlatform platform, string inputText, string expectedTypeName)
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
		var marshall = changes.Value.ReturnType.ToMarshallType ();
		Assert.NotNull (marshall);
		Assert.Equal (expectedTypeName, marshall);
	}
}
