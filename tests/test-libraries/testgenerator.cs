using System;
using System.IO;
using System.Linq;
using System.Text;

static class C {
	[Flags]
	enum Architecture
	{
		None = 0,
		Sim32 = 1,
		Sim64 = 2,
		Arm32 = 4,
		Armv7k = 8,
		// Arm64 is never stret
	}

	// X86: structs > 8 + structs with 3 members.
	// X64: structs > 16
	// ARM32: all structs, except those matching an integral platform type (i.e. a struct with a single int, but not a struct with a single float).
	// ARM64: never
	// armv7k: > 16, except homogeneous types with no more than 4 elements (i.e. structs with 3 or 4 doubles).
	// the numbers below are bitmasks of Architecture values.
	static string [] structs_and_stret =  {
		/* integral types */
		"c:0", "cc:4", "ccc:5",  "cccc:4",
		"s:0", "ss:4", "sss:5",  "ssss:4",
		"i:0", "ii:4", "iii:5",  "iiii:5",  "iiiii:15",
		"l:4", "ll:5", "lll:15", "llll:15", "lllll:15",
		/* floating point types */
		"f:4", "ff:4", "fff:5", "ffff:5",  "fffff:15",
		"d:4", "dd:5", "ddd:7", "dddd:7",  "ddddd:15",
		/* mixed types */
		"if:4", "fi:4", // 8 bytes
		"iff:5", // 12 bytes
		"iiff:5", // 16 bytes
		"id:5", "di:5", // 16 bytes
		"iid:5", // 16 bytes
		"idi:15", // 16 bytes on i386 and 24 bytes on x86_64 (due to alignment)
		"ddi:15", // 24 bytes
		"didi:15", // 24 bytes on 32-bit arch, 32 bytes on 64-bit arch
		"idid:15", // 24 bytes on 32-bit arch, 32 bytes on 64-bit arch
		"dldl:15",
		"ldld:15",
		"fifi:5",
		"ifif:5",
	};

	static string [] structs = structs_and_stret.Select ((v) => v.IndexOf (':') >= 0 ? v.Substring (0, v.IndexOf (':')) : v).ToArray ();
	static Architecture [] strets = structs_and_stret.Select ((v) => v.IndexOf (':') >= 0 ? (Architecture) int.Parse (v.Substring (v.IndexOf (':') + 1)) : Architecture.None).ToArray ();

	static string GetNativeName (char t)
	{
		switch (t) {
		case 'f': return "float";
		case 'd': return "double";
		case 'c': return "char";
		case 's': return "short";
		case 'i': return "int";
		case 'l': return "long long";
		default:
			throw new NotImplementedException ();
		}
	}

	static string GetManagedName (char t)
	{
		switch (t) {
		case 'f': return "float";
		case 'd': return "double";
		case 'c': return "byte";
		case 's': return "short";
		case 'i': return "int";
		case 'l': return "long";
		default:
			throw new NotImplementedException ();
		}
	}

	static string GetValue (char t, int i, int multiplier = 1)
	{
		switch (t) {
		case 'c':
		case 's':
		case 'i':
		case 'l': return ((i + 1) * multiplier).ToString ();
		case 'f': return (3.14f * (i + 1) * multiplier) + "f";
		case 'd': return (1.23f * (i + 1) * multiplier).ToString ();
		default:
			throw new NotImplementedException ();
		}
	}

	static void WriteLibTestStructH ()
	{
		var w = new StringBuilder ();

		foreach (var s in structs) {
			w.Append ($"struct S{s} {{ ");
			for (int i = 0; i < s.Length; i++) {
				w.Append (GetNativeName (s [i])).Append (" x").Append (i).Append ("; ");
			}
			w.AppendLine ($"}} S{s};");
		}

		File.WriteAllText ("libtest.structs.h", w.ToString ());
	}

