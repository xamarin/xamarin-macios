using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Xharness.Jenkins.TestTasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness {
	public class TestProject
	{
		XmlDocument xml;

		public string Path;
		public string SolutionPath;
		public string Name;
		public bool IsExecutableProject;
		public bool IsNUnitProject;
		public string [] Configurations;
		public Func<Task> Dependency;
		public string FailureMessage;
		public bool RestoreNugetsInProject;
		public string MTouchExtraArgs;
		public double TimeoutMultiplier = 1;

		public IEnumerable<TestProject> ProjectReferences;

		// Optional
		public MonoNativeInfo MonoNativeInfo { get; set; }

		public TestProject ()
		{
		}

		public TestProject (string path, bool isExecutableProject = true)
		{
			Path = path;
			IsExecutableProject = isExecutableProject;
		}

		public XmlDocument Xml {
			get {
				if (xml == null) {
					xml = new XmlDocument ();
					xml.LoadWithoutNetworkAccess (Path);
				}
				return xml;
			}
		}

		public virtual TestProject Clone ()
		{
			TestProject rv = (TestProject) Activator.CreateInstance (GetType ());
			rv.Path = Path;
			rv.IsExecutableProject = IsExecutableProject;
			rv.RestoreNugetsInProject = RestoreNugetsInProject;
			rv.Name = Name;
			rv.MTouchExtraArgs = MTouchExtraArgs;
			rv.TimeoutMultiplier = TimeoutMultiplier;
			return rv;
		}

		internal async Task<TestProject> CreateCloneAsync (AppleTestTask test)
		{
			var rv = Clone ();
			await rv.CreateCopyAsync (test);
			return rv;
		}

		internal async Task CreateCopyAsync (AppleTestTask test = null)
		{
			var directory = DirectoryUtilities.CreateTemporaryDirectory (test?.TestName ?? System.IO.Path.GetFileNameWithoutExtension (Path));
			Directory.CreateDirectory (directory);
			var original_path = Path;
			Path = System.IO.Path.Combine (directory, System.IO.Path.GetFileName (Path));

			await Task.Yield ();

			XmlDocument doc;
			doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (original_path);
			var original_name = System.IO.Path.GetFileName (original_path);
			if (original_name.Contains ("GuiUnit_NET") || original_name.Contains ("GuiUnit_xammac_mobile")) {
				// The GuiUnit project files writes stuff outside their project directory using relative paths,
				// but override that so that we don't end up with multiple cloned projects writing stuff to
				// the same location.
				doc.SetOutputPath ("bin\\$(Configuration)");
				doc.SetNode ("DocumentationFile", "bin\\$(Configuration)\\nunitlite.xml");
			}
			doc.ResolveAllPaths (original_path);

			var projectReferences = new List<TestProject> ();
			foreach (var pr in doc.GetProjectReferences ()) {
				var tp = new TestProject (pr.Replace ('\\', '/'));
				await tp.CreateCopyAsync (test);
				doc.SetProjectReferenceInclude (pr, tp.Path.Replace ('/', '\\'));
				projectReferences.Add (tp);
			}
			this.ProjectReferences = projectReferences;

			doc.Save (Path);
		}

		public override string ToString()
		{
			return Name;
		}
	}

}

