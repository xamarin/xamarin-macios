#nullable enable

using System.IO.Compression;

namespace Xamarin.Tests {
	public class PackTest : TestBaseClass {

		// Create a temporary directory for OutputPath
		// Uses a path relative to the current directory on Windows (as opposed to an absolute path),
		// because otherwise we run into limitations in our build where building with an absolute
		// OutputPath doesn't work.
		static string CreateTemporaryDirectoryForOutputPath ()
		{
			string tmpdir;
			if (Configuration.IsBuildingRemotely) {
				int counter = 0;
				do {
					tmpdir = Path.Combine ("bin", $"tmp-dir-{++counter}");
					if (counter > 10000)
						throw new InvalidOperationException ("Too many temporary directories");
				} while (Directory.Exists (tmpdir));
			} else {
				tmpdir = Cache.CreateTemporaryDirectory ();
			}
			return tmpdir;
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[Category ("WindowsInclusive")]
		public void BindingOldStyle (ApplePlatform platform)
		{
			BindingOldStyleImpl (platform);
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[Category ("RemoteWindows")]
		public void BindingOldStyleOnRemoteWindows (ApplePlatform platform)
		{
			Configuration.IgnoreIfNotOnWindows ();
			BindingOldStyleImpl (platform, AddRemoteProperties ());
		}

		void BindingOldStyleImpl (ApplePlatform platform, Dictionary<string, string>? properties = null)
		{
			var project = "BindingOldStyle";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);

			var tmpdir = CreateTemporaryDirectoryForOutputPath ();
			var outputPath = Path.Combine (tmpdir, "OutputPath");
			var intermediateOutputPath = Path.Combine (tmpdir, "IntermediateOutputPath");
			properties = GetDefaultProperties (extraProperties: properties);
			properties ["OutputPath"] = outputPath + Path.DirectorySeparatorChar;
			properties ["IntermediateOutputPath"] = intermediateOutputPath + Path.DirectorySeparatorChar;

			var rv = DotNet.AssertPackFailure (project_path, properties, msbuildParallelism: false);
			var errors = BinLog.GetBuildLogErrors (rv.BinLogPath).ToArray ();
			Assert.That (errors.Length, Is.EqualTo (1), "Error count");
			Assert.That (errors [0].Message, Is.EqualTo ($"Creating a NuGet package is not supported for projects that have ObjcBindingNativeLibrary items. Migrate to use NativeReference items instead."), "Error message");
		}

		[Test]
		[Category ("Multiplatform")]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		// The bindings-framework-test project doesn't work on iOS or tvOS.
		public void BindingFrameworksProject (ApplePlatform platform, bool noBindingEmbedding)
		{
			var project = "bindings-framework-test";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = Path.Combine (Configuration.RootPath, "tests", project, "dotnet", platform.AsString (), $"{project}.csproj");
			Clean (project_path);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var outputPath = Path.Combine (tmpdir, "OutputPath");
			var intermediateOutputPath = Path.Combine (tmpdir, "IntermediateOutputPath");
			var properties = GetDefaultProperties ();
			properties ["OutputPath"] = outputPath + Path.DirectorySeparatorChar;
			properties ["IntermediateOutputPath"] = intermediateOutputPath + Path.DirectorySeparatorChar;
			properties ["NoBindingEmbedding"] = noBindingEmbedding ? "true" : "false";

			DotNet.AssertPack (project_path, properties, msbuildParallelism: false);

			var nupkg = Path.Combine (outputPath, project + ".1.0.0.nupkg");
			Assert.That (nupkg, Does.Exist, "nupkg existence");

			using var archive = ZipFile.OpenRead (nupkg);
			var files = archive.Entries.Select (v => v.FullName).ToHashSet ();
			var tfm = platform.ToFrameworkWithPlatformVersion (isExecutable: false);
			var hasSymlinks = noBindingEmbedding && (platform == ApplePlatform.MacCatalyst || platform == ApplePlatform.MacOSX);
			if (noBindingEmbedding) {
				Assert.That (archive.Entries.Count, Is.EqualTo (hasSymlinks ? 6 : 10), $"nupkg file count - {nupkg}");
			} else {
				Assert.That (archive.Entries.Count, Is.EqualTo (5), $"nupkg file count - {nupkg}");
			}
			Assert.That (files, Does.Contain (project + ".nuspec"), "nuspec");
			Assert.That (files, Does.Contain ("_rels/.rels"), ".rels");
			Assert.That (files, Does.Contain ("[Content_Types].xml"), "[Content_Types].xml");
			Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.dll"), $"{project}.dll");
			Assert.That (files, Has.Some.Matches<string> (v => v.StartsWith ("package/services/metadata/core-properties/", StringComparison.Ordinal) && v.EndsWith (".psmdcp", StringComparison.Ordinal)), "psmdcp");
			if (noBindingEmbedding) {
				if (hasSymlinks) {
					Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources.zip"), $"{project}.resources.zip");
				} else {
					Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources/XStaticArTest.framework/XStaticArTest"), $"XStaticArTest.framework/XStaticArTest");
					Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources/XStaticObjectTest.framework/XStaticObjectTest"), $"XStaticObjectTest.framework/XStaticObjectTest");
					Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources/XTest.framework/XTest"), $"XTest.framework/XTest");
					Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources/XTest.framework/Info.plist"), $"XTest.framework/Info.plist");
					Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources/manifest"), $"manifest");
				}
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		public void BindingMergeableFrameworksProject (ApplePlatform platform, bool noBindingEmbedding)
		{
			var project = "bindings-framework-test-mergeable";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = Path.Combine (Configuration.RootPath, "tests", "mergeable-framework-test", platform.AsString (), $"{project}.csproj");
			Clean (project_path);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var outputPath = Path.Combine (tmpdir, "OutputPath");
			var intermediateOutputPath = Path.Combine (tmpdir, "IntermediateOutputPath");
			var properties = GetDefaultProperties ();
			properties ["OutputPath"] = outputPath + Path.DirectorySeparatorChar;
			properties ["IntermediateOutputPath"] = intermediateOutputPath + Path.DirectorySeparatorChar;
			properties ["NoBindingEmbedding"] = noBindingEmbedding ? "true" : "false";

			DotNet.AssertPack (project_path, properties, msbuildParallelism: false);

			var nupkg = Path.Combine (outputPath, project + ".1.0.0.nupkg");
			Assert.That (nupkg, Does.Exist, "nupkg existence");

			using var archive = ZipFile.OpenRead (nupkg);
			var files = archive.Entries.Select (v => v.FullName).ToHashSet ();
			var tfm = platform.ToFrameworkWithPlatformVersion (isExecutable: false);
			Assert.That (files, Does.Contain (project + ".nuspec"), "nuspec");
			Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.dll"), $"{project}.dll");
			if (noBindingEmbedding) {
				// The XMergeableTest framework is always packaged as a .resources.zip
				Assert.That (files, Does.Contain ($"lib/{tfm}/{project}.resources.zip"), $"{project}.resources.zip");
			}
		}

		[Test]
		[Category ("Multiplatform")]
		[TestCase (ApplePlatform.iOS, true, true, true)]
		[TestCase (ApplePlatform.iOS, true, true, false)]
		[TestCase (ApplePlatform.iOS, true, false, true)]
		[TestCase (ApplePlatform.iOS, true, false, false)]
		[TestCase (ApplePlatform.iOS, false, false, false)]
		[TestCase (ApplePlatform.MacCatalyst, true, true, true)]
		[TestCase (ApplePlatform.MacCatalyst, true, true, false)]
		[TestCase (ApplePlatform.MacCatalyst, true, false, true)]
		[TestCase (ApplePlatform.MacCatalyst, true, false, false)]
		[TestCase (ApplePlatform.MacCatalyst, false, false, false)]
		[TestCase (ApplePlatform.TVOS, true, true, true)]
		[TestCase (ApplePlatform.TVOS, false, false, false)]
		[TestCase (ApplePlatform.MacOSX, true, false, false)]
		[TestCase (ApplePlatform.MacOSX, false, false, false)]
		public void BindingXcFrameworksProject (ApplePlatform platform, bool noBindingEmbedding, bool platformSpecificXcframework, bool compressedXcframework)
		{
			BindingXcFrameworksProjectImpl (platform, noBindingEmbedding, platformSpecificXcframework, compressedXcframework);
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true, true, true)]
		[TestCase (ApplePlatform.iOS, true, true, false)]
		[TestCase (ApplePlatform.iOS, true, false, true)]
		[TestCase (ApplePlatform.iOS, false, false, false)]
		[TestCase (ApplePlatform.MacCatalyst, true, true, true)]
		[TestCase (ApplePlatform.MacCatalyst, true, false, true)]
		[TestCase (ApplePlatform.MacCatalyst, false, false, false)]
		[TestCase (ApplePlatform.TVOS, true, true, true)]
		[TestCase (ApplePlatform.TVOS, false, false, false)]
		[TestCase (ApplePlatform.MacOSX, true, false, true)]
		[TestCase (ApplePlatform.MacOSX, false, false, false)]
		[Category ("Windows")]
		public void BindingXcFrameworksProjectOnWindows (ApplePlatform platform, bool noBindingEmbedding, bool platformSpecificXcframework, bool compressedXcframework)
		{
			Configuration.IgnoreIfNotOnWindows ();
			BindingXcFrameworksProjectImpl (platform, noBindingEmbedding, platformSpecificXcframework, compressedXcframework);
		}

		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, true, true, true)]
		[TestCase (ApplePlatform.iOS, true, true, false)]
		[TestCase (ApplePlatform.iOS, true, false, true)]
		[TestCase (ApplePlatform.iOS, false, false, false)]
		public void BindingXcFrameworksProjectOnRemoteWindows (ApplePlatform platform, bool noBindingEmbedding, bool platformSpecificXcframework, bool compressedXcframework)
		{
			Configuration.IgnoreIfNotOnWindows ();
			BindingXcFrameworksProjectImpl (platform, noBindingEmbedding, platformSpecificXcframework, compressedXcframework, AddRemoteProperties ());
		}

