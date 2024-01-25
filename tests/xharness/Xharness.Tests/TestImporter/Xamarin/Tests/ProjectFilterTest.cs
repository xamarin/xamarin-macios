using System.IO;
using System.Linq;

using Moq;

using NUnit.Framework;

using Xharness.TestImporter;
using Xharness.TestImporter.Xamarin;

namespace Xharness.Tests.TestImporter.Xamarin.Tests {

	[TestFixture]
	public class ProjectFilterTest {

		// some assemblies we want to use to make sure that the ignore files are working as expected
		static string [] assemblies = {
			// iOS, tvOS and watchOS assemblies
			"monotouch_System.Core_test.dll",
			"monotouch_System.Numerics_test.dll",
			"monotouch_System.Runtime.Serialization_test.dll",
			"monotouch_System.Transactions_test.dll",
			"monotouch_System.Xml_test.dll",
			"monotouch_System.Xml.Linq_test.dll",
			"monotouch_Mono.Security_test.dll",
			"monotouch_System.ComponentModel.DataAnnotations_test.dll",
			"monotouch_System.Json_test.dll",
			"monotouch_System.ServiceModel.Web_test.dll",
			"monotouch_System.IO.Compression_test.dll",
			"monotouch_System.IO.Compression.FileSystem_test.dll",
			"monotouch_Mono.CSharp_test.dll",
			"monotouch_System.Security_test.dll",
			"monotouch_Mono.Data.Sqlite_test.dll",
			"monotouch_Mono.Runtime.Tests_test.dll",
			// mac os x dlls
			"xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll",
			"xammac_net_4_5_System.Net.Http.WebRequest_test.dll",
			"xammac_net_4_5_System.Messaging_test.dll",
			"xammac_net_4_5_System.IdentityModel_test.dll",
			"xammac_net_4_5_System.Data.Linq_test.dll",
			"xammac_net_4_5_Mono.Posix_test.dll",
			"xammac_net_4_5_Mono.Messaging_test.dll",
			"xammac_net_4_5_System.Data_test.dll",
			"xammac_net_4_5_System.Configuration_test.dll",
		};
		static string [] traitsFiles = {
			 "nunit-excludes.txt",
			 "xunit-excludes.txt",
		};
		string ignoreFilesRootDir;
		string traitsFilesRootDir;
		Mock<IAssemblyLocator> assemblyLocator;
		ProjectFilter projectFilter;

		string GetTraitExpectedPath (Platform platform) => platform switch {
			Platform.iOS => Path.Combine (traitsFilesRootDir, "ios-bcl", "monotouch", "tests"),
			Platform.TvOS => Path.Combine (traitsFilesRootDir, "ios-bcl", "monotouch_tv", "tests"),
			Platform.WatchOS => Path.Combine (traitsFilesRootDir, "ios-bcl", "monotouch_watch", "tests"),
			Platform.MacOSFull => Path.Combine (traitsFilesRootDir, "mac-bcl", "xammac_net_4_5", "tests"),
			Platform.MacOSModern => Path.Combine (traitsFilesRootDir, "mac-bcl", "xammac", "tests"),
			_ => null,
		};

		void BuildIgnoreDir (out string ignoreRootDir, params string [] assemblies)
		{
			ignoreRootDir = Path.GetTempFileName ();
			File.Delete (ignoreFilesRootDir);
			Directory.CreateDirectory (ignoreFilesRootDir);
			// create a few ignore files
			foreach (var a in assemblies) {
				File.WriteAllText (Path.Combine (ignoreRootDir, $"iOS-{a}.ignore"), string.Empty);
				File.WriteAllText (Path.Combine (ignoreRootDir, $"macOSFull-{a}.ignore"), string.Empty);
				File.WriteAllText (Path.Combine (ignoreRootDir, $"macOS-{a}.ignore"), string.Empty);
				File.WriteAllText (Path.Combine (ignoreRootDir, $"tvOS-{a}.ignore"), string.Empty);
				File.WriteAllText (Path.Combine (ignoreRootDir, $"watchOS-{a}.ignore"), string.Empty);
			}
		}

		void BuildTraitsDir (out string traitsRootDir)
		{
			traitsRootDir = Path.GetTempFileName ();
			File.Delete (traitsRootDir);
			Directory.CreateDirectory (traitsRootDir);
			// create a few ignore files
			foreach (var t in traitsFiles)
				File.WriteAllText (Path.Combine (traitsRootDir, t), string.Empty);
		}

		[SetUp]
		public void SetUp ()
		{
			// build some data dirs so that we can ensure that the project filter does return the correct
			// ignore files
			BuildIgnoreDir (out ignoreFilesRootDir);
			BuildTraitsDir (out traitsFilesRootDir);

			assemblyLocator = new Mock<IAssemblyLocator> ();
			projectFilter = new ProjectFilter {
				AssemblyLocator = assemblyLocator.Object,
				IgnoreFilesRootDir = ignoreFilesRootDir,
			};
		}

		[TearDown]
		public void TearDown ()
		{
			if (Directory.Exists (ignoreFilesRootDir))
				Directory.Delete (ignoreFilesRootDir, true);
			if (Directory.Exists (traitsFilesRootDir))
				Directory.Delete (traitsFilesRootDir, true);

		}

		[Test]
		public void ExludeDllTest ()
		{
			// loop over the assemblies we know should be ignored per platform and assert the correct value is returned
			foreach (var a in ProjectFilter.CommonIgnoredAssemblies) {
				foreach (var p in new [] { Platform.iOS, Platform.TvOS, Platform.WatchOS, Platform.MacOSFull, Platform.MacOSModern })
					Assert.IsTrue (projectFilter.ExcludeDll (p, a), $"Common {p} {a}");
			}
			foreach (var a in ProjectFilter.iOSIgnoredAssemblies)
				Assert.IsTrue (projectFilter.ExcludeDll (Platform.iOS, a), $"{Platform.iOS} {a}");
			foreach (var a in ProjectFilter.tvOSIgnoredAssemblies)
				Assert.IsTrue (projectFilter.ExcludeDll (Platform.TvOS, a), $"{Platform.TvOS} {a}");
			foreach (var a in ProjectFilter.watcOSIgnoredAssemblies)
				Assert.IsTrue (projectFilter.ExcludeDll (Platform.WatchOS, a), $"{Platform.WatchOS} {a}");
			foreach (var (assembly, platforms) in ProjectFilter.macIgnoredAssemblies) {
				foreach (var p in platforms)
					Assert.IsTrue (projectFilter.ExcludeDll (p, assembly), $"{p} {assembly}");
			}
		}

		[Test]
		public void ExcludeProjectTest ()
		{

		}

		[Test]
		public void GetIgnoreFilesTest ()
		{

		}

		[Test]
		public void GetTraitFiles ()
		{
			assemblyLocator.Setup (l => l.GetAssembliesRootLocation (It.IsAny<Platform> ())).Returns (traitsFilesRootDir);
			// ensure that we do get the traits from the correct path
			foreach (var p in new [] { Platform.iOS, Platform.MacOSFull, Platform.MacOSModern, Platform.TvOS, Platform.WatchOS }) {
				var rootPath = GetTraitExpectedPath (p);
				var files = projectFilter.GetTraitsFiles (p);
				Assert.AreEqual (2, files.Count (), "files count");
				foreach (var f in projectFilter.GetTraitsFiles (p)) {
					StringAssert.StartsWith (rootPath, f, "full path");
					Assert.IsTrue (f.EndsWith ("nunit-excludes.txt") || f.EndsWith ("xunit-excludes.txt"), "filename");
				}
			}
		}
	}
}
