using System;
using System.IO;
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
		[TestCase (Profile.iOS)]
		public void BI1036 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.ApiDefinitions.Add (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bi1036.cs"));
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1036, "The last parameter in the method 'NS.Foo.Method' must be a delegate (it's 'System.String').");
		}

		[Test]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.macOSClassic)]
		public void BI1037 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "protocol-duplicate-abstract-error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1037, "The selector Identifier on type Derived is found multiple times with both read only and write only versions, with no read/write version.");
		}

		[Test]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.macOSClassic)]
		public void BI1038 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "protocol-duplicate-method-diff-return.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1038, "The selector DoIt on type Derived is found multiple times with different return types.");
		}

		[Test]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.macOSClassic)]
		public void BI1039 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "protocol-duplicate-method-diff-length.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1039, "The selector doit:itwith:more: on type Derived is found multiple times with different argument length 3 : 4.");
		}

		[Test]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.macOSClassic)]
		public void BI1040 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "protocol-duplicate-method-diff-out.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1040, "The selector doit:withmore on type Derived is found multiple times with different argument out states on argument 1.");
		}

		[Test]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.macOSClassic)]
		public void BI1041 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "protocol-duplicate-method-diff-type.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertErrorPattern (1041, "The selector doit:with:more: on type Derived is found multiple times with different argument types on argument 2 - System.Int32 : .*Foundation.NSObject.");
		}

		[Test]
		public void BI1046 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("bi1046.cs");
			bgen.CreateTemporaryBinding ();
			bgen.ProcessEnums = true;
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1046, "The [Field] constant HMAccessoryCategoryTypeGarageDoorOpener cannot only be used once inside enum HMAccessoryCategoryType.");
		}

		[Test]
		public void BI1048 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bindas1048error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1048, "Unsupported type String decorated with [BindAs]");
		}

		[Test]
		public void BI1049 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bindas1049error.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1049, "Could not unbox type String from NSNumber container used on member BindAs1049ErrorTests.MyFooClass.StringMethod decorated with [BindAs].");
		}

		[Test]
		public void BI1050_model ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bindas1050modelerror.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1050, "[BindAs] cannot be used inside Protocol or Model types. Type: MyFooClass");
		}

		[Test]
		public void BI1050_protocol ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bindas1050protocolerror.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1050, "[BindAs] cannot be used inside Protocol or Model types. Type: MyFooClass");
		}

		[Test]
		public void BI1060 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug42855.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1060, "The Bug42855Tests.MyFooClass protocol is decorated with [Model], but not [BaseType]. Please verify that [Model] is relevant for this protocol; if so, add [BaseType] as well, otherwise remove [Model].");
		}

		[Test]
		public void BI1112_Bug37527_WrongProperty ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("bug37527-wrong-property.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1112, "Property 'TestProperty' should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work.");
		}

		[Test]
		public void BI1113_Bug37527_MissingProperty ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("bug37527-missing-property.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1113, "BaseType.Delegates were set but no properties could be found. Do ensure that the WrapAttribute is used on the right properties.");
		}

		[Test]
		public void BI1117 ()
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
			bgen.AssertWarning (1117, "The member 'SomeMethod' is decorated with [Static] and its container class Bug52570Tests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into Bug52570Tests.FooObject class.");
		}

		[Test]
		public void BI1117_classinternal ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug52570classinternal.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		public void BI1117_methodinternal ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug52570methodinternal.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		public void BI1117_allowstaticmembers ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug52570allowstaticmembers.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
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

		[Test]
		public void BI1062_NoAsyncMethodRefHandlerTest ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
		public void BI1062_NoAsyncMethodOutHandlerTest ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
		public void BI1062_NoAsyncMethodOutParameterTest ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
		public void BI1062_NoAsyncMethodRefParameterTest ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
		public void BI1063_NoDoubleWrapTest ()
		{
			var bgen = new BGenTool {
				Profile = Profile.iOS,
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
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void WarnAsError (Profile profile)
		{
			const string message = "The member 'SomeMethod' is decorated with [Static] and its container class warnaserrorTests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into warnaserrorTests.FooObject class.";
			{
				// Enabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.WarnAsError = string.Empty;
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "warnaserror.cs")));
				bgen.AssertExecuteError ("build");
				bgen.AssertError (1117, message);
			}

			{
				// Disabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "warnaserror.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1116
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.WarnAsError = "1116";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "warnaserror.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1117
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.WarnAsError = "1117";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "warnaserror.cs")));
				bgen.AssertExecuteError ("build");
				bgen.AssertError (1117, message);
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void NoWarn (Profile profile)
		{
			const string message = "The member 'SomeMethod' is decorated with [Static] and its container class nowarnTests.FooObject_Extensions is decorated with [Category] this leads to hard to use code. Please inline SomeMethod into nowarnTests.FooObject class.";
			{
				// Enabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.NoWarn = string.Empty;
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertNoWarnings ();
			}

			{
				// Disabled
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1116
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.NoWarn = "1116";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertWarning (1117, message);
			}

			{
				// Only 1117
				var bgen = new BGenTool ();
				bgen.Profile = profile;
				bgen.Defines = BGenTool.GetDefaultDefines (profile);
				bgen.NoWarn = "1117";
				bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "nowarn.cs")));
				bgen.AssertExecute ("build");
				bgen.AssertNoWarnings ();
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void MissingExportOnProperty (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "missing-export-property.cs")));
			bgen.AssertExecuteError ("build");
			bgen.AssertError (1018, "No [Export] attribute on property Test.NSTextInputClient.SelectedRange");
		}
	}
}
