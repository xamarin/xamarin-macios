using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Utilities {
	// A class that creates temporary directories next to the test assembly, and cleans the output on startup
	// Advantages:
	// * The temporary directories are automatically cleaned on Wrench (unlike /tmp, which isn't)
	// * The temporary directories stay after a test is run (until a new test run is started),
	//   which makes it easier to re-run (copy-paste) commands that failed.
	public static class DirectoryUtilities {
		static string root;
		static int lastNumber;

		static DirectoryUtilities ()
		{
			root = Path.Combine (Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location), "tmp-test-dir");
			if (Directory.Exists (root))
				Directory.Delete (root, true);
			Directory.CreateDirectory (root);
		}

		[DllImport ("libc", SetLastError = true)]
		static extern int mkdir (string path, ushort mode);

		public static string CreateTemporaryDirectory (string name = null)
		{
			if (string.IsNullOrEmpty (name)) {
				var calling_method = new System.Diagnostics.StackFrame (1).GetMethod ();
				if (calling_method != null) {
					name = calling_method.DeclaringType.FullName + "." + calling_method.Name;
				} else {
					name = "unknown-test";
				}
			}

			var rv = Path.Combine (root, name);
			for (int i = lastNumber; i < 10000 + lastNumber; i++) {
				// There's no way to know if Directory.CreateDirectory
				// created the directory or not (which would happen if the directory
				// already existed). Checking if the directory exists before
				// creating it would result in a race condition if multiple
				// threads create temporary directories at the same time.
				if (mkdir (rv, Convert.ToUInt16 ("777", 8)) == 0) {
					lastNumber = i;
					return rv;
				}
				rv = Path.Combine (root, name + i);
			}

			throw new Exception ("Could not create temporary directory");
		}
	}
}
