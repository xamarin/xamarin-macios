using System;
namespace xharness
{
	public class TestProject
	{
		public string Path;
		public bool IsExecutableProject;

		public TestProject ()
		{
		}

		public TestProject (string path, bool isExecutableProject = true)
		{
			Path = path;
			IsExecutableProject = isExecutableProject;
		}
	}


}

