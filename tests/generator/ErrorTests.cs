using System;

using NUnit.Framework;

using Xamarin.Tests;

namespace GeneratorTests
{
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class ErrorTests
	{
		[Test]
		public void BI0086 ()
		{
			var bgen = new BGenTool ();
			bgen.CreateTemporaryBinding ("");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (86, "A target framework (--target-framework) must be specified.");
		}

		[Test]
		public void BI1061 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (@"using System;
using Foundation;

namespace Bug52570Tests {

	[Category]
	[BaseType (typeof (FooObject))]
	interface FooObject_Extensions {

		[Static]
		[Export (""someMethod:"")]
		bool SomeMethod (NSRange range);
	}

	[BaseType (typeof (NSObject))]
	interface FooObject {
	
	}
}");
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1117, "The SomeMethod member is decorated with [Static] and its container class Bug52570Tests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into Bug52570Tests.FooObject class.");
		}

		[Test]
		public void BindAsNoMultidimensionalArrays ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using AVFoundation;
using ObjCRuntime;

namespace Bug57795Tests {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[BindAs (typeof (AVMediaTypes [,]))]
		[Export (""strongAVMediaTypesPropertiesMulti:"")]
		NSString [,] StrongAVMediaTypesPropertiesMulti { get; set; }
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type AVMediaTypes[,] decorated with [BindAs]");
		}

		[Test]
		public void BindAsNullableNSStringArrayError ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797

			var bgen = new BGenTool {
				Profile = Profile.iOS
			};
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using CoreAnimation;

namespace Bug57797Tests {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[BindAs (typeof (AVMediaTypes? []))]
		[Export (""strongNullableAVMediaTypesProperties"")]
		NSString [] StrongNullableAVMediaTypesProperties { get; set; }
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type AVMediaTypes?[] decorated with [BindAs]");
		}

		[Test]
		public void BindAsNullableNSValueArrayError ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797

			var bgen = new BGenTool {
				Profile = Profile.iOS
			};
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using CoreAnimation;

namespace Bug57797Tests {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[BindAs (typeof (CATransform3D? []))]
		[Export (""PCATransform3DNullableArray"")]
		NSValue [] PCATransform3DNullableArrayValue { get; set; }
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type CATransform3D?[] decorated with [BindAs]");
		}

		[Test]
		public void BindAsNullableNSNumberArrayError ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797

			var bgen = new BGenTool {
				Profile = Profile.iOS,
				ProcessEnums = true
			};
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using CoreAnimation;

namespace Bug57797Tests {

	[Native]
	enum Foo : long {
		One,
		Two
	}

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[BindAs (typeof (Foo? []))]
		[Export (""strongNullableAVMediaTypesProperties"")]
		NSNumber[] StrongNullableFoo { get; set; }
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type Foo?[] decorated with [BindAs]");
		}

		[Test]
		public void BindAsNoRefParam ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace Bug57804TestsRef {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Export (""setCGAffineTransformValueRefNonNullable:"")]
		void SetCGAffineTransformValueRefNonNullable ([BindAs (typeof (CGAffineTransform))] ref NSValue value);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type 'ref/out NSValue' decorated with [BindAs]");
		}

		[Test]
		public void BindAsNoOutParam ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace Bug57804TestsRef {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Export (""setCGAffineTransformValueOutNonNullable:"")]
		void SetCGAffineTransformValueOutNonNullable ([BindAs (typeof (CGAffineTransform))] out NSValue value);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type 'ref/out NSValue' decorated with [BindAs]");
		}

		[Test]
		public void Bug57094Test ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57094
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;
using ObjCRuntime;

namespace Bug57094 {

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Field (""SomeField"", ""__Internal"")]
		byte [] SomeField { get; }
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1014, "Unsupported type for Fields: byte[] for 'Bug57094.FooObject SomeField'.");
		}
	}
}
