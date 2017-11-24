using System;
using System.IO;

namespace InstallSources
{
	public class PathManglerFactory
	{
		public string InstallDir { get; set; }
		public string MonoSourcePath { get; set; }
		public string XamarinSourcePath { get; set; }
		public string FrameworkPath { get; set; }
		public string OpenTKSourcePath { get; set; }

		MonoPathMangler monoMangler;
		OpenTKSourceMangler openTKMangler;
		XamarinSourcesPathMangler xamarinPathMangler;

		MonoPathMangler MonoPathMangler {
			get {
				if (monoMangler == null)
					monoMangler = new MonoPathMangler {
						InstallDir = InstallDir,
						MonoSourcePath = MonoSourcePath
					};
				return monoMangler;
			}
		}

		OpenTKSourceMangler OpenTKSourceMangler {
			get {
				if (openTKMangler == null)
					openTKMangler = new OpenTKSourceMangler {
						InstallDir = InstallDir,
						OpenTKSourcePath = OpenTKSourcePath
					};
				return openTKMangler;
			}
		}

		XamarinSourcesPathMangler XamarinSourcesPathMangler {
			get {
				if (xamarinPathMangler == null)
					xamarinPathMangler = new XamarinSourcesPathMangler {
						InstallDir = InstallDir,
						XamarinSourcePath = XamarinSourcePath,
						FrameworkPath = FrameworkPath
					};
				return xamarinPathMangler;
			}
		}

		public bool IsMonoPath (string path)
		{
			// remove the intall dir and append the mono source path
			if (path.StartsWith(InstallDir, StringComparison.Ordinal)) {
				// dealing with the jenkins paths
				var srcDir = Path.Combine (InstallDir, (InstallDir.Contains("Xamarin.iOS") ? "Xamarin.iOS" : "Xamarin.Mac"), "src");
				var relative = path.Remove (0, srcDir.Length);
				if (relative.StartsWith("/", StringComparison.Ordinal))
					relative = path.Remove (0, 1);
				var monoPath = Path.Combine (MonoSourcePath, relative);
				return File.Exists(monoPath);
			} else {
				if (path.StartsWith(XamarinSourcePath, StringComparison.Ordinal))
					return false;
				var xamarinRuntimePath = XamarinSourcePath.Replace("/src/", "/runtime/");
				if (path.StartsWith(xamarinRuntimePath, StringComparison.Ordinal))
					return false;
				return path.StartsWith(MonoSourcePath, StringComparison.Ordinal);
			}
		}

		public bool IsOpenTKPath (string path)
		{
			if (path.StartsWith(InstallDir, StringComparison.Ordinal)) {
				// dealing with the jenkins paths
				var srcDir = Path.Combine (InstallDir, (InstallDir.Contains("Xamarin.iOS") ? "Xamarin.iOS" : "Xamarin.Mac"), "src");
				var relative = path.Remove (0, srcDir.Length);
				if (relative.StartsWith("/", StringComparison.Ordinal))
					relative = path.Remove (0, 1);
				var openTKPath = Path.Combine (OpenTKSourcePath, relative);
				return File.Exists(openTKPath);
			} else {
				return path.Contains(OpenTKSourcePath);
			}
		}
		
		public bool IsIgnored (string path)
		{
			return path.Contains ("/mcs/mcs/");
		}
		
		public IPathMangler GetMangler (string path)
		{
			if (IsIgnored(path))
				return null;

			if (IsOpenTKPath (path)) {
				return OpenTKSourceMangler;
			}
			
			if (IsMonoPath(path)) {
				return MonoPathMangler;
			}
			
			return XamarinSourcesPathMangler;
		}
	}
}
