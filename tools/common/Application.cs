using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Mono.Cecil;
using Mono.Cecil.Mdb;

using Xamarin.Utils;

namespace Xamarin.Bundler {

	[Flags]
	public enum RegistrarOptions {
		Default = 0,
		Trace = 1,
	}

	public enum LinkMode {
		None,
		SDKOnly,
		All,
	}

	public partial class Application
	{
		public string AppDirectory;
		public bool DeadStrip = true;
		public bool EnableDebug;
		internal RuntimeOptions RuntimeOptions;
		public RegistrarMode Registrar = RegistrarMode.Default;
		public RegistrarOptions RegistrarOptions = RegistrarOptions.Default;

		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();

		public ApplePlatform Platform { get { return Driver.TargetFramework.Platform; } }

		// Linker config
		public LinkMode LinkMode = LinkMode.All;
		public List<string> LinkSkipped = new List<string> ();
		public List<string> Definitions = new List<string> ();
		public Mono.Linker.I18nAssemblies I18n;

		public bool? EnableCoopGC;

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.WatchOS:
					return "watchOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				default:
					throw new NotImplementedException ();
				}
			}
		}

		public static bool IsUptodate (string source, string target)
		{
			if (Driver.Force)
				return false;

			var tfi = new FileInfo (target);
			
			if (!tfi.Exists) {
				Driver.Log (3, "Target '{0}' does not exist.", target);
				return false;
			}

			var sfi = new FileInfo (source);

			if (sfi.LastWriteTimeUtc <= tfi.LastWriteTimeUtc) {
				Driver.Log (3, "Prerequisite '{0}' is older than the target '{1}'.", source, target);
				return true;
			} else {
				Driver.Log (3, "Prerequisite '{0}' is newer than the target '{1}'.", source, target);
				return false;
			}
		}

		public static void RemoveResource (ModuleDefinition module, string name)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				EmbeddedResource embedded = module.Resources[i] as EmbeddedResource;
				
				if (embedded == null || embedded.Name != name)
					continue;
				
				module.Resources.RemoveAt (i);
				break;
			}
		}

		public static void SaveAssembly (AssemblyDefinition assembly, string destination)
		{
			bool symbols = assembly.MainModule.HasSymbols;
			// re-write symbols, if available, so the new tokens will match
			assembly.Write (destination, new WriterParameters () { WriteSymbols = symbols });

			if (symbols) {
				// re-load symbols (cecil will dispose MdbReader and will crash later if we need to save again)
				var provider = new MdbReaderProvider ();
				assembly.MainModule.ReadSymbols (provider.GetSymbolReader (assembly.MainModule, destination));
			} else {
				// if we're not saving the symbols then we must not leave stale/old files to be used by other tools
				string dest_mdb = destination + ".mdb";
				if (File.Exists (dest_mdb))
					File.Delete (dest_mdb);
			}
		}

		public static void ExtractResource (ModuleDefinition module, string name, string path, bool remove)
		{
			for (int i = 0; i < module.Resources.Count; i++) {
				EmbeddedResource embedded = module.Resources[i] as EmbeddedResource;
				
				if (embedded == null || embedded.Name != name)
					continue;
				
				string dirname = Path.GetDirectoryName (path);
				if (!Directory.Exists (dirname))
					Directory.CreateDirectory (dirname);

				using (Stream ostream = File.OpenWrite (path)) {
					embedded.GetResourceStream ().CopyTo (ostream);
				}
				
				if (remove)
					module.Resources.RemoveAt (i);
				
				break;
			}
		}

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

		[DllImport (Constants.libSystemLibrary)]
		static extern IntPtr copyfile_state_alloc ();

		[DllImport (Constants.libSystemLibrary)]
		static extern int copyfile_state_free (IntPtr state);

		[DllImport (Constants.libSystemLibrary)]
		static extern int copyfile_state_set (IntPtr state, CopyFileState flag, IntPtr value);

		delegate CopyFileResult CopyFileCallbackDelegate (CopyFileWhat what, CopyFileStep stage, IntPtr state, string src, string dst, IntPtr ctx);

		[DllImport (Constants.libSystemLibrary, SetLastError = true)]
		static extern int copyfile (string @from, string @to, IntPtr state, CopyFileFlags flags);

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
				int rv = copyfile (source, target, state, CopyFileFlags.Data | CopyFileFlags.Recursive | CopyFileFlags.Nofollow);
				if (rv != 0)
					throw ErrorHelper.CreateError (1022, "Could not copy the directory '{0}' to '{1}': {2}", source, target, Target.strerror (Marshal.GetLastWin32Error ()));
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
						Driver.Log (1, "Copied {0} to {1}", source, target);
					return CopyFileResult.Continue;
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date", target);
					return CopyFileResult.Skip;
				}
			case CopyFileWhat.Dir:
			case CopyFileWhat.DirCleanup:
			case CopyFileWhat.CopyData:
			case CopyFileWhat.CopyXattr:
				return CopyFileResult.Continue;
			case CopyFileWhat.Error:
				throw ErrorHelper.CreateError (1021, "Could not copy the file '{0}' to '{1}': {2}", source, target, Target.strerror (Marshal.GetLastWin32Error ()));
			default:
				return CopyFileResult.Continue;
			}
		}

		public static void UpdateFile (string source, string target)
		{
			if (!Application.IsUptodate (source, target))
				CopyFile (source, target);
			else
				Driver.Log (3, "Target '{0}' is up-to-date", target);
		}

		// Checks if any of the source files have a time stamp later than any of the target files.
		public static bool IsUptodate (IEnumerable<string> sources, IEnumerable<string> targets)
		{
			if (Driver.Force)
				return false;

			DateTime max_source = DateTime.MinValue;
			string max_s = null;

			if (sources.Count () == 0 || targets.Count () == 0)
				ErrorHelper.Error (1013, "Dependency tracking error: no files to compare. Please file a bug report at http://bugzilla.xamarin.com with a test case.");

			foreach (var s in sources) {
				var sfi = new FileInfo (s);
				if (!sfi.Exists) {
					Driver.Log (3, "Prerequisite '{0}' does not exist.", s);
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
					Driver.Log (3, "Target '{0}' does not exist.", t);
					return false;
				}

				var lwt = tfi.LastWriteTimeUtc;
				if (max_source > lwt) {
					Driver.Log (3, "Prerequisite '{0}' is newer than target '{1}' ({2} vs {3}).", max_s, t, max_source, lwt);
					return false;
				}
			}

			Driver.Log (3, "Prerequisite(s) '{0}' are all older than the target(s) '{1}'.", string.Join ("', '", sources.ToArray ()), string.Join ("', '", targets.ToArray ()));

			return true;
		}

		[DllImport (Constants.libSystemLibrary)]
		static extern int readlink (string path, IntPtr buf, int len);

		// A file copy that will replace symlinks with the source file
		// File.Copy will copy the source to the target of the symlink instead
		// of replacing the symlink.
		public static void CopyFile (string source, string target)
		{
			if (readlink (target, IntPtr.Zero, 0) != -1) {
				// Target is a symlink, delete it.
				File.Delete (target);
			} else if (File.Exists (target)) {
				// Also delete the target file if it already exists,
				// since it may not have write permissions.
				File.Delete (target);
			}

			var dir = Path.GetDirectoryName (target);
			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			File.Copy (source, target, true);
			// Make sure the target file is r/w.
			var attrs = File.GetAttributes (target);
			if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes (target, attrs & ~FileAttributes.ReadOnly);
			Driver.Log (1, "Copied {0} to {1}", source, target);
		}

		public static void TryDelete (string path)
		{
			try {
				if (File.Exists (path))
					File.Delete (path);
			} catch {
			}
		}

		public void InitializeCommon ()
		{
			if (Platform == ApplePlatform.WatchOS && EnableCoopGC.HasValue && !EnableCoopGC.Value)
				throw ErrorHelper.CreateError (88, "Cannot disable the Coop GC for watchOS apps. Please remove the --coop:false argument to mtouch.");

			if (!EnableCoopGC.HasValue)
				EnableCoopGC = Platform == ApplePlatform.WatchOS;
		}
	}
}
