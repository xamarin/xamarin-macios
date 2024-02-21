using System;
using System.IO;

namespace InstallSources {
	public class PathManglerFactory {
		public bool Verbose { get; set; }
		public string InstallDir { get; set; }
		public string DestinationDir { get; set; }
		public string MonoSourcePath { get; set; }
		public string XamarinSourcePath { get; set; }
		public string FrameworkPath { get; set; }
		public string OpenTKSourcePath { get; set; }

		MonoPathMangler monoMangler;
		OpenTKSourceMangler openTKMangler;
		XamarinSourcesPathMangler xamarinPathMangler;

		readonly string srcSubPath = "src";
		readonly string runtimeSubPath = "runtime";
		readonly string xamariniOSDir = "Xamarin.iOS";
		readonly string xamarinMacDir = "Xamarin.Mac";

		MonoPathMangler MonoPathMangler {
			get {
				if (monoMangler is null)
					monoMangler = new MonoPathMangler {
						InstallDir = InstallDir,
						DestinationDir = DestinationDir,
						MonoSourcePath = MonoSourcePath,
						XamarinSourcePath = XamarinSourcePath,
					};
				return monoMangler;
			}
		}

		OpenTKSourceMangler OpenTKSourceMangler {
			get {
				if (openTKMangler is null)
					openTKMangler = new OpenTKSourceMangler {
						InstallDir = InstallDir,
						DestinationDir = DestinationDir,
						OpenTKSourcePath = OpenTKSourcePath
					};
				return openTKMangler;
			}
		}

		XamarinSourcesPathMangler XamarinSourcesPathMangler {
			get {
				if (xamarinPathMangler is null)
					xamarinPathMangler = new XamarinSourcesPathMangler {
						InstallDir = InstallDir,
						DestinationDir = DestinationDir,
						XamarinSourcePath = XamarinSourcePath,
						FrameworkPath = FrameworkPath
					};
				return xamarinPathMangler;
			}
		}

		public bool IsMonoPath (string path)
		{
			// remove the intall dir and append the mono source path
			if (path.StartsWith (MonoPathMangler.iOSFramework, StringComparison.Ordinal) || path.StartsWith (MonoPathMangler.MacFramework, StringComparison.Ordinal)) {
				// dealing with the jenkins paths
				if (Verbose) {
					Console.WriteLine ($"Install dir is {InstallDir}");
					Console.WriteLine ($"Original path os {path}");
				}

				var srcDir = path.Contains (xamariniOSDir) ? MonoPathMangler.iOSFramework : MonoPathMangler.MacFramework;
				if (Verbose)
					Console.WriteLine ($"Src path to remove {srcDir}");
				var relative = path.Remove (0, srcDir.Length);
				if (Verbose)
					Console.WriteLine ($"Relative path is {relative}");
				if (relative.StartsWith ("/", StringComparison.Ordinal))
					relative = relative.Remove (0, 1);
				var monoPath = Path.Combine (MonoSourcePath, relative);
				if (Verbose)
					Console.WriteLine ($"Mono path is {monoPath}");
				return File.Exists (monoPath);
			}
			// check if the path is the xamarin source path + the mono external submodule
			var monoSubmodule = Path.Combine (XamarinSourcePath.Replace ("src/", ""), "external", "mono");
			if (path.StartsWith (monoSubmodule, StringComparison.Ordinal))
				return true;
			if (path.StartsWith (XamarinSourcePath, StringComparison.Ordinal))
				return false;
			var xamarinRuntimePath = XamarinSourcePath.Replace ($"/{srcSubPath}/", $"/{runtimeSubPath}/");
			if (path.StartsWith (xamarinRuntimePath, StringComparison.Ordinal))
				return false;
			return path.StartsWith (MonoSourcePath, StringComparison.Ordinal);
		}

		public bool IsOpenTKPath (string path)
		{
			if (path.StartsWith (InstallDir, StringComparison.Ordinal)) {
				// dealing with the jenkins paths
				var srcDir = Path.Combine (InstallDir, srcSubPath,
					(InstallDir.Contains (xamariniOSDir) ? xamariniOSDir : xamarinMacDir));
				var relative = path.Remove (0, srcDir.Length);
				if (relative.StartsWith ("/", StringComparison.Ordinal))
					relative = relative.Remove (0, 1);
				var openTKPath = Path.Combine (OpenTKSourcePath, relative);
				return File.Exists (openTKPath);
			} else {
				return path.Contains (OpenTKSourcePath);
			}
		}

		public bool IsIgnored (string path)
		{
			return path.Contains ("/mcs/mcs/") ||
				path.Contains ("xammac-parser.cs") ||       // this would require adding sources to the Mono mac archive
				path.Contains ("xammac_net_4_5-parser.cs"); // which would add a lot of duplicate files, so just ignore these
		}

		public IPathMangler GetMangler (string path)
		{
			if (IsIgnored (path))
				return null;

			if (IsOpenTKPath (path)) {
				return OpenTKSourceMangler;
			}

			if (IsMonoPath (path)) {
				return MonoPathMangler;
			}

			return XamarinSourcesPathMangler;
		}
	}
}
