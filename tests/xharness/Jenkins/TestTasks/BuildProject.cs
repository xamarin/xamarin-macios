using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.Common.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
#nullable enable

namespace Xharness.Jenkins.TestTasks {
	public class BuildProject : BuildTool {
		public IResourceManager ResourceManager { get; set; }
		public IEnvManager EnvironmentManager { get; set; }
		public IEventLogger EventLogger { get; set; }
		readonly Func<string> msbuildPath;

		public string? SolutionPath { get; set; }

		public BuildProject (Func<string> msbuildPath, IProcessManager processManager, IResourceManager resourceManager, IEventLogger eventLogger, IEnvManager envManager) : base (processManager)
		{
			this.msbuildPath = msbuildPath ?? throw new ArgumentNullException (nameof (msbuildPath));
			ResourceManager = resourceManager ?? throw new ArgumentNullException (nameof (resourceManager));
			EventLogger = eventLogger ?? throw new ArgumentNullException (nameof (eventLogger));
			EnvironmentManager = envManager ?? throw new ArgumentNullException (nameof (envManager));
		}

		public bool SupportsParallelExecution {
			get {
				return false;
			}
		}

		static IEnumerable<string> GetNestedReferenceProjects (string csproj)
		{
			if (!File.Exists (csproj))
				throw new FileNotFoundException ("Could not find the project whose reference projects needed to be found.", csproj);
			var result = new List<string> ();
			var doc = new XmlDocument ();
			doc.Load (csproj.Replace ("\\", "/"));
			foreach (var referenceProject in doc.GetProjectReferences ()) {
				var fixPath = referenceProject.Replace ("\\", "/"); // do the replace in case we use win paths
				result.Add (fixPath);
				// get all possible references
				if (!Path.IsPathRooted (fixPath))
					fixPath = Path.Combine (Path.GetDirectoryName (csproj)!, fixPath);
				result.AddRange (GetNestedReferenceProjects (fixPath));
			}
			return result;
		}
	}
}
