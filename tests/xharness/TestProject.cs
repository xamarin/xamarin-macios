using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace xharness
{
	public class TestProject
	{
		XmlDocument xml;

		public string Path;
		public string Name;
		public bool IsExecutableProject;
		public bool GenerateVariations = true;

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
			return new TestProject ()
			{
				Path = Path,
				IsExecutableProject = IsExecutableProject,
				GenerateVariations = GenerateVariations,
			};
		}

		internal async Task<TestProject> CreateCloneAsync (TestTask test)
		{
			var rv = Clone ();
			await rv.CreateCopyAsync (test);
			return rv;
		}

		internal async Task CreateCopyAsync (TestTask test)
		{
			var directory = Xamarin.Cache.CreateTemporaryDirectory (test.TestName);
			Directory.CreateDirectory (directory);
			var original_path = Path;
			Path = System.IO.Path.Combine (directory, System.IO.Path.GetFileName (Path));

			await Task.Yield ();

			XmlDocument doc;
			doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (original_path);
			doc.ResolveAllPaths (original_path);

			foreach (var pr in doc.GetProjectReferences ()) {
				var tp = new TestProject (pr.Replace ('\\', '/'));
				await tp.CreateCopyAsync (test);
				doc.SetProjectReferenceInclude (pr, tp.Path.Replace ('/', '\\'));
			}

			doc.Save (Path);
		}
	}

	public class MacTestProject : TestProject
	{
		public bool SkipXMVariations;

		public MacTestProject () : base ()
		{
		}

		public MacTestProject (string path, bool isExecutableProject = true, bool generateVariations = true, bool skipXMVariations = false) : base (path, isExecutableProject, generateVariations)
		{
			SkipXMVariations = skipXMVariations;
		}
	}
}

