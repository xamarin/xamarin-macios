using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xamarin.Bundler {
	public static class FileCopier {
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
		public delegate void LogCallback (int verbosity, string format, params object [] arguments);
		public delegate void ReportErrorCallback (int code, string format, params object [] arguments);

		[ThreadStatic]
		static LogCallback logCallback;

		[ThreadStatic]
		static ReportErrorCallback reportErrorCallback;

		static void Log (int min_verbosity, string format, params object [] arguments)
		{
			if (logCallback is not null) {
				logCallback (min_verbosity, format, arguments);
				return;
			}

#if MMP || MTOUCH || BUNDLER
			// LogMessage and LogError are instance objects on the tasks themselves and bubbling an event up is not ideal
			Driver.Log (min_verbosity, format, arguments);
#else
			Console.WriteLine (format, arguments);
#endif
		}

		static void ReportError (int code, string format, params object [] arguments)
		{
			if (reportErrorCallback is not null) {
				reportErrorCallback (code, format, arguments);
				return;
			}

#if MMP || MTOUCH || BUNDLER
			throw ErrorHelper.CreateError (code, format, arguments);
#else
			// msbuild handles uncaught exceptions as a task error
			throw new Exception ($"{code} {string.Format (format, arguments)}");
#endif
		}

		public static void UpdateDirectory (string source, string target, ReportErrorCallback reportErrorCallback, LogCallback logCallback)
		{
			try {
				FileCopier.reportErrorCallback = reportErrorCallback;
				FileCopier.logCallback = logCallback;
				UpdateDirectory (source, target);
			} finally {
				FileCopier.reportErrorCallback = null;
				FileCopier.logCallback = null;
			}
		}

#if MMP || MTOUCH || BUNDLER
		public static void UpdateDirectory (string source, string target)
#else
		static void UpdateDirectory (string source, string target)
#endif
		{
			// first chance, try to update existing content inside `target`
			if (TryUpdateDirectory (source, target, out var err))
				return;

			// 2nd chance, remove `target` then copy everything
			Log (1, "Could not update `{0}` content (error #{1} : {2}), trying to overwrite everything...", target, err, strerror (err));
			Directory.Delete (target, true);
			if (!TryUpdateDirectory (source, target, out err))
				ReportError (1022, Errors.MT1022, source, target, err, strerror (err));
		}

		static bool TryUpdateDirectory (string source, string target, out int errno)
		{
			Directory.CreateDirectory (target);

			// Mono's File.Copy can't handle symlinks (the symlinks are followed instead of copied),
			// so we need to use native functions directly. Luckily OSX provides exactly what we need.
			IntPtr state = copyfile_state_alloc ();
			try {
				CopyFileCallbackDelegate del = CopyFileCallback;
				copyfile_state_set (state, CopyFileState.StatusCB, Marshal.GetFunctionPointerForDelegate (del));
				int rv = copyfile (source, target, state, CopyFileFlags.Data | CopyFileFlags.Recursive | CopyFileFlags.Nofollow | CopyFileFlags.Clone);
				if (rv == 0) {
					errno = 0; // satisfy compiler and make sure not to pick up some older error code
					return true;
				} else {
					errno = Marshal.GetLastWin32Error (); // might not be very useful since the callback signaled an error (CopyFileResult.Quit)
					return false;
				}
			} finally {
				copyfile_state_free (state);
			}
		}

		// do not call `Marshal.GetLastWin32Error` inside this method since it's called while the p/invoke is executing and will return `260`
		static CopyFileResult CopyFileCallback (CopyFileWhat what, CopyFileStep stage, IntPtr state, string source, string target, IntPtr ctx)
		{
			//			Console.WriteLine ("CopyFileCallback ({0}, {1}, 0x{2}, {3}, {4}, 0x{5})", what, stage, state.ToString ("x"), source, target, ctx.ToString ("x"));
			switch (what) {
			case CopyFileWhat.File:
				if (!IsUptodate (source, target)) {
					if (stage == CopyFileStep.Finish)
						Log (1, "Copied {0} to {1}", source, target);
					else if (stage == CopyFileStep.Err) {
						Log (1, "Could not copy the file '{0}' to '{1}'", source, target);
						return CopyFileResult.Quit;
					} else if (stage == CopyFileStep.Start) {
						if (File.Exists (target) || Directory.Exists (target)) {
							Log (1, "Deleted target {0}, it's not up-to-date", target);
							// This callback won't be called for directories, but we can get here for symlinks to directories.
							// This means that File.Delete should always work (no need to check for a directory to call Directory.Delete)
							File.Delete (target);
						}
					}
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
				Log (1, "Could not copy the file '{0}' to '{1}'", source, target);
				return CopyFileResult.Quit;
			default:
				return CopyFileResult.Continue;
			}
		}

		public static bool IsUptodate (string source, string target, ReportErrorCallback reportErrorCallback, LogCallback logCallback, bool check_contents = false, bool check_stamp = true)
		{
			try {
				FileCopier.reportErrorCallback = reportErrorCallback;
				FileCopier.logCallback = logCallback;
				return IsUptodate (source, target, check_contents, check_stamp);
			} finally {
				FileCopier.reportErrorCallback = null;
				FileCopier.logCallback = null;
			}
		}

		// Checks if the source file has a time stamp later than the target file.
		//
		// Optionally check if the contents of the files are different after checking the timestamp.
		//
		// If check_stamp is true, the function will use the timestamp of a "target".stamp file
		// if it's later than the timestamp of the "target" file itself.
#if MMP || MTOUCH || BUNDLER
		public static bool IsUptodate (string source, string target, bool check_contents = false, bool check_stamp = true)
#else
		static bool IsUptodate (string source, string target, bool check_contents = false, bool check_stamp = true)
#endif
		{
#if MMP || MTOUCH || BUNDLER   // msbuild does not have force                                  
			if (Driver.Force)
				return false;
#endif

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

#if MMP || MTOUCH || BUNDLER   // msbuild usages do not require CompareFiles optimization                                                              
			if (check_contents && Cache.CompareFiles (source, target)) {
				Log (3, "Prerequisite '{0}' is newer than the target '{1}', but the contents are identical.", source, target);
				return true;
			}
#else
			if (check_contents)
				throw new NotImplementedException ("Checking file contents is not supported");
#endif

			Log (3, "Prerequisite '{0}' is newer than the target '{1}'.", source, target);
			return false;
		}

		public static bool IsUptodate (IEnumerable<string> sources, IEnumerable<string> targets, ReportErrorCallback reportErrorCallback, LogCallback logCallback, bool check_stamp = true)
		{
			try {
				FileCopier.reportErrorCallback = reportErrorCallback;
				FileCopier.logCallback = logCallback;
				return IsUptodate (sources, targets, check_stamp);
			} finally {
				FileCopier.reportErrorCallback = null;
				FileCopier.logCallback = null;
			}
		}

		// Checks if any of the source files have a time stamp later than any of the target files.
		//
		// If check_stamp is true, the function will use the timestamp of a "target".stamp file
		// if it's later than the timestamp of the "target" file itself.
#if MMP || MTOUCH || BUNDLER
		public static bool IsUptodate (IEnumerable<string> sources, IEnumerable<string> targets, bool check_stamp = true)
#else
		static bool IsUptodate (IEnumerable<string> sources, IEnumerable<string> targets, bool check_stamp = true)
#endif
		{
#if MMP || MTOUCH || BUNDLER  // msbuild does not have force
			if (Driver.Force)
				return false;
#endif

			DateTime max_source = DateTime.MinValue;
			string max_s = null;

			if (sources.Count () == 0 || targets.Count () == 0)
				throw ErrorHelper.CreateError (1013, Errors.MT1013);

			foreach (var s in sources) {
				var sfi = new FileInfo (s);
				if (!sfi.Exists) {
					Log (3, "Prerequisite '{0}' does not exist.", s);
					return false;
				}

				var st = sfi.LastWriteTimeUtc;
				if (st > max_source) {
					max_source = st;
					max_s = s;
				}
			}


			foreach (var t in targets) {
				var tfi = new FileInfo (t);
				if (!tfi.Exists) {
					Log (3, "Target '{0}' does not exist.", t);
					return false;
				}

				if (check_stamp) {
					var tfi_stamp = new FileInfo (t + ".stamp");
					if (tfi_stamp.Exists && tfi_stamp.LastWriteTimeUtc > tfi.LastWriteTimeUtc) {
						Log (3, "Target '{0}' has a stamp file with newer timestamp ({1} > {2}), using the stamp file's timestamp", t, tfi_stamp.LastWriteTimeUtc, tfi.LastWriteTimeUtc);
						tfi = tfi_stamp;
					}
				}

				var lwt = tfi.LastWriteTimeUtc;
				if (max_source > lwt) {
					Log (3, "Prerequisite '{0}' is newer than target '{1}' ({2} vs {3}).", max_s, t, max_source, lwt);
					return false;
				}
			}

			Log (3, "Prerequisite(s) '{0}' are all older than the target(s) '{1}'.", string.Join ("', '", sources.ToArray ()), string.Join ("', '", targets.ToArray ()));

			return true;
		}

		[DllImport ("/usr/lib/libSystem.dylib", SetLastError = true, EntryPoint = "strerror")]
		static extern IntPtr _strerror (int errno);

		internal static string strerror (int errno)
		{
			return Marshal.PtrToStringAuto (_strerror (errno));
		}
	}
}
