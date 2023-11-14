using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

#nullable enable

namespace Xamarin {
	// A class that creates temporary directories next to the test assembly, and cleans the output on startup
	// Advantages:
	// * The temporary directories are automatically cleaned on Wrench (unlike /tmp, which isn't)
	// * The temporary directories stay after a test is run (until a new test run is started),
	//   which makes it easier to re-run (copy-paste) commands that failed.
	public static class Cache // Not really a cache (since the root directory is cleaned in the cctor), but I couldn't come up with a better name.
	{
		static string root;
		static int last_number;

		static Cache ()
		{
#if NATIVEAOT
			root = Path.Combine (Path.GetDirectoryName (Environment.ProcessPath)!, "tmp-test-dir");
#else
			root = Path.Combine (Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location)!, "tmp-test-dir");
#endif
			if (Directory.Exists (root)) {
				var movedRoot = root + DateTime.UtcNow.Ticks.ToString () + "-deletion-in-progress";
				// The temporary directory can be big, and it can take a while to clean it out.
				// So move it to a different name (which should be fast), and then do the deletion on a different thread.
				// This should speed up startup in some cases.
				if (!Directory.Exists (movedRoot)) {
					try {
						Directory.Move (root, movedRoot);
						ThreadPool.QueueUserWorkItem ((v) => {
							Directory.Delete (movedRoot, true);
						});
					} catch {
						// Just delete the root if we can't move the temporary directory.
						Directory.Delete (root, true);
					}
				} else {
					Directory.Delete (root, true);
				}
			}
			Directory.CreateDirectory (root);
		}

		[DllImport ("libc", SetLastError = true)]
		static extern int mkdir (string path, ushort mode);

		[DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool CreateDirectory (string path, IntPtr lpSecurityAttributes);

		static bool TryCreateDirectory (string directory)
		{
			// There's no way to know if Directory.CreateDirectory
			// created the directory or not (which would happen if the directory
			// already existed). Checking if the directory exists before
			// creating it would result in a race condition if multiple
			// threads create temporary directories at the same time.
			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				return CreateDirectory (directory, IntPtr.Zero);
			} else {
				return mkdir (directory, Convert.ToUInt16 ("777", 8)) == 0;
			}
		}

		public static string CreateTemporaryDirectory (string? name = null)
		{
			if (string.IsNullOrEmpty (name)) {
				var calling_method = new System.Diagnostics.StackFrame (1).GetMethod ();
				if (calling_method is not null) {
					name = calling_method.DeclaringType!.FullName + "." + calling_method.Name;
				} else {
					name = "unknown-test";
				}
			}

			var rv = Path.Combine (root, name);
			for (int i = last_number; i < 10000 + last_number; i++) {
				if (TryCreateDirectory (rv)) {
					last_number = i;
					return rv;
				}
				rv = Path.Combine (root, name + i);
			}

			throw new Exception ("Could not create temporary directory");
		}
	}
}
