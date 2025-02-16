// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class GetObjCMessageSendMethodNameTests : BaseGeneratorTestClass {

	class TestDataGetObjCMessageSendMethodName : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string propertyNSObjectGetter = @"
using System;
using ARKit;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property>(""identifier"")]
	public NSUuid Identifier { get; }
}
";
			yield return [propertyNSObjectGetter, "NativeHandle_objc_msgSend", null!, false, false];

			const string propertyNSObjectGetterSetter = @"
using System;
using ARKit;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property>(""identifier"")]
	public NSUuid Identifier { get; set; }
}
";
			yield return [propertyNSObjectGetterSetter, "NativeHandle_objc_msgSend", "void_objc_msgSend_NativeHandle", false, false];

			const string propertyNSStringGetter = @"
using System;
using ARKit;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property>(""name"")]
	public string Name { get; }
}
";

			yield return [propertyNSStringGetter, "NativeHandle_objc_msgSend", null!, false, false];

			const string propertyNSStringGetterSetter = @"
using System;
using ARKit;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property>(""name"")]
	public string Name { get; set; }
}
";

			yield return [propertyNSStringGetterSetter, "NativeHandle_objc_msgSend", "void_objc_msgSend_NativeHandle", false, false];

			const string customMarshall = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""transform"")]
	NMatrix4 Transform {
		[Export<ObjCBindings.Property> (
			""transform"", 
			Flags = ObjCBindings.Property.CustomMarshalDirective, 
			NativePrefix = ""xamarin_simd__"", 
			Library = ""__Internal"")]
		get;
	}
}
";

			yield return [customMarshall, "xamarin_simd__NMatrix4_objc_msgSend", null!, false, false];

			const string floatPropertyGetterSetter = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {
		[Export<ObjCBindings.Property> (""radius"")]
		float Radius { get; set; }	
}
";
			yield return [floatPropertyGetterSetter, "float_objc_msgSend", "void_objc_msgSend_float", false, false];

			const string nativeEnum = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[Native]
public enum ARAppClipCodeUrlDecodingState : long {
	Decoding,
	Failed,
	Decoded,
}

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""urlDecodingState"")]
	ARAppClipCodeUrlDecodingState UrlDecodingState { get; set; }

}
";

			yield return [nativeEnum, "IntPtr_objc_msgSend", "void_objc_msgSend_IntPtr", false, false];

			const string nativeEnumUnsigned = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[Native]
public enum ARAppClipCodeUrlDecodingState : ulong {
	Decoding,
	Failed,
	Decoded,
}

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""urlDecodingState"")]
	ARAppClipCodeUrlDecodingState UrlDecodingState { get; set; }

}
";

			yield return [nativeEnumUnsigned, "UIntPtr_objc_msgSend", "void_objc_msgSend_UIntPtr", false, false];

			const string boolProperty = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""tracked"")]
	bool IsTracked { get; set; }

}
";

			yield return [boolProperty, "bool_objc_msgSend", "void_objc_msgSend_bool", false, false];

			const string nfloatProperty = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""tracked"")]
	nfloat IsTracked { get; set; }

}
";

			yield return [nfloatProperty, "nfloat_objc_msgSend", "void_objc_msgSend_nfloat", false, false];

			const string nintProperty = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""tracked"")]
	nint IsTracked { get; set; }

}
";

			yield return [nintProperty, "IntPtr_objc_msgSend", "void_objc_msgSend_IntPtr", false, false];

			const string nuintProperty = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""tracked"")]
	nuint IsTracked { get; set; }

}
";

			yield return [nuintProperty, "UIntPtr_objc_msgSend", "void_objc_msgSend_UIntPtr", false, false];

			const string cgsizeProperty = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""tracked"")]
	CGSize IsTracked { get; set; }

}
";

			yield return [cgsizeProperty, "CGSize_objc_msgSend", "void_objc_msgSend_CGSize", false, false];

			const string doubleProperty = @"
using System;
using ARKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {

	[Export<ObjCBindings.Property> (""tracked"")]
	double IsTracked { get; set; }

}
";

			yield return [doubleProperty, "Double_objc_msgSend", "void_objc_msgSend_Double", false, false];

			const string stringArrayProperty = @"
using System;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using ObjCBindings;

namepsace NS;

[BindingType<ObjCBindings.Class>]
class ARAnchor {
		[Export<ObjCBindings.Property> (""includedKeys"", ArgumentSemantic.Copy)]
		string [] IncludedKeys { get; set; }
}
";

			yield return [stringArrayProperty, "NativeHandle_objc_msgSend", "void_objc_msgSend_NativeHandle", false, false];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataGetObjCMessageSendMethodNameSuper : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var simple = new TestDataGetObjCMessageSendMethodName ();
			foreach (var args in simple) {
				// modify the first argument to be the correct msg send 
				args [1] = ((string) args [1]).Replace ("objc_msgSend", "objc_msgSendSuper");
				if (args [2] is not null)
					args [2] = ((string) args [2]).Replace ("objc_msgSend", "objc_msgSendSuper");
				args [3] = true;
				yield return args;
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class TestDataGetObjCMessageSendMethodNameStret : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var simple = new TestDataGetObjCMessageSendMethodName ();
			foreach (var args in simple) {
				// modify the first argument to be the correct msg send 
				args [1] = ((string) args [1]).Replace ("objc_msgSend", "objc_msgSend_stret");
				if (args [2] is not null)
					args [2] = ((string) args [2]).Replace ("objc_msgSend", "objc_msgSend_stret");
				args [4] = true;
				yield return args;
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataGetObjCMessageSendMethodName>]
	[AllSupportedPlatformsClassData<TestDataGetObjCMessageSendMethodNameSuper>]
	[AllSupportedPlatformsClassData<TestDataGetObjCMessageSendMethodNameStret>]
	void GetObjCMessageSendMethodNamePropertiesTests (ApplePlatform platform, string inputText, string expectedGetter, string expectedSetter, bool isSuper, bool isStret)
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
		var methods = GetObjCMessageSendMethods (changes.Value, isSuper: isSuper, isStret: isStret);
		Assert.Equal (expectedGetter, methods.Getter);
		Assert.Equal (expectedSetter, methods.Setter);
	}
}
