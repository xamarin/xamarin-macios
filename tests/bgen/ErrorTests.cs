using System.IO;
using System.Text;

using Xamarin.Tests;

namespace GeneratorTests {
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class ErrorTests {
		[Test]
		[NonParallelizable]
		public void ApiDefinitionSourceIsNotSupported ()
		{
			var output = new StringBuilder ();
			ThreadStaticTextWriter.ReplaceConsole (output);
			try {
				Assert.That (BindingTouch.Main (new [] { "api.cs" }), Is.EqualTo (1));
			} finally {
				ThreadStaticTextWriter.RestoreConsole ();
			}

			Assert.That (output.ToString (), Does.Contain ("API definition source files are no longer supported"));
		}

		[Test]
		[NonParallelizable]
		public void CompileCommandIsNotSupported ()
		{
			var output = new StringBuilder ();
			ThreadStaticTextWriter.ReplaceConsole (output);
			try {
				Assert.That (BindingTouch.Main (new [] { "--compile-command=csc" }), Is.EqualTo (1));
			} finally {
				ThreadStaticTextWriter.RestoreConsole ();
			}

			Assert.That (output.ToString (), Does.Contain ("Unknown option"));
		}

		[Test]
		[NonParallelizable]
		public void GeneratedSourceFileListIsRequired ()
		{
			var output = new StringBuilder ();
			ThreadStaticTextWriter.ReplaceConsole (output);
			try {
				Assert.That (BindingTouch.Main (new [] { "--compiled-api-definition-assembly=api.dll", "--tmpdir=generated" }), Is.EqualTo (1));
			} finally {
				ThreadStaticTextWriter.RestoreConsole ();
			}

			Assert.That (output.ToString (), Does.Contain ("no generated source file list provided"));
		}

		[Test]
		[NonParallelizable]
		public void PersistentGeneratedSourceDirectoryIsRequired ()
		{
			var output = new StringBuilder ();
			ThreadStaticTextWriter.ReplaceConsole (output);
			try {
				Assert.That (BindingTouch.Main (new [] { "--compiled-api-definition-assembly=api.dll", "--sourceonly=generated-sources.txt" }), Is.EqualTo (1));
			} finally {
				ThreadStaticTextWriter.RestoreConsole ();
			}

			Assert.That (output.ToString (), Does.Contain ("no persistent generated source output directory provided"));
		}

		[Test]
		public void BI0086 ()
		{
			var bgen = new BGenTool ();
			bgen.CreateTemporaryBinding ("");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (86, "A target framework (--target-framework) must be specified.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void InvalidSelectorDoesNotExitProcess (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool {
				Profile = profile,
			};
			bgen.AddTestApiDefinition ("invalid-selector.cs");

			bgen.AssertExecuteError ("invalid selector");
			bgen.AssertError (1129, "The selector 'invalid(selector' on 'InvalidSelector.InvalidSelectorType.InvalidSelector' contains invalid characters.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1036 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.ApiDefinitions.Add (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bi1036.cs"));
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1036, "The last parameter in the method 'NS.Foo.Method' must be a delegate (it's 'System.String').");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BI1037 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "protocol-duplicate-abstract-error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1037, "The selector Identifier on type Derived is found multiple times with both read only and write only versions, with no read/write version.");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BI1038 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "protocol-duplicate-method-diff-return.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1038, "The selector DoIt on type Derived is found multiple times with different return types.");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BI1039 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "protocol-duplicate-method-diff-length.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1039, "The selector doit:itwith:more: on type Derived is found multiple times with different argument length 3 : 4.");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BI1040 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "protocol-duplicate-method-diff-out.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1040, "The selector doit:withmore on type Derived is found multiple times with different argument out states on argument 1.");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BI1041 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "protocol-duplicate-method-diff-type.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertErrorPattern (1041, "The selector doit:with:more: on type Derived is found multiple times with different argument types on argument 2 - System.Int32 : .*Foundation.NSObject.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1042 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("bi1042.cs");
			bgen.CreateTemporaryBinding ();
			bgen.ProcessEnums = true;
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1042, "Missing '[Field (LibraryName=value)]' for BindingTests.Tools.DoorOpener (e.g.\"__Internal\")");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1046 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("bi1046.cs");
			bgen.CreateTemporaryBinding ();
			bgen.ProcessEnums = true;
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1046, "The [Field] constant HMAccessoryCategoryTypeGarageDoorOpener cannot only be used once inside enum HMAccessoryCategoryType.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1048 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bindas1048error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type String decorated with [BindAs]");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1049 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bindas1049error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1049, "Could not unbox type String from NSNumber container used on member BindAs1049ErrorTests.MyFooClass.StringMethod decorated with [BindAs].");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void GH6863_property (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "ghissue6863_property.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1071, "The BindAs type for the member \"GH6863_property.MyFooClass.StringProp\" must be an array when the member's type is an array.");
		}


		[Test]
		[TestCase (Profile.iOS)]
		public void GH6863_method (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "ghissue6863_method.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1072, "The BindAs type for the parameter \"id_test\" in the method \"GH6863_method.MyFooClass.StringMethod\" must be an array when the parameter's type is an array.");
		}


		[Test]
		[TestCase (Profile.iOS)]
		public void BI1050_model (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bindas1050modelerror.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1050, "[BindAs] cannot be used on properties or return values inside Protocol or Model types. Type: MyFooClass");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1050_protocol (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bindas1050protocolerror.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1050, "[BindAs] cannot be used on properties or return values inside Protocol or Model types. Type: MyFooClass");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1059 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bi1059.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1084, "Found 2 Foundation.PreserveAttribute attributes on the type BI1059. At most one was expected.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1060 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bug42855.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1060, "The Bug42855Tests.MyFooClass protocol is decorated with [Model], but not [BaseType]. Please verify that [Model] is relevant for this protocol; if so, add [BaseType] as well, otherwise remove [Model].");
		}

		// https://github.com/dotnet/macios/issues/10015
		[Test]
		[TestCase (Profile.iOS)]
		public void Issue10015 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "issue10015.cs")));
			bgen.AssertExecute ("build");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1077 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bi1077.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1077, "Async method System.Void A(BI1077.ADelegate) with more than one result parameter in the callback by neither ResultTypeName or ResultType");
			bgen.AssertError (1077, "Async method System.Void B(BI1077.BDelegate) with more than one result parameter in the callback by neither ResultTypeName or ResultType");
			bgen.AssertErrorCount (2);
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1112_Bug37527_WrongProperty (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("bug37527-wrong-property.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1112, "Property 'TestProperty' should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1113_Bug37527_MissingProperty (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("bug37527-missing-property.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1113, "BaseType.Delegates were set but no properties could be found. Do ensure that the WrapAttribute is used on the right properties.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1117 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
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
			bgen.AssertWarning (1117, "The member 'SomeMethod' is decorated with [Static] and its container class Bug52570Tests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into Bug52570Tests.FooObject class.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1117_classinternal (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bug52570classinternal.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1117_methodinternal (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bug52570methodinternal.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BindAsNoMultidimensionalArrays (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
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
		[TestCase (Profile.iOS)]
		public void BindAsNullableNSStringArrayError (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797

			var bgen = new BGenTool {
				Profile = profile,
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
		[TestCase (Profile.iOS)]
		public void BindAsNullableNSValueArrayError (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797

			var bgen = new BGenTool {
				Profile = profile,
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
		[TestCase (Profile.iOS)]
		public void BindAsNullableNSNumberArrayError (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797

			var bgen = new BGenTool {
				Profile = profile,
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
		[TestCase (Profile.iOS)]
		public void BindAsNoRefParam (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
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
			bgen.AssertError (1080, "Unsupported type 'ref/out NSValue' decorated with [BindAs]");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BindAsNoOutParam (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
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
			bgen.AssertError (1080, "Unsupported type 'ref/out NSValue' decorated with [BindAs]");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void Bug57094Test (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57094
			var bgen = new BGenTool ();
			bgen.Profile = profile;
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

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1128 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (@"
using Foundation;

namespace BI1128Tests {

	[Static]
	interface FieldConstants {
		[Field (""InvalidSymbolAddress"", ""__Internal"", SymbolAddress = true)]
		int InvalidSymbolAddress { get; }
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1128, "The [Field] member 'BI1128Tests.FieldConstants.InvalidSymbolAddress' sets SymbolAddress, but its type is 'System.Int32'; only System.IntPtr is supported.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1062_NoAsyncMethodRefHandlerTest (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;

namespace BI1062Tests {

	delegate void MyHandler (ref bool staaph, NSError error);

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Async]
		[Export (""fooMethod:"")]
		void FooMethod (MyHandler handler);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1062, "The member 'FooObject.FooMethod' contains ref/out parameters and must not be decorated with [Async].");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1062_NoAsyncMethodOutHandlerTest (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;

namespace BI1062Tests {

	delegate void MyHandler (out bool staaph, NSError error);

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Async]
		[Export (""fooMethod:"")]
		void FooMethod (MyHandler handler);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1062, "The member 'FooObject.FooMethod' contains ref/out parameters and must not be decorated with [Async].");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1062_NoAsyncMethodOutParameterTest (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;

namespace BI1062Tests {

	delegate void MyHandler (bool staaph, NSError error);

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Async]
		[Export (""fooMethod:completion:"")]
		void FooMethod (out NSObject obj, MyHandler handler);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1062, "The member 'FooObject.FooMethod' contains ref/out parameters and must not be decorated with [Async].");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1062_NoAsyncMethodRefParameterTest (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;

namespace BI1062Tests {

	delegate void MyHandler (bool staaph, NSError error);

	[BaseType (typeof (NSObject))]
	interface FooObject {

		[Async]
		[Export (""fooMethod:completion:"")]
		void FooMethod (ref NSObject obj, Action<bool, NSError> handler);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1062, "The member 'FooObject.FooMethod' contains ref/out parameters and must not be decorated with [Async].");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1063_NoDoubleWrapTest (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool {
				Profile = profile,
				ProcessEnums = true
			};
			bgen.CreateTemporaryBinding (@"
using System;
using Foundation;

namespace BI1063Tests {

	enum PersonRelationship {
		[Field (null)]
		None,

		[Field (""INPersonRelationshipFather"", ""__Internal"")]
		Father,

		[Field (""INPersonRelationshipMother"", ""__Internal"")]
		Mother
	}

	[BaseType (typeof (NSObject))]
	interface Wrappers {

		// SmartEnum -- Normal Wrap getter Property

		[Export (""presenceType"")]
		NSString _PresenceType { get; }

		[Wrap (""PersonRelationshipExtensions.GetValue (_PresenceType)"")]
		PersonRelationship PresenceType {
			[Wrap (""PersonRelationshipExtensions.GetValue (_PresenceType)"")]
			get;
		}
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1063, "The 'WrapAttribute' can only be used at the property or at getter/setter level at a given time. Property: 'BI1063Tests.Wrappers.PresenceType'");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1064 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool {
				Profile = profile,
				ProcessEnums = true
			};
			bgen.CreateTemporaryBinding (@"
using System;
using ObjCRuntime;
using Foundation;

namespace BI1064Errors
{
	[BaseType (typeof (NSObject))]
	interface C
	{
		[Export (""testINativeObjectArray:a:b:"")]
		void TestINativeObjectArray (int action, ref INativeObject[] refValues, out INativeObject[] outValues);

		[Export (""invalid1:a:"")]
		void TestInvalid1 (ref DateTime[] refInvalid, out DateTime[] outInvalid);

		[Export (""invalid2:a:"")]
		void TestInvalid2 (ref object[] refInvalid, out object[] outInvalid);

		[Export (""invalid3:a:"")]
		void TestInvalid3 (ref int[] refInvalid, out int[] outInvalid);

		[Export (""invalid4:a:"")]
		void TestInvalid4 (ref object refInvalid, out object outInvalid);

		[Export (""testINativeObject:a:b:"")]
		void TestINativeObject (int action, ref INativeObject refValue, out INativeObject outValue);

		[Export (""testSelectorArray:a:b:"")] // Can't put SEL into NSArray (SEL isn't an NSObject)
		void TestSelectorArray (int action, ref Selector[] refValues, out Selector[] outValues);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'ObjCRuntime.INativeObject' for the parameter 'refValue' in BI1064Errors.C.TestINativeObject.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'ObjCRuntime.INativeObject' for the parameter 'outValue' in BI1064Errors.C.TestINativeObject.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'ObjCRuntime.INativeObject[]' for the parameter 'refValues' in BI1064Errors.C.TestINativeObjectArray.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'ObjCRuntime.INativeObject[]' for the parameter 'outValues' in BI1064Errors.C.TestINativeObjectArray.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.DateTime[]' for the parameter 'refInvalid' in BI1064Errors.C.TestInvalid1.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.DateTime[]' for the parameter 'outInvalid' in BI1064Errors.C.TestInvalid1.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.Object[]' for the parameter 'refInvalid' in BI1064Errors.C.TestInvalid2.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.Object[]' for the parameter 'refInvalid' in BI1064Errors.C.TestInvalid2.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.Int32[]' for the parameter 'outInvalid' in BI1064Errors.C.TestInvalid3.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.Int32[]' for the parameter 'outInvalid' in BI1064Errors.C.TestInvalid3.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.Object' for the parameter 'refInvalid' in BI1064Errors.C.TestInvalid4.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'System.Object' for the parameter 'refInvalid' in BI1064Errors.C.TestInvalid4.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'ObjCRuntime.Selector[]' for the parameter 'refValues' in BI1064Errors.C.TestSelectorArray.");
			bgen.AssertError (1064, "Unsupported ref/out parameter type 'ObjCRuntime.Selector[]' for the parameter 'outValues' in BI1064Errors.C.TestSelectorArray.");
			bgen.AssertErrorCount (14);
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1065 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool {
				Profile = profile,
				ProcessEnums = true
			};
			bgen.CreateTemporaryBinding (@"
using System;
using ObjCRuntime;
using Foundation;

namespace BI1065Errors
{
	[BaseType (typeof (NSObject))]
	interface C
	{
		// Can't put SEL into NSArray (SEL isn't an NSObject), so a Selector[] parameter/return value doesn't make sense
		[Export (""testSelectorArray:"")]
		void TestSelectorArray (Selector[] values);
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1065, "Unsupported parameter type 'ObjCRuntime.Selector[]' for the parameter 'values' in BI1065Errors.C.TestSelectorArray.");
			bgen.AssertErrorCount (1);
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1066 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool {
				Profile = profile,
				ProcessEnums = true
			};
			bgen.CreateTemporaryBinding (@"
using System;
using ObjCRuntime;
using Foundation;

namespace BI1066Errors
{
	[BaseType (typeof (NSObject))]
	interface C
	{
		// Can't put SEL into NSArray (SEL isn't an NSObject), so a Selector[] parameter/return value doesn't make sense
		[Export (""testSelectorArrayReturnValue"")]
		Selector[] TestSelectorArrayReturnValue ();
	}
}");
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1066, "Unsupported return type 'ObjCRuntime.Selector[]' in BI1066Errors.C.TestSelectorArrayReturnValue.");
			bgen.AssertErrorCount (2); // We show the same error twice.
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1067_1070 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			BGenTool bgen = new BGenTool {
				Profile = profile,
			};
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "diamond-protocol-errors.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1067, "The type 'DiamondProtocol.A.C' is trying to inline the property 'P1' from the protocols 'DiamondProtocol.A.P1' and 'DiamondProtocol.A.P2', but the inlined properties don't share the same accessors ('DiamondProtocol.A.P1 P1' is read-only, while '$DiamondProtocol.A.P2 P1' is write-only).");
			bgen.AssertWarning (1068, "The type 'DiamondProtocol.D.C' is trying to inline the property 'P1' from the protocols 'DiamondProtocol.D.P1' and 'DiamondProtocol.D.P2', and the inlined properties use different selectors (P1.P1 uses 'pA', and P2.P1 uses 'pB'.");
			bgen.AssertError (1069, "The type 'DiamondProtocol.Y.C' is trying to inline the methods binding the selector 'm1:' from the protocols 'DiamondProtocol.Y.P1' and 'DiamondProtocol.Y.P2', using methods with different signatures ('System.Void M1(System.Int32)' vs 'System.Int32 M1(System.Boolean)').");
			bgen.AssertError (1070, "The type 'DiamondProtocol.C.C' is trying to inline the property 'P1' from the protocols 'DiamondProtocol.C.P1' and 'DiamondProtocol.C.P2', but the inlined properties are of different types ('DiamondProtocol.C.P1 P1' is int, while 'DiamondProtocol.C.P2 P1' is int).");
			bgen.AssertErrorCount (3);
			bgen.AssertWarningCount (1);
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.macOSMobile)]
		public void WarnAsError (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			const string message = "The member 'SomeMethod' is decorated with [Static] and its container class warnaserrorTests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into warnaserrorTests.FooObject class.";
			{
				// Enabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.WarnAsError = string.Empty;
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "warnaserror.cs")));
				bgen.AssertExecuteError ("build");
				bgen.AssertError (1117, message);
			}

			{
				// Disabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "warnaserror.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1116
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.WarnAsError = "1116";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "warnaserror.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1117
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.WarnAsError = "1117";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "warnaserror.cs")));
				bgen.AssertExecuteError ("build");
				bgen.AssertError (1117, message);
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.macOSMobile)]
		public void NoWarn (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			if (!Configuration.XcodeIsStable)
				Assert.Ignore ("This test doesn't work when using a preview Xcode, because we have to pass -nowarn to enable preview builds, which interferes with the test.");

			const string message = "The member 'SomeMethod' is decorated with [Static] and its container class nowarnTests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into nowarnTests.FooObject class.";
			{
				// Enabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.NoWarn = string.Empty;
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertNoWarnings ();
			}

			{
				// Disabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1116
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.NoWarn = "1116";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1117
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.NoWarn = "1117";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertNoWarnings ();
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.macOSMobile)]
		public void MissingExportOnProperty (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "missing-export-property.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1018, "No [Export] attribute on property Test.NSTextInputClient.SelectedRange");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void ErrorDomain_NoLibraryName (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = true;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "errordomain-nolibraryname.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1087, "Missing value for the 'LibraryName' property for the '[ErrorDomain]' attribute for ErrorDomainNS.EWithDomain (e.g. '[ErrorDomain (\"MyDomain\", LibraryName = \"__Internal\")]')");
		}

		[Test]
		[TestCase (Profile.MacCatalyst)]
		public void StrongDictionaryErrors (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());

			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = true;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "strongdictionary-errors.cs")));
			bgen.AssertExecuteError ("build");

			var errorMessages = new string []
			{
				"The strong enum 'NSIntegerFieldType' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.NSIntegerValue', because its backing type is 'System.IntPtr'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'NSUIntegerFieldType' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.NSUIntegerValue', because its backing type is 'System.UIntPtr'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'Int64FieldType' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.Int64Value', because its backing type is 'System.Int64'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'UInt32FieldType' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.UInt32Value', because its backing type is 'System.UInt32'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'UInt64FieldType' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.UInt64Value', because its backing type is 'System.UInt64'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",

				"The strong enum 'NSIntegerFieldType[]' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.ArrayOfNSIntegerValue', because its backing type is 'System.IntPtr'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'NSUIntegerFieldType[]' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.ArrayOfNSUIntegerValue', because its backing type is 'System.UIntPtr'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'Int64FieldType[]' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.ArrayOfInt64Value', because its backing type is 'System.Int64'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'UInt32FieldType[]' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.ArrayOfUInt32Value', because its backing type is 'System.UInt32'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
				"The strong enum 'UInt64FieldType[]' is not a valid strong dictionary field for the property 'BackingField.StrongDictionary.ArrayOfUInt64Value', because its backing type is 'System.UInt64'. Only enums with backing type 'NSString', 'NSNumber' or 'System.Int32' are supported in strong dictionaries.",
			};

			foreach (var msg in errorMessages)
				bgen.AssertError (1121, msg);

			bgen.AssertErrorCount (errorMessages.Length);
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1126 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "factory-method-noninit.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1126, "The [FactoryMethod] attribute on 'FactoryMethodNonInitTest.BadWidget.CreateWithFoo' can only be used with an Objective-C 'init' selector (the selector must be 'init' or start with 'init' followed by an uppercase letter), but the selector is 'createWithFoo:'.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BI1127 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "factory-method-named-error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1127, "The [FactoryMethod] attribute on 'FactoryMethodNamedError.BadWidget.CreateWithFoo' can't specify a method name when applied to a method that isn't a constructor. Remove the method name from the [FactoryMethod] attribute; the name of the binding method is used instead.");
		}
	}
}
