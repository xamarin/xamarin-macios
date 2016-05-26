using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Tests;

using NUnit.Framework;

namespace MTouchTests
{
	[TestFixture]
	public class Registrar
	{
		enum R {
			OldStatic = 1,
			OldDynamic = 2,
			Static = 4,
			Dynamic = 8,
			AllOld = OldStatic | OldDynamic,
			AllNew = Static | Dynamic,
			All = AllOld | AllNew,
			AllStatic = OldStatic | Static,
			AllDynamic = OldDynamic | Dynamic,
		}

		[Test]
		public void InvalidParameterTypes ()
		{
			var code = @"
class Foo : NSObject {
	[Export (""bar1:"")]
	public void Bar1 (object[] arg)
	{
	}

	[Export (""bar2:"")]
	public void Bar2 (ref object arg)
	{
	}

	[Export (""bar3:"")]
	public void Bar3 (out object arg)
	{
		arg = null;
	}

	[Export (""bar4:"")]
	public void Bar4 (object arg)
	{
	}

	[Export (""bar5"")]
	public object Bar5 ()
	{
		return null;
	}

	[Export (""bar6:"")]
	public void Bar6 (int? arg)
	{
	}

	[Export (""bar7"")]
	public int? Bar7 ()
	{
		return null;
	}

	[Export (""bar8:"")]
	public void Bar8 (ref int?[] arg)
	{
	}

	[Export (""bar9:"")]
	public static void Bar9 (Attribute attribute)
	{
	}

	[Export (""bar10"")]
	public object Bar10 { get; set; }

	public object[] Bar11 {
		[Export (""bar11Getter"")]
		get { return null; }
		[Export (""bar11Setter:"")]
		set {}
	}
}
";
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4138: The registrar cannot marshal the property type 'System.Object' of the property 'Foo.Bar10'.",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Object[]' of the parameter 'arg' in the method 'Foo.Bar1(System.Object[])'", 
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Object&' of the parameter 'arg' in the method 'Foo.Bar2(System.Object&)'",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Object&' of the parameter 'arg' in the method 'Foo.Bar3(System.Object&)'",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Object' of the parameter 'arg' in the method 'Foo.Bar4(System.Object)'",
				".*/Test.cs(.*): error MT4104: The registrar cannot marshal the return value of type `System.Object` in the method `Foo.Bar5()`.",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Nullable`1<System.Int32>' of the parameter 'arg' in the method 'Foo.Bar6(System.Nullable`1<System.Int32>)'",
				".*/Test.cs(.*): error MT4104: The registrar cannot marshal the return value of type `System.Nullable`1<System.Int32>` in the method `Foo.Bar7()`.",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Nullable`1<System.Int32>[]&' of the parameter 'arg' in the method 'Foo.Bar8(System.Nullable`1<System.Int32>[]&)'",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Attribute' of the parameter 'attribute' in the method 'Foo.Bar9(System.Attribute)'",
				".*/Test.cs(.*): error MT4104: The registrar cannot marshal the return value of type `System.Object[]` in the method `Foo.get_Bar11()`.",
				".*/Test.cs(.*): error MT4136: The registrar cannot marshal the parameter type 'System.Object[]' of the parameter 'value' in the method 'Foo.set_Bar11(System.Object[])'");

			Verify (R.OldStatic, code, false, "error MT4111: The registrar cannot build a signature for type `System.Object' in method `Foo.get_Bar10`.");
		}

		[Test]
		public void MT4102 ()
		{
			var code = @"
class DateMembers : NSObject {
	[Export (""F1:"")]
	void F1 (DateTime a) {}

	[Export (""F2"")]
	DateTime F2 () { throw new Exception (); }

	[Export (""F3:"")]
	void F3 (ref DateTime d) {}

	[Export (""F4"")]
	DateTime F4 { get; set; }
}
";

			Verify (R.Static, code, false, 
				".*/Test.cs(.*): error MT4138: The registrar cannot marshal the property type 'System.DateTime' of the property 'DateMembers.F4'.",
				".*/Test.cs(.*): error MT4102: The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F1`. Use `MonoTouch.Foundation.NSDate` instead.",
				".*/Test.cs(.*): error MT4102: The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F2`. Use `MonoTouch.Foundation.NSDate` instead.",
				".*/Test.cs(.*): error MT4102: The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F3`. Use `MonoTouch.Foundation.NSDate` instead.");
			Verify (R.OldStatic, code, false,
				"error MT4102: The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.set_F4`. Use `MonoTouch.Foundation.NSDate` instead.",
				"error MT4102: The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F1`. Use `MonoTouch.Foundation.NSDate` instead.",
				"error MT4102: The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F3`. Use `MonoTouch.Foundation.NSDate` instead.");
		}

		[Test]
		public void MT4117 ()
		{
			var code = @"
class ArgCount : NSObject {
	[Export (""F1"")]
	void F1 (int a) {}

	[Export (""F2:"")]
	void F2 () {}

	[Export (""F3"", IsVariadic = true)]
	void F3 () {}
}
";

			Verify (R.Static, code, false, 
				".*/Test.cs(.*): error MT4117: The registrar found a signature mismatch in the method 'ArgCount.F1' - the selector 'F1' indicates the method takes 0 parameters, while the managed method has 1 parameters.",
				".*/Test.cs(.*): error MT4117: The registrar found a signature mismatch in the method 'ArgCount.F2' - the selector 'F2:' indicates the method takes 1 parameters, while the managed method has 0 parameters.",
				".*/Test.cs(.*): error MT4140: The registrar found a signature mismatch in the method 'ArgCount.F3' - the selector 'F3' indicates the variadic method takes 1 parameters, while the managed method has 0 parameters.");
			Verify (R.OldStatic, code, true);
		}

		[Test]
		public void MT4123 ()
		{
			var code = @"
class ArgCount : NSObject {
	[Export (""F2"", IsVariadic = true)]
	void F2 (System.IntPtr foo) {}

	[Export (""F3"", IsVariadic = true)]
	void F3 (int foo) {}

	[Export (""F4:"", IsVariadic = true)]
	void F4 (int foo, IntPtr bar) {}
}
";

			Verify (R.Static, code, false, ".*/Test.cs(.*): error MT4123: The type of the variadic parameter in the variadic function 'F3(System.Int32)' must be System.IntPtr.");
			Verify (R.OldStatic, code, true);
		}

		[Test]
		public void MT4127 ()
		{
			var str1 = @"	
[Protocol]
interface IFoo1 {
	[Preserve (Conditional = true)]
	[Export (""foo"")]
	void GetFoo ();
}

[Protocol]
interface IFoo2 {
	[Preserve (Conditional = true)]
	[Export (""foo"")]
	void GetFoo ();
}

[Preserve ()]
class MyObjectErr : NSObject, IFoo1, IFoo2
{
	public void GetFoo ()
	{
		throw new NotImplementedException ();
	}
}
";

			Verify (R.Static, str1, false, 
				"error MT4127: Cannot register more than one interface method for the method 'MyObjectErr.GetFoo' (which is implementing 'IFoo1.GetFoo' and 'IFoo2.GetFoo').");

		}

		[Test]
		public void MT4134 ()
		{
			if (!Directory.Exists (Configuration.xcode6_root))
				Assert.Ignore ("Xcode 6 ({0}) is required for this test.", Configuration.xcode6_root);
			
			// This test will have to be updated when new frameworks are introduced.
			VerifyWithXcode (R.Static, MTouch.Profile.Unified, string.Empty, false, Configuration.xcode6_root, "8.0",
				"warning MT0079: The recommended Xcode version for Xamarin.iOS .* is Xcode .* or later. The current Xcode version .found in " + Configuration.xcode6_root + ". is 6.*.",
				"error MT4134: Your application is using the 'Contacts' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'CoreSpotlight' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'GameplayKit' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'MetalKit' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'ModelIO' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'MetalPerformanceShaders' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'WatchKit' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 8.2, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'ContactsUI' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'ReplayKit' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.",
				"error MT4134: Your application is using the 'WatchConnectivity' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS 9.0, while you're building with the iOS 8.0 SDK.) This configuration is only supported with the legacy registrar (pass --registrar:legacy as an additional mtouch argument in your project's iOS Build option to select). Alternatively select a newer SDK in your app's iOS Build options.");
			VerifyWithXcode (R.OldStatic, string.Empty, true, Configuration.xcode6_root, "8.0",
				"warning MT0079: The recommended Xcode version for Xamarin.iOS .* is Xcode .* or later. The current Xcode version .found in " + Configuration.xcode6_root + ". is 6.*.");
		}

		[Test]
		public void MT4135 ()
		{
			var code = @"
class C : NSObject {
	[Export ()]
	public void Foo () {}

	[Export (null)]
	public void Bar () {}

	[Export ("""")]
	public void Zap () {}
}";
			Verify (R.Static, code, false, 
				".*Test.cs(.*): error MT4135: The member 'C.Foo' has an Export attribute without a selector. A selector is required.",
				".*Test.cs(.*): error MT4135: The member 'C.Bar' has an Export attribute without a selector. A selector is required.",
				".*Test.cs(.*): error MT4135: The member 'C.Zap' has an Export attribute without a selector. A selector is required.");
		}
			
		[Test]
		public void MT4138 ()
		{
			var code = @"
class C : NSObject {
	[Export (""foo"")]
	object Foo { get { return null; } }
}
";
			Verify (R.Static, code, false, 
				".*/Test.cs(.*): error MT4138: The registrar cannot marshal the property type 'System.Object' of the property 'C.Foo'.");
			Verify (R.OldStatic, code, false, "error MT4111: The registrar cannot build a signature for type `System.Object' in method `C.get_Foo`.");
		}

		[Test]
		public void MT4139 ()
		{
			var code = @"
class C : NSObject {
	[Connect]
	object P1 { get { return null; } }

	[Connect]
	int P2 { get { return 0; } }
}
";
			Verify (R.Static, code, false, 
				".*/Test.cs(.*): error MT4139: The registrar cannot marshal the property type 'System.Object' of the property 'C.P1'. Properties with the .Connect. attribute must have a property type of NSObject (or a subclass of NSObject).",
				".*/Test.cs(.*): error MT4139: The registrar cannot marshal the property type 'System.Int32' of the property 'C.P2'. Properties with the .Connect. attribute must have a property type of NSObject (or a subclass of NSObject).");
			Verify (R.OldStatic, code, true);
		}

		[Test]
		public void MT4141 ()
		{
			var code = @"
class C : NSObject {
	[Export (""retain"")]
	new void Retain () {}

	[Export (""release"")]
	new void Release () {}

	[Export (""dealloc"")]
	new void Dealloc () {}
}
";
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4141: Cannot register the selector 'retain' on the member 'C.Retain' because Xamarin.iOS implicitly registers this selector.",
				".*/Test.cs(.*): error MT4141: Cannot register the selector 'release' on the member 'C.Release' because Xamarin.iOS implicitly registers this selector.");
			Verify (R.OldStatic, code, true);
		}

		[Test]
		public void MT4145 ()
		{
			var code = @"
[Native]
public enum Foo : int
{
}
class C : NSObject {
	[Export (""nativeEnum1:"")]
	void NativeEnum1 (Foo foo) {}
}
";
			VerifyDual (R.Static, code, false, "error MT4145: Invalid enum 'Foo': enums with the [Native] attribute must have a underlying enum type of either 'long' or 'ulong'.");

			Verify (R.Static, code, true);
		}

		[Test]
		public void MT4146 ()
		{
			var code = @"
[Register ("" C"")]
class C : NSObject {
}
";
			VerifyDual (R.Static, code, true, "warning MT4146: The Name parameter of the Registrar attribute on the class 'C' contains an invalid character: ' ' (0x20)");
			Verify (R.Static, code, true, "warning MT4146: The Name parameter of the Registrar attribute on the class 'C' contains an invalid character: ' ' (0x20)");
		}

		[Test]
		public void MT4148 ()
		{
			var code = @"
[Protocol]
interface IProtocol<T> {
}
[Protocol]
interface IProtocol2 {
	[Export (""M"")]
	void M<T> ();
}
";
			VerifyDual (R.Static, code, false,
				"error MT4148: The registrar found a generic protocol: 'IProtocol`1'. Exporting generic protocols is not supported.",
				"error MT4113: The registrar found a generic method: 'IProtocol2.M()'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes");
			Verify (R.Static, code, false,
				"error MT4148: The registrar found a generic protocol: 'IProtocol`1'. Exporting generic protocols is not supported.",
				"error MT4113: The registrar found a generic method: 'IProtocol2.M()'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes");
		}

		[Test]
		public void MT4149 ()
		{
			var code = @"
[Category (typeof (NSString))]
public static class Category
{
	[Export (""foo"")]
	public static void Foo (this int bar) {}
}
";
			VerifyDual (R.Static, code, false,
				".*/Test.cs(.*): error MT4149: Cannot register the extension method 'Category.Foo' because the type of the first parameter ('System.Int32') does not match the category type ('Foundation.NSString').");
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4149: Cannot register the extension method 'Category.Foo' because the type of the first parameter ('System.Int32') does not match the category type ('MonoTouch.Foundation.NSString').");
		}

		[Test]
		public void MT4150 ()
		{
			var code = @"
[Category (typeof (string))]
public static class Category
{
}
";
			VerifyDual (R.Static, code, false,
				"error MT4150: Cannot register the type 'Category' because the category type 'System.String' in its Category attribute does not inherit from NSObject.");
			Verify (R.Static, code, false,
				"error MT4150: Cannot register the type 'Category' because the category type 'System.String' in its Category attribute does not inherit from NSObject.");
		}

		[Test]
		public void MT4151 ()
		{
			var code = @"
[Category (null)]
public static class Category
{
}
";
			VerifyDual (R.Static, code, false,
				"error MT4151: Cannot register the type 'Category' because the Type property in its Category attribute isn't set.");
			Verify (R.Static, code, false,
				"error MT4151: Cannot register the type 'Category' because the Type property in its Category attribute isn't set.");
		}

		[Test]
		public void MT4152 ()
		{
			var code = @"
[Category (typeof (NSString))]
public class Category1 : NSObject
{
}
[Category (typeof (NSString))]
public class Category2 : INativeObject
{
	public IntPtr Handle { get { return IntPtr.Zero; } }
}
";
			VerifyDual (R.Static, code, false,
				"error MT4152: Cannot register the type 'Category1' as a category because it implements INativeObject or subclasses NSObject.",
				"error MT4152: Cannot register the type 'Category2' as a category because it implements INativeObject or subclasses NSObject.");
			Verify (R.Static, code, false,
				"error MT4152: Cannot register the type 'Category1' as a category because it implements INativeObject or subclasses NSObject.",
				"error MT4152: Cannot register the type 'Category2' as a category because it implements INativeObject or subclasses NSObject.");
		}

		[Test]
		public void MT4153 ()
		{
			var code = @"
[Category (typeof (NSString))]
public class Category<T>
{
}
";
			VerifyDual (R.Static, code, false,
				"error MT4153: Cannot register the type 'Category`1' as a category because it's generic.");
			Verify (R.Static, code, false,
				"error MT4153: Cannot register the type 'Category`1' as a category because it's generic.");
		}

		[Test]
		public void MT4154 ()
		{
			var code = @"
[Category (typeof (NSString))]
public class Category
{
	[Export (""foo"")]
	public static void Foo<T> () {}
}
";
			VerifyDual (R.Static, code, false,
				".*/Test.cs(.*): error MT4154: Cannot register the method 'Category.Foo' as a category method because it's generic.");
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4154: Cannot register the method 'Category.Foo' as a category method because it's generic.");
		}

		// MT4155 only happens with the dynamic registrar

		[Test]
		public void MT4156 ()
		{
			var code = @"
[Category (typeof (NSString), Name = ""C"")]
public class Category1
{
}
[Category (typeof (NSString), Name = ""C"")]
public class Category2
{
}
";
			VerifyDual (R.Static, code, false,
				"error MT4156: Cannot register two categories ('Category2, Test' and 'Category1, Test') with the same native name ('C')");
			Verify (R.Static, code, false,
				"error MT4156: Cannot register two categories ('Category2, Test' and 'Category1, Test') with the same native name ('C')");
		}

		// MT4157 Can't be produced using C#, since it requires using [Extension] manually:
		// Test.cs(14,35): error CS1112: Do not use `System.Runtime.CompilerServices.ExtensionAttribute' directly. Use parameter modifier `this' instead

		[Test]
		public void MT4158 ()
		{
			var code = @"
[Category (typeof (NSString))]
public class Category
{
	[Export (""init"")]
	public Category () {}
}
";
			VerifyDual (R.Static, code, false,
				".*/Test.cs(.*): error MT4158: Cannot register the constructor Category..ctor() in the category Category because constructors in categories are not supported.");
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4158: Cannot register the constructor Category..ctor() in the category Category because constructors in categories are not supported.");
		}
			
		[Test]
		public void MT4159 ()
		{
			var code = @"
[Category (typeof (NSString))]
public class Category
{
	[Export (""foo"")]
	public void Foo () {}
}
";
			VerifyDual (R.Static, code, false,
				".*/Test.cs(.*): error MT4159: Cannot register the method 'Category.Foo' as a category method because category methods must be static.");
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4159: Cannot register the method 'Category.Foo' as a category method because category methods must be static.");
		}

		[Test]
		public void MT4160 ()
		{
			var code = @"
public class TestInvalidChar : NSObject
{
	[Export (""xy z"")]
	public void XyZ () {}
	[Export (""ab\tc"")]
	public void AbC () {}
}
";
			VerifyDual (R.Static, code, false,
				".*/Test.cs(.*): error MT4160: Invalid character ' ' (0x20) found in selector 'xy z' for 'TestInvalidChar.XyZ()'",
				".*/Test.cs(.*): error MT4160: Invalid character '\t' (0x9) found in selector 'ab\tc' for 'TestInvalidChar.AbC()'"
				);
			Verify (R.Static, code, false,
				".*/Test.cs(.*): error MT4160: Invalid character ' ' (0x20) found in selector 'xy z' for 'TestInvalidChar.Xyz()'",
				".*/Test.cs(.*): error MT4160: Invalid character '\t' (0x9) found in selector 'ab\tc' for 'TestInvalidChar.AbC()'");
		}

		[Test]
		public void MT4161 ()
		{
			var code = @"
public class TestInvalidChar : NSObject
{
	[Export (""foo1"")]
	public FooA Foo1 () { return new FooA (); }

	[Export (""foo2:"")]
	public void Foo2 (FooB foo) { }

	[Export (""foo3:"")]
	public void Foo3 (ref FooC foo) { }

	[Export (""foo4:"")]
	public void Foo4 (out FooD foo) { foo = new FooD (); }

	[Export (""foo5:"")]
	public void Foo5 (FooE[] foo) { }

	[Export (""foo6"")]
	public FooF[] Foo6 () { return null; }
}
public struct FooA { public NSObject Obj; }
public struct FooB { public NSObject Obj; }
public struct FooC { public NSObject Obj; }
public struct FooD { public NSObject Obj; }
public struct FooE { public NSObject Obj; }
public struct FooF { public NSObject Obj; }
";
			VerifyDual (R.Static, code, false,
				".*/Test.cs(.*): error MT4161: The registrar found an unsupported structure 'FooA': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).",
				".*/Test.cs(.*): error MT4161: The registrar found an unsupported structure 'FooB': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).",
				".*/Test.cs(.*): error MT4161: The registrar found an unsupported structure 'FooC': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).",
				".*/Test.cs(.*): error MT4161: The registrar found an unsupported structure 'FooD': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).",
				".*/Test.cs(.*): error MT4111: The registrar cannot build a signature for type `FooE[]' in method `TestInvalidChar.Foo5`.",
				".*/Test.cs(.*): error MT4111: The registrar cannot build a signature for type `FooF[]' in method `TestInvalidChar.Foo6`."

			);
		}


		[Test]
		[TestCase (MTouch.Profile.Unified, "iOS")]
		[TestCase (MTouch.Profile.TVOS, "tvOS")]
		//[TestCase (MTouch.Profile.WatchOS, "watchOS")] // MT0077 interferes
		public void MT4162 (MTouch.Profile profile, string name)
		{
			var code = @"
	[Introduced (PlatformName.iOS, 99, 0, 0, PlatformArchitecture.All, ""use Z instead"")]
	[Introduced (PlatformName.TvOS, 99, 0, 0, PlatformArchitecture.All, ""use Z instead"")]
	[Introduced (PlatformName.WatchOS, 99, 0, 0, PlatformArchitecture.All, ""use Z instead"")]
	[Introduced (PlatformName.iOS, 89, 0, 0, PlatformArchitecture.All)]
	[Introduced (PlatformName.TvOS, 89, 0, 0, PlatformArchitecture.All)]
	[Introduced (PlatformName.WatchOS, 89, 0, 0, PlatformArchitecture.All)]
	[Register (IsWrapper = true)]
	class FutureType : NSObject
	{
		public FutureType () {}
	}

	class CurrentType : FutureType
	{
		[Export (""foo:"")]
		public void Foo (FutureType ft)
		{
		}

		[Export (""bar"")]
		public FutureType Bar ()
		{
			throw new NotImplementedException ();
		}

		[Export (""zap"")]
		public FutureType Zap { get; set; }

		// This is actually working now, but only because we erase the generic argument when converting to ObjC.
		[Export (""zaps"")]
		public NSSet<FutureType> Zaps { get; set; }
	}

	[Protocol (Name = ""FutureProtocol"")]
	[ProtocolMember (IsRequired = false, IsProperty = true, IsStatic = false, Name = ""FutureProperty"", Selector = ""futureProperty"", PropertyType = typeof (FutureEnum), GetterSelector = ""futureProperty"", SetterSelector = ""setFutureProperty:"", ArgumentSemantic = ArgumentSemantic.UnsafeUnretained)]
	[ProtocolMember (IsRequired = true, IsProperty = false, IsStatic = false, Name = ""FutureMethod"", Selector = ""futureMethod"", ReturnType = typeof (FutureEnum), ParameterType = new Type [] { typeof (FutureEnum) }, ParameterByRef = new bool [] { false })]
	public interface IFutureProtocol : INativeObject, IDisposable
	{
	}

	[Introduced (PlatformName.iOS, 99, 0, 0, PlatformArchitecture.All)]
	[Introduced (PlatformName.TvOS, 99, 0, 0, PlatformArchitecture.All)]
	[Introduced (PlatformName.WatchOS, 99, 0, 0, PlatformArchitecture.All)]
	public enum FutureEnum {
	}
";
			
			Verify (R.Static, profile, code, false,
			        $"error MT4162: The type 'FutureType' (used as a base type of CurrentType) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $"error MT4162: The type 'FutureType' (used as a base type of CurrentType) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $".*/Test.cs(.*): error MT4162: The type 'FutureType' (used as the property type of CurrentType.Zap) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $".*/Test.cs(.*): error MT4162: The type 'FutureType' (used as the property type of CurrentType.Zap) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $".*/Test.cs(.*): error MT4162: The type 'FutureType' (used as a parameter in CurrentType.Foo) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $".*/Test.cs(.*): error MT4162: The type 'FutureType' (used as a parameter in CurrentType.Foo) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $".*/Test.cs(.*): error MT4162: The type 'FutureType' (used as a return type in CurrentType.Bar) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
			        $".*/Test.cs(.*): error MT4162: The type 'FutureType' (used as a return type in CurrentType.Bar) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
					$"error MT4162: The type 'FutureEnum' (used as the property type of IFutureProtocol.FutureProperty) is not available in {name} .* (it was introduced in {name} 99.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
					$"error MT4162: The type 'FutureEnum' (used as a return type in IFutureProtocol.FutureMethod) is not available in {name} .* (it was introduced in {name} 99.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).",
					$"error MT4162: The type 'FutureEnum' (used as a parameter in IFutureProtocol.FutureMethod) is not available in {name} .* (it was introduced in {name} 99.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode)."

			);
		}

		[Test]
		public void MT4164 ()
		{
			var code = @"
	class FutureType : NSObject
	{
		[Export (""auto"")] string Auto { get; set; }
		[Export (""break"")] string Break { get; set; }
		[Export (""case"")] string Case { get; set; }
		[Export (""char"")] string Char { get; set; }
		[Export (""const"")] string Const { get; set; }
		[Export (""continue"")] string Continue { get; set; }
		[Export (""default"")] string Default { get; set; }
		[Export (""do"")] string Do { get; set; }
		[Export (""double"")] string Double { get; set; }
		[Export (""else"")] string Else { get; set; }
		[Export (""enum"")] string Enum { get; set; }
		[Export (""export"")] string Export { get; set; }
		[Export (""extern"")] string Extern { get; set; }
		[Export (""float"")] string Float { get; set; }
		[Export (""for"")] string For { get; set; }
		[Export (""goto"")] string Goto { get; set; }
		[Export (""if"")] string If { get; set; }
		[Export (""inline"")] string Inline { get; set; }
		[Export (""int"")] string Int { get; set; }
		[Export (""long"")] string Long { get; set; }
		[Export (""register"")] string Register { get; set; }
		[Export (""return"")] string Return { get; set; }
		[Export (""short"")] string Short { get; set; }
		[Export (""signed"")] string Signed { get; set; }
		[Export (""sizeof"")] string Sizeof { get; set; }
		[Export (""static"")] string Static { get; set; }
		[Export (""struct"")] string Struct { get; set; }
		[Export (""switch"")] string Switch { get; set; }
		[Export (""template"")] string Template { get; set; }
		[Export (""typedef"")] string Typedef { get; set; }
		[Export (""union"")] string Union { get; set; }
		[Export (""unsigned"")] string Unsigned { get; set; }
		[Export (""void"")] string Void { get; set; }
		[Export (""volatile"")] string Volatile { get; set; }
		[Export (""while"")] string While { get; set; }
		[Export (""_Bool"")] string Bool { get; set; }
		[Export (""_Complex"")] string Complex { get; set; }
	}
";

			Verify (R.Static, MTouch.Profile.Unified, code, false, MTouch.Target.Sim,
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Auto' because its selector 'auto' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Break' because its selector 'break' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Case' because its selector 'case' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Char' because its selector 'char' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Const' because its selector 'const' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Continue' because its selector 'continue' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Default' because its selector 'default' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Do' because its selector 'do' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Double' because its selector 'double' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Else' because its selector 'else' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Enum' because its selector 'enum' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Export' because its selector 'export' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Extern' because its selector 'extern' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Float' because its selector 'float' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'For' because its selector 'for' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Goto' because its selector 'goto' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'If' because its selector 'if' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Inline' because its selector 'inline' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Int' because its selector 'int' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Long' because its selector 'long' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Register' because its selector 'register' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Return' because its selector 'return' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Short' because its selector 'short' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Signed' because its selector 'signed' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Sizeof' because its selector 'sizeof' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Static' because its selector 'static' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Struct' because its selector 'struct' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Switch' because its selector 'switch' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Template' because its selector 'template' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Typedef' because its selector 'typedef' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Union' because its selector 'union' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Unsigned' because its selector 'unsigned' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Void' because its selector 'void' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Volatile' because its selector 'volatile' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'While' because its selector 'while' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Bool' because its selector '_Bool' is an Objective-C keyword. Please use a different name.",
			        ".*/Test.cs(.*): error MT4164: Cannot export the property 'Complex' because its selector '_Complex' is an Objective-C keyword. Please use a different name.");
		}

		[Test]
		public void MultiplePropertiesInHierarchy ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=18337

			var code = @"
class C : NSObject {
	[Outlet]
	string Foo { get; set; }
}
class D : C {
	[Outlet]
	string Foo { get; set; }
}

class E : NSObject {
	[Export ()]
	string Foo { get; set; }
}

class F : E {
	[Export ()]
	string Foo { get; set; }
}

class G : NSObject {
	[Connect ()]
	NSString Foo { get; set; }
}

class H : G {
	[Connect ()]
	NSString Foo { get; set; }
}
";

			Verify (R.AllStatic, code, true);
		}

		void Verify (R registrars, string code, bool success, params string [] expected_messages)
		{
			VerifyWithXcode (registrars,  MTouch.Profile.Classic, code, success, Configuration.xcode_root, Configuration.sdk_version, expected_messages);
		}

		void Verify (R registrars, MTouch.Profile profile, string code, bool success, params string [] expected_messages)
		{
			Verify (registrars, profile, code, success, MTouch.Target.Dev, expected_messages);
		}

		void Verify (R registrars, MTouch.Profile profile, string code, bool success, MTouch.Target target, params string [] expected_messages)
		{
			VerifyWithXcode (registrars,  profile, code, success, Configuration.xcode_root, MTouch.GetSdkVersion (profile), target, expected_messages);
		}

		void VerifyDual (R registrars, string code, bool success, params string [] expected_messages)
		{
			VerifyWithXcode (registrars, MTouch.Profile.Unified, code, success, Configuration.xcode_root, Configuration.sdk_version, expected_messages);
		}

		void VerifyWithXcode (R registrars, string code, bool success, string xcode, string sdk_version, params string [] expected_messages)
		{
			VerifyWithXcode (registrars, MTouch.Profile.Classic, code, success, xcode, sdk_version, expected_messages);
		}

		void VerifyWithXcode (R registrars, MTouch.Profile profile, string code, bool success, string xcode, string sdk_version, params string [] expected_messages)
		{
			VerifyWithXcode (registrars, profile, code, success, xcode, sdk_version, MTouch.Target.Dev, expected_messages);
		}

		void VerifyWithXcode (R registrars, MTouch.Profile profile, string code, bool success, string xcode, string sdk_version, MTouch.Target target, params string [] expected_messages)
		{
			foreach (R value in Enum.GetValues (typeof (R))) {
				if ((registrars & value) == 0)
					continue;

				if (value != R.Dynamic && value != R.Static && value != R.OldDynamic && value != R.OldStatic)
					continue;

				string result = string.Empty;

				try {
					var header = @"
using System;
using System.Collections.Generic;";

					if (profile != MTouch.Profile.Classic) {
						header += @"
using Foundation;
using UIKit;
using ObjCRuntime;";
					} else {
						header += @"
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;";
					}

					header += @"
class Test {
	static void Main () { Console.WriteLine (typeof (NSObject)); }
}";


					result = CreateTestApp (profile, header + code, "--registrar:" + value.ToString ().ToLower (), xcode, sdk_version, target);
					Assert.IsTrue (success, string.Format ("Expected '{0}' to show the error(s) '{1}' with --registrar:\n\t{2}", code, string.Join ("\n\t", expected_messages), value.ToString ().ToLower ()));
				} catch (TestExecutionException mee) {
					Assert.IsFalse (success, string.Format ("Expected '{0}' to compile with --registrar:{1}", code, value.ToString ().ToLower ()));
					result = mee.Message;
				}

				var split = result.Split (new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
				var actual_messages = new HashSet<string> (split, new PatternEquality ());
				var exp_messages = new HashSet<string> (expected_messages, new PatternEquality ());

				actual_messages.Remove (".*Test.app built successfully.");
				actual_messages.Remove ("Xamarin.iOS .* using.*");
				actual_messages.Remove ("warning MT5303: Native linking warning: warning: relocatable dylibs (e.g. embedded frameworks) are only supported on iOS 8.0 and later .*");
				actual_messages.Remove ("warning MT5303: Native linking warning: warning: embedded dylibs/frameworks are only supported on iOS 8.0 and later (@rpath/PushKit.framework/PushKit)");

				actual_messages.ExceptWith (exp_messages);
				exp_messages.ExceptWith (split);

				var text = new StringBuilder ();
				foreach (var a in actual_messages)
					text.AppendFormat ("Unexpected error/warning with --registrar:{0}:\n\t{1}\n", value.ToString ().ToLower (), a);
				foreach (var a in exp_messages)
					text.AppendFormat ("Expected with --registrar:{0} to show:\n\t{1}\n", value.ToString ().ToLower (), a);
				if (text.Length != 0)
					Assert.Fail (text.ToString ());
			}
		}


		internal class PatternEquality : IEqualityComparer<string> 
		{
			public bool Equals (string x, string y)
			{
				if (x == y)
					return true;
				try {
					if (Regex.IsMatch (x, "^" + y.Replace ("(", "[(]").Replace (")", "[)]").Replace ("[]", "[[][]]") + "$")) {
						return true;
					}
				} catch {
					// do nothing
				}

				try {
					if (Regex.IsMatch (y, "^" +  x.Replace ("(", "[(]").Replace (")", "[)]").Replace ("[]", "[[][]]") + "$")) {
						return true;
					}
				} catch {
					// do nothing
				}

				return false;
			}
			public int GetHashCode (string obj)
			{
				return 0;
			}
		}

		[Test]
		public void GenericType_Warnings ()
		{
			var code = @"
class Open<T> : NSObject {}
class Closed : Open<UIView> {}
class Generic2<T> : NSObject where T: struct {}
class Generic3<T> : NSObject where T: System.IConvertible {}
";
			// here we're testing the warnings for the old static registrar.
			Verify (R.OldStatic, code, true, 
				"warning MT4112: The registrar found a generic type: Open`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4112: The registrar found a generic type: Generic2`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4112: The registrar found a generic type: Generic3`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information."
				);

			// and the lack of warnings/errors in the new static registrar.
			Verify (R.Static, code, true);
		}

		[Test]
		public void GenericType_NonConstrainedTypeArguments ()
		{
			var code = @"
		class Open<U, V> : NSObject where U: NSObject
		{
			[Export (""foo:"")]
			public void Foo (V arg) {} // Not OK

			[Export (""foozap"")]
			public V FooZap { get { return default (V); } } // Not OK

			[Export (""bar"")]
			public V Bar { get; set; }
		}";
			Verify (R.Static, code, false, 
				".*Test.cs.*: error MT4132: The registrar found an invalid generic return type 'V' in the property 'Open`2.FooZap'. The return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4132: The registrar found an invalid generic return type 'V' in the property 'Open`2.Bar'. The return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'V' in the parameter arg of the method 'Open`2.Foo(V)'. The generic parameter must have an 'NSObject' constraint.");
			Verify (R.OldStatic, code, false, 
				"warning MT4112: The registrar found a generic type: Open`2. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`2.get_FooZap'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`2.get_Bar'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`2.set_Bar'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`2.Foo'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4104: The registrar cannot marshal the return value for type `V` in signature for method `Open`2.get_FooZap`.",
				"error MT4104: The registrar cannot marshal the return value for type `V` in signature for method `Open`2.get_Bar`.",
				"error MT4105: The registrar cannot marshal the parameter of type `V` in signature for method `Open`2.set_Bar`.",
				"error MT4105: The registrar cannot marshal the parameter of type `V` in signature for method `Open`2.Foo`.");
		}

		[Test]
		public void GenericType_StaticMembersOnOpenGenericType ()
		{
			var code = @"
class Open<U> : NSObject
{	
	[Export (""foo"")]
	public static void Foo () {} // Not OK

	[Export (""foo:"")]
	public static void Foo (U arg) {} // Not OK

	[Export (""zap"")]
	public static string Zap { get; set; } // Not OK
}

";
			Verify (R.Static, code, false, 
				".*Test.cs.*: error MT4131: The registrar cannot export static properties in generic classes ('Open`1.Zap').",
				".*Test.cs.*: error MT4130: The registrar cannot export static methods in generic classes ('Open`1.Foo()').",
				".*Test.cs.*: error MT4130: The registrar cannot export static methods in generic classes ('Open`1.Foo(U)').");
			Verify (R.OldStatic, code, false, 
				"warning MT4112: The registrar found a generic type: Open`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.Foo'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4105: The registrar cannot marshal the parameter of type `U` in signature for method `Open`1.Foo`.");
		}

		[Test]
		public void NestedGenericType ()
		{
			var code = @"

		class Parent<T> {
			public class Nested : NSObject {
				[Export (""foo:"")]
				public void Foo (T foo) {
				}
			}
		}
";
			Verify (R.Static, code, false, 
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'T' in the parameter foo of the method 'Parent`1/Nested.Foo(T)'. The generic parameter must have an 'NSObject' constraint.");

			Verify (R.OldStatic, code, false,
				"warning MT4112: The registrar found a generic type: Parent`1/Nested. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Parent`1/Nested.Foo'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4105: The registrar cannot marshal the parameter of type `T` in signature for method `Parent`1/Nested.Foo`.");
		}

		[Test]
		public void GenericType_WithDerivedClosedOverride ()
		{
			var code = @"
		class GenericTestClass<T> : NSObject
		{
			[Export (""arg1:"")]
			public virtual void Arg1 (T t) {}
		}

		class DerivedClosed : GenericTestClass <UIView> 
		{
			public override void Arg1 (UIView t) {}
		}
";

			Verify (R.Static, code, false,
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'T' in the parameter t of the method 'GenericTestClass`1.Arg1(T)'. The generic parameter must have an 'NSObject' constraint.");
			Verify (R.OldStatic, code, false,
				"warning MT4112: The registrar found a generic type: GenericTestClass`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'GenericTestClass`1.Arg1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'GenericTestClass`1.Arg1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4105: The registrar cannot marshal the parameter of type `T` in signature for method `GenericTestClass`1.Arg1`.",
				"error MT4105: The registrar cannot marshal the parameter of type `T` in signature for method `DerivedClosed.Arg1`.");
		}

		[Test]
		public void GenericType_WithInvalidParameterTypes ()
		{
			var code = @"
		class Open<U> : NSObject where U: NSObject
		{
			[Export (""bar:"")]
			public void Bar (List<U> arg) {} // Not OK, can't marshal lists.
		}
";

			Verify (R.Static, code, false, 
				".*Test.cs.*: error MT4136: The registrar cannot marshal the parameter type 'System.Collections.Generic.List`1<U>' of the parameter 'arg' in the method 'Open`1.Bar(System.Collections.Generic.List`1<U>)'");
			Verify (R.OldStatic, code, false,
				"warning MT4112: The registrar found a generic type: Open`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.Bar'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4108: The registrar cannot get the ObjectiveC type for managed type `System.Collections.Generic.List`1`.");
		}

		[Test]
		public void GenericType_WithINativeObjectConstraint ()
		{
			var code = @"
		class Open<U> : NSObject where U: INativeObject
		{
			[Export (""bar:"")]
			public void Bar (U arg) {} // Not OK

			[Export (""zap:"")]
			public void Zap (U[] arg) {} // Not OK

			[Export (""xyz"")]
			public U XyZ () { return default (U); } // Not OK

			[Export (""barzap"")]
			public U BarZap { get { return default (U); } } // Not OK

			[Export (""zapbar"")]
			public Action<U> ZapBar () { return null; } // Not OK

			[Export (""xyz"")]
			public void XyZ (Action<U> f) {}

			[Export (""foozap"")]
			public void FooZap (List<List<List<List<U>>>> f) {}

			[Export (""zapfoo"")]
			public List<List<U>> ZapBoo () { return null; }

			[Export (""f1"")]
			public U F1 { get; set; }

			[Export (""f2"")]
			public List<List<U>> F2 { get { return null; } }

			[Export (""f3"")]
			public Action<U> F3 { set { } }
		}
";

			Verify (R.Static, code, false, 
				".*Test.cs.*: error MT4132: The registrar found an invalid generic return type 'U' in the property 'Open`1.BarZap'. The return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4132: The registrar found an invalid generic return type 'U' in the property 'Open`1.F1'. The return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4132: The registrar found an invalid generic return type 'System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>' in the property 'Open`1.F2'. The return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4132: The registrar found an invalid generic return type 'System.Action`1<U>' in the property 'Open`1.F3'. The return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'U' in the parameter arg of the method 'Open`1.Bar(U)'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'U[]' in the parameter arg of the method 'Open`1.Zap(U[])'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4129: The registrar found an invalid generic return type 'U' in the method 'Open`1.XyZ()'. The generic return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4129: The registrar found an invalid generic return type 'System.Action`1<U>' in the method 'Open`1.ZapBar()'. The generic return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'System.Action`1<U>' in the parameter f of the method 'Open`1.XyZ(System.Action`1<U>)'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>>>' in the parameter f of the method 'Open`1.FooZap(System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>>>)'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4129: The registrar found an invalid generic return type 'System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>' in the method 'Open`1.ZapBoo()'. The generic return type must have an 'NSObject' constraint."
				);
			Verify (R.OldStatic, code, false, 
				"warning MT4112: The registrar found a generic type: Open`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.get_BarZap'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.get_F1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.set_F1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.get_F2'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.Bar'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.XyZ'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.ZapBar'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.FooZap'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'Open`1.ZapBoo'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4104: The registrar cannot marshal the return value for type `U` in signature for method `Open`1.get_BarZap`.",
				"error MT4104: The registrar cannot marshal the return value for type `U` in signature for method `Open`1.get_F1`.",
				"error MT4105: The registrar cannot marshal the parameter of type `U` in signature for method `Open`1.set_F1`.",
				"error MT4104: The registrar cannot marshal the return value for type `System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>` in signature for method `Open`1.get_F2`.",
				"error MT4105: The registrar cannot marshal the parameter of type `U` in signature for method `Open`1.Bar`.",
				"error MT4105: The registrar cannot marshal the parameter of type `U[]` in signature for method `Open`1.Zap`.",
				"error MT4104: The registrar cannot marshal the return value for type `U` in signature for method `Open`1.XyZ`.",
				"error MT4104: The registrar cannot marshal the return value for type `System.Action`1<U>` in signature for method `Open`1.ZapBar`.",
				"error MT4108: The registrar cannot get the ObjectiveC type for managed type `System.Collections.Generic.List`1`.",
				"error MT4104: The registrar cannot marshal the return value for type `System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>` in signature for method `Open`1.ZapBoo`.");
		}

		[Test]
		public void GenericType_WithINativeObjectConstraint2 ()
		{
			var code = @"
		unsafe class Open<U> : NSObject where U: INativeObject
		{
			[Export (""bar:"")]
			public void Bar (ref U arg) {} // Not OK

			[Export (""zap:"")]
			public void Zap (out U[] arg) { arg = null; } // Not OK
		}
";

			Verify (R.Static, code, false, 
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'U&' in the parameter arg of the method 'Open`1.Bar(U&)'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'U[]&' in the parameter arg of the method 'Open`1.Zap(U[]&)'. The generic parameter must have an 'NSObject' constraint.");
			Verify (R.OldStatic, code, false, 
				"warning MT4112: The registrar found a generic type: Open`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4105: The registrar cannot marshal the parameter of type `U&` in signature for method `Open`1.Bar`.",
				"error MT4105: The registrar cannot marshal the parameter of type `U[]&` in signature for method `Open`1.Zap`.");
		}

		[Test]
		public void GenericMethods ()
		{
			var str1 = @"
[Register (""GenericMethodClass"")]
class GenericMethodClass : NSObject {
	[Export (""foo:"")]
	public static void Foo<T> (T foo) {}
	[Export (""GenericMethod"")]
	public virtual void GenericMethod<T> (T foo) { }

}
";
			Verify (R.OldStatic, str1, false, 
				"warning MT4113: The registrar found a generic method: 'GenericMethodClass.Foo'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'GenericMethodClass.GenericMethod'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4105: The registrar cannot marshal the parameter of type `T` in signature for method `GenericMethodClass.GenericMethod`.",
				"error MT4105: The registrar cannot marshal the parameter of type `T` in signature for method `GenericMethodClass.Foo`.");
			Verify (R.Static, str1, false, 
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'GenericMethodClass.GenericMethod(T)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes",
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'GenericMethodClass.Foo(T)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes"
				);
		}

		[Test]
		public void GenericMethods1 ()
		{
			var code = @"
class GenericMethodClass : NSObject {
	[Export (""Foo:"")]
	public virtual void Foo (System.Action<string> func) {}
}
";
			Verify (R.AllStatic, code, true);
		}

		[Test]
		public void GenericMethods2 ()
		{
			var str1 = @"
class NullableGenericTestClass<T> : NSObject where T: struct
{
	[Export (""init:"")]
	public NullableGenericTestClass (T? foo)
	{
	}

	[Export (""Z1:"")]
	public void Z1<Z> (Z? foo) where Z: struct
	{
	}

	[Export (""Z2"")]
	public Z Z2<Z> () where Z: struct
	{
	throw new System.NotImplementedException ();
	}

	[Export (""Z3"")]
	public void Z3<Z> (Z foo)
	{
	}

	[Export (""T1"")]
	public void T1 (T foo)
	{
	}

	[Export (""T2:"")]
	public void T2 (T? foo)
	{
	}

	[Export (""T3"")]
	public T T3 ()
	{
	throw new System.NotImplementedException ();
	}
}
";
			Verify (R.Static, str1, false, 
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.Z1(System.Nullable`1<Z>)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes",
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.Z2()'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes",
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.Z3(Z)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'T' in the parameter foo of the method 'NullableGenericTestClass`1.T1(T)'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4128: The registrar found an invalid generic parameter type 'System.Nullable`1<T>' in the parameter foo of the method 'NullableGenericTestClass`1.T2(System.Nullable`1<T>)'. The generic parameter must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4129: The registrar found an invalid generic return type 'T' in the method 'NullableGenericTestClass`1.T3()'. The generic return type must have an 'NSObject' constraint.",
				".*Test.cs.*: error MT4136: The registrar cannot marshal the parameter type 'System.Nullable`1<T>' of the parameter 'foo' in the method 'NullableGenericTestClass`1..ctor(System.Nullable`1<T>)'");

			Verify (R.OldStatic, str1, false,
				"warning MT4112: The registrar found a generic type: NullableGenericTestClass`1. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.", 
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1..ctor'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.", 
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.Z1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.Z2'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.Z3'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.T1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.T2'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'NullableGenericTestClass`1.T3'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4105: The registrar cannot marshal the parameter of type `System.Nullable`1` in signature for method `NullableGenericTestClass`1..ctor`.",
				"error MT4105: The registrar cannot marshal the parameter of type `System.Nullable`1` in signature for method `NullableGenericTestClass`1.Z1`.",
				"error MT4104: The registrar cannot marshal the return value for type `Z` in signature for method `NullableGenericTestClass`1.Z2`.",
				"error MT4105: The registrar cannot marshal the parameter of type `Z` in signature for method `NullableGenericTestClass`1.Z3`.",
				"error MT4105: The registrar cannot marshal the parameter of type `T` in signature for method `NullableGenericTestClass`1.T1`.",
				"error MT4105: The registrar cannot marshal the parameter of type `System.Nullable`1` in signature for method `NullableGenericTestClass`1.T2`.",
				"error MT4104: The registrar cannot marshal the return value for type `T` in signature for method `NullableGenericTestClass`1.T3`.");
		}

		[Test]
		public void GenericMethods3 ()
		{
			var code = @"

class G : NSObject {
	[Export (""fooX1:"")]
	public virtual X Foo1<X> (int x) where X : NSObject { return default(X); }// Not OK

	[Export (""fooX2:"")]
	public virtual X Foo2<X> (int x) { return default(X); } // Not OK

	[Export (""fooX3:"")]
	public void Foo3<X> (X x) { } // Not OK
}
";

			Verify (R.Static, code, false,
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'G.Foo1(System.Int32)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes",
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'G.Foo2(System.Int32)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes",
				".*Test.cs.*: error MT4113: The registrar found a generic method: 'G.Foo3(X)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes");
			Verify (R.OldStatic, code, false,
				"warning MT4113: The registrar found a generic method: 'G.Foo1'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'G.Foo2'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"warning MT4113: The registrar found a generic method: 'G.Foo3'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.",
				"error MT4104: The registrar cannot marshal the return value for type `X` in signature for method `G.Foo1`.",
				"error MT4104: The registrar cannot marshal the return value for type `X` in signature for method `G.Foo2`.",
				"error MT4105: The registrar cannot marshal the parameter of type `X` in signature for method `G.Foo3`."
				);
		}

		[Test]
		public void ConformsToProtocolTest ()
		{
			var code = @"
class CTP : NSObject {
	public override bool ConformsToProtocol (IntPtr protocol) { return base.ConformsToProtocol (protocol); }
}

class CTP1 : NSObject {
	public override bool ConformsToProtocol (IntPtr protocol) { return base.ConformsToProtocol (protocol); }
}
class CTP2 : CTP1 {
	public override bool ConformsToProtocol (IntPtr protocol) { return base.ConformsToProtocol (protocol); }
}


class CTP3 : NSObject { }
class CTP4 : CTP3 {
	public override bool ConformsToProtocol (IntPtr protocol) { return base.ConformsToProtocol (protocol); }
}
";
			Verify (R.Static, code, true);
			Verify (R.OldStatic, code, true);
		}

#region Helper functions
		// Creates an app with the specified source as the executable.
		// Compiles it using smcs, will throw a McsException if it fails.
		// Then runs mtouch to try to create an app (for device), will throw MTouchException if it fails.
		// This method should not leave anything behind on disk.
		static string CreateTestApp (MTouch.Profile profile, string source, string extra_args = "", string xcode = null, string sdk_version = null, MTouch.Target target = MTouch.Target.Dev)
		{
			string path = MTouch.GetTempDirectory ();
			try {
				string cs = Path.Combine (path, "Test.cs");
				string exe = Path.Combine (path, "Test.exe");
				File.WriteAllText (cs, source);
				Compile (cs, profile);
				string app = Path.Combine (path, "Test.app");
				string cache = Path.Combine (path, "cache");
				Directory.CreateDirectory (cache);
				Directory.CreateDirectory (app);

				if (xcode == null)
					xcode = Configuration.xcode_root;

				if (sdk_version == null)
					sdk_version = MTouch.GetSdkVersion (profile);

				return ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("{0} {10} {1} --sdk {2} -targetver {2} --abi={9} {3} --sdkroot {4} --cache {5} --nolink {7} --debug -r:{6} --target-framework:{8}", exe, app, sdk_version, extra_args, xcode, cache, MTouch.GetBaseLibrary (profile), profile != MTouch.Profile.Classic ? string.Empty : "--nosign", MTouch.GetTargetFramework (profile), MTouch.GetArchitecture (profile, target), target == MTouch.Target.Sim ? "-sim" : "-dev"), hide_output: false);
			} finally {
				Directory.Delete (path, true);
			}
		}

		// Compile the filename with mcs
		// Does not clean up anything.
		static void Compile (string filename, MTouch.Profile profile = MTouch.Profile.Classic)
		{			
			StringBuilder output = new StringBuilder ();
			using (var p = new Process ()) {
				var args = new StringBuilder ();
				args.Append (" /unsafe /debug:full /nologo ");
				args.Append (MTouch.Quote (filename));
				args.Append (" -r:").Append (MTouch.Quote (MTouch.GetBaseLibrary (profile)));
				p.StartInfo.FileName = MTouch.GetCompiler (profile, args);
				p.StartInfo.Arguments = args.ToString ();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				Console.WriteLine ("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
				p.Start ();
				p.BeginErrorReadLine ();
				p.BeginOutputReadLine ();
				p.ErrorDataReceived += (sender, e) => 
				{
					if (e.Data != null) {
						lock (output) {
							output.AppendLine (e.Data);
						}
					}
				};
				p.OutputDataReceived += (sender, e) => 
				{
					if (e.Data != null) {
						lock (output) {
							output.AppendLine (e.Data);
						}
					}
				};
				p.WaitForExit ();
				
				Console.WriteLine (output);
				
				if (p.ExitCode != 0)
					throw new McsException (output.ToString ());
			}
		}
#endregion
	}
}

