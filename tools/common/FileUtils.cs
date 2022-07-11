using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Xamarin.Utils {
	internal static class FileUtils {

		public static bool CompareFiles (string a, string b)
		{
			using (var astream = new FileStream (a, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				using (var bstream = new FileStream (b, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					bool rv;
					rv = CompareStreams (astream, bstream);
					return rv;
				}
			}
		}

		public unsafe static bool CompareStreams (Stream astream, Stream bstream)
		{
			if (astream.Length != bstream.Length)
				return false;

			var ab = new byte [2048];
			var bb = new byte [2048];

			do {
				int ar = astream.Read (ab, 0, ab.Length);
				int br = bstream.Read (bb, 0, bb.Length);

				if (ar != br)
					return false;

				if (ar == 0)
					return true;

				fixed (byte* aptr = ab, bptr = bb) {
					long* l1 = (long*) aptr;
					long* l2 = (long*) bptr;
					int len = ar;
					// Compare one long at a time.
					for (int i = 0; i < len / 8; i++) {
						if (l1 [i] != l2 [i])
							return false;
					}
					// Compare any remaining bytes.
					int mod = len % 8;
					if (mod > 0) {
						for (int i = len - mod; i < len; i++) {
							if (ab [i] != bb [i])
								return false;
						}
					}
				}
			} while (true);
		}

		// Returns true if the target file was updated, false if the target file was already up-to-date.
		public static bool UpdateFile (string targetFile, Action<string> createOutput)
		{
			var tmpFile = Path.GetTempFileName ();
			try {
				createOutput (tmpFile);
				if (File.Exists (targetFile) && FileUtils.CompareFiles (tmpFile, targetFile)) {
					// File is up-to-date
					return false;
				} else {
					Directory.CreateDirectory (Path.GetDirectoryName (targetFile));
					File.Copy (tmpFile, targetFile, true);
					return true;
				}
			} finally {
				File.Delete (tmpFile);
			}

		}
	}
}
