using System;
using System.IO;
using System.Threading.Tasks;

namespace Xharness.BCLTestImporter.Templates {
	// interface that represent a project that is created from a template.
	// The interface should be able to generate a project that will later be
	// used by the AppRunner to execute tests.
	public interface ITemplatedProject {
		Stream GetProjectTemplate (Platform platform);
		Stream GetProjectTemplate (WatchAppType appType);
		Stream GetPlistTemplate (Platform platform);
		Stream GetPlistTemplate (WatchAppType appType);
		Stream GetRegisterTypeTemplate ();
		Task GenerateSource (string srcOuputPath);
	}
}
