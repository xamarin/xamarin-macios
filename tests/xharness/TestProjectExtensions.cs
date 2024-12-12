using System;
using System.IO;
using System.Linq;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

using Xharness.Targets;

namespace Xharness {
	// particular methods used within xamarin-macios
	public static class TestProjectExtensions {

		public static TestProject AsTvOSProject (this TestProject self)
		{
			var clone = self.Clone ();
			var suffix = string.Empty;
			if (self.IsDotNetProject)
				suffix = "-dotnet";
			clone.Path = Path.Combine (Path.GetDirectoryName (self.Path), Target.ProjectsDir, "tvos" + suffix, Path.GetFileNameWithoutExtension (self.Path) + "-tvos" + Path.GetExtension (self.Path));
			return clone;
		}
	}
}
