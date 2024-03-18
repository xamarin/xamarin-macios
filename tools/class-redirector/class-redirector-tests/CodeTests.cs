using System;
using Mono.Cecil;
using ClassRedirector;

namespace ClassRedirectorTests;

[TestFixture]
public class CodeTests {
	const string commonCode = @"
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	public class Runtime
	{

		public static class ClassHandles
		{
			public static unsafe void InitializeClassHandles (MTClassMap* map)
			{
			}
		}

		[Flags]
		public enum MTTypeFlags : uint {
			None = 0,
			CustomType = 1,
			UserType = 2,
		}

		[StructLayout (LayoutKind.Sequential, Pack = 1)]
		public struct MTClassMap {
			public MTClassMap (IntPtr handle, uint type_reference, MTTypeFlags flags)
			{
				this.handle = handle;
				this.type_reference = type_reference;
				this.flags = flags;
			}
			public IntPtr handle;
			public uint type_reference;
			public MTTypeFlags flags;
		}
	}

	public struct NativeHandle : IEquatable<NativeHandle> {
		readonly IntPtr handle;

		public IntPtr Handle {
			get { return handle; }
		}

		public static NativeHandle Zero = default (NativeHandle);

		public NativeHandle (IntPtr handle)
		{
			this.handle = handle;
		}

		public static bool operator == (NativeHandle left, IntPtr right)
		{
			return left.handle == right;
		}

		public static bool operator == (NativeHandle left, NativeHandle right)
		{
			return left.handle == right.handle;
		}

		public static bool operator == (IntPtr left, NativeHandle right)
		{
			return left == right.Handle;
		}

		public static bool operator != (NativeHandle left, IntPtr right)
		{
			return left.handle != right;
		}

		public static bool operator != (IntPtr left, NativeHandle right)
		{
			return left != right.Handle;
		}

		public static bool operator != (NativeHandle left, NativeHandle right)
		{
			return left.handle != right.Handle;
		}

		// Should this be made explicit? The JIT seems to optimize conversions away to
		// treat everything as a plain IntPtr, so I'm not sure there's any reason
		// to not keep it implicit.
		public static implicit operator IntPtr (NativeHandle value)
		{
			return value.Handle;
		}

		// Should this be made explicit? The JIT seems to optimize conversions away to
		// treat everything as a plain IntPtr, so I'm not sure there's any reason
		// to not keep it implicit.
		public static implicit operator NativeHandle (IntPtr value)
		{
			return new NativeHandle (value);
		}

		public unsafe static explicit operator void* (NativeHandle value)
		{
			return (void *) (IntPtr) value;
		}

		public unsafe static explicit operator NativeHandle (void * value)
		{
			return new NativeHandle ((IntPtr) value);
		}

		public override bool Equals (object o)
		{
			if (o is NativeHandle nh)
				return nh.handle == this.handle;
			return false;
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		public bool Equals (NativeHandle other)
		{
			return other.handle == handle;
		}

		public override string ToString ()
		{
			return ""0x"" + handle.ToString (""x"");
		}
	}
}
";

	const string initTestCode = @"
namespace ObjCRuntime {
	public class Program {
		public static int Main (string[] args) {
			var map = new Runtime.MTClassMap [] {
				new Runtime.MTClassMap(new IntPtr (42), 0, Runtime.MTTypeFlags.None)
			};
			unsafe {
				fixed (Runtime.MTClassMap* mapPtr = &map [0]) {
					Runtime.ClassHandles.InitializeClassHandles (mapPtr);
				}
			}
			var foo = new Foo ();
			Console.WriteLine (foo.ClassHandle);
			return 0;
		}
	}

	public class Foo {
		static NativeHandle class_ptr = NativeHandle.Zero;
		public Foo () { }
		public NativeHandle ClassHandle => class_ptr;
	}
}
";

	[Test]
	public void CompilesOK ()
	{
		var testCode = commonCode + initTestCode;
		var result = Compiler.Compile (testCode);
		Assert.That (String.IsNullOrEmpty (result.Error), $"Compile failure: {result.Error}");
	}

	[Test]
	public void CompliesRunWithDefaultOutput ()
	{
		var testCode = commonCode + initTestCode;
		var result = Compiler.Compile (testCode);
		Assert.That (String.IsNullOrEmpty (result.Error), $"Compile failure: {result.Error}");

		var codeOutput = Compiler.Run ("mono", new List<string> () { result.OutputFileName });
		Assert.That (codeOutput, Is.EqualTo ("0x0\n"), "incorrect executable output");
	}

	[Test]
	public void BasicProcessing ()
	{
		var testCode = commonCode + initTestCode;
		var result = Compiler.Compile (testCode);
		Assert.That (String.IsNullOrEmpty (result.Error), $"Compile failure: {result.Error}");

		var map = new CSToObjCMap () {
			["ObjCRuntime.Foo"] = new ObjCNameIndex ("xxx", 0),
		};


		var rewriter = new Rewriter (map, result.OutputFileName, new string [] { result.OutputFileName });
		rewriter.Process ();
		var codeOutput = Compiler.Run ("mono", new List<string> () { result.OutputFileName });
		Assert.That (codeOutput, Is.EqualTo ("0x2a\n"), "incorrect executable output");
	}

	const string noChangeTestCode = @"
namespace ObjCRuntime {
	public class Program {
		public static int Main (string[] args) {
			var map = new Runtime.MTClassMap [] {
				new Runtime.MTClassMap(new IntPtr (42), 0, Runtime.MTTypeFlags.None)
			};
			unsafe {
				fixed (Runtime.MTClassMap* mapPtr = &map [0]) {
					Runtime.ClassHandles.InitializeClassHandles (mapPtr);
				}
			}
			var bar = new Bar ();
			Console.WriteLine (bar.ClassHandle);
			return 0;
		}
	}

