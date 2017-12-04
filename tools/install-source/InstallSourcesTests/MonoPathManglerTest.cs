using System;
using NUnit.Framework;

using InstallSources;

namespace InstallSourcesTests
{
	[TestFixture]
	public class MonoPathManglerTest
	{
		MonoPathMangler mangler;
		string monoPath;
		string installDir;
		string destinationDir;

		[SetUp]
		public void SetUp ()
		{
			monoPath = "/Users/test/xamarin-macios/external/mono/";
			installDir = "/Users/test/xamarin-macios/_ios-build//Library/Frameworks/Xamarin.iOS.framework/Versions/git";
			destinationDir = "/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git";
			mangler = new MonoPathMangler { 
				InstallDir = installDir,
				MonoSourcePath = monoPath,
				DestinationDir = destinationDir,
			};
		}

		[TestCase ("/Users/test/xamarin-macios/external/mono/mcs/class/Mono.Data.Tds/Mono.Data.Tds.Protocol/TdsAsyncState.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/mcs/class/Mono.Security/Mono.Security.X509/X509StoreManager.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/mcs/class/dlr/Runtime/Microsoft.Scripting.Core/Actions/UpdateDelegates.Generated.cs")]
		public void TestGetSourcePath (string path)
		{
			Assert.AreEqual (path, mangler.GetSourcePath (path), "Failed getting path for '{0}'", path);
		}

		[TestCase ("/Users/test/xamarin-macios/external/mono/mcs/class/Mono.Data.Tds/Mono.Data.Tds.Protocol/TdsAsyncState.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/mcs/class/Mono.Security/Mono.Security.X509/X509StoreManager.cs")]
		[TestCase ("/Users/test/xamarin-macios/external/mono/mcs/class/dlr/Runtime/Microsoft.Scripting.Core/Actions/UpdateDelegates.Generated.cs")]
		public void TestGetTargetPath (string path)
		{
			var targetPath = mangler.GetTargetPath (path);
			Assert.IsFalse (targetPath.StartsWith (monoPath, StringComparison.InvariantCulture), "Path starts with the mono path.");
			Assert.IsTrue (targetPath.StartsWith (destinationDir, StringComparison.InvariantCulture), "Path does not start with the install dir");
			Assert.IsTrue (!targetPath.Contains ("mono"), "Path does contain 'mono'");
		}
	}
}
