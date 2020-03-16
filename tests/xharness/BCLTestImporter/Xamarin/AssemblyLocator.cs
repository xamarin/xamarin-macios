using System;
using Xharness.BCLTestImporter.Templates;

namespace Xharness.BCLTestImporter.Xamarin {

	/// <summary>
	/// Implemenation of the assembly locator that will return the root path of the mono bcl artifact.
	/// </summary>
	public class AssemblyLocator : IAssemblyLocator {

		public string iOSMonoSDKPath { get; set; }
		public string MacMonoSDKPath { get; set; }

		public string GetAssembliesRootLocation (Platform platform)
		{
			switch (platform) {
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				// simply, try to find the dir with the pattern
				return iOSMonoSDKPath;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				return MacMonoSDKPath;
			default:
				return null;
			}
		}
	}
}