		[Category ("RemoteWindows")]
		[TestCase (ApplePlatform.iOS, true, true, true)]
		[TestCase (ApplePlatform.iOS, true, true, false)]
		[TestCase (ApplePlatform.iOS, true, false, true)]
		[TestCase (ApplePlatform.iOS, false, false, false)]
		public void BindingXcFrameworksProjectOnRemoteWindowsUsingFallback (ApplePlatform platform, bool noBindingEmbedding, bool platformSpecificXcframework, bool compressedXcframework)
		{
			Configuration.IgnoreIfNotOnWindows ();
			var properties = AddRemoteProperties ();
			properties ["BuildBindingProjectLocally"] = "false";
			BindingXcFrameworksProjectImpl (platform, noBindingEmbedding, platformSpecificXcframework, compressedXcframework, properties);
		}

		void BindingXcFrameworksProjectImpl (ApplePlatform platform, bool noBindingEmbedding, bool platformSpecificXcframework, bool compressedXcframework, Dictionary<string, string>? properties = null)
		{
			var project = "bindings-xcframework-test";
			var assemblyName = "bindings-framework-test";

			if (!noBindingEmbedding) {
				Assert.That (platformSpecificXcframework, Is.False, "Invalid test variation: platformSpecificXcframework");
				Assert.That (compressedXcframework, Is.False, "Invalid test variation: compressedXcframework");
			}

			if (!platformSpecificXcframework) {
				// This tests gets really complicated if not all platforms are included,
				// because the (number of) files included in the nupkg depends not only
				// on the current platform, but on the other included platforms as well.
				// For example: if either macOS or Mac Catalyst is included, then some
				// parts of the .xcframework will be zipped differently (due to symlinks
				// in the xcframework).
				Configuration.IgnoreIfAnyIgnoredPlatforms ();
			} else {
				Configuration.IgnoreIfIgnoredPlatform (platform);
			}

			var project_path = Path.Combine (Configuration.RootPath, "tests", project, "dotnet", platform.AsString (), $"{project}.csproj");
			Clean (project_path);

			var tmpdir = CreateTemporaryDirectoryForOutputPath ();
			var outputPath = Path.Combine (tmpdir, "OutputPath");
			var intermediateOutputPath = Path.Combine (tmpdir, "IntermediateOutputPath");
			properties = GetDefaultProperties (extraProperties: properties);
			properties ["OutputPath"] = outputPath + Path.DirectorySeparatorChar;
			properties ["IntermediateOutputPath"] = intermediateOutputPath + Path.DirectorySeparatorChar;
			properties ["NoBindingEmbedding"] = noBindingEmbedding ? "true" : "false";
			if (platformSpecificXcframework)
				properties ["UsePlatformSpecificXcframework"] = "true";
			if (compressedXcframework)
				properties ["UseZippedXcframework"] = "true";

			DotNet.AssertPack (project_path, properties, msbuildParallelism: false);

			string nupkg;
			if (Configuration.IsBuildingRemotely) {
				nupkg = Path.Combine (Path.GetDirectoryName (project_path)!, outputPath, assemblyName + ".1.0.0.nupkg");
			} else {
				nupkg = Path.Combine (outputPath, assemblyName + ".1.0.0.nupkg");
			}
			Assert.That (nupkg, Does.Exist, "nupkg existence");

			using var archive = ZipFile.OpenRead (nupkg);
			var files = archive.Entries.Select (v => v.FullName).ToHashSet ();
			var tfm = platform.ToFrameworkWithPlatformVersion (isExecutable: false);

			var expectedZipFiles = new List<string> () {
				$"{assemblyName}.nuspec",
				"_rels/.rels",
				"[Content_Types].xml",
				$"lib/{tfm}/{assemblyName}.dll",
			};
			expectedZipFiles.Add (files.Where (v => v.StartsWith ("package/services/metadata/core-properties/", StringComparison.Ordinal) && v.EndsWith (".psmdcp", StringComparison.Ordinal)).Single ());

			if (noBindingEmbedding) {
				bool isCompressedBindingPackage;
				if (compressedXcframework) {
					isCompressedBindingPackage = false; // there are no symlinks if we only have zip files, so binding package isn't compressed
				} else {
					isCompressedBindingPackage = UsesCompressedBindingResourcePackage (platform);
				}

				if (isCompressedBindingPackage) {
					expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources.zip");
				} else {
					expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/manifest");
					if (compressedXcframework) {
						expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework.zip");
						expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework.zip");
						expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework.zip");
					} else {
						expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/Info.plist");
						expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/Info.plist");
						expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/Info.plist");
						if (!platformSpecificXcframework || platform == ApplePlatform.iOS) {
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/ios-arm64/XStaticArTest.framework/XStaticArTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/ios-arm64-simulator/XStaticArTest.framework/XStaticArTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/ios-arm64/XStaticObjectTest.framework/XStaticObjectTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/ios-arm64-simulator/XStaticObjectTest.framework/XStaticObjectTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64/XTest.framework/Info.plist");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64/XTest.framework/XTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64-simulator/XTest.framework/Info.plist");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64-simulator/XTest.framework/XTest");
						}
						if (!platformSpecificXcframework || platform == ApplePlatform.MacCatalyst) {
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/ios-arm64_x86_64-maccatalyst/XStaticArTest.framework/XStaticArTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/ios-arm64_x86_64-maccatalyst/XStaticObjectTest.framework/XStaticObjectTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Resources");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/XTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/A/Resources/Info.plist");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/A/XTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/ios-arm64_x86_64-maccatalyst/XTest.framework/Versions/Current");
						}
						if (!platformSpecificXcframework || platform == ApplePlatform.MacOSX) {
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/macos-arm64_x86_64/XStaticArTest.framework/XStaticArTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/macos-arm64_x86_64/XStaticObjectTest.framework/XStaticObjectTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/macos-arm64_x86_64/XTest.framework/Resources");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/macos-arm64_x86_64/XTest.framework/XTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/A/Resources/Info.plist");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/A/XTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/macos-arm64_x86_64/XTest.framework/Versions/Current");
						}
						if (!platformSpecificXcframework || platform == ApplePlatform.TVOS) {
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/tvos-arm64/XStaticArTest.framework/XStaticArTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework/tvos-arm64-simulator/XStaticArTest.framework/XStaticArTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/tvos-arm64/XStaticObjectTest.framework/XStaticObjectTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework/tvos-arm64-simulator/XStaticObjectTest.framework/XStaticObjectTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/tvos-arm64/XTest.framework/Info.plist");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/tvos-arm64/XTest.framework/XTest");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/tvos-arm64-simulator/XTest.framework/Info.plist");
							expectedZipFiles.Add ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework/tvos-arm64-simulator/XTest.framework/XTest");
						}
					}
				}
			}

			expectedZipFiles.Sort ();

			Assert.That (files.OrderBy (v => v), Is.EqualTo (expectedZipFiles), "nupkg");
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[Category ("RemoteWindows")]
		public void BindingCompressedXcFrameworksProjectOnRemoteWindows (ApplePlatform platform, bool compressed)
		{
			Configuration.IgnoreIfNotOnWindows ();
			BindingCompressedXcFrameworksProjectImpl (platform, compressed, AddRemoteProperties ());
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[Category ("WindowsInclusive")]
		public void BindingCompressedXcFrameworksProject (ApplePlatform platform, bool compressed)
		{
			BindingCompressedXcFrameworksProjectImpl (platform, compressed);
		}

		void BindingCompressedXcFrameworksProjectImpl (ApplePlatform platform, bool compressed, Dictionary<string, string>? properties = null)
		{
			var project = "BindingWithCompressedXCFramework";
			var assemblyName = project;
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);

			var tmpdir = CreateTemporaryDirectoryForOutputPath ();
			var outputPath = Path.Combine (tmpdir, "OutputPath");
			var intermediateOutputPath = Path.Combine (tmpdir, "IntermediateOutputPath");
			properties = GetDefaultProperties (extraProperties: properties);
			properties ["OutputPath"] = outputPath + Path.DirectorySeparatorChar;
			properties ["IntermediateOutputPath"] = intermediateOutputPath + Path.DirectorySeparatorChar;
			properties ["CompressBindingResourcePackage"] = compressed ? "true" : "false";

			DotNet.AssertPack (project_path, properties, msbuildParallelism: false);

			string nupkg;
			if (Configuration.IsBuildingRemotely) {
				nupkg = Path.Combine (Path.GetDirectoryName (project_path)!, outputPath, assemblyName + ".1.0.0.nupkg");
			} else {
				nupkg = Path.Combine (outputPath, assemblyName + ".1.0.0.nupkg");
			}
			Assert.That (nupkg, Does.Exist, "nupkg existence");

			using var archive = ZipFile.OpenRead (nupkg);
			var files = archive.Entries.Select (v => v.FullName).ToHashSet ();
			var tfm = platform.ToFrameworkWithPlatformVersion (isExecutable: false);
			Assert.That (archive.Entries.Count, Is.EqualTo (compressed ? 6 : 9), $"nupkg file count - {nupkg}");
			Assert.That (files, Does.Contain (assemblyName + ".nuspec"), "nuspec");
			Assert.That (files, Does.Contain ("_rels/.rels"), ".rels");
			Assert.That (files, Does.Contain ("[Content_Types].xml"), "[Content_Types].xml");
			Assert.That (files, Does.Contain ($"lib/{tfm}/{assemblyName}.dll"), $"{assemblyName}.dll");
			Assert.That (files, Has.Some.Matches<string> (v => v.StartsWith ("package/services/metadata/core-properties/", StringComparison.Ordinal) && v.EndsWith (".psmdcp", StringComparison.Ordinal)), "psmdcp");
			string? manifest;
			if (compressed) {
				var resourcesZip = $"lib/{tfm}/{assemblyName}.resources.zip";
				Assert.That (files, Does.Contain (resourcesZip), $"{assemblyName}.resources.zip");
				var innerZip = ZipHelpers.ListInnerZip (nupkg, resourcesZip);
				var innerZipContents = new string [] {
					"manifest",
					"XTest.xcframework.zip",
					"XStaticArTest.xcframework.zip",
					"XStaticObjectTest.xcframework.zip",
				};
				Assert.That (innerZip.OrderBy (v => v), Is.EqualTo (innerZipContents.OrderBy (v => v)), "Inner zip");
				manifest = ZipHelpers.GetInnerString (nupkg, resourcesZip, "manifest");
			} else {
				Assert.That (files, Does.Contain ($"lib/{tfm}/{assemblyName}.resources/manifest"), $"manifest");
				Assert.That (files, Does.Contain ($"lib/{tfm}/{assemblyName}.resources/XTest.xcframework.zip"), $"XTest.xcframework.zip");
				Assert.That (files, Does.Contain ($"lib/{tfm}/{assemblyName}.resources/XStaticArTest.xcframework.zip"), $"XStaticArTest.xcframework.zip");
				Assert.That (files, Does.Contain ($"lib/{tfm}/{assemblyName}.resources/XStaticObjectTest.xcframework.zip"), $"XStaticObjectTest.xcframework.zip");
				manifest = ZipHelpers.GetString (nupkg, $"lib/{tfm}/{assemblyName}.resources/manifest");
			}
			var expectedManifest = $"""
			<BindingAssembly>
				<NativeReference Name="XTest.xcframework.zip">
					<ForceLoad></ForceLoad>
					<Frameworks></Frameworks>
					<IdentityWithoutPathSeparatorSuffix>../../../test-libraries/.libs/XTest.xcframework.zip</IdentityWithoutPathSeparatorSuffix>
					<IsCxx></IsCxx>
					<Kind>Framework</Kind>
					<LinkerFlags></LinkerFlags>
					<LinkWithSwiftSystemLibraries></LinkWithSwiftSystemLibraries>
					<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
					<SmartLink></SmartLink>
					<WeakFrameworks></WeakFrameworks>
				</NativeReference>
				<NativeReference Name="XStaticArTest.xcframework.zip">
					<ForceLoad></ForceLoad>
					<Frameworks>CoreLocation ModelIO</Frameworks>
					<IdentityWithoutPathSeparatorSuffix>../../../test-libraries/.libs/XStaticArTest.xcframework.zip</IdentityWithoutPathSeparatorSuffix>
					<IsCxx></IsCxx>
					<Kind>Static</Kind>
					<LinkerFlags></LinkerFlags>
					<LinkWithSwiftSystemLibraries></LinkWithSwiftSystemLibraries>
					<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
					<SmartLink></SmartLink>
					<WeakFrameworks></WeakFrameworks>
				</NativeReference>
				<NativeReference Name="XStaticObjectTest.xcframework.zip">
					<ForceLoad></ForceLoad>
					<Frameworks>CoreLocation ModelIO</Frameworks>
					<IdentityWithoutPathSeparatorSuffix>../../../test-libraries/.libs/XStaticObjectTest.xcframework.zip</IdentityWithoutPathSeparatorSuffix>
					<IsCxx></IsCxx>
					<Kind>Static</Kind>
					<LinkerFlags></LinkerFlags>
					<LinkWithSwiftSystemLibraries></LinkWithSwiftSystemLibraries>
					<NeedsGccExceptionHandling></NeedsGccExceptionHandling>
					<SmartLink></SmartLink>
					<WeakFrameworks></WeakFrameworks>
				</NativeReference>
			</BindingAssembly>
			""";
			Assert.That (manifest, Is.EqualTo (expectedManifest), "manifest contents");
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		[Category ("WindowsInclusive")]
		public void LibraryProject (ApplePlatform platform)
		{
			LibraryProjectImpl (platform);
		}

		[TestCase (ApplePlatform.iOS)]
		[Category ("RemoteWindows")]
		public void LibraryProjectOnRemoteWindows (ApplePlatform platform)
		{
			Configuration.IgnoreIfNotOnWindows ();
			LibraryProjectImpl (platform, AddRemoteProperties ());
		}

		void LibraryProjectImpl (ApplePlatform platform, Dictionary<string, string>? properties = null)
		{
			var project = "MyClassLibrary";
			var configuration = "Release";
			Configuration.IgnoreIfIgnoredPlatform (platform);

			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);
			properties = GetDefaultProperties (extraProperties: properties);

			DotNet.AssertPack (project_path, properties);

			var nupkg = Path.Combine (Path.GetDirectoryName (project_path)!, "bin", configuration, project + ".1.0.0.nupkg");
			Assert.That (nupkg, Does.Exist, "nupkg existence");

			using var archive = ZipFile.OpenRead (nupkg);
			var files = archive.Entries.Select (v => v.FullName).ToHashSet ();
			Assert.That (archive.Entries.Count, Is.EqualTo (5), "nupkg file count");
			Assert.That (files, Does.Contain (project + ".nuspec"), "nuspec");
			Assert.That (files, Does.Contain ("_rels/.rels"), ".rels");
			Assert.That (files, Does.Contain ("[Content_Types].xml"), "[Content_Types].xml");
			Assert.That (files, Does.Contain ($"lib/{platform.ToFrameworkWithPlatformVersion (isExecutable: false)}/{project}.dll"), $"{project}.dll");
			Assert.That (files, Has.Some.Matches<string> (v => v.StartsWith ("package/services/metadata/core-properties/", StringComparison.Ordinal) && v.EndsWith (".psmdcp", StringComparison.Ordinal)), "psmdcp");
		}

		[Test]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacOSX)]
		public void MultiTargetLibraryProject (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			// Get all the supported API versions
			var supportedApiVersion = Configuration.GetVariableArray ($"SUPPORTED_API_VERSIONS_{platform.AsString ().ToUpperInvariant ()}");
			supportedApiVersion = DotNetProjectTest.RemovePostCurrentOnMacCatalyst (supportedApiVersion, platform);
			var targetFrameworks = string.Join (";", supportedApiVersion.Select (v => v.Replace ("-", "-" + platform.AsString ().ToLowerInvariant ())));

			var project = "MultiTargetingLibrary";
			var configuration = "Release";
			var project_path = GetProjectPath (project, platform: platform);
			Clean (project_path);

			var properties = GetDefaultProperties ();
			properties ["cmdline:AllTheTargetFrameworks"] = targetFrameworks;

			// If any of the api versions we support are higher than the api version we're built for, we need to ignore any XCODE_*_PREVIEW warnings.
			AddNoWarnForPreviewVersions (platform, supportedApiVersion, properties);

			DotNet.AssertPack (project_path, properties);

			var nupkg = Path.Combine (Path.GetDirectoryName (project_path)!, "bin", configuration, project + ".1.0.0.nupkg");
			Assert.That (nupkg, Does.Exist, "nupkg existence");

			using var archive = ZipFile.OpenRead (nupkg);
			var files = archive.Entries.Select (v => v.FullName).ToHashSet ();
			Assert.That (archive.Entries.Count, Is.EqualTo (4 + supportedApiVersion.Count), "nupkg file count");
			Assert.That (files, Does.Contain (project + ".nuspec"), "nuspec");
			Assert.That (files, Does.Contain ("_rels/.rels"), ".rels");
			Assert.That (files, Does.Contain ("[Content_Types].xml"), "[Content_Types].xml");
			foreach (var sav in supportedApiVersion) {
				Assert.That (files, Does.Contain ($"lib/{sav.Replace ("-", "-" + platform.AsString ().ToLowerInvariant ())}/{project}.dll"), $"{project}.dll");
			}
			Assert.That (files, Has.Some.Matches<string> (v => v.StartsWith ("package/services/metadata/core-properties/", StringComparison.Ordinal) && v.EndsWith (".psmdcp", StringComparison.Ordinal)), "psmdcp");
		}
	}
}
