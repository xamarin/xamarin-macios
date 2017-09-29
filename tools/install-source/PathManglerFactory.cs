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
		CoreFXPathMangler corefxPathMangler;

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
		
		CoreFXPathMangler CoreFXPathMangler {
			get {
				if (corefxPathMangler == null)
					corefxPathMangler = new CoreFXPathMangler() {
						InstallDir = InstallDir,
						MonoSourcePath = MonoSourcePath
					};
				return corefxPathMangler;
			}
		}

		public bool IsMonoPath (string path)
		{
			return path.StartsWith (MonoSourcePath, StringComparison.Ordinal)
				&& !path.Contains ("corefx");
		}

		public bool IsOpenTKPath (string path)
		{
			return path.Contains (OpenTKSourcePath);
		}
		
		public bool IsCoreFXPath (string path)
		{
			return path.StartsWith (MonoSourcePath, StringComparison.Ordinal)
				&& path.Contains ("corefx");
		}

		public IPathMangler GetMangler (string path)
		{
			if (IsMonoPath (path) && !IsCoreFXPath (path))
				return MonoPathMangler;

			if (IsCoreFXPath (path))
				return CoreFXPathMangler;

			if (IsOpenTKPath (path))
				return OpenTKSourceMangler;

			return XamarinSourcesPathMangler;
		}
	}
}
