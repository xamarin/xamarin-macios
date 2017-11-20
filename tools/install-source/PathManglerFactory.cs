using System;

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
			if (path.StartsWith(XamarinSourcePath, StringComparison.Ordinal))
				return false;
			var xamarinRuntimePath = XamarinSourcePath.Replace ("/src/", "/runtime/");
			if (path.StartsWith(xamarinRuntimePath, StringComparison.Ordinal))
				return false;
			return path.StartsWith(MonoSourcePath, StringComparison.Ordinal);
		}

		public bool IsOpenTKPath (string path)
		{
			return path.Contains (OpenTKSourcePath);
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
