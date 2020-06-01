using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Utilities {

	public static class Helpers {

		// We want guids that nobody else has, but we also want to generate the same guid
		// on subsequent invocations (so that csprojs don't change unnecessarily, which is
		// annoying when XS reloads the projects, and also causes unnecessary rebuilds).
		// Nothing really breaks when the sequence isn't identical from run to run, so
		// this is just a best minimal effort.
		static Random guid_generator = new Random (unchecked((int) 0xdeadf00d));
		public static Guid GenerateStableGuid (string seed = null)
		{
			var bytes = new byte [16];
			if (seed == null) guid_generator.NextBytes (bytes);
			else {
				using (var provider = MD5.Create ()) {
					var inputBytes = Encoding.UTF8.GetBytes (seed);
					bytes = provider.ComputeHash (inputBytes);
				}
			}
			return new Guid (bytes);
		}

		public static string Timestamp => $"{DateTime.Now:yyyyMMdd_HHmmss}";
	}
}
