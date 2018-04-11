using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Tests;
using Xamarin.Utils;
using NUnit.Framework;

using MTouchLinker = Xamarin.Tests.LinkerOption;
using MTouchRegistrar = Xamarin.Tests.RegistrarOption;

namespace Xamarin
{
	[TestFixture]
	public class Registrar
	{
		enum R {
			Static = 4,
			Dynamic = 8,
		}

		[Test]
		public void InvalidParameterTypes ()
		{
			var code = @"
using System;
using Foundation;
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
class C { static void Main () {} }
";

			using (var mtouch = new MTouchTool ()) {
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (code: code, extraArg: "-debug");
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (4138, "The registrar cannot marshal the property type 'System.Object' of the property 'Foo.Bar10'.", "testApp.cs", 54);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Object[]' of the parameter 'arg' in the method 'Foo.Bar1(System.Object[])'", "testApp.cs", 7);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Object&' of the parameter 'arg' in the method 'Foo.Bar2(System.Object&)'", "testApp.cs", 12);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Object&' of the parameter 'arg' in the method 'Foo.Bar3(System.Object&)'", "testApp.cs", 17);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Object' of the parameter 'arg' in the method 'Foo.Bar4(System.Object)'", "testApp.cs", 23);
				mtouch.AssertError (4104, "The registrar cannot marshal the return value of type `System.Object` in the method `Foo.Bar5()`.", "testApp.cs", 28);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Nullable`1<System.Int32>' of the parameter 'arg' in the method 'Foo.Bar6(System.Nullable`1<System.Int32>)'", "testApp.cs", 34);
				mtouch.AssertError (4104, "The registrar cannot marshal the return value of type `System.Nullable`1<System.Int32>` in the method `Foo.Bar7()`.", "testApp.cs", 39);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Nullable`1<System.Int32>[]&' of the parameter 'arg' in the method 'Foo.Bar8(System.Nullable`1<System.Int32>[]&)'", "testApp.cs", 45);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Attribute' of the parameter 'attribute' in the method 'Foo.Bar9(System.Attribute)'", "testApp.cs", 50);
				mtouch.AssertError (4104, "The registrar cannot marshal the return value of type `System.Object[]` in the method `Foo.get_Bar11()`.", "testApp.cs", 58);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Object[]' of the parameter 'value' in the method 'Foo.set_Bar11(System.Object[])'", "testApp.cs", 60);
				mtouch.AssertErrorCount (12);
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using System;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4138, "The registrar cannot marshal the property type 'System.DateTime' of the property 'DateMembers.F4'.", "testApp.cs", 14);
				mtouch.AssertError (4102, "The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F1`. Use `Foundation.NSDate` instead.", "testApp.cs", 5);
				mtouch.AssertError (4102, "The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F2`. Use `Foundation.NSDate` instead.", "testApp.cs", 8);
				mtouch.AssertError (4102, "The registrar found an invalid type `System.DateTime` in signature for method `DateMembers.F3`. Use `Foundation.NSDate` instead.", "testApp.cs", 11);
				mtouch.AssertErrorCount (4);
				mtouch.AssertNoWarnings ();
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4117, "The registrar found a signature mismatch in the method 'ArgCount.F1' - the selector 'F1' indicates the method takes 0 parameters, while the managed method has 1 parameters.", "testApp.cs", 5);
				mtouch.AssertError (4117, "The registrar found a signature mismatch in the method 'ArgCount.F2' - the selector 'F2:' indicates the method takes 1 parameters, while the managed method has 0 parameters.", "testApp.cs", 8);
				mtouch.AssertError (4140, "The registrar found a signature mismatch in the method 'ArgCount.F3' - the selector 'F3' indicates the variadic method takes 1 parameters, while the managed method has 0 parameters.", "testApp.cs", 11);
				mtouch.AssertErrorCount (3);
				mtouch.AssertNoWarnings ();
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using System;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4123, "The type of the variadic parameter in the variadic function 'F3(System.Int32)' must be System.IntPtr.", "testApp.cs", 8);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: str1, usings: "using Foundation; using System;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4127, "Cannot register more than one interface method for the method 'MyObjectErr.GetFoo' (which is implementing 'IFoo1.GetFoo' and 'IFoo2.GetFoo').");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		public void MT4134 ()
		{
			var xcodeRoot = Configuration.xcode83_root;
			if (!Directory.Exists (xcodeRoot))
				Assert.Ignore ("Xcode 8 ({0}) is required for this test.", xcodeRoot);
			
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.SdkRoot = xcodeRoot;
				mtouch.Sdk = "10.3";

				// Create a source file that references most platform types.
				var sb = new StringBuilder ();
				var xidll = Mono.Cecil.AssemblyDefinition.ReadAssembly (MTouch.GetBaseLibrary (mtouch.Profile));
				var xitypes = xidll.MainModule.Types
						.Where ((v) => v.IsPublic && !v.Name.Contains ("`"))
						.Where ((v) => {
							switch (v.Namespace) {
							case "CoreML":
							case "CoreImage":
								// These run into other registrar errors, so just skip them
								return false;
							default:
								return true;
							}
						});
				sb.AppendLine ("[Preserve (AllMembers = true)] class preserveTypes { static void M () {");
				foreach (var t in xitypes)
					sb.AppendLine ($"System.Console.WriteLine (typeof ({t.Namespace}.{t.Name}));");
				sb.AppendLine ("}}");

				mtouch.CreateTemporaryApp (extraCode: sb.ToString (), usings: "using System; using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				var invalidFrameworks = new [] {
					new { Framework = "IdentityLookup", Version = "11.0" },
					new { Framework = "FileProviderUI", Version = "11.0" },
					new { Framework = "FileProvider", Version = "11.0" },
					new { Framework = "DeviceCheck", Version = "11.0" },
					new { Framework = "CoreNFC", Version = "11.0" },
					new { Framework = "Vision", Version = "11.0" },
					new { Framework = "PDFKit", Version = "11.0" },
					new { Framework = "IOSurface", Version = "11.0" },
					new { Framework = "ARKit", Version = "11.0" },
					new { Framework = "BusinessChat", Version = "11.3" },
				};
				foreach (var framework in invalidFrameworks)
					mtouch.AssertError (4134, $"Your application is using the '{framework.Framework}' framework, which isn't included in the iOS SDK you're using to build your app (this framework was introduced in iOS {framework.Version}, while you're building with the iOS {mtouch.Sdk} SDK.) Please select a newer SDK in your app's iOS Build options.");
				mtouch.AssertErrorCount (invalidFrameworks.Length);
				mtouch.AssertWarningCount (0);
			}
		}

		[Test]
		public void MT4135 ()
		{
			var code = @"
class C : NSObject {
	[Export (null)]
	public void Bar () {}

	[Export ("""")]
	public void Zap () {}
}";
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4135, "The member 'C.Bar' has an Export attribute without a selector. A selector is required.", "testApp.cs", 5);
				mtouch.AssertError (4135, "The member 'C.Zap' has an Export attribute without a selector. A selector is required.", "testApp.cs", 8);
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4138, "The registrar cannot marshal the property type 'System.Object' of the property 'C.Foo'.", "testApp.cs", 5);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4139, "The registrar cannot marshal the property type 'System.Object' of the property 'C.P1'. Properties with the [Connect] attribute must have a property type of NSObject (or a subclass of NSObject).", "testApp.cs", 5);
				mtouch.AssertError (4139, "The registrar cannot marshal the property type 'System.Int32' of the property 'C.P2'. Properties with the [Connect] attribute must have a property type of NSObject (or a subclass of NSObject).", "testApp.cs", 8);
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4141, "Cannot register the selector 'retain' on the member 'C.Retain' because Xamarin.iOS implicitly registers this selector.", "testApp.cs", 5);
				mtouch.AssertError (4141, "Cannot register the selector 'release' on the member 'C.Release' because Xamarin.iOS implicitly registers this selector.", "testApp.cs", 8);
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4145, "Invalid enum 'Foo': enums with the [Native] attribute must have a underlying enum type of either 'long' or 'ulong'.");
				mtouch.AssertErrorCount (5); // The same MT4145 is printed 5 times
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		public void MT4146 ()
		{
			var code = @"
[Register ("" C"")]
class C : NSObject {
}
";
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecute ();
				mtouch.AssertWarning (4146, "The Name parameter of the Registrar attribute on the class 'C' (' C') contains an invalid character: ' ' (0x20).");
				mtouch.AssertErrorCount (0);
				mtouch.AssertWarningCount (1);
			}
		}

		[Test]
		public void MT4146_b ()
		{
			var code = @"
[Register (""A C"")]
class C : NSObject {
}
";
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4146, "The Name parameter of the Registrar attribute on the class 'C' ('A C') contains an invalid character: ' ' (0x20).");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4148, "The registrar found a generic protocol: 'IProtocol`1'. Exporting generic protocols is not supported.");
				mtouch.AssertError (4113, "The registrar found a generic method: 'IProtocol2.M()'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes");
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4149, "Cannot register the extension method 'Category.Foo' because the type of the first parameter ('System.Int32') does not match the category type ('Foundation.NSString').", "testApp.cs", 7);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4150, "Cannot register the type 'Category' because the category type 'System.String' in its Category attribute does not inherit from NSObject.");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4151, "Cannot register the type 'Category' because the Type property in its Category attribute isn't set.");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using System; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4152, "Cannot register the type 'Category1' as a category because it implements INativeObject or subclasses NSObject.");
				mtouch.AssertError (4152, "Cannot register the type 'Category2' as a category because it implements INativeObject or subclasses NSObject.");
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4153, "Cannot register the type 'Category`1' as a category because it's generic.");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4154, "Cannot register the method 'Category.Foo' as a category method because it's generic.", "testApp.cs", 7);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4156, "Cannot register two categories ('Category2, testApp' and 'Category1, testApp') with the same native name ('C')");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4158, "Cannot register the constructor Category..ctor() in the category Category because constructors in categories are not supported.", "testApp.cs", 7);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4159, "Cannot register the method 'Category.Foo' as a category method because category methods must be static.", "testApp.cs", 7);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
		}

		// This list is duplicated in src/ObjCRuntime/Registrar.cs
		static char[] invalidSelectorCharacters = new char[] { ' ', '\t', '?', '\\', '!', '|', '@', '"', '\'', '%', '&', '/', '(', ')', '=', '^', '[', ']', '{', '}', ',', '.', ';', '-', '\n' };

		[Test]
		public void MT4160 ()
		{
			// newline is invalid, but it messes up the error message and testing is just annoying, so skip it.
			var testInvalidCharacters = invalidSelectorCharacters.Where ((v) => v != '\n').ToArray ();
			using (var mtouch = new MTouchTool ()) {
				var sb = new StringBuilder ();
				sb.AppendLine ("public class TestInvalidChar : Foundation.NSObject {");
				for (int i = 0; i < testInvalidCharacters.Length; i++) {
					var c = testInvalidCharacters [i];
					var str = c.ToString ();
					switch (c) {
					case '"':
						str = "\"\"";
						break;
					}
					sb.AppendLine ($"\t[Foundation.Export (@\"X{str}\")]");
					sb.AppendLine ($"\tpublic void X{i} () {{}}");
				}
				sb.AppendLine ("}");
				mtouch.CreateTemporaryApp (extraCode: sb.ToString (), extraArg: "-debug");
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				for (int i = 0; i < testInvalidCharacters.Length; i++) {
					var c = testInvalidCharacters [i];
					mtouch.AssertError (4160, $"Invalid character '{c}' (0x{((int)c).ToString ("x")}) found in selector 'X{c}' for 'TestInvalidChar.X{i}()'", "testApp.cs", 3 + i * 2);
				}
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4161, "The registrar found an unsupported structure 'FooA': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).", "testApp.cs", 6);
				mtouch.AssertError (4161, "The registrar found an unsupported structure 'FooB': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).", "testApp.cs", 9);
				mtouch.AssertError (4161, "The registrar found an unsupported structure 'FooC': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).", "testApp.cs", 12);
				mtouch.AssertError (4161, "The registrar found an unsupported structure 'FooD': All fields in a structure must also be structures (field 'Obj' with type 'Foundation.NSObject' is not a structure).", "testApp.cs", 15);
				mtouch.AssertError (4111, "The registrar cannot build a signature for type `FooE[]' in method `TestInvalidChar.Foo5`.", "testApp.cs", 18);
				mtouch.AssertError (4111, "The registrar cannot build a signature for type `FooF[]' in method `TestInvalidChar.Foo6`.", "testApp.cs", 21);
				mtouch.AssertErrorCount (6);
				mtouch.AssertNoWarnings ();
			}
		}


		[Test]
		[TestCase (Profile.iOS, "iOS", MTouchLinker.DontLink)]
		[TestCase (Profile.tvOS, "tvOS", MTouchLinker.DontLink)]
		[TestCase (Profile.iOS, "iOS", MTouchLinker.LinkAll)]
		//[TestCase (Profile.WatchOS, "watchOS")] // MT0077 interferes
		public void MT4162 (Profile profile, string name, MTouchLinker linker)
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
			
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.Linker = linker;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: code, extraArg: "-debug", usings: "using System;\nusing Foundation;\nusing ObjCRuntime;\n");
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as a base type of CurrentType) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as a base type of CurrentType) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as the property type of CurrentType.Zap) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", "testApp.cs", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as the property type of CurrentType.Zap) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", "testApp.cs", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as a parameter in CurrentType.Foo) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", "testApp.cs", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as a parameter in CurrentType.Foo) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", "testApp.cs", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as a return type in CurrentType.Bar) is not available in {name} .* (it was introduced in {name} 99.0.0): 'use Z instead'. Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", "testApp.cs", custom_pattern_syntax: true);
				mtouch.AssertErrorPattern (4162, $"The type 'FutureType' (used as a return type in CurrentType.Bar) is not available in {name} .* (it was introduced in {name} 89.0.0). Please build with a newer {name} SDK (usually done by using the most recent version of Xcode).", "testApp.cs", custom_pattern_syntax: true);
			}
		}

		static string [] objective_c_keywords = new string [] {
			"auto",
			"break",
			"case", "char", "const", "continue",
			"default", "do", "double",
			"else", "enum", "export", "extern",
			"float", "for",
			"goto",
			"if", "inline", "int",
			"long",
			"register", "return",
			"short", "signed", "sizeof", "static", "struct", "switch",
			"template", "typedef", "union",
			"unsigned",
			"void", "volatile",
			"while",
			"_Bool",
			"_Complex",
		};

		[Test]
		public void MT4164 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var sb = new StringBuilder ();
				sb.AppendLine ("class FutureType : Foundation.NSObject {");
				foreach (var kw in objective_c_keywords)
					sb.AppendLine ($"[Foundation.Export (\"{kw}\")] string X{kw} {{ get; set; }}");
				sb.AppendLine ("}");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: sb.ToString (), extraArg: "-debug");
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				foreach (var kw in objective_c_keywords)
					mtouch.AssertError (4164, $"Cannot export the property 'X{kw}' because its selector '{kw}' is an Objective-C keyword. Please use a different name.", "testApp.cs");
			}
		}

		[Test]
		public void MT4167 ()
		{
			var code = @"
class X : ReplayKit.RPBroadcastControllerDelegate
{
	public override void DidUpdateServiceInfo (ReplayKit.RPBroadcastController broadcastController, NSDictionary<NSString, INSCoding> serviceInfo)
	{
		throw new NotImplementedException ();
	}
}
";
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using System; using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecute ();
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		public void MT4168 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var sb = new StringBuilder ();
				foreach (var kw in objective_c_keywords) {
					sb.AppendLine ($"[Foundation.Register (\"{kw}\")]");
					sb.AppendLine ($"class X{kw} : Foundation.NSObject {{}}");
				}
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: sb.ToString ());
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				foreach (var kw in objective_c_keywords)
					mtouch.AssertError (4168, $"Cannot register the type 'X{kw}' because its Objective-C name '{kw}' is an Objective-C keyword. Please use a different name.");
			}
		}

		[Test]
		public void MT4169 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var sb = new StringBuilder ();
				sb.AppendLine (@"		
		[Foundation.Preserve (AllMembers = true)]
		public class C
		{
			[System.Runtime.InteropServices.DllImport (""/usr/lib/libobjc.dylib"")]
			static extern void objc_msgSend (object something);
		}");
				mtouch.Linker = MTouchLinker.LinkAll; // faster test
				mtouch.CustomArguments = new string [] { "--marshal-objectivec-exceptions=throwmanaged" };
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: sb.ToString (), extraArg: "-debug:full");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (4169, $"Failed to generate a P/Invoke wrapper for objc_msgSend(System.Object): The registrar cannot get the ObjectiveC type for managed type `System.Object`.");
			}
		}

		[Test]
		public void MT4170 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var code = @"
				namespace NS {
					using System;
					using Foundation;
					using ObjCRuntime;
					class X : NSObject {
						[Export (""a"")]
						[return: BindAs (typeof (DateTime), OriginalType = typeof (NSNumber))] 
						DateTime A () { throw new NotImplementedException (); }
						[Export (""b"")]
						[return: BindAs (typeof (DateTime?), OriginalType = typeof (NSNumber))] 
						DateTime? B () { throw new NotImplementedException (); }
					}
				}";
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: code, extraArg: "-debug");
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (4170, "The registrar can't convert from 'System.DateTime' to 'Foundation.NSNumber' for the return value in the method NS.X.A.", "testApp.cs", 9);
				mtouch.AssertError (4170, "The registrar can't convert from 'System.Nullable`1<System.DateTime>' to 'Foundation.NSNumber' for the return value in the method NS.X.B.", "testApp.cs", 12);
				mtouch.AssertErrorCount (4 /* errors are duplicated */);
			}
		}

		[Test]
		public void MT4171 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var code = @"
				namespace NS {
					using System;
					using Foundation;
					using ObjCRuntime;
					class X : NSObject {
						[Export (""a:"")]
						void A ([BindAs (typeof (DateTime), OriginalType = typeof (NSNumber))] ConsoleColor value) {}

						[Export (""b:"")]
						void B ([BindAs (typeof (DateTime?), OriginalType = typeof (NSNumber))] ConsoleColor? value) {}

						[Export (""C"")]
						[return: BindAs (typeof (DateTime), OriginalType = typeof (NSNumber))] 
						ConsoleColor C () { throw new NotImplementedException (); }

						[Export (""d"")]
						[return: BindAs (typeof (DateTime?), OriginalType = typeof (NSNumber))] 
						ConsoleColor? D () { throw new NotImplementedException (); }

						[Export (""E"")]
						[BindAs (typeof (DateTime), OriginalType = typeof (NSNumber))] 
						ConsoleColor E { get; set; }

						[Export (""F"")]
						[BindAs (typeof (DateTime?), OriginalType = typeof (NSNumber))] 
						ConsoleColor? F { get; set; }
					}
				}";
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: code, extraArg: "-debug");
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (4138, "The registrar cannot marshal the property type 'System.ConsoleColor' of the property 'NS.X.E'.", "testApp.cs", 23);
				mtouch.AssertError (4138, "The registrar cannot marshal the property type 'System.Nullable`1<System.ConsoleColor>' of the property 'NS.X.F'.", "testApp.cs", 27);
				mtouch.AssertError (4171, "The BindAs attribute on the parameter #1 is invalid: the BindAs type System.DateTime is different from the parameter type System.ConsoleColor.", "testApp.cs", 8);
				mtouch.AssertError (4171, "The BindAs attribute on the parameter #1 is invalid: the BindAs type System.Nullable`1<System.DateTime> is different from the parameter type System.Nullable`1<System.ConsoleColor>.", "testApp.cs", 11);
				mtouch.AssertError (4171, "The BindAs attribute on the return value of the method NS.X.C is invalid: the BindAs type System.DateTime is different from the return type System.ConsoleColor.", "testApp.cs", 15);
				mtouch.AssertError (4171, "The BindAs attribute on the return value of the method NS.X.D is invalid: the BindAs type System.Nullable`1<System.DateTime> is different from the return type System.Nullable`1<System.ConsoleColor>.", "testApp.cs", 19);
				mtouch.AssertErrorCount (8 /* 2 errors are duplicated */);
			}
		}

		[Test]
		public void MT4172 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var code = @"
				namespace NS {
					using System;
					using Foundation;
					using ObjCRuntime;
					class X : NSObject {
						[Export (""a:"")]
						void A ([BindAs (typeof (DateTime), OriginalType = typeof (NSNumber))] DateTime value) {}
						[Export (""b:"")]
						void B ([BindAs (typeof (DateTime?), OriginalType = typeof (NSNumber))] DateTime? value) {}
						[Export (""d:"")]
						void D ([BindAs (typeof (int?[]), OriginalType = typeof (NSNumber[]))] int?[] value) {}
						[Export (""e:"")]
						void E ([BindAs (typeof (int), OriginalType = typeof (NSNumber))] ref int value) {}
						[Export (""f:"")]
						void F ([BindAs (typeof (int), OriginalType = typeof (NSNumber))] out int value) { throw new NotImplementedException (); }
						[Export (""g:"")]
						void G ([BindAs (typeof (int[,]), OriginalType = typeof (NSNumber[,]))] int[,] value) {}
						[Export (""h:"")]
						void H ([BindAs (typeof (int?[,]), OriginalType = typeof (NSNumber[,]))] int?[,] value) {}
					}
					enum E {
						V,
					}
					class EClass : NSObject {
						[Export (""a:"")]
						void A ([BindAs (typeof (E), OriginalType = typeof (NSString))] E value) {}
						[Export (""d:"")]
						void D ([BindAs (typeof (E?[]), OriginalType = typeof (NSString[]))] E?[] value) {}
					}
				}";
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: code, extraArg: "-debug");
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (4172, "The registrar can't convert from 'System.DateTime' to 'Foundation.NSNumber' for the parameter 'value' in the method NS.X.A.", "testApp.cs", 8);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Nullable`1<System.DateTime>' to 'Foundation.NSNumber' for the parameter 'value' in the method NS.X.B.", "testApp.cs", 10);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Nullable`1<System.Int32>[]' to 'Foundation.NSNumber[]' for the parameter 'value' in the method NS.X.D.", "testApp.cs", 12);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Int32&' to 'Foundation.NSNumber' for the parameter 'value' in the method NS.X.E.", "testApp.cs", 14);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Int32&' to 'Foundation.NSNumber' for the parameter 'value' in the method NS.X.F.", "testApp.cs", 16);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Int32[0...,0...]' to 'Foundation.NSNumber[,]' for the parameter 'value' in the method NS.X.G.", "testApp.cs", 18);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Nullable`1<System.Int32>[0...,0...]' to 'Foundation.NSNumber[,]' for the parameter 'value' in the method NS.X.H.", "testApp.cs", 20);
				mtouch.AssertError (4172, "The registrar can't convert from 'NS.E' to 'Foundation.NSString' for the parameter 'value' in the method NS.EClass.A.", "testApp.cs", 27);
				mtouch.AssertError (4172, "The registrar can't convert from 'System.Nullable`1<NS.E>[]' to 'Foundation.NSString[]' for the parameter 'value' in the method NS.EClass.D.", "testApp.cs", 29);
				mtouch.AssertErrorCount (9);
			}
		}

		[Test]
		public void MT4174 ()
		{
			using (var mtouch = new MTouchTool ()) {
				var code = @"
namespace NS {
	using System;
	using Foundation;
	using ObjCRuntime;

	public class Consumer : NSObject, IProtocolWithOptionalMembers
	{
		[Export (""resolveRecipientsForSearchForMessages:withCompletion:"")]
		public void ResolveRecipients (int arg, Action<bool> completion)
		{
			throw new NotImplementedException ();
		}
	}


	[Protocol (Name = ""INSendMessageIntentHandling"", WrapperType = typeof (ProtocolWithOptionalMembersWrapper))]
	[ProtocolMember (IsRequired = false, IsProperty = false, IsStatic = false, Name = ""ResolveRecipients"", Selector = ""resolveRecipientsForSendMessage:withCompletion:"", ParameterType = new Type [] { typeof (bool), typeof (global::System.Action<bool>) }, ParameterByRef = new bool [] { false, false })]
	public interface IProtocolWithOptionalMembers : INativeObject, IDisposable
	{
	}

	public static partial class ProtocolWithOptionalMembers_Extensions {
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public static void ResolveRecipients (this IProtocolWithOptionalMembers This, bool arg, [BlockProxy (typeof (NIDActionArity1V89))]global::System.Action<bool> completion)
		{
		}
	}

	internal sealed class ProtocolWithOptionalMembersWrapper : BaseWrapper, IProtocolWithOptionalMembers {
		[Preserve (Conditional = true)]
		public ProtocolWithOptionalMembersWrapper (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}
	}

	[UserDelegateType (typeof (global::System.Action<bool>))]
	internal delegate void DActionArity1V89 (IntPtr block, IntPtr obj);

	static internal class SDActionArity1V89 {
		static internal readonly DActionArity1V89 Handler = Invoke;

		[MonoPInvokeCallback (typeof (DActionArity1V89))]
		static void Invoke (IntPtr block, IntPtr obj) {
			throw new NotImplementedException ();
		}
	}

	internal class NIDActionArity1V89 {
		IntPtr blockPtr;
		DActionArity1V89 invoker;

		[Preserve (Conditional=true)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public NIDActionArity1V89 (ref BlockLiteral block)
		{
			throw new NotImplementedException ();
		}

		[Preserve (Conditional=true)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		~NIDActionArity1V89 ()
		{
			throw new NotImplementedException ();
		}

		[Preserve (Conditional=true)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public static global::System.Action<bool> Create (IntPtr block)
		{
			throw new NotImplementedException ();
		}

		[Preserve (Conditional=true)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		void Invoke (bool obj)
		{
		}
	}
}

";
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.CreateTemporaryApp (extraCode: code, extraArg: "-debug");
				mtouch.WarnAsError = new int [] { 4174 };
				mtouch.AssertExecuteFailure ("build");
				mtouch.AssertError (4174, "Unable to locate the block to delegate conversion method for the method NS.Consumer.ResolveRecipients's parameter #2.", "testApp.cs", 11);
				mtouch.AssertErrorCount (1);
			}
		}

		[Test]
		public void NoWarnings ()
		{
			var code = @"
	public partial class MyTableViewCell : UITableViewCell {
		protected MyTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
";
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using UIKit; using Foundation; using System;");
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Linker = MTouchLinker.DontLink; // so that as much as possible is registered
				mtouch.Verbosity = 9; // Increase verbosity, otherwise linker warnings aren't shown
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertNoWarnings ();
				foreach (var line in mtouch.OutputLines) {
					if (line.Contains ("warning: method 'paymentAuthorizationViewController:didAuthorizePayment:handler:' in protocol 'PKPaymentAuthorizationViewControllerDelegate' not implemented [-Wprotocol]"))
						continue; // Xcode 9 beta 1: this method changed from optional to required.
					Assert.That (line, Does.Not.Match ("warning:"), "no warnings");
				}
			}
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
	[Export (""foo"")]
	string Foo { get; set; }
}

class F : E {
	[Export (""foo"")]
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecute ();
				mtouch.AssertNoWarnings ();
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

			// and the lack of warnings/errors in the new static registrar.
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using UIKit;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecute ();
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4132, "The registrar found an invalid generic return type 'V' in the property 'Open`2.FooZap'. The return type must have an 'NSObject' constraint.", "testApp.cs", 9);
				mtouch.AssertError (4132, "The registrar found an invalid generic return type 'V' in the property 'Open`2.Bar'. The return type must have an 'NSObject' constraint.", "testApp.cs", 12);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'V' in the parameter arg of the method 'Open`2.Foo(V)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 6);
				mtouch.AssertErrorCount (3);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4131, "The registrar cannot export static properties in generic classes ('Open`1.Zap').", "testApp.cs", 12);
				mtouch.AssertError (4130, "The registrar cannot export static methods in generic classes ('Open`1.Foo()').", "testApp.cs", 6);
				mtouch.AssertError (4130, "The registrar cannot export static methods in generic classes ('Open`1.Foo(U)').", "testApp.cs", 9);
				mtouch.AssertErrorCount (3);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'T' in the parameter foo of the method 'Parent`1/Nested.Foo(T)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 7);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
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


			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using UIKit;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'T' in the parameter t of the method 'GenericTestClass`1.Arg1(T)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 6);
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		public void GenericType_WithInvalidParameterTypes ()
		{
			var code = @"
		using System.Collections.Generic;
		using Foundation;
		class Open<U> : NSObject where U: NSObject
		{
			[Export (""bar:"")]
			public void Bar (List<U> arg) {} // Not OK, can't marshal lists.
		}
		class C { static void Main () {} }
";

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (code: code, extraArg: "-debug:full");
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Linker = MTouchLinker.DontLink; // faster test
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Collections.Generic.List`1<U>' of the parameter 'arg' in the method 'Open`1.Bar(System.Collections.Generic.List`1<U>)'", "testApp.cs", 7);
				mtouch.AssertErrorCount (1);
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using System; using System.Collections.Generic; using ObjCRuntime;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4132, "The registrar found an invalid generic return type 'U' in the property 'Open`1.BarZap'. The return type must have an 'NSObject' constraint.", "testApp.cs", 15);
				mtouch.AssertError (4132, "The registrar found an invalid generic return type 'U' in the property 'Open`1.F1'. The return type must have an 'NSObject' constraint.", "testApp.cs", 30);
				mtouch.AssertError (4132, "The registrar found an invalid generic return type 'System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>' in the property 'Open`1.F2'. The return type must have an 'NSObject' constraint.", "testApp.cs", 33);
				mtouch.AssertError (4132, "The registrar found an invalid generic return type 'System.Action`1<U>' in the property 'Open`1.F3'. The return type must have an 'NSObject' constraint.", "testApp.cs", 36);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'U' in the parameter arg of the method 'Open`1.Bar(U)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 6);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'U[]' in the parameter arg of the method 'Open`1.Zap(U[])'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 9);
				mtouch.AssertError (4129, "The registrar found an invalid generic return type 'U' in the method 'Open`1.XyZ()'. The generic return type must have an 'NSObject' constraint.", "testApp.cs", 12);
				mtouch.AssertError (4129, "The registrar found an invalid generic return type 'System.Action`1<U>' in the method 'Open`1.ZapBar()'. The generic return type must have an 'NSObject' constraint.", "testApp.cs", 18);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'System.Action`1<U>' in the parameter f of the method 'Open`1.XyZ(System.Action`1<U>)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 21);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>>>' in the parameter f of the method 'Open`1.FooZap(System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>>>)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 24);
				mtouch.AssertError (4129, "The registrar found an invalid generic return type 'System.Collections.Generic.List`1<System.Collections.Generic.List`1<U>>' in the method 'Open`1.ZapBoo()'. The generic return type must have an 'NSObject' constraint.", "testApp.cs", 27);
				mtouch.AssertErrorCount (11);
				mtouch.AssertNoWarnings ();
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation; using ObjCRuntime;", extraArg: "/debug:full /unsafe");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'U&' in the parameter arg of the method 'Open`1.Bar(U&)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 6);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'U[]&' in the parameter arg of the method 'Open`1.Zap(U[]&)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 9);
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: str1, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4113, "The registrar found a generic method: 'GenericMethodClass.Foo(T)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 6);
				mtouch.AssertError (4113, "The registrar found a generic method: 'GenericMethodClass.GenericMethod(T)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 8);
				mtouch.AssertErrorCount (2);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "/debug:full");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.AssertExecute ();
				mtouch.AssertWarning (4174, "Unable to locate the block to delegate conversion method for the method GenericMethodClass.Foo's parameter #1.", "testApp.cs", 5);
				mtouch.AssertWarningCount (1);
			}
		}

		[Test]
		public void GenericMethods2 ()
		{
			var str1 = @"
using Foundation;
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
class C { static void Main () {} }
";
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (code: str1, extraArg: "-debug:full");
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Linker = MTouchLinker.DontLink; // faster test
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (4113, "The registrar found a generic method: 'NullableGenericTestClass`1.Z1(System.Nullable`1<Z>)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 12);
				mtouch.AssertError (4113, "The registrar found a generic method: 'NullableGenericTestClass`1.Z2()'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 17);
				mtouch.AssertError (4113, "The registrar found a generic method: 'NullableGenericTestClass`1.Z3(Z)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 23);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'T' in the parameter foo of the method 'NullableGenericTestClass`1.T1(T)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 28);
				mtouch.AssertError (4128, "The registrar found an invalid generic parameter type 'System.Nullable`1<T>' in the parameter foo of the method 'NullableGenericTestClass`1.T2(System.Nullable`1<T>)'. The generic parameter must have an 'NSObject' constraint.", "testApp.cs", 33);
				mtouch.AssertError (4129, "The registrar found an invalid generic return type 'T' in the method 'NullableGenericTestClass`1.T3()'. The generic return type must have an 'NSObject' constraint.", "testApp.cs", 38);
				mtouch.AssertError (4136, "The registrar cannot marshal the parameter type 'System.Nullable`1<T>' of the parameter 'foo' in the method 'NullableGenericTestClass`1..ctor(System.Nullable`1<T>)'", "testApp.cs", 6);
				mtouch.AssertErrorCount (7);
			}
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

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using Foundation;", extraArg: "-debug:full");
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Linker = MTouchLinker.DontLink; // faster test
				mtouch.AssertExecuteFailure ();
				mtouch.AssertError (4113, "The registrar found a generic method: 'G.Foo1(System.Int32)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 6);
				mtouch.AssertError (4113, "The registrar found a generic method: 'G.Foo2(System.Int32)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 9);
				mtouch.AssertError (4113, "The registrar found a generic method: 'G.Foo3(X)'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", "testApp.cs", 12);
				mtouch.AssertErrorCount (3);
				mtouch.AssertNoWarnings ();
			}
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code, usings: "using System; using Foundation;");
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute ();
				mtouch.AssertNoWarnings ();
			}
		}
	}
}

