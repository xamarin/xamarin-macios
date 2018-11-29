using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xamarin.Bundler {
	public static class FileCopier 
	{
		enum CopyFileFlags : uint {
			ACL = 1 << 0,
			Stat = 1 << 1,
			Xattr = 1 << 2,
			Data = 1 << 3,
			Security = Stat | ACL,
			Metadata = Security | Xattr,
			All = Metadata | Data,

			Recursive = 1 << 15,
			NoFollow_Src = 1 << 18,
			NoFollow_Dst = 1 << 19,
			Unlink = 1 << 21,
			Nofollow = NoFollow_Src | NoFollow_Dst,
			Clone = 1 << 24,
		}

		enum CopyFileState : uint {
			StatusCB = 6,
		}

		enum CopyFileStep {
			Start = 1,
			Finish = 2,
			Err = 3,
			Progress = 4,
		}

		enum CopyFileResult {
			Continue = 0,
			Skip = 1,
			Quit = 2,
		}

		enum CopyFileWhat {
			Error = 0,
			File = 1,
			Dir = 2,
			DirCleanup = 3,
			CopyData = 4,
			CopyXattr = 5,
		}

		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern IntPtr copyfile_state_alloc ();

		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern int copyfile_state_free (IntPtr state);

		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern int copyfile_state_set (IntPtr state, CopyFileState flag, IntPtr value);

		delegate CopyFileResult CopyFileCallbackDelegate (CopyFileWhat what, CopyFileStep stage, IntPtr state, string src, string dst, IntPtr ctx);

		[DllImport ("/usr/lib/libSystem.dylib", SetLastError = true)]
		static extern int copyfile (string @from, string @to, IntPtr state, CopyFileFlags flags);

		// This code is shared between our packaging tools (mmp\mtouch) and msbuild tasks
#if MMP || MTOUCH
		public static void Log (int min_verbosity, string format, params object[] args) => Driver.Log (min_verbosity, format, args);
		public static bool Force => Driver.Force;
		public static Exception CreateError (int code, string message, params object[] args) => ErrorHelper.CreateError (code, message, args);
#else
		public static void Log (int min_verbosity, string format, params object[] args) => Console.WriteLine (format, args);
		public static bool Force => false;
		public static Exception CreateError (int code, string message, params object[] args) => throw new Exception ($"{code} {string.Format (message, args)}");
#endif

		public static void UpdateDirectory (string source, string target)
		{
			if (!Directory.Exists (target))
				Directory.CreateDirectory (target);

			// Mono's File.Copy can't handle symlinks (the symlinks are followed instead of copied),
			// so we need to use native functions directly. Luckily OSX provides exactly what we need.
			IntPtr state = copyfile_state_alloc ();
			try {
				CopyFileCallbackDelegate del = CopyFileCallback;
				copyfile_state_set (state, CopyFileState.StatusCB, Marshal.GetFunctionPointerForDelegate (del));
				int rv = copyfile (source, target, state, CopyFileFlags.Data | CopyFileFlags.Recursive | CopyFileFlags.Nofollow | CopyFileFlags.Clone);
				if (rv != 0)
					throw CreateError (1022, "Could not copy the directory '{0}' to '{1}': {2}", source, target, strerror (Marshal.GetLastWin32Error ()));
			} finally {
				copyfile_state_free (state);
			}
		}

		static CopyFileResult CopyFileCallback (CopyFileWhat what, CopyFileStep stage, IntPtr state, string source, string target, IntPtr ctx)
		{
//			Console.WriteLine ("CopyFileCallback ({0}, {1}, 0x{2}, {3}, {4}, 0x{5})", what, stage, state.ToString ("x"), source, target, ctx.ToString ("x"));
			switch (what) {
			case CopyFileWhat.File:
				if (!IsUptodate (source, target)) {
					if (stage == CopyFileStep.Finish)
						Log (1, "Copied {0} to {1}", source, target);
					return CopyFileResult.Continue;
				} else {
					Log (3, "Target '{0}' is up-to-date", target);
					return CopyFileResult.Skip;
				}
			case CopyFileWhat.Dir:
			case CopyFileWhat.DirCleanup:
			case CopyFileWhat.CopyData:
			case CopyFileWhat.CopyXattr:
				return CopyFileResult.Continue;
			case CopyFileWhat.Error:
				throw CreateError (1021, "Could not copy the file '{0}' to '{1}': {2}", source, target, strerror (Marshal.GetLastWin32Error ()));
			default:
				return CopyFileResult.Continue;
			}
		}

		// Checks if the source file has a time stamp later than the target file.
		//
		// Optionally check if the contents of the files are different after checking the timestamp.
		//
		// If check_stamp is true, the function will use the timestamp of a "target".stamp file
		// if it's later than the timestamp of the "target" file itself.
		public static bool IsUptodate (string source, string target, bool check_contents = false, bool check_stamp = true)
		{
			if (Force)
				return false;

			var tfi = new FileInfo (target);
			
			if (!tfi.Exists) {
				Log (3, "Target '{0}' does not exist.", target);
				return false;
			}

			if (check_stamp) {
				var tfi_stamp = new FileInfo (target + ".stamp");
				if (tfi_stamp.Exists && tfi_stamp.LastWriteTimeUtc > tfi.LastWriteTimeUtc) {
					Log (3, "Target '{0}' has a stamp file with newer timestamp ({1} > {2}), using the stamp file's timestamp", target, tfi_stamp.LastWriteTimeUtc, tfi.LastWriteTimeUtc);
					tfi = tfi_stamp;
				}
			}

			var sfi = new FileInfo (source);

			if (sfi.LastWriteTimeUtc <= tfi.LastWriteTimeUtc) {
				Log (3, "Prerequisite '{0}' is older than the target '{1}'.", source, target);
				return true;
			}

			if (check_contents && CompareFiles (source, target)) {
				Log (3, "Prerequisite '{0}' is newer than the target '{1}', but the contents are identical.", source, target);
				return true;
			}

			Log (3, "Prerequisite '{0}' is newer than the target '{1}'.", source, target);
			return false;
		}
		
		[DllImport ("/usr/lib/libSystem.dylib", SetLastError = true, EntryPoint = "strerror")]
		static extern IntPtr _strerror (int errno);

		internal static string strerror (int errno)
		{
			return Marshal.PtrToStringAuto (_strerror (errno));
		}

		public static bool CompareFiles (string a, string b, bool ignore_cache = false)
		{
#if !MMP && !MTOUCH
			if (ignore_cache)
				throw new NotImplementedException ("CompareFiles with ignore_cache");
#endif

			if (!File.Exists (b)) {
				Log (6, "Files {0} and {1} are considered different because the latter doesn't exist.", a, b);
				return false;
			}

			using (var astream = new FileStream (a, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				using (var bstream = new FileStream (b, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					bool rv;
					Log (6, "Comparing files {0} and {1}...", a, b);
					rv = CompareStreams (astream, bstream, ignore_cache);
					Log (6, " > {0}", rv ? "Identical" : "Different");
					return rv;
				}
			}
		}

		public unsafe static bool CompareStreams (Stream astream, Stream bstream, bool ignore_cache = false)
		{
#if !MMP && !MTOUCH
			if (ignore_cache)
				throw new NotImplementedException ("CompareStreams with ignore_cache");
#endif

			if (astream.Length != bstream.Length) {
				Log (6, " > streams are considered different because their lengths do not match.");
				return false;
			}

			var ab = new byte[2048];
			var bb = new byte[2048];
			
			do {
				int ar = astream.Read (ab, 0, ab.Length);
				int br = bstream.Read (bb, 0, bb.Length);

				if (ar != br) {
					Log (6, " > streams are considered different because their lengths do not match.");
					return false;
				}

				if (ar == 0)
					return true;

				fixed (byte *aptr = ab, bptr = bb) {
					long *l1 = (long *) aptr;
					long *l2 = (long *) bptr;
					int len = ar;
					// Compare one long at a time.
					for (int i = 0; i < len / 8; i++) {
						if (l1 [i] != l2 [i]) {
							Log (6, " > streams differ at index {0}-{1}", i, i + 8);
							return false;
						}
					}
					// Compare any remaining bytes.
					int mod = len % 8;
					if (mod > 0) {
						for (int i = len - mod; i < len; i++) {
							if (ab [i] != bb [i]) {						
								Log (6, " > streams differ at byte index {0}", i);
								return false;
							}
						}
					}
				}
			} while (true);
		}


	}
}
