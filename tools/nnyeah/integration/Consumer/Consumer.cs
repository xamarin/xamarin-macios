using System.Text;

using IntegrationAPI;
using System.Runtime.InteropServices;

namespace ConsumerTests
{
	public static class Consumer {
		public static string Test ()
		{
			var output = new StringBuilder ();

			NIntTest (output);
			NUIntTest (output);
			NFloatTest (output);

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
			if (n.Sum (3, 4) != 7) {
				output.AppendLine ("nint sum failure");
			}
			if (n.Prod (5, 6) != 30) {
				output.AppendLine ("nint product failure");
			}
			if (n.ToLong (42) != 42) {
				output.AppendLine ("nint conversion failure");
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
			if (n.Sum ((NFloat)3.0, (NFloat)4) != 7.0) {
				output.AppendLine ("nfloat sum failure");
			}
			if (n.Prod ((NFloat)5.0, (NFloat)6.0) != 30.0) {
				output.AppendLine ("nfloat product failure");
			}
			if (n.ToDouble ((NFloat)42.0) != 42.0) {
				output.AppendLine ("nfloat conversion failure");
			}
		}
	}
}
