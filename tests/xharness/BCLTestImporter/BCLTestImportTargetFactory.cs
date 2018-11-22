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
			var projectTemplatePath = outputDir;
			var registerTypesTemplatePath = Path.Combine (outputDir, "RegisterType.cs.in");
			var plistTemplatePath = outputDir;

			projectGenerator = new BCLTestProjectGenerator (outputDir, Harness.MONO_PATH, projectTemplatePath, registerTypesTemplatePath, plistTemplatePath) {
				Override = true
			};
		}
		
		// generate all the different test targets.
		public List<iOSTestProject> GetiOSBclTargets ()
		{
			var result = new List<iOSTestProject> ();
			// generate all projects, then create a new iOSTarget per project
			foreach (var (name, path, xunit, platforms) in projectGenerator.GenerateAlliOSTestProjects ()) {
				var prefix = xunit ? "xUnit" : "NUnit";
				result.Add (new iOSTestProject (path) {
					Name = $"[{prefix}] Mono {name}",
					SkiptvOSVariation = !platforms.Contains (Platform.TvOS),
					SkipwatchOSVariation = !platforms.Contains (Platform.WatchOS),
					Dependency = async () => {
						var rv = await Harness.BuildBclTests ();
						if (!rv.Succeeded)
							throw new Exception ($"Failed to build BCL tests, exit code: {rv.ExitCode}. Check the harness log for more details.");
					}
				});
			}
			return result;
		}
		
		public List<MacTestProject> GetMacBclTargets ()
		{
			var result = new List<MacTestProject> ();
			foreach (var (name, path, xunit) in projectGenerator.GenerateAllMacTestProjects ()) {
				var prefix = "XamMac " + (xunit ? "xUnit" : "NUnit");
				result.Add (new MacTestProject (path) {
					Name = $"[{prefix}] Mono {name}",
				});
			}
			return result;
		}
	}
}
