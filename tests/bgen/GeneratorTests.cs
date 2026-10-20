using System.IO;
using System.Linq;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

namespace GeneratorTests {
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class BGen {
		[Test]
		[TestCase (Profile.iOS)]
		public void ResponseFile (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CreateTemporaryBinding ("");
			bgen.ResponseFile = Path.Combine (Cache.CreateTemporaryDirectory (), "rspfile");

			var arguments = new List<string> ();
			var targetFramework = BGenTool.GetTargetFramework (profile);
			var tf = TargetFramework.Parse (targetFramework);
			arguments.Add ($"--baselib={Configuration.GetBaseLibrary (tf)}");
			arguments.Add ($"--attributelib={Configuration.GetBindingAttributePath (tf)}");
			arguments.AddRange (Directory.GetFiles (Configuration.DotNetBclDir, "*.dll").Select (v => $"-r:{v}"));
			arguments.Add ($"--target-framework={targetFramework}");

			File.WriteAllLines (bgen.ResponseFile, arguments.ToArray ());
			bgen.AssertExecute ("response file");
			bgen.AssertNoWarnings ();
		}
	}
}
