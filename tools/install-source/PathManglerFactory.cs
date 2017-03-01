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
			return path.StartsWith (MonoSourcePath, StringComparison.Ordinal);
		}

		public bool IsOpenTKPath (string path)
		{
			return path.Contains (OpenTKSourcePath);
		}

		public IPathMangler GetMangler (string path)
		{
			if (IsMonoPath (path))
				return MonoPathMangler;

			if (IsOpenTKPath (path))
				return OpenTKSourceMangler;

			return XamarinSourcesPathMangler;
		}
	}
}
