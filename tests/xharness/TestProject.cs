using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xharness.Jenkins.TestTasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

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

		public TestProject AsTvOSProject ()
		{
			var clone = Clone ();
			clone.Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), System.IO.Path.GetFileNameWithoutExtension (Path) + "-tvos" + System.IO.Path.GetExtension (Path));
			return clone;
		}

		public TestProject AsWatchOSProject ()
		{
			var clone = Clone ();
			var fileName = System.IO.Path.GetFileNameWithoutExtension (Path);
			clone.Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (Path), fileName + (fileName.Contains("-watchos")?"":"-watchos") + System.IO.Path.GetExtension (Path));
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

		public bool IsMonotouch => Name.Contains ("monotouch");

		public bool IsBclxUnit => IsBclTest && (Name.Contains ("xUnit") || IsMscorlib);


		public bool IsMscorlib => Name.Contains ("mscorlib");


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

		internal async Task<TestProject> CreateCloneAsync (TestTask test)
		{
			var rv = Clone ();
			await rv.CreateCopyAsync (test);
			return rv;
		}

		internal async Task CreateCopyAsync (TestTask test = null)
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

	public class iOSTestProject : TestProject
	{
		public bool SkipiOSVariation;
		public bool SkipwatchOSVariation; // skip both
		public bool SkipwatchOSARM64_32Variation;
		public bool SkipwatchOS32Variation;
		public bool SkiptvOSVariation;
		public bool BuildOnly;

		public iOSTestProject ()
		{
		}

		public iOSTestProject (string path, bool isExecutableProject = true)
			: base (path, isExecutableProject)
		{
			Name = System.IO.Path.GetFileNameWithoutExtension (path);
		}

		public bool IsSupported (DevicePlatform devicePlatform, string productVersion)
		{
			if (MonoNativeInfo == null)
				return true;
			var min_version = MonoNativeHelper.GetMinimumOSVersion (devicePlatform, MonoNativeInfo.Flavor);
			return Version.Parse (productVersion) >= Version.Parse (min_version);
		}
	}

	[Flags]
	public enum MacFlavors {
		Modern = 1, // Xamarin.Mac/Modern app
		Full = 2, // Xamarin.Mac/Full app
		System = 4, // Xamarin.Mac/System app
		Console = 8, // Console executable
	}

	public class MacTestProject : TestProject
	{
		public MacFlavors TargetFrameworkFlavors;

		public bool GenerateFull => GenerateVariations && (TargetFrameworkFlavors & MacFlavors.Full) == MacFlavors.Full;
		public bool GenerateSystem => GenerateVariations && (TargetFrameworkFlavors & MacFlavors.System) == MacFlavors.System;

		public bool GenerateVariations {
			get {
				// If a bitwise combination of flavors, then we're generating variations
				return TargetFrameworkFlavors != MacFlavors.Modern && TargetFrameworkFlavors != MacFlavors.Full && TargetFrameworkFlavors != MacFlavors.System && TargetFrameworkFlavors != MacFlavors.Console;
			}
		}

		public string Platform = "x86";

		public MacTestProject () : base ()
		{
		}

		public MacTestProject (string path, bool isExecutableProject = true, MacFlavors targetFrameworkFlavor = MacFlavors.Full | MacFlavors.Modern) : base (path, isExecutableProject)
		{
			TargetFrameworkFlavors = targetFrameworkFlavor;
		}

		public override TestProject Clone()
		{
			var rv = (MacTestProject) base.Clone ();
			rv.TargetFrameworkFlavors = TargetFrameworkFlavors;
			rv.Platform = Platform;
			return rv;
		}

		public override string ToString ()
		{
			return base.ToString () + " (" + TargetFrameworkFlavors.ToString () + ")";
		}
	}
}

