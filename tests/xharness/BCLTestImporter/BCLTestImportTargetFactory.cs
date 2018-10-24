using System;
using System.IO;
using System.Collections.Generic;
using BCLTestImporter;

namespace xharness.BCLTestImporter {
	// Class that is use as the connection between xharness and the BCLImporter
	// This class knows about both worlds, will get the required information, 
	// from xharness, that will then be passed to the importer to generate
	// the different projects, once those projects are generater, the factory
	// will return the iOS targets to be executed.
	public class BCLTestImportTargetFactory {
		public Harness Harness;
		BCLTestProjectGenerator projectGenerator;
		
		public BCLTestImportTargetFactory (Harness harness)
		{
			Harness = harness;
			var outputDir = Path.GetFullPath (Path.Combine (Harness.RootDirectory, "bcl-test", "BCLTests"));
			var projectTemplatePath = Path.Combine (outputDir, "BCLTests.csproj.in");
			var registerTypesTemplatePath = Path.Combine (outputDir, "RegisterType.cs.in");
			var plistTemplatePath = Path.Combine (outputDir, "Info.plist.in");

			projectGenerator = new BCLTestProjectGenerator (outputDir, Harness.MONO_PATH, projectTemplatePath, registerTypesTemplatePath, plistTemplatePath) {
				Override = true
			};
		}
		
		// generate all the different test targets.
		public List<iOSTestProject> GetBclTargets ()
		{
			var result = new List<iOSTestProject> ();
			// generate all projects, then create a new iOSTarget per project
			foreach (var (name, path, xunit) in projectGenerator.GenerateAllTestProjects ()) {
				var prefix = xunit ? "xUnit" : "NUnit";
				result.Add (new iOSTestProject (path) { Name = $"[{prefix}] Mono {name}", SkiptvOSVariation=true, SkipwatchOSVariation=true});
			}
			return result;
		}
	}
}
