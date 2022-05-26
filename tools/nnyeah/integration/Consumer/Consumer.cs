using System.Text;

using IntegrationAPI;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace ConsumerTests
{
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
				return output.ToString();
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
		}

		static void NFloatTest (StringBuilder output)
		{
			var n = new NFloatAPI ();

			if (n.EchoMethod ((NFloat)(-12.0)) != -12.0) {
				output.AppendLine ("nfloat method failure");
			}
			n.Prop = (NFloat)13.0;
			if (n.Prop != 13.0) {
				output.AppendLine ("nfloat prop failure");
			}
			n.Field = (NFloat)14.0;
			if (n.Field != 14.0) {
				output.AppendLine ("nfloat field failure");
			}
		}

		static void NSObjectCtor (StringBuilder output)
		{
			var n = new NSObjectDerived (NativeHandle.Zero);
			var n2 = new NSObjectDerived (NativeHandle.Zero, true);
		}
	}
}
