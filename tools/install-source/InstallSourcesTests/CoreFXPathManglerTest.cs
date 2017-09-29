using System;
using System.IO;
using NUnit.Framework;

using InstallSources;

namespace InstallSourcesTests
{
	[TestFixture]
	public class CoreFXPathManglerTest
	{
		CoreFXPathMangler mangler;
		string monoPath;
		string installDir;

		[SetUp]
		public void SetUp ()
		{
			monoPath = "/Users/test/xamarin-macios/external/mono/";
			installDir = "/Users/test/xamarin-macios/_ios-build//Library/Frameworks/Xamarin.iOS.framework/Versions/git";
			mangler = new CoreFXPathMangler { 
				InstallDir = installDir,
				MonoSourcePath = monoPath 
			};
		}

		[TestCase ("/Users/test/xamarin-macios/external/mono/external/corefx/src/Microsoft.CSharp/src/Microsoft/CSharp/RuntimeBinder/ArgumentObject.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/external/corefx/src/Microsoft.CSharp/src/Microsoft/CSharp/RuntimeBinder/BinderHelper.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/external/corefx/src/Microsoft.CSharp/src/Microsoft/CSharp/RuntimeBinder/CSharpArgumentInfo.cs")]
		public void TestGetSourcePath (string path)
		{
			Assert.AreEqual (path, mangler.GetSourcePath (path), "Failed getting path for '{0}'", path);
		}

		[TestCase ("/Users/test/xamarin-macios/external/mono/external/corefx/src/Microsoft.CSharp/src/Microsoft/CSharp/RuntimeBinder/ArgumentObject.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/external/corefx/src/Microsoft.CSharp/src/Microsoft/CSharp/RuntimeBinder/BinderHelper.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/external/corefx/src/Microsoft.CSharp/src/Microsoft/CSharp/RuntimeBinder/CSharpArgumentInfo.cs")]
		public void TestGetTargetPath (string path)
		{
			var targetPath = mangler.GetTargetPath (path);
			Assert.IsFalse (targetPath.StartsWith (monoPath, StringComparison.InvariantCulture), "Path starts with the mono path.");
			Assert.IsTrue (targetPath.StartsWith (installDir, StringComparison.InvariantCulture), "Path does not start with the install dir");
			Assert.IsTrue (targetPath.Contains ("/src/mono/external/corefx"), "Path does not contain 'src'");
		}
	}
}