	static void WriteLibTestDecompileM ()
	{
		var w = new StringBuilder ();

		// This is code to be disassembled to see how it's compiled by clang
		// to see if a particular structure is using objc_msgSend_stret or not.
		// 
		// To disassemble:
		// otool -vVt .libs/ios/libtest.armv7.o
		//
		// Then in the _decompile_me output, look for the _____* function call,
		// matching the structure you want to check, and then backtrack until
		// you see either an objc_msgSend or objc_msgSend_stret call, and you
		// have your answer.
#if false
		w.AppendLine ("extern \"C\" {");
		foreach (var s in structs)
			w.AppendLine ($"void _____________________________________{s} (struct S{s} x)  __attribute__ ((optnone)) {{ }}");
		w.AppendLine ("void decompile_me () __attribute__ ((optnone))");
		w.AppendLine ("{");
		w.AppendLine ("\tObjCRegistrarTest *obj = NULL;");
		foreach (var s in structs) {
			w.AppendLine ($"\t_____________________________________{s} ([obj PS{s}]);");
		}
		w.AppendLine ("}");
		w.AppendLine ("}");
#endif

		File.WriteAllText ("libtest.decompile.m", w.ToString ());
	}

	static void WriteLibTestPropertiesH ()
	{
		var w = new StringBuilder ();

		foreach (var s in structs)
			w.AppendLine ($"\t@property struct S{s} PS{s};");

		File.WriteAllText ("libtest.properties.h", w.ToString ());
	}

