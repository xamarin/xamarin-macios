using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xamarin;

namespace xharness
{
	public class TestProject
	{
		XmlDocument xml;

		public string Path;
		public string SolutionPath;
		public string Name;
		public bool IsExecutableProject;
		public bool IsNUnitProject;
		public bool GenerateVariations = true;
		public string [] Configurations;
		public Func<Task> Dependency;
		public string FailureMessage;

		public IEnumerable<TestProject> ProjectReferences;

		public TestProject ()
		{
		}

		public TestProject (string path, bool isExecutableProject = true, bool generateVariations = true)
		{
			Path = path;
			IsExecutableProject = isExecutableProject;
			GenerateVariations = generateVariations;
		}

		public TestProject AsTvOSProject ()
		{
			var clone = Clone ();
			clone.Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), System.IO.Path.GetFileNameWithoutExtension (Path) + "-tvos" + System.IO.Path.GetExtension (Path));
			return clone;
		}

		public TestProject AsWatchOSProject ()
		{
			var clone = Clone ();
			clone.Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), System.IO.Path.GetFileNameWithoutExtension (Path) + "-watchos" + System.IO.Path.GetExtension (Path));
			return clone;
		}

		public TestProject AsTodayExtensionProject ()
		{
			var clone = Clone ();
			clone.Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), System.IO.Path.GetFileNameWithoutExtension (Path) + "-today" + System.IO.Path.GetExtension (Path));
			return clone;
		}

		// Get the referenced today extension project (if any)
		public TestProject GetTodayExtension ()
		{
			var extensions = Xml.GetExtensionProjectReferences ().ToArray ();
			if (!extensions.Any ())
				return null;
			if (extensions.Count () != 1)
				throw new NotImplementedException ();
			return new TestProject
			{
				Path = System.IO.Path.GetFullPath (System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), extensions.First ().Replace ('\\', '/'))),
			};
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

		public bool IsBclTest {
			get {
				return Path.Contains ("bcl-test");
			}
		}

		public virtual TestProject Clone ()
		{
			TestProject rv = (TestProject) Activator.CreateInstance (GetType ());
			rv.Path = Path;
			rv.IsExecutableProject = IsExecutableProject;
			rv.GenerateVariations = GenerateVariations;
			rv.Name = Name;
			return rv;
		}

		internal async Task<TestProject> CreateCloneAsync (TestTask test)
		{
			var rv = Clone ();
			await rv.CreateCopyAsync (test);
			return rv;
		}

		internal async Task CreateCopyAsync (TestTask test = null)
		{
			var directory = Xamarin.Cache.CreateTemporaryDirectory (test?.TestName ?? System.IO.Path.GetFileNameWithoutExtension (Path));
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

	public class iOSTestProject : TestProject
	{
		public bool SkipiOSVariation;
		public bool SkipwatchOSVariation;
		public bool SkiptvOSVariation;
		public bool BuildOnly;

		// Optional
		public BCLTestInfo BCLInfo { get; set; }

		public iOSTestProject ()
		{
		}

		public iOSTestProject (string path, bool isExecutableProject = true, bool generateVariations = true)
			: base (path, isExecutableProject, generateVariations)
		{
		}
	}

	public enum MacFlavors { All, Modern, Full, System, NonSystem }

	public class MacTestProject : TestProject
	{
		public MacFlavors TargetFrameworkFlavor;

		// Optional
		public MacBCLTestInfo BCLInfo { get; set; }

		public bool GenerateModern => TargetFrameworkFlavor == MacFlavors.All || TargetFrameworkFlavor == MacFlavors.NonSystem || TargetFrameworkFlavor == MacFlavors.Modern;
		public bool GenerateFull => TargetFrameworkFlavor == MacFlavors.All || TargetFrameworkFlavor == MacFlavors.NonSystem || TargetFrameworkFlavor == MacFlavors.Full;
		public bool GenerateSystem => TargetFrameworkFlavor == MacFlavors.All || TargetFrameworkFlavor == MacFlavors.System;

		public string Platform = "x86";

		public MacTestProject () : base ()
		{
		}

		public MacTestProject (string path, bool isExecutableProject = true, bool generateVariations = true, MacFlavors targetFrameworkFlavor = MacFlavors.NonSystem) : base (path, isExecutableProject, generateVariations)
		{
			TargetFrameworkFlavor = targetFrameworkFlavor;
		}

		public override TestProject Clone()
		{
			var rv = (MacTestProject) base.Clone ();
			rv.TargetFrameworkFlavor = TargetFrameworkFlavor;
			rv.BCLInfo = BCLInfo;
			rv.Platform = Platform;
			return rv;
		}
	}
}

