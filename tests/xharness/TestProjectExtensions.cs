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

		public static TestProject AsWatchOSProject (this TestProject self)
		{
			var clone = self.Clone ();
			var fileName = Path.GetFileNameWithoutExtension (self.Path);
			clone.Path = Path.Combine (Path.GetDirectoryName (self.Path), Target.ProjectsDir, "watchos", fileName + (fileName.Contains ("-watchos") ? "" : "-watchos") + Path.GetExtension (self.Path));
			return clone;
		}

		public static TestProject AsTodayExtensionProject (this TestProject self)
		{
			var clone = self.Clone ();
			clone.Path = Path.Combine (Path.GetDirectoryName (self.Path), Target.ProjectsDir, "today", Path.GetFileNameWithoutExtension (self.Path) + "-today" + Path.GetExtension (self.Path));
			return clone;
		}

		// Get the referenced today extension project (if any)
		public static TestProject GetTodayExtension (this TestProject self)
		{
			var extensions = self.Xml.GetExtensionProjectReferences ().ToArray ();
			if (!extensions.Any ())
				return null;
			if (extensions.Count () != 1)
				throw new NotImplementedException ();
			return new TestProject (self.Label, Path.GetFullPath (Path.Combine (Path.GetDirectoryName (self.Path), Target.ProjectsDir, "today-extension", extensions.First ().Replace ('\\', '/'))));
		}
	}
}