	static void WriteApiDefinition ()
	{
		var w = new StringBuilder ();

		w.AppendLine (@"using System;
#if !__WATCHOS__
using System.Drawing;
#endif

#if __UNIFIED__
using ObjCRuntime;
using Foundation;
using UIKit;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace Bindings.Test {
	partial interface ObjCRegistrarTest {

");
		
		foreach (var s in structs) {
			w.AppendLine ($"\t\t[Export (\"PS{s}\")]");
			w.AppendLine ($"\t\tS{s} PS{s} {{ get; set; }}");
			w.AppendLine ();
		}

		w.AppendLine (@"	}
}");

		File.WriteAllText ("../bindings-test/ApiDefinition.generated.cs", w.ToString ());
	}

	static void WriteStructsAndEnums ()
	{
		var w = new StringBuilder ();

		w.AppendLine (@"using System;
using System.Runtime.InteropServices;

#if !__UNIFIED__
using nint=System.Int32;
#endif

namespace Bindings.Test
{
");

		foreach (var s in structs) {
			w.AppendLine ($"\tpublic struct S{s} {{ ");
			w.Append ("\t\t");
			for (int i = 0; i < s.Length; i++) {
				w.Append ("public ").Append (GetManagedName (s [i])).Append (" x").Append (i).Append ("; ");
			}
			w.AppendLine ();
			w.Append ($"\t\tpublic override string ToString () {{ return $\"S{s} [");
			for (int i = 0; i < s.Length; i++) {
				w.Append ("{x").Append (i).Append ("};");
			}
			w.Length--;
			w.AppendLine ("]\"; } ");
			w.AppendLine ("\t}");
			w.AppendLine ();
		}

		w.AppendLine (@"}");

		File.WriteAllText ("../bindings-test/StructsAndEnums.generated.cs", w.ToString ());
	}

	static void WriteRegistrarTests ()
	{
		var w = new StringBuilder ();

		w.AppendLine (@"
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using MonoTouchException=ObjCRuntime.RuntimeException;
using NativeException=Foundation.MonoTouchException;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouchException=MonoTouch.RuntimeException;
using NativeException=MonoTouch.Foundation.MonoTouchException;
#endif
using NUnit.Framework;
using Bindings.Test;

using XamarinTests.ObjCRuntime;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RegistrarTestGenerated {");

		foreach (var s in structs) {
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void Test_{s} ()");
			w.AppendLine ("\t\t{");
			w.AppendLine ("\t\t\tusing (var tc = new ObjCRegistrarTest ()) {");
			w.AppendLine ($"\t\t\t\tvar s = tc.PS{s};");
			for (int i = 0; i < s.Length; i++)
				w.AppendLine ($"\t\t\t\tAssert.AreEqual (0, s.x{i}, \"pre-#{i}\");");
			w.Append ($"\t\t\t\tvar k = new S{s} () {{ ");
			for (int i = 0; i < s.Length; i++)
				w.Append ($"x{i} = ").Append (GetValue (s [i], i)).Append (", ");
			w.Length -= 2;
			w.AppendLine ("};");
			w.AppendLine ($"\t\t\t\ttc.PS{s} = k;");
			w.AppendLine ($"\t\t\t\ts = tc.PS{s};");
			for (int i = 0; i < s.Length; i++)
				w.AppendLine ($"\t\t\t\tAssert.AreEqual (k.x{i}, s.x{i}, \"post-#{i}\");");
			w.AppendLine ("\t\t\t}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();
		}

		w.AppendLine (@"	}
}");

		File.WriteAllText ("../monotouch-test/ObjCRuntime/RegistrarTest.generated.cs", w.ToString ());
	}

	static void WriteTrampolineTests ()
	{
		var w = new StringBuilder ();

		w.AppendLine (@"
using System;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;
using Bindings.Test;

using XamarinTests.ObjCRuntime;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TrampolineTestGenerated {");
		w.AppendLine ("\t\tconst string LIBOBJC_DYLIB = \"/usr/lib/libobjc.dylib\";");
		w.AppendLine ();

		w.AppendLine ("\t\t[Register (\"GeneratedStretTrampolines\")]");
		w.AppendLine ("\t\t[Preserve (AllMembers = true)]");
		w.AppendLine ("\t\tpublic class GeneratedStretTrampolines : NSObject {");
		foreach (var s in structs) {
			w.AppendLine ();
			w.AppendLine ($"\t\t\t// {s}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t[Export (\"Test_{s}Struct\")]");
			w.AppendLine ($"\t\t\tS{s} Test_{s}Struct ()");
			w.AppendLine ($"\t\t\t{{");
			w.AppendLine ($"\t\t\t\treturn {GenerateNewExpression (s, 1)};");
			w.AppendLine ($"\t\t\t}}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t[Export (\"Test_Static{s}Struct\")]");
			w.AppendLine ($"\t\t\tstatic S{s} Test_Static{s}Struct ()");
			w.AppendLine ($"\t\t\t{{");
			w.AppendLine ($"\t\t\t\treturn {GenerateNewExpression (s, 2)};");
			w.AppendLine ($"\t\t\t}}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\tS{s} Test_{s}StructProperty {{");
			w.AppendLine ($"\t\t\t\t[Export (\"Test_{s}StructProperty\")]");
			w.AppendLine ($"\t\t\t\tget {{ return {GenerateNewExpression (s, 3)}; }}");
			w.AppendLine ($"\t\t\t}}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\tstatic S{s} Test_Static{s}StructProperty {{");
			w.AppendLine ($"\t\t\t\t[Export (\"Test_Static{s}StructProperty\")]");
			w.AppendLine ($"\t\t\t\tget {{ return {GenerateNewExpression (s, 4)}; }}");
			w.AppendLine ($"\t\t\t}}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t[Export (\"Test_{s}Struct_out_double:\")]");
			w.AppendLine ($"\t\t\tS{s} Test_{s}Struct (out double x0)");
			w.AppendLine ($"\t\t\t{{");
			w.AppendLine ($"\t\t\t\tx0 = 3.14;");
			w.AppendLine ($"\t\t\t\treturn {GenerateNewExpression (s, 5)};");
			w.AppendLine ($"\t\t\t}}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t[Export (\"Test_Static{s}Struct_out_float:\")]");
			w.AppendLine ($"\t\t\tstatic S{s} Test_Static{s}Struct (out float x0)");
			w.AppendLine ($"\t\t\t{{");
			w.AppendLine ($"\t\t\t\tx0 = 3.15f;");
			w.AppendLine ($"\t\t\t\treturn {GenerateNewExpression (s, 6)};");
			w.AppendLine ($"\t\t\t}}");
		}
		w.AppendLine ("\t\t}");

		foreach (var s in structs) {
			if (s.Length == 1 || s.Contains ('c'))
				continue; // our trampolines don't currently like structs with a single member, nor char members

			bool never;
			w.AppendLine ();
			w.AppendLine ($"\t\t[Test]");
			w.AppendLine ($"\t\tpublic void Test_{s} ()");
			w.AppendLine ($"\t\t{{");
			w.AppendLine ($"\t\t\tIntPtr class_ptr = Class.GetHandle (typeof (GeneratedStretTrampolines));");
			w.AppendLine ($"\t\t\tS{s} rv = new S{s} ();");
			w.AppendLine ($"\t\t\tdouble rvd;");
			w.AppendLine ($"\t\t\tfloat rvf;");
			w.AppendLine ($"\t\t\tusing (var obj = new GeneratedStretTrampolines ()) {{");

			WriteStretConditions (w, s, out never);
			if (never) {
				w.AppendLine ($"\t\t\t\trv = S{s}_objc_msgSend (obj.Handle, new Selector (\"Test_{s}Struct\").Handle);");
			} else {
				w.AppendLine ($"\t\t\t\t\tS{s}_objc_msgSend_stret (out rv, obj.Handle, new Selector (\"Test_{s}Struct\").Handle);");
				w.AppendLine ($"\t\t\t\t}} else {{");
				w.AppendLine ($"\t\t\t\t\trv = S{s}_objc_msgSend (obj.Handle, new Selector (\"Test_{s}Struct\").Handle);");
				w.AppendLine ($"\t\t\t\t}}");
			}
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (({GenerateNewExpression (s, 1)}).ToString (), rv.ToString (), \"a\");");
			w.AppendLine ();

			WriteStretConditions (w, s, out never);
			if (never) {
				w.AppendLine ($"\t\t\t\trv = S{s}_objc_msgSend (class_ptr, new Selector (\"Test_Static{s}Struct\").Handle);");
			} else {
				w.AppendLine ($"\t\t\t\t\tS{s}_objc_msgSend_stret (out rv, class_ptr, new Selector (\"Test_Static{s}Struct\").Handle);");
				w.AppendLine ($"\t\t\t\t}} else {{");
				w.AppendLine ($"\t\t\t\t\trv = S{s}_objc_msgSend (class_ptr, new Selector (\"Test_Static{s}Struct\").Handle);");
				w.AppendLine ($"\t\t\t\t}}");
			}
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (({GenerateNewExpression (s, 2)}).ToString (), rv.ToString (), \"a\");");
			w.AppendLine ();

			WriteStretConditions (w, s, out never);
			if (never) {
				w.AppendLine ($"\t\t\t\trv = S{s}_objc_msgSend (obj.Handle, new Selector (\"Test_{s}StructProperty\").Handle);");
			} else {
				w.AppendLine ($"\t\t\t\t\tS{s}_objc_msgSend_stret (out rv, obj.Handle, new Selector (\"Test_{s}StructProperty\").Handle);");
				w.AppendLine ($"\t\t\t\t}} else {{");
				w.AppendLine ($"\t\t\t\t\trv = S{s}_objc_msgSend (obj.Handle, new Selector (\"Test_{s}StructProperty\").Handle);");
				w.AppendLine ($"\t\t\t\t}}");
			}
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (({GenerateNewExpression (s, 3)}).ToString (), rv.ToString (), \"a\");");
			w.AppendLine ();

			WriteStretConditions (w, s, out never);
			if (never) {
				w.AppendLine ($"\t\t\t\trv = S{s}_objc_msgSend (class_ptr, new Selector (\"Test_Static{s}StructProperty\").Handle);");
			} else {
				w.AppendLine ($"\t\t\t\t\tS{s}_objc_msgSend_stret (out rv, class_ptr, new Selector (\"Test_Static{s}StructProperty\").Handle);");
				w.AppendLine ($"\t\t\t\t}} else {{");
				w.AppendLine ($"\t\t\t\t\trv = S{s}_objc_msgSend (class_ptr, new Selector (\"Test_Static{s}StructProperty\").Handle);");
				w.AppendLine ($"\t\t\t\t}}");
			}
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (({GenerateNewExpression (s, 4)}).ToString (), rv.ToString (), \"a\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\trvd = 0;");
			WriteStretConditions (w, s, out never);
			if (never) {
				w.AppendLine ($"\t\t\t\trv = S{s}_objc_msgSend_out_double (obj.Handle, new Selector (\"Test_{s}Struct_out_double:\").Handle, out rvd);");
			} else {
				w.AppendLine ($"\t\t\t\t\tS{s}_objc_msgSend_stret_out_double (out rv, obj.Handle, new Selector (\"Test_{s}Struct_out_double:\").Handle, out rvd);");
				w.AppendLine ($"\t\t\t\t}} else {{");
				w.AppendLine ($"\t\t\t\t\trv = S{s}_objc_msgSend_out_double (obj.Handle, new Selector (\"Test_{s}Struct_out_double:\").Handle, out rvd);");
				w.AppendLine ($"\t\t\t\t}}");
			}
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (({GenerateNewExpression (s, 5)}).ToString (), rv.ToString (), \"a\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (3.14, rvd, \"double out\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\trvf = 0;");
			WriteStretConditions (w, s, out never);
			if (never) {
				w.AppendLine ($"\t\t\t\trv = S{s}_objc_msgSend_out_float (class_ptr, new Selector (\"Test_Static{s}Struct_out_float:\").Handle, out rvf);");
			} else {
				w.AppendLine ($"\t\t\t\t\tS{s}_objc_msgSend_stret_out_float (out rv, class_ptr, new Selector (\"Test_Static{s}Struct_out_float:\").Handle, out rvf);");
				w.AppendLine ($"\t\t\t\t}} else {{");
				w.AppendLine ($"\t\t\t\t\trv = S{s}_objc_msgSend_out_float (class_ptr, new Selector (\"Test_Static{s}Struct_out_float:\").Handle, out rvf);");
				w.AppendLine ($"\t\t\t\t}}");
			}
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (({GenerateNewExpression (s, 6)}).ToString (), rv.ToString (), \"a\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (3.15f, rvf, \"float out\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ($"\t\t}}");


			// objc_msgSend variants
			w.AppendLine ();
			w.AppendLine ($"\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			w.AppendLine ($"\t\textern static S{s} S{s}_objc_msgSend (IntPtr received, IntPtr selector);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			w.AppendLine ($"\t\textern static S{s} S{s}_objc_msgSend_out_float (IntPtr received, IntPtr selector, out float x1);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			w.AppendLine ($"\t\textern static S{s} S{s}_objc_msgSend_out_double (IntPtr received, IntPtr selector, out double x1);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend_stret\")]");
			w.AppendLine ($"\t\textern static void S{s}_objc_msgSend_stret (out S{s} rv, IntPtr received, IntPtr selector);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend_stret\")]");
			w.AppendLine ($"\t\textern static void S{s}_objc_msgSend_stret_out_float (out S{s} rv, IntPtr received, IntPtr selector, out float x1);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend_stret\")]");
			w.AppendLine ($"\t\textern static void S{s}_objc_msgSend_stret_out_double (out S{s} rv, IntPtr received, IntPtr selector, out double x1);");
		}

		w.AppendLine (@"	}
}");

		File.WriteAllText ("../monotouch-test/ObjCRuntime/TrampolineTest.generated.cs", w.ToString ());
	}

	static void WriteStretConditions (StringBuilder w, string s, out bool never)
	{
		var stret = strets [Array.IndexOf (structs, s)];
		if (stret == Architecture.None) {
			never = true;
		} else {
			never = false;
			w.Append ("\t\t\t\tif (");
			if ((stret & Architecture.Arm32) == Architecture.Arm32)
				w.Append ("TrampolineTest.IsArm32 || ");
			if ((stret & Architecture.Armv7k) == Architecture.Armv7k)
				w.Append ("TrampolineTest.IsArmv7k || ");
			if ((stret & Architecture.Sim32) == Architecture.Sim32)
				w.Append ("TrampolineTest.IsSim32 || ");
			if ((stret & Architecture.Sim64) == Architecture.Sim64)
				w.Append ("TrampolineTest.IsSim64 || ");
			w.Length -= 4;
			w.AppendLine (") {");
		}
	}

	static string GenerateNewExpression (string s, int multiplier = 1)
	{
		var sb = new StringBuilder ();
		sb.Append ($"new S{s} () {{ ");
		for (int i = 0; i < s.Length; i++)
			sb.Append ("x").Append (i).Append (" = ").Append (GetValue (s [i], i, multiplier)).Append (", ");
		sb.Length -= 2;
		sb.Append (" }");
		return sb.ToString ();
	}

	static void Main ()
	{
		while (Path.GetFileName (Environment.CurrentDirectory) != "test-libraries")
			Environment.CurrentDirectory = Path.GetDirectoryName (Environment.CurrentDirectory);

		/* native code */
		WriteLibTestStructH ();
		WriteLibTestDecompileM ();
		WriteLibTestPropertiesH ();

		/* binding code */
		WriteApiDefinition ();
		WriteStructsAndEnums ();

		/* tests */
		WriteRegistrarTests ();
		WriteTrampolineTests ();

		Console.WriteLine ("Generated test files");
	}
}
