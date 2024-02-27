using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ComputeCodesignItemsTaskTests : TestBase {

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.WatchOS, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		public void Compute (ApplePlatform platform, bool isDotNet)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();

			var currentDir = Environment.CurrentDirectory;
			try {
				Environment.CurrentDirectory = tmpdir;
				var codesignItems = new List<ITaskItem> ();
				var codesignBundle = new List<ITaskItem> ();
				var generateDSymItems = new List<ITaskItem> ();
				var nativeStripItems = new List<ITaskItem> ();

				string codeSignatureSubdirectory = string.Empty;
				switch (platform) {
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					codeSignatureSubdirectory = "Contents/";
					break;
				}

				var bundleAppMetadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "CompiledEntitlements.plist" },
					{ "CodesignExtraArgs", "bundle-app-extra-args" },
					{ "CodesignKeychain", "bundle-app-keychain" },
					{ "CodesignResourceRules", "bundle-app-resource-rules" },
					{ "CodesignSigningKey", "bundle-app-signing-key" },
					{ "CodesignStampFile", "bundle-app-stamp-file" },
					{ "CodesignUseHardenedRuntime", "bundle-app-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "bundle-app-use-secure-timestamp" },
					{ "RequireCodeSigning", "true" },
				};
				var bundleAppMetadataNativeLibraries = new Dictionary<string, string> (bundleAppMetadata);
				bundleAppMetadataNativeLibraries.Remove ("CodesignEntitlements");

				var p1Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "p1.appex-entitlements" },
					{ "CodesignExtraArgs", "p1.appex-extra-args" },
					{ "CodesignKeychain", "p1.appex-keychain" },
					{ "CodesignResourceRules", "p1.appex-resource-rules" },
					{ "CodesignSigningKey", "" }, // empty code signing key
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "p1.appex-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "p1.appex-use-secure-timestamp" },
					{ "RequireCodeSigning", "false" }, // don't require code signing
				};
				var p1MetadataNativeLibraries = new Dictionary<string, string> (p1Metadata);
				p1MetadataNativeLibraries ["CodesignSigningKey"] = "-";
				p1MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var p2Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "p2.appex-entitlements" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "p2.appex-extra-args" },
					{ "CodesignKeychain", "p2.appex-keychain" },
					{ "CodesignResourceRules", "p2.appex-resource-rules" },
					{ "CodesignSigningKey", "p2.appex-signing-key" },
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "p2.appex-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "p2.appex-use-secure-timestamp" },
				};
				var p2MetadataNativeLibraries = new Dictionary<string, string> (p2Metadata);
				p2MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var p3Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "p3.appex-entitlements" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "p3.appex-extra-args" },
					{ "CodesignKeychain", "p3.appex-keychain" },
					{ "CodesignResourceRules", "p3.appex-resource-rules" },
					{ "CodesignSigningKey", "p3.appex-signing-key" },
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "p3.appex-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "p3.appex-use-secure-timestamp" },
				};
				var p3MetadataNativeLibraries = new Dictionary<string, string> (p3Metadata);
				p3MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var w1Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "CompiledEntitlements.plist" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "bundle-app-extra-args" },
					{ "CodesignKeychain", "bundle-app-keychain" },
					{ "CodesignResourceRules", "bundle-app-resource-rules" },
					{ "CodesignSigningKey", "bundle-app-signing-key" },
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "bundle-app-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "bundle-app-use-secure-timestamp" },
				};
				var w1MetadataNativeLibraries = new Dictionary<string, string> (w1Metadata);
				w1MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var wp1Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "wp1.appex-entitlements" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "wp1.appex-extra-args" },
					{ "CodesignKeychain", "wp1.appex-keychain" },
					{ "CodesignResourceRules", "wp1.appex-resource-rules" },
					{ "CodesignSigningKey", "" }, // empty code signing key
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "wp1.appex-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "wp1.appex-use-secure-timestamp" },
				};
				var wp1MetadataNativeLibraries = new Dictionary<string, string> (wp1Metadata);
				wp1MetadataNativeLibraries ["CodesignSigningKey"] = "-";
				wp1MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var wp2Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "wp2.appex-entitlements" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "wp2.appex-extra-args" },
					{ "CodesignKeychain", "wp2.appex-keychain" },
					{ "CodesignResourceRules", "wp2.appex-resource-rules" },
					{ "CodesignSigningKey", "wp2.appex-signing-key" },
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "wp2.appex-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "wp2.appex-use-secure-timestamp" },
				};
				var wp2MetadataNativeLibraries = new Dictionary<string, string> (wp2Metadata);
				wp2MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var wp3Metadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "wp3.appex-entitlements" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "wp3.appex-extra-args" },
					{ "CodesignKeychain", "wp3.appex-keychain" },
					{ "CodesignResourceRules", "wp3.appex-resource-rules" },
					{ "CodesignSigningKey", "wp3.appex-signing-key" },
					{ "CodesignStampFile", "" }, // empty stamp file
					{ "CodesignUseHardenedRuntime", "wp3.appex-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "wp3.appex-use-secure-timestamp" },
				};
				var wp3MetadataNativeLibraries = new Dictionary<string, string> (wp3Metadata);
				wp3MetadataNativeLibraries.Remove ("CodesignEntitlements");

				var createDumpMetadata = new Dictionary<string, string> {
					{ "CodesignDisableTimestamp", "true" },
					{ "CodesignEntitlements" , "createdump-entitlements" },
					{ "RequireCodeSigning", "true" },
					{ "CodesignExtraArgs", "createdump-extra-args" },
					{ "CodesignKeychain", "createdump-keychain" },
					{ "CodesignResourceRules", "createdump-resource-rules" },
					{ "CodesignSigningKey", "createdump-signing-key" },
					{ "CodesignStampFile", "createdump-stamp-file" },
					{ "CodesignUseHardenedRuntime", "createdump-use-hardened-runtime" },
					{ "CodesignUseSecureTimestamp", "createdump-use-secure-timestamp" },
				};

				codesignItems = new List<ITaskItem> {
					new TaskItem ("Bundle.app/Contents/MonoBundle/createdump", createDumpMetadata),
				};

				codesignBundle = new List<ITaskItem> {
					new TaskItem ("Bundle.app", bundleAppMetadata),
					new TaskItem ("Bundle.app/PlugIns/P1.appex", p1Metadata),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/PlugIns/P2.appex", p2Metadata),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/PlugIns/P2.appex/PlugIns/P3.appex", p3Metadata),
					new TaskItem ("Bundle.app/Watch/W1.app", w1Metadata),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex", wp1Metadata),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex", wp2Metadata),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex", wp3Metadata),
				};

				nativeStripItems = new List<ITaskItem> {
					new TaskItem ("Bundle.app/Bundle", new Dictionary<string, string> { { "StripStampFile", "bundle-strip-stamp-file" } } ),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/P1"),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/PlugIns/P2.appex/P2"),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/PlugIns/P2.appex/PlugIns/P3.appex/P3", new Dictionary<string, string> { { "StripStampFile", "p3-strip-stamp-file" } } ),
					new TaskItem ("Bundle.app/Watch/W1.app/W1"),
					new TaskItem ("Bundle.app/Watch/W1W1app/PlugIns/WP1.appex/WP1"),
					new TaskItem ("Bundle.app/Watch/Watch.app/PlugIns/WP1.appex/PlugIns/WP2.appex/WP2"),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/WP3", new Dictionary<string, string> { { "StripStampFile", "wp3-strip-stamp-file" } } ),
				};

				generateDSymItems = new List<ITaskItem> {
					new TaskItem ("Bundle.app/Bundle", new Dictionary<string, string> { { "dSYMUtilStampFile", "Bundle.app.dSYM/Contents/Info.plist" } } ),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/P1", new Dictionary<string, string> { { "dSYMUtilStampFile", "P1.appex.dSYM/Contents/Info.plist" } }),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/PlugIns/P2.appex/P2", new Dictionary<string, string> { { "dSYMUtilStampFile", "P2.appex.dSYM/Contents/Info.plist" } }),
					new TaskItem ("Bundle.app/PlugIns/P1.appex/PlugIns/P2.appex/PlugIns/P3.appex/P3", new Dictionary<string, string> { { "dSYMUtilStampFile", "P3.appex.dSYM/Contents/Info.plist" } }),
					new TaskItem ("Bundle.app/Watch/W1.app/W1", new Dictionary<string, string> { { "dSYMUtilStampFile", "W1.app.dSYM/Contents/Info.plist" } }),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/WP1", new Dictionary<string, string> { { "dSYMUtilStampFile", "WP1.appex.dSYM/Contents/Info.plist" } }),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/WP2", new Dictionary<string, string> { { "dSYMUtilStampFile", "WP2.appex.dSYM/Contents/Info.plist" } }),
					new TaskItem ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/WP3", new Dictionary<string, string> { { "dSYMUtilStampFile", "WP3.appex.dSYM/Contents/Info.plist" } }),
				};

				var infos = new CodesignInfo [] {
					new CodesignInfo ("Bundle.app", Platforms.All, bundleAppMetadata.Set ("CodesignAdditionalFilesToTouch", "Bundle.app.dSYM/Contents/Info.plist;bundle-strip-stamp-file")),
					new CodesignInfo ("Bundle.app/a.dylib", Platforms.Mobile, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/a.dylib")),
					new CodesignInfo ("Bundle.app/Contents/b.dylib", Platforms.All, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/b.dylib")),
					new CodesignInfo ("Bundle.app/Contents/MonoBundle/c.dylib", Platforms.All, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/MonoBundle/c.dylib")),
					new CodesignInfo ("Bundle.app/Contents/MonoBundle/SubDir/d.dylib", Platforms.All, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/MonoBundle/SubDir/d.dylib")),
					new CodesignInfo ("Bundle.app/M1.metallib", Platforms.Mobile, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/M1.metallib")),
					new CodesignInfo ("Bundle.app/Resources/M2.metallib", Platforms.Mobile, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Resources/M2.metallib")),
					new CodesignInfo ("Bundle.app/Contents/Resources/M3.metallib", Platforms.All, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/Resources/M3.metallib")),
					new CodesignInfo ("Bundle.app/Contents/Resources/SubDir/M4.metallib", Platforms.All, bundleAppMetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/Resources/SubDir/M4.metallib")),
					new CodesignInfo (
						"Bundle.app/PlugIns/P1.appex",
						Platforms.None,
						p1Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/P1.appex/.stampfile")
					),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/P1a.dylib", Platforms.Mobile, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/P1a.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/Contents/P1b.dylib", Platforms.All, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/Contents/P1b.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/Contents/MonoBundle/P1c.dylib", Platforms.All, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/Contents/MonoBundle/P1c.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/Contents/MonoBundle/SubDir/P1d.dylib", Platforms.All, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/Contents/MonoBundle/SubDir/P1d.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/PM1.metallib", Platforms.Mobile, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/PM1.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/Resources/PM2.metallib", Platforms.Mobile, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/Resources/PM2.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/Contents/Resources/PM3.metallib", Platforms.All, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/Contents/Resources/PM3.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/Contents/Resources/SubDir/PM4.metallib", Platforms.All, p1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/Contents/Resources/SubDir/PM4.metallib")),
					new CodesignInfo (
						"Bundle.app/PlugIns/P1.appex/plugins/P2.appex",
						Platforms.All,
						p2Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/P2.appex/.stampfile").
							Set ("CodesignAdditionalFilesToTouch", "P2.appex.dSYM/Contents/Info.plist")
					),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/P2a.dylib", Platforms.Mobile, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/P2a.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/P2b.dylib", Platforms.All, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/P2b.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/MonoBundle/P2c.dylib", Platforms.All, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/MonoBundle/P2c.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/MonoBundle/SubDir/P2d.dylib", Platforms.All, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/MonoBundle/SubDir/P2d.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/P2M1.metallib", Platforms.Mobile, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/P2M1.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Resources/P2M2.metallib", Platforms.Mobile, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Resources/P2M2.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/Resources/P2M3.metallib", Platforms.All, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/Resources/P2M3.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/Resources/SubDir/P2M4.metallib", Platforms.All, p2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/Contents/Resources/SubDir/P2M4.metallib")),
					new CodesignInfo (
						"Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex",
						Platforms.All,
						p3Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/P3.appex/.stampfile").
							Set ("CodesignAdditionalFilesToTouch", "P3.appex.dSYM/Contents/Info.plist;p3-strip-stamp-file")
					),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/P3a.dylib", Platforms.Mobile, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/P3a.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/P3b.dylib", Platforms.All, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/P3b.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/MonoBundle/P3c.dylib", Platforms.All, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/MonoBundle/P3c.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/MonoBundle/SubDir/P3d.dylib", Platforms.All, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/MonoBundle/SubDir/P3d.dylib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/P3M1.metallib", Platforms.Mobile, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/P3M1.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Resources/P3M2.metallib", Platforms.Mobile, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Resources/P3M2.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/Resources/P3M3.metallib", Platforms.All, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/Resources/P3M3.metallib")),
					new CodesignInfo ("Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/Resources/SubDir/P3M4.metallib", Platforms.All, p3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/PlugIns/P1.appex/plugins/P2.appex/PlugIns/P3.appex/Contents/Resources/SubDir/P3M4.metallib")),
					new CodesignInfo (
						"Bundle.app/Watch/W1.app",
						Platforms.All,
						w1Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/W1.app/.stampfile").
							Set ("CodesignAdditionalFilesToTouch", "W1.app.dSYM/Contents/Info.plist")
					),
					new CodesignInfo ("Bundle.app/Watch/W1.app/Contents/b.dylib", Platforms.All, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/Contents/b.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/Contents/MonoBundle/c.dylib", Platforms.All, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/Contents/MonoBundle/c.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/Contents/MonoBundle/SubDir/d.dylib", Platforms.All, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/Contents/MonoBundle/SubDir/d.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/W1M1.metallib", Platforms.Mobile, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/W1M1.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/Resources/W1M2.metallib", Platforms.Mobile, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/Resources/W1M2.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/Contents/Resources/W1M3.metallib", Platforms.All, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/Contents/Resources/W1M3.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/Contents/Resources/SubDir/W1M4.metallib", Platforms.All, w1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/Contents/Resources/SubDir/W1M4.metallib")),
					new CodesignInfo (
						"Bundle.app/Watch/W1.app/PlugIns/WP1.appex",
						Platforms.All,
						wp1Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/WP1.appex/.stampfile").
							Set ("CodesignAdditionalFilesToTouch", "WP1.appex.dSYM/Contents/Info.plist")
					),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/W1a.dylib", Platforms.Mobile, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/W1a.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/W1b.dylib", Platforms.All, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/W1b.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/MonoBundle/W1c.dylib", Platforms.All, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/MonoBundle/W1c.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/MonoBundle/SubDir/W1d.dylib", Platforms.All, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/MonoBundle/SubDir/W1d.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/W1M1.metallib", Platforms.Mobile, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/W1M1.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Resources/W1M2.metallib", Platforms.Mobile, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Resources/W1M2.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/Resources/W1M3.metallib", Platforms.All, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/Resources/W1M3.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/Resources/SubDir/W1M4.metallib", Platforms.All, wp1MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/Contents/Resources/SubDir/W1M4.metallib")),
					new CodesignInfo (
						"Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex",
						Platforms.All,
						wp2Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/WP2.appex/.stampfile").
							Set ("CodesignAdditionalFilesToTouch", "WP2.appex.dSYM/Contents/Info.plist")
					),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/W2a.dylib", Platforms.Mobile, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/W2a.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/W2b.dylib", Platforms.All, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/W2b.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/MonoBundle/W2c.dylib", Platforms.All, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/MonoBundle/W2c.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/MonoBundle/SubDir/W2c.dylib", Platforms.All, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/MonoBundle/SubDir/W2c.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/W2M1.metallib", Platforms.Mobile, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/W2M1.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Resources/W2M2.metallib", Platforms.Mobile, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Resources/W2M2.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/Resources/W2M3.metallib", Platforms.All, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/Resources/W2M3.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/Resources/SubDir/W2M4.metallib", Platforms.All, wp2MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/Contents/Resources/SubDir/W2M4.metallib")),
					new CodesignInfo (
						"Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex",
						Platforms.All,
						wp3Metadata.
							Set ("CodesignStampFile", $"codesign-stamp-path/WP3.appex/.stampfile").
							Set ("CodesignAdditionalFilesToTouch", "WP3.appex.dSYM/Contents/Info.plist;wp3-strip-stamp-file")
					),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/W3a.dylib", Platforms.Mobile, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/W3a.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/W3b.dylib", Platforms.All, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/W3b.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/MonoBundle/W3c.dylib", Platforms.All, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/MonoBundle/W3c.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/MonoBundle/SubDir/W3c.dylib", Platforms.All, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/MonoBundle/SubDir/W3c.dylib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/W3M1.metallib", Platforms.Mobile, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/W3M1.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Resources/W3M2.metallib", Platforms.Mobile, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Resources/W3M2.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/Resources/W3M3.metallib", Platforms.All, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/Resources/W3M3.metallib")),
					new CodesignInfo ("Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/Resources/SubDir/W3M4.metallib", Platforms.All, wp3MetadataNativeLibraries.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Watch/W1.app/PlugIns/WP1.appex/PlugIns/WP2.appex/PlugIns/WP3.appex/Contents/Resources/SubDir/W3M4.metallib")),

					new CodesignInfo ("Bundle.app/Contents/MonoBundle/createdump", Platforms.All, createDumpMetadata),
				};

				var allFiles = infos.Select (v => v.ItemSpec).ToArray ();
				Touch (tmpdir, allFiles);

				var task = CreateTask<ComputeCodesignItems> ();
				task.AppBundleDir = "Bundle.app";
				task.CodesignBundle = codesignBundle.ToArray ();
				task.CodesignItems = codesignItems.ToArray ();
				task.CodesignStampPath = "codesign-stamp-path/";
				task.GenerateDSymItems = generateDSymItems.ToArray ();
				task.NativeStripItems = nativeStripItems.ToArray ();
				task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, isDotNet).ToString ();
				Assert.IsTrue (task.Execute (), "Execute");
				Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "Warning Count");

				VerifyCodesigningResults (infos, task.OutputCodesignItems, platform);
			} finally {
				Environment.CurrentDirectory = currentDir;
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.WatchOS, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		public void Duplicated (ApplePlatform platform, bool isDotNet)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();

			var currentDir = Environment.CurrentDirectory;
			try {
				Environment.CurrentDirectory = tmpdir;
				var codesignItems = new List<ITaskItem> ();
				var codesignBundle = new List<ITaskItem> ();

				string codeSignatureSubdirectory = string.Empty;
				switch (platform) {
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					codeSignatureSubdirectory = "Contents/";
					break;
				}

				var bundleAppMetadata = new Dictionary<string, string> {
					{ "RequireCodeSigning", "true" },
				};

				var createDumpMetadata = new Dictionary<string, string> {
					{ "RequireCodeSigning", "true" },
				};

				codesignItems = new List<ITaskItem> {
					new TaskItem ("Bundle.app/Contents/MonoBundle/createdump", createDumpMetadata),
					new TaskItem ("Bundle.app/Contents/MonoBundle/createdump", createDumpMetadata),
				};

				codesignBundle = new List<ITaskItem> {
					new TaskItem ("Bundle.app", bundleAppMetadata),
				};

				var infos = new CodesignInfo [] {
					new CodesignInfo ("Bundle.app", Platforms.All, bundleAppMetadata.Set ("CodesignStampFile", $"codesign-stamp-path/Bundle.app/.stampfile")),
					new CodesignInfo ("Bundle.app/Contents/MonoBundle/createdump", Platforms.All, createDumpMetadata.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/MonoBundle/createdump")),
				};

				var allFiles = infos.Select (v => v.ItemSpec).ToArray ();
				Touch (tmpdir, allFiles);

				var task = CreateTask<ComputeCodesignItems> ();
				task.AppBundleDir = "Bundle.app";
				task.CodesignBundle = codesignBundle.ToArray ();
				task.CodesignItems = codesignItems.ToArray ();
				task.CodesignStampPath = "codesign-stamp-path/";
				task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, isDotNet).ToString ();
				Assert.IsTrue (task.Execute (), "Execute");
				Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "Warning Count");

				VerifyCodesigningResults (infos, task.OutputCodesignItems, platform);
			} finally {
				Environment.CurrentDirectory = currentDir;
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.WatchOS, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		public void DuplicatedWithDifferentMetadata (ApplePlatform platform, bool isDotNet)
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();

			var currentDir = Environment.CurrentDirectory;
			try {
				Environment.CurrentDirectory = tmpdir;
				var codesignItems = new List<ITaskItem> ();
				var codesignBundle = new List<ITaskItem> ();

				string codeSignatureSubdirectory = string.Empty;
				switch (platform) {
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.MacOSX:
					codeSignatureSubdirectory = "Contents/";
					break;
				}

				var bundleAppMetadata = new Dictionary<string, string> {
					{ "RequireCodeSigning", "true" },
				};

				var createDumpMetadata1 = new Dictionary<string, string> {
					{ "RequireCodeSigning", "true" },
					{ "OnlyIn1", "true" },
					{ "InOneAndTwoWithDifferentValues", "1" },
				};
				var createDumpMetadata2 = new Dictionary<string, string> {
					{ "RequireCodeSigning", "true" },
					{ "OnlyIn2", "true" },
					{ "InOneAndTwoWithDifferentValues", "2" },
				};
				var createDumpMetadata3 = new Dictionary<string, string> {
					{ "RequireCodeSigning", "true" },
				};

				codesignItems = new List<ITaskItem> {
					new TaskItem ("Bundle.app/Contents/MonoBundle/createdump", createDumpMetadata1),
					new TaskItem ("Bundle.app/Contents/MonoBundle/createdump", createDumpMetadata2),
					new TaskItem ("Bundle.app/Contents/MonoBundle/createdump", createDumpMetadata3),
				};

				codesignBundle = new List<ITaskItem> {
					new TaskItem ("Bundle.app", bundleAppMetadata),
				};

				var infos = new CodesignInfo [] {
					new CodesignInfo ("Bundle.app", Platforms.All, bundleAppMetadata.Set ("CodesignStampFile", $"codesign-stamp-path/Bundle.app/.stampfile")),
					new CodesignInfo ("Bundle.app/Contents/MonoBundle/createdump", Platforms.All, createDumpMetadata1.Set ("CodesignStampFile", "codesign-stamp-path/Bundle.app/Contents/MonoBundle/createdump")),
				};

				var allFiles = infos.Select (v => v.ItemSpec).ToArray ();
				Touch (tmpdir, allFiles);

				var task = CreateTask<ComputeCodesignItems> ();
				task.AppBundleDir = "Bundle.app";
				task.CodesignBundle = codesignBundle.ToArray ();
				task.CodesignItems = codesignItems.ToArray ();
				task.CodesignStampPath = "codesign-stamp-path/";
				task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform, isDotNet).ToString ();
				Assert.IsTrue (task.Execute (), "Execute");
				Assert.AreEqual (3, Engine.Logger.WarningsEvents.Count, "Warning Count");
				Assert.AreEqual ("Code signing has been requested multiple times for 'Bundle.app/Contents/MonoBundle/createdump', with different metadata. The metadata 'OnlyIn1=true' has been set for one item, but not the other.", Engine.Logger.WarningsEvents [0].Message, "Message #0");
				Assert.AreEqual ("Code signing has been requested multiple times for 'Bundle.app/Contents/MonoBundle/createdump', with different metadata. The metadata 'InOneAndTwoWithDifferentValues' has different values for each item (once it's '1', another time it's '2').", Engine.Logger.WarningsEvents [1].Message, "Message #1");
				Assert.AreEqual ("Code signing has been requested multiple times for 'Bundle.app/Contents/MonoBundle/createdump', with different metadata. The metadata for one are: 'RequireCodeSigning, OnlyIn1, InOneAndTwoWithDifferentValues, CodesignStampFile', while the metadata for the other are: 'RequireCodeSigning, CodesignStampFile'", Engine.Logger.WarningsEvents [2].Message, "Message #2");

				VerifyCodesigningResults (infos, task.OutputCodesignItems, platform);
			} finally {
				Environment.CurrentDirectory = currentDir;
			}
		}
		void VerifyCodesigningResults (CodesignInfo [] infos, ITaskItem [] outputCodesignItems, ApplePlatform platform)
		{
			Assert.That (outputCodesignItems.Select (v => v.ItemSpec), Is.Unique, "Uniqueness");

			var failures = new List<string> ();
			var itemsFound = new List<ITaskItem> ();
			foreach (var info in infos) {
				var item = outputCodesignItems.SingleOrDefault (v => string.Equals (v.ItemSpec, info.ItemSpec, StringComparison.OrdinalIgnoreCase));
				info.CodesignItem = item;
				if (IsPlatform (info.SignedOn, platform)) {
					if (item is null) {
						failures.Add ($"Expected '{info.ItemSpec}' to be signed.");
						continue;
					}
				} else {
					if (item is not null) {
						failures.Add ($"Did not expect '{info.ItemSpec}' to be signed.");
						continue;
					}
				}

				if (item is null)
					continue;
				itemsFound.Add (item);

				foreach (var kvp in info.Metadata) {
					var metadata = item.GetMetadata (kvp.Key);
					if (metadata == string.Empty && kvp.Value != string.Empty) {
						failures.Add ($"Item '{info.ItemSpec}': Expected metadata '{kvp.Key}' not found (with value '{kvp.Value}').");
					} else if (!string.Equals (metadata, kvp.Value)) {
						failures.Add ($"Item '{info.ItemSpec}': Expected value '{kvp.Value}' for metadata '{kvp.Key}', but got '{metadata}' instead.\nExpected: {kvp.Value}\nActual:   {metadata}");
					}
				}

				var customMetadata = item.CopyCustomMetadata ();
				var unexpectedMetadata = customMetadata.Keys.ToHashSet ();
				unexpectedMetadata.ExceptWith (info.Metadata.Keys);
				unexpectedMetadata.Remove ("OriginalItemSpec");
				foreach (var unexpected in unexpectedMetadata) {
					if (string.IsNullOrEmpty (customMetadata [unexpected]))
						continue;
					failures.Add ($"Item '{info.ItemSpec}': Unexpected metadata '{unexpected}' with value '{customMetadata [unexpected]}'.");
				}
			}

			var itemsNotFound = outputCodesignItems.Where (v => !itemsFound.Contains (v)).ToArray ();
			foreach (var itemNotFound in itemsNotFound) {
				failures.Add ($"Did not expect '{itemNotFound.ItemSpec}' to be signed.");
			}

			if (failures.Count > 0) {
				Console.WriteLine ($"{failures.Count} failures");
				foreach (var f in failures)
					Console.WriteLine (f);
				Console.WriteLine ($"{failures.Count} failures");
			}
			Assert.That (failures, Is.Empty, "Failures");
		}

		bool IsPlatform (Platforms platforms, ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				return (platforms & Platforms.iOS) == Platforms.iOS;
			case ApplePlatform.TVOS:
				return (platforms & Platforms.tvOS) == Platforms.tvOS;
			case ApplePlatform.MacOSX:
				return (platforms & Platforms.macOS) == Platforms.macOS;
			case ApplePlatform.WatchOS:
				return (platforms & Platforms.watchOS) == Platforms.watchOS;
			case ApplePlatform.MacCatalyst:
				return (platforms & Platforms.MacCatalyst) == Platforms.MacCatalyst;
			default:
				throw new NotImplementedException ();
			}
		}

		void Touch (string root, params string [] files)
		{
			foreach (var f in files) {
				var file = Path.Combine (root, f);
				if (file.EndsWith (".appex", StringComparison.OrdinalIgnoreCase) || file.EndsWith (".app", StringComparison.OrdinalIgnoreCase)) {
					Directory.CreateDirectory (f);
				} else {
					Directory.CreateDirectory (Path.GetDirectoryName (file));
					File.WriteAllText (file, string.Empty);
				}
			}
		}

		class CodesignInfo {
			public string ItemSpec;
			public Platforms SignedOn;
			public Dictionary<string, string> Metadata;
			public ITaskItem? CodesignItem;

			public CodesignInfo (string item, Platforms signedOn, Dictionary<string, string>? metadata = null)
			{
				ItemSpec = item;
				SignedOn = signedOn;
				Metadata = metadata ?? new Dictionary<string, string> ();
			}
		}

		// As opposed to ApplePlatform, this enum is a bitfield, and can represent multiple platforms in a single value.
		[Flags]
		enum Platforms {
			None = 0,
			iOS = 1,
			tvOS = 2,
			watchOS = 4,
			macOS = 8,
			MacCatalyst = 16,
			Mobile = iOS | tvOS | watchOS,
			Desktop = macOS | MacCatalyst,
			All = Mobile | Desktop,
		}

	}

	public static class Dictionary_Extensions {
		public static Dictionary<string, string> Set (this Dictionary<string, string> self, string key, string value)
		{
			var rv = new Dictionary<string, string> (self);
			rv [key] = value;
			return rv;
		}
	}

	public static class ITaskItem_Extensions {
		public static Dictionary<string, string> CopyCustomMetadata (this ITaskItem self)
		{
			var rv = new Dictionary<string, string> ();
			foreach (DictionaryEntry de in self.CloneCustomMetadata ()) {
				rv [(string) de.Key] = (string) de.Value;
			}
			return rv;
		}
	}
}

