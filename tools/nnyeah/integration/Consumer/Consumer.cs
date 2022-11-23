using System.Text;

using IntegrationAPI;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace ConsumerTests {
	public static class Consumer {
		public static string Test ()
		{
			var output = new StringBuilder ();

			NIntTest (output);
			NUIntTest (output);
			NFloatTest (output);
			NSObjectCtor (output);

			if (output.Length == 0) {
				return "Test Successful";
			} else {
				return output.ToString ();
			}
		}

		static void NIntTest (StringBuilder output)
		{
			var n = new NIntAPI ();

			if (n.EchoMethod (-12) != -12) {
				output.AppendLine ("nint method failure");
			}
			n.Prop = 13;
			if (n.Prop != 13) {
				output.AppendLine ("nint prop failure");
			}
			n.Field = 14;
			if (n.Field != 14) {
				output.AppendLine ("nint field failure");
			}
			if (n.Sum (3, 4) != 7) {
				output.AppendLine ("nint sum failure");
			}
			if (n.Prod (5, 6) != 30) {
				output.AppendLine ("nint product failure");
			}
			if (n.ToLong (42) != 42) {
				output.AppendLine ("nint conversion failure");
			}
			if (n.Less (4, 3)) {
				output.AppendLine ("nint less failure (0)");
			}
			if (n.Less (2, 2)) {
				output.AppendLine ("nint less failure (1)");
			}
			if (!n.Less (2, 3)) {
				output.AppendLine ("nint less failure (2)");
			}
			if (!n.Greater (4, 3)) {
				output.AppendLine ("nint greater failure (0)");
			}
			if (n.Greater (2, 2)) {
				output.AppendLine ("nint greater failure (1)");
			}
			if (n.Greater (2, 3)) {
				output.AppendLine ("nint greater failure (2)");
			}
			if (n.And (7, 3) != 3) {
				output.AppendLine ("nint And failure");
			}
			if (n.Or (9, 3) != 12) {
				output.AppendLine ("nint Or failure");
			}
			if (n.Xor (9, 3) != 11) {
				output.AppendLine ("nint Xor failure");
			}
			if (n.ToNint ((sbyte) 7) != (nint) 7) {
				output.AppendLine ("nint implicit failure (sbyte)");
			}
			if (n.ToNint ((sbyte) 8) != (nint) 8) {
				output.AppendLine ("nint implicit failure (byte)");
			}
			if (n.ToNint ((short) 9) != (nint) 9) {
				output.AppendLine ("nint implicit failure (short)");
			}
			if (n.ToNint ((char) 10) != (nint) 10) {
				output.AppendLine ("nint implicit failure (char)");
			}
			if (n.ToNint (11) != (nint) 11) {
				output.AppendLine ("nint implicit failure (int)");
			}
			if (n.PlusOne (7) != (nint) 8) {
				output.AppendLine ("nint ++ failure");
			}
			if (n.NumberZero () != (nint) 0) {
				output.AppendLine ("nested usage failure");
			}
		}

		static void NUIntTest (StringBuilder output)
		{
			var n = new NUIntAPI ();

			if (n.EchoMethod (12) != 12) {
				output.AppendLine ("nuint method failure");
			}
			n.Prop = 13;
			if (n.Prop != 13) {
				output.AppendLine ("nuint prop failure");
			}
			n.Field = 14;
			if (n.Field != 14) {
				output.AppendLine ("nuint field failure");
			}
			if (n.Sum (3, 4) != 7) {
				output.AppendLine ("nuint sum failure");
			}
			if (n.Prod (5, 6) != 30) {
				output.AppendLine ("nuint product failure");
			}
			if (n.ToLong (42) != 42) {
				output.AppendLine ("nuint conversion failure");
			}
			if (n.Less (4, 3)) {
				output.AppendLine ("nuint less failure (0)");
			}
			if (n.Less (2, 2)) {
				output.AppendLine ("nuint less failure (1)");
			}
			if (!n.Less (2, 3)) {
				output.AppendLine ("nuint less failure (2)");
			}
			if (!n.Greater (4, 3)) {
				output.AppendLine ("nuint greater failure (0)");
			}
			if (n.Greater (2, 2)) {
				output.AppendLine ("nuint greater failure (1)");
			}
			if (n.Greater (2, 3)) {
				output.AppendLine ("nuint greater failure (2)");
			}
			if (n.Eq (4, 3)) {
				output.AppendLine ("nuint equal failure (0)");
			}
			if (!n.Eq (2, 2)) {
				output.AppendLine ("nuint equal failure (1)");
			}
			if (n.Eq (2, 3)) {
				output.AppendLine ("nuint equal failure (2)");
			}
			if (n.And (7, 3) != 3) {
				output.AppendLine ("nuint And failure");
			}
			if (n.Or (9, 3) != 12) {
				output.AppendLine ("nuint Or failure");
			}
			if (n.Xor (9, 3) != 11) {
				output.AppendLine ("nuint Xor failure");
			}
			if (n.ToNuint ((byte) 5) != (nuint) 5) {
				output.AppendLine ("nuint implicit failure (byte)");
			}
			if (n.ToNuint ((ushort) 6) != (nuint) 6) {
				output.AppendLine ("nuint implicit failure (ushort)");
			}
			if (n.ToNuint ((uint) 7) != (nuint) 7) {
				output.AppendLine ("nuint implicit failure (uint)");
			}
			if (n.ToNuint ((char) 8) != (nuint) 8) {
				output.AppendLine ("nuint implicit failure (char)");
			}
			if (n.PlusOne (27) != (nuint) 27) {
				output.AppendLine ("nuint ++ failure");
			}
		}

		static void NFloatTest (StringBuilder output)
		{
			var n = new NFloatAPI ();

			if (n.EchoMethod ((NFloat) (-12.0)) != -12.0) {
				output.AppendLine ("nfloat method failure");
			}
			n.Prop = (NFloat) 13.0;
			if (n.Prop != 13.0) {
				output.AppendLine ("nfloat prop failure");
			}
			n.Field = (NFloat) 14.0;
			if (n.Field != 14.0) {
				output.AppendLine ("nfloat field failure");
			}
			if (n.Sum ((NFloat) 3.0, (NFloat) 4) != 7.0) {
				output.AppendLine ("nfloat sum failure");
			}
			if (n.Prod ((NFloat) 5.0, (NFloat) 6.0) != 30.0) {
				output.AppendLine ("nfloat product failure");
			}
			if (n.ToDouble ((NFloat) 42.0) != 42.0) {
				output.AppendLine ("nfloat conversion failure");
			}
			if (n.Less ((NFloat) 4.0, (NFloat) 3.0)) {
				output.AppendLine ("nfloat less failure (0)");
			}
			if (n.Less ((NFloat) 2.0, (NFloat) 2.0)) {
				output.AppendLine ("nfloat less failure (1)");
			}
			if (!n.Less ((NFloat) 2.0, (NFloat) 3.0)) {
				output.AppendLine ("nfloat less failure (2)");
			}
			if (!n.Greater ((NFloat) 4.0, (NFloat) 3.0)) {
				output.AppendLine ("nfloat greater failure (0)");
			}
			if (n.Greater ((NFloat) 2.0, (NFloat) 2.0)) {
				output.AppendLine ("nfloat greater failure (1)");
			}
			if (n.Greater ((NFloat) 2.0, (NFloat) 3.0)) {
				output.AppendLine ("nfloat greater failure (2)");
			}
			if (n.Eq ((NFloat) 4.0, (NFloat) 3.0)) {
				output.AppendLine ("nfloat equal failure (0)");
			}
			if (!n.Eq ((NFloat) 2.0, (NFloat) 2.0)) {
				output.AppendLine ("nfloat equal failure (1)");
			}
			if (n.Eq ((NFloat) 2.0, (NFloat) 3.0)) {
				output.AppendLine ("nfloat equal failure (2)");
			}
			if (n.ToNFloat ((sbyte) 4) != (NFloat) 4) {
				output.AppendLine ("nfloat implicit failure (sbyte)");
			}
			if (n.ToNFloat ((byte) 5) != (NFloat) 5) {
				output.AppendLine ("nfloat implicit failure (byte)");
			}
			if (n.ToNFloat ((char) 6) != (NFloat) 6) {
				output.AppendLine ("nfloat implicit failure (char)");
			}
			if (n.ToNFloat ((short) 7) != (NFloat) 7) {
				output.AppendLine ("nfloat implicit failure (short)");
			}
			if (n.ToNFloat ((ushort) 8) != (NFloat) 8) {
				output.AppendLine ("nfloat implicit failure (ushort)");
			}
			if (n.ToNFloat ((int) 9) != (NFloat) 9) {
				output.AppendLine ("nfloat implicit failure (int)");
			}
			if (n.ToNFloat ((uint) 10) != (NFloat) 10) {
				output.AppendLine ("nfloat implicit failure (uint)");
			}
			if (n.ToNFloat ((long) 11) != (NFloat) 11) {
				output.AppendLine ("nfloat implicit failure (long)");
			}
			if (n.ToNFloat ((ulong) 12) != (NFloat) 12) {
				output.AppendLine ("nfloat implicit failure (ulong)");
			}
			if (n.ToNFloat ((float) 13) != (NFloat) 13) {
				output.AppendLine ("nfloat implicit failure (float)");
			}
		}

		static void NSObjectCtor (StringBuilder output)
		{
			var n = new NSObjectDerived (NativeHandle.Zero);
			var n2 = new NSObjectDerived (NativeHandle.Zero, true);
		}
	}
}