	public class Foo {
		static NativeHandle class_ptr = NativeHandle.Zero;
		public Foo () { }
		public NativeHandle ClassHandle => class_ptr;
	}

	public class Bar {
		static NativeHandle class_ptr = NativeHandle.Zero;
		public Bar () { }
		public NativeHandle ClassHandle => class_ptr;
	}
}
";

	[Test]
	public void LeavesBarAlone ()
	{
		var testCode = commonCode + noChangeTestCode;
		var result = Compiler.Compile (testCode);
		Assert.That (String.IsNullOrEmpty (result.Error), $"Compile failure: {result.Error}");

		var map = new CSToObjCMap () {
			["ObjCRuntime.Foo"] = new ObjCNameIndex ("xxx", 0),
		};


		var rewriter = new Rewriter (map, result.OutputFileName, new string [] { result.OutputFileName });
		rewriter.Process ();
		var codeOutput = Compiler.Run ("mono", new List<string> () { result.OutputFileName });
		Assert.That (codeOutput, Is.EqualTo ("0x0\n"), "incorrect executable output");
	}

	const string noCCtorTestCode = @"
namespace ObjCRuntime {
	public class Program {
		public static int Main (string[] args) {
			var map = new Runtime.MTClassMap [] {
				new Runtime.MTClassMap(new IntPtr (42), 0, Runtime.MTTypeFlags.None)
			};
			unsafe {
				fixed (Runtime.MTClassMap* mapPtr = &map [0]) {
					Runtime.ClassHandles.InitializeClassHandles (mapPtr);
				}
			}
			var foo = new Foo ();
			Console.WriteLine (foo.ClassHandle);
			return 0;
		}
	}

	public class Class {
		public static NativeHandle GetHandle (string s) {
			return NativeHandle.Zero;
		}
	}

	public class Foo {
		static NativeHandle class_ptr = Class.GetHandle (""nothing"");
		public Foo () { }
		public NativeHandle ClassHandle => class_ptr;
	}
}
";

	[Test]
	public void RemovesCCtor ()
	{
		var testCode = commonCode + noCCtorTestCode;
		var result = Compiler.Compile (testCode);
		Assert.That (String.IsNullOrEmpty (result.Error), $"Compile failure: {result.Error}");

		var map = new CSToObjCMap () {
			["ObjCRuntime.Foo"] = new ObjCNameIndex ("xxx", 0),
		};


		var rewriter = new Rewriter (map, result.OutputFileName, new string [] { result.OutputFileName });
		rewriter.Process ();
		var codeOutput = Compiler.Run ("mono", new List<string> () { result.OutputFileName });
		Assert.That (codeOutput, Is.EqualTo ("0x2a\n"), "incorrect executable output");
		var module = ModuleDefinition.ReadModule (result.OutputFileName);
		var type = module.Types.FirstOrDefault (t => t.Name == "Foo");
		Assert.NotNull (type, "didn't find Foo");
		var cctor = type.Methods.FirstOrDefault (m => m.Name == ".cctor");
		Assert.IsNull (cctor, "we had a cctor - oops");
	}

	const string multiObjectCode = @"
namespace ObjCRuntime {
	public class Program {
		public static int Main (string[] args) {
			var map = new Runtime.MTClassMap [] {
				new Runtime.MTClassMap(new IntPtr (42), 0, Runtime.MTTypeFlags.None),
				new Runtime.MTClassMap(new IntPtr (43), 1, Runtime.MTTypeFlags.None),
				new Runtime.MTClassMap(new IntPtr (44), 2, Runtime.MTTypeFlags.None),
			};
			unsafe {
				fixed (Runtime.MTClassMap* mapPtr = &map [0]) {
					Runtime.ClassHandles.InitializeClassHandles (mapPtr);
				}
			}
			var baz = new Baz ();
			Console.WriteLine (baz.ClassHandle);
			return 0;
		}
	}

	public class Class {
		public static NativeHandle GetHandle (string s) {
			return NativeHandle.Zero;
		}
	}

	public class Foo {
		static NativeHandle class_ptr = Class.GetHandle (""nothing"");
		public Foo () { }
		public NativeHandle ClassHandle => class_ptr;
	}

	public class Bar {
		static NativeHandle class_ptr = Class.GetHandle (""nothing"");
		public Bar () { }
		public NativeHandle ClassHandle => class_ptr;
	}

	public class Baz {
		static NativeHandle class_ptr = Class.GetHandle (""nothing"");
		public Baz () { }
		public NativeHandle ClassHandle => class_ptr;
	}
}
";

	[Test]
	public void MultiObjects ()
	{
		var testCode = commonCode + multiObjectCode;
		var result = Compiler.Compile (testCode);
		Assert.That (String.IsNullOrEmpty (result.Error), $"Compile failure: {result.Error}");

		var map = new CSToObjCMap () {
			["ObjCRuntime.Foo"] = new ObjCNameIndex ("xxx", 0),
			["ObjCRuntime.Bar"] = new ObjCNameIndex ("yyy", 1),
			["ObjCRuntime.Baz"] = new ObjCNameIndex ("zzz", 2),
		};


		var rewriter = new Rewriter (map, result.OutputFileName, new string [] { result.OutputFileName });
		rewriter.Process ();
		var codeOutput = Compiler.Run ("mono", new List<string> () { result.OutputFileName });
		Assert.That (codeOutput, Is.EqualTo ("0x2c\n"), "incorrect executable output");
	}
}

