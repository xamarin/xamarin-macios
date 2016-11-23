using System;
namespace xharness
{
	public class TestProject
	{
		public string Path;
		public bool IsExecutableProject;
		public bool GenerateVariations = true;

		public TestProject ()
		{
		}

		public TestProject (string path, bool isExecutableProject = true, bool generateVariations = true)
		{
			Path = path;
			IsExecutableProject = isExecutableProject;
			GenerateVariations = generateVariations;
		}
	}

	public class MacTestProject : TestProject
	{
		public bool SkipXMVariations;

		public MacTestProject () : base ()
		{
		}

		public MacTestProject (string path, bool isExecutableProject = true, bool generateVariations = true, bool skipXMVariations = false) : base (path, isExecutableProject, generateVariations)
		{
			SkipXMVariations = skipXMVariations;
		}
	}
}

