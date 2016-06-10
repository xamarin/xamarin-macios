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


}

