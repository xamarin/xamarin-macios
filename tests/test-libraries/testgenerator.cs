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

	class BindAsData
	{
		public string Managed;
		public string Native;
		public string ManagedCondition;
		public string ManagedNewExpression;
		public string Map;
		public string MapFrom;
		public Version MinXcodeVersion;
		public string ToNSNumberCastExpression;
		public string FromNSNumberCastExpression;
	}

	static BindAsData [] bindas_nsnumber = new [] {
		new BindAsData { Managed = "Byte", Native = "uint8_t", ManagedNewExpression = "((byte) 1)", Map = ".ByteValue" },
		new BindAsData { Managed = "SByte", Native = "int8_t", ManagedNewExpression = "((sbyte) 1)", Map = ".SByteValue" },
		new BindAsData { Managed = "Int16", Native = "int16_t", ManagedNewExpression = "((short) 1)", Map = ".Int16Value" },
		new BindAsData { Managed = "UInt16", Native = "uint16_t", ManagedNewExpression = "((ushort) 1)", Map = ".UInt16Value" },
		new BindAsData { Managed = "Int32", Native = "int32_t", ManagedNewExpression = "((int) 1)", Map = ".Int32Value" },
		new BindAsData { Managed = "UInt32", Native = "uint32_t", ManagedNewExpression = "((uint) 1)", Map = ".UInt32Value" },
		new BindAsData { Managed = "Int64", Native = "int64_t", ManagedNewExpression = "((long) 1)", Map = ".Int64Value" },
		new BindAsData { Managed = "UInt64", Native = "uint64_t", ManagedNewExpression = "((ulong) 1)", Map = ".UInt64Value" },
		new BindAsData { Managed = "Single", Native = "float", ManagedNewExpression = "((float) 1.1)", Map = ".FloatValue" },
		new BindAsData { Managed = "Double", Native = "double", ManagedNewExpression = "((double) 1.2)", Map = ".DoubleValue" },
		new BindAsData { Managed = "nint", Native = "NSInteger", ManagedNewExpression = "((nint) 1)", Map = ".NIntValue" },
		new BindAsData { Managed = "nuint", Native = "NSUInteger", ManagedNewExpression = "((nuint) 1)", Map = ".NUIntValue" },
		new BindAsData { Managed = "nfloat", Native = "NSFloat", ManagedNewExpression = "((nfloat) 1)", Map = ".NFloatValue" },
		new BindAsData { Managed = "Boolean", Native = "BOOL", ManagedNewExpression = "true", Map = ".BoolValue" },
		new BindAsData { Managed = "NSStreamStatus", Native = "NSStreamStatus", ManagedNewExpression = "NSStreamStatus.Closed", Map = ".UInt64Value", ToNSNumberCastExpression = "(ulong) ", FromNSNumberCastExpression = "(NSStreamStatus) " },
	};
	static BindAsData[] bindas_nsvalue = new [] {
		new BindAsData { Managed = "CGAffineTransform", Native = "CGAffineTransform", ManagedNewExpression = "new CGAffineTransform (1, 2, 3, 4, 5, 6)", Map = ".CGAffineTransformValue", MapFrom = "FromCGAffineTransform" },
		new BindAsData { Managed = "NSRange", Native = "NSRange", ManagedNewExpression = "new NSRange (7, 8)", Map = ".RangeValue", MapFrom = "FromRange" },
		new BindAsData { Managed = "CGVector", Native = "CGVector", ManagedNewExpression = "new CGVector (9, 10)", Map = ".CGVectorValue", MapFrom = "FromCGVector" },
		new BindAsData { Managed = "SCNMatrix4", Native = "SCNMatrix4", ManagedNewExpression = "SCNMatrix4.Identity", Map = ".SCNMatrix4Value", MapFrom = "FromSCNMatrix4" },
		new BindAsData { Managed = "CLLocationCoordinate2D", Native = "CLLocationCoordinate2D", ManagedNewExpression = "new CLLocationCoordinate2D (11, 12)", Map = ".CoordinateValue", MapFrom = "FromMKCoordinate" },
		new BindAsData { Managed = "SCNVector3", Native = "SCNVector3", ManagedNewExpression = "new SCNVector3 (13, 14, 15)", Map = ".Vector3Value", MapFrom = "FromVector" },
		new BindAsData { Managed = "SCNVector4", Native = "SCNVector4", ManagedNewExpression = "new SCNVector4 (16, 17, 18, 19)", Map = ".Vector4Value", MapFrom = "FromVector" },
		new BindAsData { Managed = "CGPoint", Native = "CGPoint", ManagedCondition = "XAMCORE_2_0", ManagedNewExpression = "new CGPoint (19, 20)", Map = ".CGPointValue", MapFrom = "FromCGPoint" },
		new BindAsData { Managed = "CGSize", Native = "CGSize", ManagedCondition = "XAMCORE_2_0", ManagedNewExpression = "new CGSize (21, 22)", Map = ".CGSizeValue", MapFrom = "FromCGSize" },
		new BindAsData { Managed = "CGRect", Native = "CGRect", ManagedCondition = "XAMCORE_2_0", ManagedNewExpression = "new CGRect (23, 24, 25, 26)", Map = ".CGRectValue", MapFrom = "FromCGRect" },
		new BindAsData { Managed = "UIEdgeInsets", Native = "UIEdgeInsets", ManagedCondition = "HAVE_UIKIT", ManagedNewExpression = "new UIEdgeInsets (27, 28, 29, 30)", Map = ".UIEdgeInsetsValue", MapFrom = "FromUIEdgeInsets" },
		new BindAsData { Managed = "UIOffset", Native = "UIOffset", ManagedCondition = "HAVE_UIKIT", ManagedNewExpression = "new UIOffset (31, 32)", Map = ".UIOffsetValue", MapFrom = "FromUIOffset" },
		new BindAsData { Managed = "MKCoordinateSpan", Native = "MKCoordinateSpan", ManagedCondition = "HAVE_MAPKIT", ManagedNewExpression = "new MKCoordinateSpan (33, 34)", Map = ".CoordinateSpanValue", MapFrom = "FromMKCoordinateSpan" },
		new BindAsData { Managed = "CMTimeRange", Native = "CMTimeRange", ManagedCondition = "HAVE_COREMEDIA", ManagedNewExpression = "new CMTimeRange { Duration = new CMTime (37, 38), Start = new CMTime (39, 40) }", Map = ".CMTimeRangeValue", MapFrom = "FromCMTimeRange"  },
		new BindAsData { Managed = "CMTime", Native = "CMTime", ManagedCondition = "HAVE_COREMEDIA",  ManagedNewExpression = "new CMTime (35, 36)", Map = ".CMTimeValue", MapFrom = "FromCMTime"  },
		new BindAsData { Managed = "CMTimeMapping", Native = "CMTimeMapping", ManagedCondition = "HAVE_COREMEDIA", ManagedNewExpression = "new CMTimeMapping { Source = new CMTimeRange { Duration = new CMTime (42, 43), Start = new CMTime (44, 45) } }", Map = ".CMTimeMappingValue", MapFrom = "FromCMTimeMapping"  },
		new BindAsData { Managed = "CATransform3D", Native = "CATransform3D", ManagedCondition = "HAVE_COREANIMATION", ManagedNewExpression = "new CATransform3D { m11 = 41 }", Map = ".CATransform3DValue", MapFrom = "FromCATransform3D"  },
		new BindAsData { Managed = "NSDirectionalEdgeInsets", Native = "NSDirectionalEdgeInsets", ManagedCondition = "HAVE_UIKIT", ManagedNewExpression = "new NSDirectionalEdgeInsets (42, 43, 44, 45)", Map = ".DirectionalEdgeInsetsValue", MapFrom = "FromDirectionalEdgeInsets", MinXcodeVersion = new Version (9, 0) },
	};

	static BindAsData [] bindas_nsstring = new [] {
		new BindAsData { Managed = "SecKeyAlgorithm", ManagedNewExpression = "SecKeyAlgorithm.RsaSignatureRaw", MinXcodeVersion = new Version (8, 0) },
	};

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

		w.AppendLine ();
		foreach (var v in bindas_nsnumber) {
			w.AppendLine ($"\t@property (retain) NSNumber* P{v.Managed}Number;");
			w.AppendLine ($"\t@property (retain) NSArray<NSNumber*> * P{v.Managed}Array;");
		}

		w.AppendLine ();
		foreach (var v in bindas_nsvalue) {
			w.AppendLine ($"\t@property (retain) NSValue* P{v.Managed}Value;");
			w.AppendLine ($"\t@property (retain) NSArray<NSValue*> * P{v.Managed}Array;");
		}

		w.AppendLine ();
		foreach (var v in bindas_nsstring) {
			w.AppendLine ($"\t@property (retain) NSString *PSmart{v.Managed}Property;");
			w.AppendLine ($"\t@property (retain) NSArray<NSString*> * PSmart{v.Managed}Properties;");
		}
		File.WriteAllText ("libtest.properties.h", w.ToString ());
	}

	static void WriteLibTestMethodsH ()
	{
		var w = new StringBuilder ();

		w.AppendLine ();
		foreach (var v in bindas_nsnumber) {
			w.AppendLine ($"\t-(NSNumber *) get{v.Managed}NumberNonNullable;");
			w.AppendLine ($"\t-(void) set{v.Managed}NumberNonNullable: (NSNumber *) value;");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSNumber *) get{v.Managed}NumberNullable;");
			w.AppendLine ($"\t-(void) set{v.Managed}NumberNullable: (NSNumber *) value;");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSArray<NSNumber *> *) get{v.Managed}Array;");
			w.AppendLine ($"\t-(void) set{v.Managed}Array: (NSArray<NSNumber *> *) value;");
			w.AppendLine ();
		}

		w.AppendLine ();
		foreach (var v in bindas_nsvalue) {
			w.AppendLine ($"\t-(NSValue *) get{v.Managed}ValueNonNullable;");
			w.AppendLine ($"\t-(void) set{v.Managed}ValueNonNullable: (NSValue *) value;");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSValue *) get{v.Managed}ValueNullable;");
			w.AppendLine ($"\t-(void) set{v.Managed}ValueNullable: (NSValue *) value;");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSArray<NSValue *> *) get{v.Managed}Array;");
			w.AppendLine ($"\t-(void) set{v.Managed}Array: (NSArray<NSValue *> *) value;");
			w.AppendLine ();
		}

		w.AppendLine ();
		foreach (var v in bindas_nsstring) {
			// plain value
			w.AppendLine ($"\t-(NSString *) getSmart{v.Managed}Value;");
			w.AppendLine ($"\t-(void) setSmart{v.Managed}Value: (NSString *) value;");
			w.AppendLine ();

			// nullable
			w.AppendLine ($"\t-(NSString *) getSmartNullable{v.Managed}Value;");
			w.AppendLine ($"\t-(void) setSmartNullable{v.Managed}Value: (NSString *) value;");
			w.AppendLine ();

			// array of plain value
			w.AppendLine ($"\t-(NSArray<NSString *> *) getSmart{v.Managed}Values;");
			w.AppendLine ($"\t-(void) setSmart{v.Managed}Values: (NSArray<NSString *> *) value;");
			w.AppendLine ();
		}

		File.WriteAllText ("libtest.methods.h", w.ToString ());
	}

	static void WriteLibTestMethodsM ()
	{
		var w = new StringBuilder ();

		w.AppendLine ();
		foreach (var v in bindas_nsnumber) {
			w.AppendLine ($"\t-(NSNumber *) get{v.Managed}NumberNonNullable {{ return self.P{v.Managed}Number; }}");
			w.AppendLine ($"\t-(void) set{v.Managed}NumberNonNullable: (NSNumber *) value {{ self.P{v.Managed}Number = value; }}");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSNumber *) get{v.Managed}NumberNullable {{ return self.P{v.Managed}Number; }}");
			w.AppendLine ($"\t-(void) set{v.Managed}NumberNullable: (NSNumber *) value {{ self.P{v.Managed}Number = value; }}");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSArray<NSNumber *> *) get{v.Managed}Array {{ return self.P{v.Managed}Array; }}");
			w.AppendLine ($"\t-(void) set{v.Managed}Array: (NSArray<NSNumber *> *) value {{ self.P{v.Managed}Array = value; }}");
			w.AppendLine ();
		}

		w.AppendLine ();
		foreach (var v in bindas_nsvalue) {
			w.AppendLine ($"\t-(NSValue *) get{v.Managed}ValueNonNullable {{ return self.P{v.Managed}Value; }}");
			w.AppendLine ($"\t-(void) set{v.Managed}ValueNonNullable: (NSValue *) value {{ self.P{v.Managed}Value = value; }}");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSValue *) get{v.Managed}ValueNullable {{ return self.P{v.Managed}Value; }}");
			w.AppendLine ($"\t-(void) set{v.Managed}ValueNullable: (NSValue *) value {{ self.P{v.Managed}Value = value; }}");
			w.AppendLine ();
			w.AppendLine ($"\t-(NSArray<NSValue *> *) get{v.Managed}Array {{ return self.P{v.Managed}Array; }}");
			w.AppendLine ($"\t-(void) set{v.Managed}Array: (NSArray<NSValue *> *) value {{ self.P{v.Managed}Array = value; }}");
			w.AppendLine ();
		}

		w.AppendLine ();
		foreach (var v in bindas_nsstring) {
			// plain value
			w.AppendLine ($"\t-(NSString *) getSmart{v.Managed}Value {{ return self.PSmart{v.Managed}Property; }}");
			w.AppendLine ($"\t-(void) setSmart{v.Managed}Value: (NSString *) value {{ self.PSmart{v.Managed}Property = value; }}");
			w.AppendLine ();

			// nullable
			w.AppendLine ($"\t-(NSString *) getSmartNullable{v.Managed}Value {{ return self.PSmart{v.Managed}Property; }}");
			w.AppendLine ($"\t-(void) setSmartNullable{v.Managed}Value: (NSString *) value {{ self.PSmart{v.Managed}Property = value; }}");
			w.AppendLine ();

			// array of plain value
			w.AppendLine ($"\t-(NSArray<NSString *> *) getSmart{v.Managed}Values {{ return self.PSmart{v.Managed}Properties; }}");
			w.AppendLine ($"\t-(void) setSmart{v.Managed}Values: (NSArray<NSString *> *) value {{ self.PSmart{v.Managed}Properties = value; }}");
			w.AppendLine ();
		}

		File.WriteAllText ("libtest.methods.m", w.ToString ());
	}

	static void WriteFrameworkDefines (StringBuilder w)
	{
		w.AppendLine (@"
#if __IOS__ || __MACOS__ || __TVOS__
#define HAVE_COREANIMATION
#endif

#if __IOS__ || __MACOS__ || __TVOS__
#define HAVE_COREMEDIA
#endif

#if __IOS__ || __WATCHOS__ || __TVOS__
#define HAVE_UIKIT
#endif

#if XAMCORE_2_0
#define HAVE_MAPKIT
#endif");
		
	}

	static void WriteApiDefinition ()
	{
		var w = new StringBuilder ();
		WriteFrameworkDefines (w);
		w.AppendLine (@"
using System;
#if !__WATCHOS__
using System.Drawing;
#endif

#if __UNIFIED__
using AVFoundation;
#if HAVE_COREANIMATION
using CoreAnimation;
#endif
using CoreGraphics;
using CoreLocation;
#if HAVE_COREMEDIA
using CoreMedia;
#endif
using Foundation;
#if HAVE_MAPKIT
using MapKit;
#endif
using ObjCRuntime;
using SceneKit;
using Security;
#if HAVE_UIKIT
using UIKit;
#endif
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

		w.AppendLine ("\t\t// BindAs: NSNumber");
		foreach (var v in bindas_nsnumber) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			// no BindAs

			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Number\")]");
			w.AppendLine ($"\t\tNSNumber P{v.Managed}Number {{ get; set; }}");

			// plain value
			w.AppendLine ();
			w.AppendLine ("\t\t[Sealed]");
			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Number\")]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}))]");
			w.AppendLine ($"\t\tNSNumber P{v.Managed}NumberNonNullable {{ get; set; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"get{v.Managed}NumberNonNullable\")]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}))]");
			w.AppendLine ($"\t\t[return: NullAllowed] // This should be the default");
			w.AppendLine ($"\t\tNSNumber Get{v.Managed}NumberNonNullable ();");
			
			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"set{v.Managed}NumberNonNullable:\")]");
			w.AppendLine ($"\t\tvoid Set{v.Managed}NumberNonNullable ([BindAs (typeof ({v.Managed}))] NSNumber value);");

			// nullable value
			w.AppendLine ();
			w.AppendLine ("\t\t[Sealed]");
			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Number\")]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?))]");
			w.AppendLine ($"\t\t[NullAllowed] // This should be the default");
			w.AppendLine ($"\t\tNSNumber P{v.Managed}NumberNullable {{ get; set; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"get{v.Managed}NumberNullable\")]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}?))]");
			w.AppendLine ($"\t\tNSNumber Get{v.Managed}NumberNullable ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"set{v.Managed}NumberNullable:\")]");
			w.AppendLine ($"\t\tvoid Set{v.Managed}NumberNullable ([BindAs (typeof ({v.Managed}?))] [NullAllowed /* this should be the default */] NSNumber value);");

			// ref/out plain value is not supported (error BI1048: bgen: Unsupported type Boolean decorated with [BindAs])
			// ref/out nullable value is not supported (error BI1048: bgen: Unsupported type Boolean decorated with [BindAs])

			// array of plain value
			w.AppendLine ();
			w.AppendLine ("\t\t[Sealed]");
			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Array\")]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[]))]");
			w.AppendLine ($"\t\t[NullAllowed]");
			w.AppendLine ($"\t\tNSNumber[] P{v.Managed}Array {{ get; set; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"get{v.Managed}Array\")]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}[]))]");
			w.AppendLine ($"\t\t[return: NullAllowed]");
			w.AppendLine ($"\t\tNSNumber[] Get{v.Managed}Array ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"set{v.Managed}Array:\")]");
			w.AppendLine ($"\t\tvoid Set{v.Managed}Array ([NullAllowed] [BindAs (typeof ({v.Managed}[]))] NSNumber[] value);");

			// multidimensional array of plain value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57795
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}Multi1Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[,]))]");
			//w.AppendLine ($"\t\tNSNumber[,] P{v.Managed}Multi1ArrayValue {{ get; set; }}");

			// BI1048: bgen: Unsupported type Boolean[][] decorated with [BindAs]
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}Multi2Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[][]))]");
			//w.AppendLine ($"\t\tNSNumber[][] P{v.Managed}Multi2ArrayValue {{ get; set; }}");

			// array of nullable value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}NullableArray\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[]))]");
			//w.AppendLine ($"\t\tNSNumber[] P{v.Managed}NullableArrayValue {{ get; set; }}");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[Export (\"get{v.Managed}NullableArray\")]");
			//w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}?[]))]");
			//w.AppendLine ($"\t\tNSNumber[] Get{v.Managed}NullableArrayValue ();");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[Export (\"set{v.Managed}NullableArray:\")]");
			//w.AppendLine ($"\t\tvoid Set{v.Managed}NullableArray ([BindAs (typeof ({v.Managed}?[]))] NSNumber[] value);");

			// multidimensional array of nullable value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}NullableMulti1Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[,]))]");
			//w.AppendLine ($"\t\tNSNumber[,] P{v.Managed}NullableMulti1ArrayValue {{ get; set; }}");

			// BI1048: bgen: Unsupported type Nullable`1[][] decorated with [BindAs]
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}NullableMulti2Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[][]))]");
			//w.AppendLine ($"\t\tNSNumber[][] P{v.Managed}NullableMulti2ArrayValue {{ get; set; }}");

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			w.AppendLine ();
		}

		w.AppendLine ("\t\t// BindAs: NSValue");
		foreach (var v in bindas_nsvalue) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			// no BindAs

			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Value\")]");
			w.AppendLine ($"\t\tNSValue P{v.Managed}Value {{ get; set; }}");

			// plain value
			w.AppendLine ();
			w.AppendLine ("\t\t[Sealed]");
			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Value\")]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}))]");
			w.AppendLine ($"\t\tNSValue P{v.Managed}ValueNonNullable {{ get; set; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"get{v.Managed}ValueNonNullable\")]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}))]");
			w.AppendLine ($"\t\t[return: NullAllowed] // This should be the default");
			w.AppendLine ($"\t\tNSValue Get{v.Managed}ValueNonNullable ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"set{v.Managed}ValueNonNullable:\")]");
			w.AppendLine ($"\t\tvoid Set{v.Managed}ValueNonNullable ([BindAs (typeof ({v.Managed}))] NSValue value);");

			// nullable

			w.AppendLine ();
			w.AppendLine ($"\t\t[Sealed]");
			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Value\")]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?))]");
			w.AppendLine ($"\t\t[NullAllowed] // This should be the default");
			w.AppendLine ($"\t\tNSValue P{v.Managed}ValueNullable {{ get; set; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"get{v.Managed}ValueNullable\")]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}?))]");
			w.AppendLine ($"\t\tNSValue Get{v.Managed}ValueNullable ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"set{v.Managed}ValueNullable:\")]");
			w.AppendLine ($"\t\tvoid Set{v.Managed}ValueNullable ([BindAs (typeof ({v.Managed}?))] [NullAllowed /* this should be the default */] NSValue value);");

			// ref/out plain value is not supported (error BI1048: bgen: Unsupported type CATransform3D decorated with [BindAs])
			// ref/out nullable value is not supported (error BI1048: bgen: Unsupported type CATransform3D decorated with [BindAs])

			// array of plain value
			w.AppendLine ();
			w.AppendLine ("\t\t[Sealed]");
			w.AppendLine ($"\t\t[Export (\"P{v.Managed}Array\")]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[]))]");
			w.AppendLine ($"\t\t[NullAllowed]");
			w.AppendLine ($"\t\tNSValue[] P{v.Managed}Array {{ get; set; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"get{v.Managed}Array\")]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}[]))]");
			w.AppendLine ($"\t\t[return: NullAllowed]");
			w.AppendLine ($"\t\tNSValue[] Get{v.Managed}Array ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"set{v.Managed}Array:\")]");
			w.AppendLine ($"\t\tvoid Set{v.Managed}Array ([NullAllowed] [BindAs (typeof ({v.Managed}[]))] NSValue[] value);");

			// multidimensional array of plain value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57795
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}Multi1Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[,]))]");
			//w.AppendLine ($"\t\tNSValue[,] P{v.Managed}Multi1ArrayValue {{ get; set; }}");

			// BI1048: bgen: Unsupported type CATransform3D[][] decorated with [BindAs]
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}Multi2Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[][]))]");
			//w.AppendLine ($"\t\tNSValue[][] P{v.Managed}Multi2ArrayValue {{ get; set; }}");

			// array of nullable value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}NullableArray\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[]))]");
			//w.AppendLine ($"\t\tNSValue[] P{v.Managed}NullableArrayValue {{ get; set; }}");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[Export (\"get{v.Managed}NullableArray\")]");
			//w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}?[]))]");
			//w.AppendLine ($"\t\tNSValue[] Get{v.Managed}NullableArrayValue ();");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[Export (\"set{v.Managed}NullableArray:\")]");
			//w.AppendLine ($"\t\tvoid Set{v.Managed}NullableArray ([BindAs (typeof ({v.Managed}?[]))] NSValue[] value);");

			// multidimensional array of nullable value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}NullableMulti1Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[,]))]");
			//w.AppendLine ($"\t\tNSValue[,] P{v.Managed}NullableMulti1 {{ get; set; }}");

			// BI1048: bgen: Unsupported type Nullable`1[][] decorated with [BindAs]
			//w.AppendLine ();
			//w.AppendLine ("\t\t[Sealed]");
			//w.AppendLine ($"\t\t[Export (\"P{v.Managed}NullableMulti2Array\")]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[][]))]");
			//w.AppendLine ($"\t\tNSValue[][] P{v.Managed}NullableMulti2ArrayValue {{ get; set; }}");


			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			w.AppendLine ();
		}

		w.AppendLine ("\t\t// BindAs: NSString");
		foreach (var v in bindas_nsstring) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");
			// plain value
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}))]");
			w.AppendLine ($"\t\t[Export (\"getSmart{v.Managed}Value\")]");
			w.AppendLine ($"\t\tNSString GetSmart{v.Managed}Value ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"setSmart{v.Managed}Value:\")]");
			w.AppendLine ($"\t\tvoid SetSmart{v.Managed}Value ([BindAs (typeof ({v.Managed}))] NSString value);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}))]");
			w.AppendLine ($"\t\t[Export (\"PSmart{v.Managed}Property\")]");
			w.AppendLine ($"\t\tNSString PSmart{v.Managed}Property {{ get; set; }}");

			// nullable
			w.AppendLine ();
			w.AppendLine ($"\t\t[return: NullAllowed]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}?))]");
			w.AppendLine ($"\t\t[Export (\"getSmartNullable{v.Managed}Value\")]");
			w.AppendLine ($"\t\tNSString GetSmartNullable{v.Managed}Value ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"setSmartNullable{v.Managed}Value:\")]");
			w.AppendLine ($"\t\tvoid SetSmartNullable{v.Managed}Value ([NullAllowed] [BindAs (typeof ({v.Managed}?))] NSString value);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[NullAllowed]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?))]");
			w.AppendLine ($"\t\t[Export (\"smartNullable{v.Managed}Property\")]");
			w.AppendLine ($"\t\tNSString SmartNullable{v.Managed}Property {{ get; set; }}");

			// array of plain value
			w.AppendLine ();
			w.AppendLine ($"\t\t[return: NullAllowed]");
			w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}[]))]");
			w.AppendLine ($"\t\t[Export (\"getSmart{v.Managed}Values\")]");
			w.AppendLine ($"\t\tNSString[] GetSmart{v.Managed}Values ();");

			w.AppendLine ();
			w.AppendLine ($"\t\t[Export (\"setSmart{v.Managed}Values:\")]");
			w.AppendLine ($"\t\tvoid SetSmart{v.Managed}Values ([NullAllowed] [BindAs (typeof ({v.Managed}[]))] NSString[] value);");

			w.AppendLine ();
			w.AppendLine ($"\t\t[NullAllowed]");
			w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[]))]");
			w.AppendLine ($"\t\t[Export (\"PSmart{v.Managed}Properties\")]");
			w.AppendLine ($"\t\tNSString[] PSmart{v.Managed}Properties {{ get; set; }}");

			// array of nullable values
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57797
			//w.AppendLine ();
			//w.AppendLine ($"\t\t[return: NullAllowed]");
			//w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}?[]))]");
			//w.AppendLine ($"\t\t[Export (\"getSmartNullable{v.Managed}Values\")]");
			//w.AppendLine ($"\t\tNSString[] GetSmartNullable{v.Managed}Values ();");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[Export (\"setSmartNullable{v.Managed}Values\")]");
			//w.AppendLine ($"\t\tvoid SetSmartNullable{v.Managed}Values ([NullAllowed] [BindAs (typeof ({v.Managed}?[]))] NSString[] value);");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[NullAllowed]");
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}?[]))]");
			//w.AppendLine ($"\t\t[Export (\"smartNullable{v.Managed}Properties:\")]");
			//w.AppendLine ($"\t\tNSString[] SmartNullable{v.Managed}Properties {{ get; set; }}");

			// multidimensional array of plain value
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57795
			//w.AppendLine ();
			//w.AppendLine ($"\t\t[return: BindAs (typeof ({v.Managed}[,]))]");
			//w.AppendLine ($"\t\t[Export (\"getSmart{v.Managed}ValuesMulti\")]");
			//w.AppendLine ($"\t\tNSString[,] GetSmart{v.Managed}ValuesMulti ();");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[Export (\"setSmart{v.Managed}ValuesMulti:\")]");
			//w.AppendLine ($"\t\tvoid SetSmart{v.Managed}ValuesMulti ([BindAs (typeof ({v.Managed}[]))] NSString[,] value);");

			//w.AppendLine ();
			//w.AppendLine ($"\t\t[BindAs (typeof ({v.Managed}[,]))]");
			//w.AppendLine ($"\t\t[Export (\"PSmart{v.Managed}PropertiesMulti:\")]");
			//w.AppendLine ($"\t\tNSString[,] PSmart{v.Managed}PropertiesMulti {{ get; set; }}");

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			w.AppendLine ();
		}


		// multi-dimensional array of plain value

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

		WriteFrameworkDefines (w);
		w.AppendLine (@"
using System;
#if XAMCORE_2_0
using AVFoundation;
#if HAVE_COREANIMATION
using CoreAnimation;
#endif
using CoreGraphics;
using CoreLocation;
#if HAVE_COREMEDIA
using CoreMedia;
#endif
using Foundation;
#if HAVE_MAPKIT
using MapKit;
#endif
using ObjCRuntime;
using SceneKit;
using Security;
#if HAVE_UIKIT
using UIKit;
#endif
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

		w.AppendLine ("\t\tinternal class BindAsTestClassGenerated : ObjCRegistrarTest {");

		w.AppendLine ("\t\t\t// BindAs: NSNumber");
		foreach (var v in bindas_nsnumber) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			w.AppendLine ($"\t\t\t{v.Managed}? _{v.Managed};");
			w.AppendLine ($"\t\t\tpublic {v.Managed}? {v.Managed}Number {{ get {{ return _{v.Managed}; }} set {{ _{v.Managed} = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}? Get{v.Managed}NumberNullable () {{ return _{v.Managed}; }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed} Get{v.Managed}NumberNonNullable () {{ return _{v.Managed}.Value; }}");
			w.AppendLine ($"\t\t\tpublic override void Set{v.Managed}NumberNullable ({v.Managed}? value) {{ _{v.Managed} = value; }}");
			w.AppendLine ($"\t\t\tpublic override void Set{v.Managed}NumberNonNullable ({v.Managed} value) {{ _{v.Managed} = value; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t{v.Managed}[] _{v.Managed}Array;");
			w.AppendLine ($"\t\t\tpublic {v.Managed}[] {v.Managed}Array {{ get {{ return _{v.Managed}Array; }} set {{ _{v.Managed}Array = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}[] Get{v.Managed}Array () {{ return _{v.Managed}Array; }}");
			w.AppendLine ($"\t\t\tpublic override void Set{v.Managed}Array ({v.Managed}[] value) {{ _{v.Managed}Array = value; }}");

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			w.AppendLine ();
		}

		w.AppendLine ("\t\t\t// BindAs: NSValue");
		foreach (var v in bindas_nsvalue) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			w.AppendLine ($"\t\t\t{v.Managed}? _{v.Managed};");
			w.AppendLine ($"\t\t\tpublic {v.Managed}? {v.Managed}Value {{ get {{ return _{v.Managed}; }} set {{ _{v.Managed} = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}? Get{v.Managed}ValueNullable () {{ return _{v.Managed}; }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed} Get{v.Managed}ValueNonNullable () {{ return _{v.Managed}.Value; }}");
			w.AppendLine ($"\t\t\tpublic override void Set{v.Managed}ValueNullable ({v.Managed}? value) {{ _{v.Managed} = value; }}");
			w.AppendLine ($"\t\t\tpublic override void Set{v.Managed}ValueNonNullable ({v.Managed} value) {{ _{v.Managed} = value; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t{v.Managed}[] _{v.Managed}Array;");
			w.AppendLine ($"\t\t\tpublic {v.Managed}[] {v.Managed}Array {{ get {{ return _{v.Managed}Array; }} set {{ _{v.Managed}Array = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}[] Get{v.Managed}Array () {{ return _{v.Managed}Array; }}");
			w.AppendLine ($"\t\t\tpublic override void Set{v.Managed}Array ({v.Managed}[] value) {{ _{v.Managed}Array = value; }}");

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			w.AppendLine ();
		}

		w.AppendLine ("\t\t\t// BindAs: NSString");
		foreach (var v in bindas_nsstring) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			w.AppendLine ($"\t\t\t{v.Managed}? _{v.Managed};");
			w.AppendLine ($"\t\t\tpublic {v.Managed}? {v.Managed}Value {{ get {{ return _{v.Managed}; }} set {{ _{v.Managed} = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed} PSmart{v.Managed}Property {{ get {{ return {v.Managed}Value.Value; }} set {{ {v.Managed}Value = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed} GetSmart{v.Managed}Value () {{ return _{v.Managed}.Value; }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}? GetSmartNullable{v.Managed}Value () {{ return _{v.Managed}; }}");
			w.AppendLine ($"\t\t\tpublic override void SetSmart{v.Managed}Value ({v.Managed} value) {{ _{v.Managed} = value; }}");
			w.AppendLine ($"\t\t\tpublic override void SetSmartNullable{v.Managed}Value ({v.Managed}? value) {{ _{v.Managed} = value; }}");

			w.AppendLine ();
			w.AppendLine ($"\t\t\t{v.Managed}[] _{v.Managed}Array;");
			w.AppendLine ($"\t\t\tpublic {v.Managed}[] {v.Managed}Array {{ get {{ return _{v.Managed}Array; }} set {{ _{v.Managed}Array = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}[] PSmart{v.Managed}Properties {{ get {{ return _{v.Managed}Array; }} set {{ _{v.Managed}Array = value; }} }}");
			w.AppendLine ($"\t\t\tpublic override {v.Managed}[] GetSmart{v.Managed}Values () {{ return _{v.Managed}Array; }}");
			w.AppendLine ($"\t\t\tpublic override void SetSmart{v.Managed}Values ({v.Managed}[] value) {{ _{v.Managed}Array = value; }}");

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			w.AppendLine ();
		}
		w.AppendLine ("\t\t}");

		foreach (var v in bindas_nsnumber) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			// Bindings
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSNumberBindAs_{v.Managed}_Bindings ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new ObjCRegistrarTest ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Number, \"initial null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}NumberNullable, \"initial nullable null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}NumberNullable (), \"initial null method\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\t{v.Managed}? value = default ({v.Managed});");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}NumberNonNullable (value.Value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.P{v.Managed}NumberNullable, \"nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.P{v.Managed}NumberNonNullable, \"non-nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.Get{v.Managed}NumberNullable (), \"nullable get method after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.Get{v.Managed}NumberNonNullable (), \"non-nullable get method after setting default value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}NumberNonNullable (value.Value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.P{v.Managed}NumberNullable, \"nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.P{v.Managed}NumberNonNullable, \"non-nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.Get{v.Managed}NumberNullable (), \"nullable get method after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.Get{v.Managed}NumberNonNullable (), \"non-nullable get method after setting custom value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = null;");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}NumberNullable (value);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Number, \"null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}NumberNullable, \"nullable null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}NumberNullable (), \"null method after setting null value\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			// Overrides
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSNumberBindAs_{v.Managed}_Overrides ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new BindAsTestClassGenerated ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Number, \"initial null\");");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}NumberNullable:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Number, \"null after setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Number = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}NumberNullable:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Number, \"null after re-setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvar value = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tusing (var input = new NSNumber ({v.ToNSNumberCastExpression}value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}NumberNullable:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.{v.Managed}Number, \"after setting A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Number = null;");
			w.AppendLine ($"\t\t\t\tusing (var input = new NSNumber ({v.ToNSNumberCastExpression}value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}NumberNonNullable:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.{v.Managed}Number.Value, \"after setting B\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Number = null;");
			w.AppendLine ($"\t\t\t\tvar number = Runtime.GetNSObject<NSNumber> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}NumberNullable\")));");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (number, \"null from getter A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Number = value;");
			w.AppendLine ($"\t\t\t\tnumber = Runtime.GetNSObject<NSNumber> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}NumberNullable\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, {v.FromNSNumberCastExpression}number{v.Map}, \"getter B\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Number = value;");
			w.AppendLine ($"\t\t\t\tnumber = Runtime.GetNSObject<NSNumber> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}NumberNonNullable\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, {v.FromNSNumberCastExpression}number{v.Map}, \"getter C\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");

			// Array_Bindings
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSNumberBindAs_{v.Managed}Array_Bindings ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new ObjCRegistrarTest ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Array, \"initial null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}Array (), \"initial null method\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\t{v.Managed}[] value = null;");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}Array (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.P{v.Managed}Array, \"nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.Get{v.Managed}Array (), \"nullable get method after setting default value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}Array (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (1, obj.P{v.Managed}Array.Length, \"nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (1, obj.Get{v.Managed}Array ().Length, \"nullable get method after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.P{v.Managed}Array [0], \"nullable property after setting custom value element\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.Get{v.Managed}Array () [0], \"nullable get method after setting custom value element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = null;");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}Array (value);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Array, \"null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}Array (), \"null method after setting null value\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			// Array_Overrides
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSNumberBindAs_{v.Managed}_Array_Overrides ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new BindAsTestClassGenerated ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Array, \"initial null\");");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Array, \"null after setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Array, \"null after re-setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvar value = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tusing (var input = NSArray.FromNSObjects<{v.Managed}> ((v) => new NSNumber ({v.ToNSNumberCastExpression}v), value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, obj.{v.Managed}Array.Length, \"after setting A\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.{v.Managed}Array [0], \"after setting A element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = null;");
			w.AppendLine ($"\t\t\t\tusing (var input = NSArray.FromNSObjects<{v.Managed}> ((v) => new NSNumber ({v.ToNSNumberCastExpression}v), value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, obj.{v.Managed}Array.Length, \"after setting B\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.{v.Managed}Array [0], \"after setting B element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = null;");
			w.AppendLine ($"\t\t\t\tvar array = Runtime.GetNSObject<NSArray> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}Array\")));");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (array, \"null from getter A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = value;");
			w.AppendLine ($"\t\t\t\tarray = Runtime.GetNSObject<NSArray> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}Array\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, array.Count, \"getter B\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], {v.FromNSNumberCastExpression}array.GetItem<NSNumber> (0){v.Map}, \"getter B element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");

			if  (v.ManagedCondition != null)
				w.AppendLine ("#endif");

			w.AppendLine ();
		}


		foreach (var v in bindas_nsvalue) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			// Bindings
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSValueBindAs_{v.Managed}_Bindings ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new ObjCRegistrarTest ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Value, \"initial null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}ValueNullable, \"initial nullable null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}ValueNullable (), \"initial null method\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\t{v.Managed}? value = default ({v.Managed});");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}ValueNonNullable (value.Value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.P{v.Managed}ValueNullable, \"nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.P{v.Managed}ValueNonNullable, \"non-nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.Get{v.Managed}ValueNullable (), \"nullable get method after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.Get{v.Managed}ValueNonNullable (), \"non-nullable get method after setting default value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}ValueNonNullable (value.Value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.P{v.Managed}ValueNullable, \"nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.P{v.Managed}ValueNonNullable, \"non-nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.Get{v.Managed}ValueNullable (), \"nullable get method after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.Get{v.Managed}ValueNonNullable (), \"non-nullable get method after setting custom value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = null;");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}ValueNullable (value);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Value, \"null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}ValueNullable, \"nullable null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}ValueNullable (), \"null method after setting null value\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			// Overrides
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSValueBindAs_{v.Managed}_Overrides ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new BindAsTestClassGenerated ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Value, \"initial null\");");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}ValueNullable:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Value, \"null after setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Value = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}ValueNullable:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Value, \"null after re-setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvar value = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tusing (var input = NSValue.{v.MapFrom} (value))");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}ValueNullable:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.{v.Managed}Value, \"after setting A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Value = null;");
			w.AppendLine ($"\t\t\t\tusing (var input = NSValue.{v.MapFrom} (value))");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}ValueNonNullable:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.{v.Managed}Value, \"after setting B\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Value = null;");
			w.AppendLine ($"\t\t\t\tvar Value = Runtime.GetNSObject<NSValue> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}ValueNullable\")));");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (Value, \"null from getter A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Value = value;");
			w.AppendLine ($"\t\t\t\tValue = Runtime.GetNSObject<NSValue> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}ValueNullable\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, Value{v.Map}, \"getter B\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Value = value;");
			w.AppendLine ($"\t\t\t\tValue = Runtime.GetNSObject<NSValue> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}ValueNonNullable\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, Value{v.Map}, \"getter C\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");

			// Array_Bindings
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSValueBindAs_{v.Managed}Array_Bindings ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new ObjCRegistrarTest ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Array, \"initial null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}Array (), \"initial null method\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\t{v.Managed}[] value = null;");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}Array (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.P{v.Managed}Array, \"nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.Get{v.Managed}Array (), \"nullable get method after setting default value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}Array (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (1, obj.P{v.Managed}Array.Length, \"nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (1, obj.Get{v.Managed}Array ().Length, \"nullable get method after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.P{v.Managed}Array [0], \"nullable property after setting custom value element\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.Get{v.Managed}Array () [0], \"nullable get method after setting custom value element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = null;");
			w.AppendLine ($"\t\t\t\tobj.Set{v.Managed}Array (value);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.P{v.Managed}Array, \"null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.Get{v.Managed}Array (), \"null method after setting null value\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			// Array_Overrides
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSValueBindAs_{v.Managed}_Array_Overrides ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new BindAsTestClassGenerated ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Array, \"initial null\");");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Array, \"null after setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.{v.Managed}Array, \"null after re-setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvar value = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tusing (var input = NSArray.FromNSObjects<{v.Managed}> ((v) => NSValue.{v.MapFrom} (v), value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, obj.{v.Managed}Array.Length, \"after setting A\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.{v.Managed}Array [0], \"after setting A element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = null;");
			w.AppendLine ($"\t\t\t\tusing (var input = NSArray.FromNSObjects<{v.Managed}> ((v) => NSValue.{v.MapFrom} (v), value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"set{v.Managed}Array:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, obj.{v.Managed}Array.Length, \"after setting B\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.{v.Managed}Array [0], \"after setting B element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = null;");
			w.AppendLine ($"\t\t\t\tvar array = Runtime.GetNSObject<NSArray> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}Array\")));");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (array, \"null from getter A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tobj.{v.Managed}Array = value;");
			w.AppendLine ($"\t\t\t\tarray = Runtime.GetNSObject<NSArray> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"get{v.Managed}Array\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, array.Count, \"getter B\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], array.GetItem<NSValue> (0){v.Map}, \"getter B element\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");
			
			w.AppendLine ();
		}

		foreach (var v in bindas_nsstring) {
			if (v.ManagedCondition != null)
				w.AppendLine ($"#if {v.ManagedCondition}");

			// Bindings
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSStringBindAs_{v.Managed}_Bindings ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new ObjCRegistrarTest ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.Throws<ArgumentNullException> (() => {{ Console.WriteLine (obj.PSmart{v.Managed}Property); }}, \"initial zero property\");");
			w.AppendLine ($"\t\t\t\tAssert.Throws<ArgumentNullException> (() => {{ Console.WriteLine (obj.GetSmart{v.Managed}Value ()); }}, \"initial zero method\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.GetSmartNullable{v.Managed}Value (), \"initial null method\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\t{v.Managed}? value = default ({v.Managed});");
			w.AppendLine ($"\t\t\t\tobj.SetSmartNullable{v.Managed}Value (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.PSmart{v.Managed}Property, \"zero property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.GetSmart{v.Managed}Value (), \"non-nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.GetSmartNullable{v.Managed}Value (), \"nullable get method after setting default value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.SetSmart{v.Managed}Value (value.Value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.PSmart{v.Managed}Property, \"non-nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.GetSmartNullable{v.Managed}Value (), \"nullable get method after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Value, obj.GetSmart{v.Managed}Value (), \"non-nullable get method after setting custom value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = null;");	
			w.AppendLine ($"\t\t\t\tobj.SetSmartNullable{v.Managed}Value (value);");
			w.AppendLine ($"\t\t\t\tAssert.Throws<ArgumentNullException> (() => {{ Console.WriteLine (obj.PSmart{v.Managed}Property); }}, \"null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.Throws<ArgumentNullException> (() => {{ Console.WriteLine (obj.GetSmart{v.Managed}Value ()); }}, \"non-nullable method after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.GetSmartNullable{v.Managed}Value (), \"null method after setting null value\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			// Overrides
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSStringBindAs_{v.Managed}_Overrides ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new BindAsTestClassGenerated ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.Throws<InvalidOperationException> (() => {{ Console.WriteLine (obj.PSmart{v.Managed}Property); }}, \"initial null\");");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmartNullable{v.Managed}Value:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.Throws<InvalidOperationException> (() => {{ Console.WriteLine (obj.PSmart{v.Managed}Property); }}, \"null after setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Property = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmartNullable{v.Managed}Value:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.Throws<InvalidOperationException> (() => {{ Console.WriteLine (obj.PSmart{v.Managed}Property); }}, \"null after re-setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvar value = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tusing (var input = value.GetConstant ())");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmartNullable{v.Managed}Value:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.PSmart{v.Managed}Property, \"after setting A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Property = 0;");
			w.AppendLine ($"\t\t\t\tusing (var input = value.GetConstant ())");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmart{v.Managed}Value:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.PSmart{v.Managed}Property, \"after setting B\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Property = 0;");
			w.AppendLine ($"\t\t\t\tvar Value = Runtime.GetNSObject<NSString> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"getSmartNullable{v.Managed}Value\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (default ({v.Managed}).GetConstant ().ToString (), Value.ToString (), \"zero from getter A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Property = value;");
			w.AppendLine ($"\t\t\t\tValue = Runtime.GetNSObject<NSString> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"getSmartNullable{v.Managed}Value\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, {v.Managed}Extensions.GetValue (Value), \"getter B\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = {v.ManagedNewExpression};");
			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Property = value;");
			w.AppendLine ($"\t\t\t\tValue = Runtime.GetNSObject<NSString> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"getSmart{v.Managed}Value\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, {v.Managed}Extensions.GetValue (Value), \"getter C\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");

			// Array_Bindings
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSStringBindAs_{v.Managed}Array_Bindings ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new ObjCRegistrarTest ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.PSmart{v.Managed}Properties, \"initial null property\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.GetSmart{v.Managed}Values (), \"initial null method\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\t{v.Managed}[] value = null;");
			w.AppendLine ($"\t\t\t\tobj.SetSmart{v.Managed}Values (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.PSmart{v.Managed}Properties, \"nullable property after setting default value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value, obj.GetSmart{v.Managed}Values (), \"nullable get method after setting default value\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tobj.SetSmart{v.Managed}Values (value);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (1, obj.PSmart{v.Managed}Properties.Length, \"nullable property after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (1, obj.GetSmart{v.Managed}Values ().Length, \"nullable get method after setting custom value\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.PSmart{v.Managed}Properties [0], \"nullable property after setting custom value element\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.GetSmart{v.Managed}Values () [0], \"nullable get method after setting custom value element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = null;");
			w.AppendLine ($"\t\t\t\tobj.SetSmart{v.Managed}Values (value);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.PSmart{v.Managed}Properties, \"null property after setting null value\");");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.GetSmart{v.Managed}Values (), \"null method after setting null value\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			// Array_Overrides
			w.AppendLine ("\t\t[Test]");
			w.AppendLine ($"\t\tpublic void NSStringBindAs_{v.Managed}_Array_Overrides ()");
			w.AppendLine ("\t\t{");
			if (v.MinXcodeVersion != null) {
				w.AppendLine ($"\t\t\tTestRuntime.AssertXcodeVersion ({v.MinXcodeVersion.Major}, {v.MinXcodeVersion.Minor});");
				w.AppendLine ();
			}
			w.AppendLine ($"\t\t\tusing (var obj = new BindAsTestClassGenerated ()) {{");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.PSmart{v.Managed}Properties, \"initial null\");");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmart{v.Managed}Values:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.PSmart{v.Managed}Properties, \"null after setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Properties = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmart{v.Managed}Values:\"), IntPtr.Zero);");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (obj.PSmart{v.Managed}Properties, \"null after re-setting null\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvar value = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tusing (var input = NSArray.FromNSObjects<{v.Managed}> ((v) => v.GetConstant (), value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmart{v.Managed}Values:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, obj.PSmart{v.Managed}Properties.Length, \"after setting A\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.PSmart{v.Managed}Properties [0], \"after setting A element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Properties = null;");
			w.AppendLine ($"\t\t\t\tusing (var input = NSArray.FromNSObjects<{v.Managed}> ((v) => v.GetConstant (), value))");
			w.AppendLine ($"\t\t\t\t\tMessaging.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle (\"setSmart{v.Managed}Values:\"), input.Handle);");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, obj.PSmart{v.Managed}Properties.Length, \"after setting B\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], obj.PSmart{v.Managed}Properties [0], \"after setting B element\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Properties = null;");
			w.AppendLine ($"\t\t\t\tvar array = Runtime.GetNSObject<NSArray> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"getSmart{v.Managed}Values\")));");
			w.AppendLine ($"\t\t\t\tAssert.IsNull (array, \"null from getter A\");");
			w.AppendLine ();

			w.AppendLine ($"\t\t\t\tvalue = new {v.Managed} [] {{ {v.ManagedNewExpression} }};");
			w.AppendLine ($"\t\t\t\tobj.PSmart{v.Managed}Properties = value;");
			w.AppendLine ($"\t\t\t\tarray = Runtime.GetNSObject<NSArray> (Messaging.IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle (\"getSmart{v.Managed}Values\")));");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value.Length, array.Count, \"getter B\");");
			w.AppendLine ($"\t\t\t\tAssert.AreEqual (value [0], {v.Managed}Extensions.GetValue (array.GetItem<NSString> (0)), \"getter B element\");");
			w.AppendLine ($"\t\t\t}}");
			w.AppendLine ("\t\t}");
			w.AppendLine ();

			if (v.ManagedCondition != null)
				w.AppendLine ("#endif");

			w.AppendLine ();
		}

		w.AppendLine (@"	}
}");

		File.WriteAllText ("RegistrarTest.generated.cs", w.ToString ());
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

		File.WriteAllText ("TrampolineTest.generated.cs", w.ToString ());
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
		WriteLibTestMethodsH ();
		WriteLibTestMethodsM ();

		/* binding code */
		WriteApiDefinition ();
		WriteStructsAndEnums ();

		/* tests */
		WriteRegistrarTests ();
		WriteTrampolineTests ();

		Console.WriteLine ("Generated test files");
	}
}
