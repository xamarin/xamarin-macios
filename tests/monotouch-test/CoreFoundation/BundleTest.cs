//
// Copyright 2015 Xamarin Inc
//
using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using CoreFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BundleTest {
#if __WATCHOS__
		const string ExpectedAppName = "monotouchtest.appex";
#elif MONOMAC && !NET
		const string ExpectedAppName = "xammac_tests.app";
#else
		const string ExpectedAppName = "monotouchtest.app";
#endif

		[Test]
		public void TestGetAll ()
		{
			var bundles = CFBundle.GetAll ();
			Assert.IsTrue (bundles.Length > 0);
			foreach (CFBundle b in bundles) {
				Assert.IsFalse (String.IsNullOrEmpty (b.Url.ToString ()),
  						String.Format ("Found bundle with null url and id {0}", b.Identifier));
			}
		}

		[Test]
		public void TestGetBundleIdMissing ()
		{
			var bundle = CFBundle.Get ("????");
			Assert.IsNull (bundle);
		}

		[Test]
		public void TestGetBundleId ()
		{
			// grab all bundles and make sure we do get the correct ones using their id
			var bundles = CFBundle.GetAll ();
			Assert.IsTrue (bundles.Length > 0);

			// There may be multiple apps providing the same bundle ID (the typical example is that we usually have multiple Xcodes installed)
			// So compute a map for bundle id -> bundle paths that's used in the second part here to verify the CFBundle.Get results.
			var dict = new Dictionary<string, List<string>> ();
			foreach (var bundle in bundles) {
				var id = bundle.Identifier;
				if (string.IsNullOrEmpty (id))
					continue;
				if (!dict.TryGetValue (id, out var list))
					dict [id] = list = new List<string> ();
				list.Add ((string) ((NSString) bundle.Url.Path).ResolveSymlinksInPath ());
			}

			foreach (CFBundle b in bundles) {
				var id = b.Identifier;
				if (!String.IsNullOrEmpty (id)) {
					var otherBundle = CFBundle.Get (id);
					Assert.AreEqual (b.Info.Type, otherBundle.Info.Type,
  							 String.Format ("Found bundle with diff type and id {0}", id));
					var bPath = (string) ((NSString) b.Url.Path).ResolveSymlinksInPath ();
					var list = dict [id];
					Assert.That (list, Does.Contain (bPath), "None of the bundles for {0} matches the path {1}", id, bPath);
				}
			}
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestGetBundleIdNull (string id)
		{
			Assert.Throws<ArgumentException> (() => CFBundle.Get (id));
		}

		[Test]
		public void TestGetMain ()
		{
			var main = CFBundle.GetMain ();
#if __WATCHOS__
			var expectedBundleId = "com.xamarin.monotouch-test_watch.watchkitapp.watchkitextension";
#elif MONOMAC && !NET
			var expectedBundleId = "com.xamarin.xammac_tests";
#else
			var expectedBundleId = "com.xamarin.monotouch-test";
#endif
			Assert.AreEqual (expectedBundleId, main.Identifier);
			Assert.IsTrue (main.HasLoadedExecutable);
		}

		[Test]
		public void TestBuiltInPlugInsUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.BuiltInPlugInsUrl.ToString (), Contains.Substring ("PlugIns/"));
		}

		[Test]
		public void TestExecutableUrl ()
		{
			var main = CFBundle.GetMain ();
#if MONOMAC && !NET
			var executableRelativePath = Path.Combine (ExpectedAppName, "Contents", "MacOS", "xammac_tests");
#elif __MACCATALYST__ || __MACOS__
			var executableRelativePath = Path.Combine (ExpectedAppName, "Contents", "MacOS", "monotouchtest");
#else
			var executableRelativePath = Path.Combine (ExpectedAppName, "monotouchtest");
#endif
			var alternativeRelativePath = executableRelativePath.Replace (ExpectedAppName, "PublicStaging.app");
			Assert.That (main.ExecutableUrl.ToString (), Does.Contain (executableRelativePath).Or.Contain (alternativeRelativePath));
		}

		[Test]
		public void TestPrivateFrameworksUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.PrivateFrameworksUrl.ToString (), Contains.Substring ("Frameworks/"));
		}

		[Test]
		public void TestResourcesDirectoryUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.ResourcesDirectoryUrl.ToString (), Does.Contain (ExpectedAppName + "/").Or.Contain ("PublicStaging.app/"));
		}

		[Test]
		public void TestSharedFrameworksUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.SharedFrameworksUrl.ToString (), Contains.Substring ("SharedFrameworks/"));
		}

		[Test]
		public void TestSharedSupportUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.SharedSupportUrl.ToString (), Contains.Substring ("SharedSupport/"));
		}

		[Test]
		public void TestSupportFilesDirectoryUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.SupportFilesDirectoryUrl.ToString (), Does.Contain (ExpectedAppName + "/").Or.Contain ("PublicStaging.app/"));
		}

		[Test]
		public void TestArchitectures ()
		{
			var main = CFBundle.GetMain ();
			Assert.IsTrue (main.Architectures.Length > 0);
		}

		[Test]
		public void TestUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.Url.ToString (), Does.Contain (ExpectedAppName + "/").Or.Contain ("PublicStaging.app/"));
		}

		[Test]
		public void TestDevelopmentRegion ()
		{
			var main = CFBundle.GetMain ();
			Assert.IsTrue (String.IsNullOrEmpty (main.DevelopmentRegion));
		}

		[Test]
		public void TestLocalizations ()
		{
			var main = CFBundle.GetMain ();
			var localizations = CFBundle.GetLocalizations (main.Url);
			Assert.AreEqual (1, localizations.Length);
		}

		[Test]
		public void TestLocalizationsNull ()
		{
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetLocalizations (null));
		}

		[Test]
		public void TestPreferredLocalizations ()
		{
			var preferred = new string [] { "en", "es" };
			var used = CFBundle.GetPreferredLocalizations (preferred);
			Assert.IsTrue (used.Length > 0);
			foreach (var u in used)
				Assert.That (preferred, Contains.Item (u), u);
		}

		[Test]
		public void TestPreferredLocalizationsNull ()
		{
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetPreferredLocalizations (null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestAuxiliaryExecutableUrlNull (string executable)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetAuxiliaryExecutableUrl (executable));
		}

		[Test]
		public void TestGetAuxiliaryExecutableUrlNull ()
		{
			var main = CFBundle.GetMain ();
			var url = main.GetAuxiliaryExecutableUrl ("fake-exe");
			Assert.IsNull (url);
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlNullName (string resourceName)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrl (resourceName, "type", null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrl ("resourceName", resourceType, null));
		}

		[Test]
		public void TestStaticResourceUrlNull ()
		{
			NSUrl url = null;
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetResourceUrl (url, "resourceName", "resourceType", null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestStaticResourceUrlNullName (string resourceName)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => CFBundle.GetResourceUrl (main.Url, resourceName, "resourceType", null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestStaticResourceUrlNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => CFBundle.GetResourceUrl (main.Url, "resourceName", resourceType, null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlsNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrls (resourceType, null));
		}

		[Test]
		public void TestStaticResourceUrlsNullType ()
		{
			NSUrl url = null;
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetResourceUrls (url, "resourceType", null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestStaticResourceUrlsNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => CFBundle.GetResourceUrls (main.Url, resourceType, null));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlLocalNameNullName (string resourceName)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrl (resourceName, "resourceType", null, "en"));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlLocalNameNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrl ("resourceName", resourceType, null, "en"));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlLocalNameNullLocale (string locale)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrl ("resourceName", "resourceType", null, locale));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlsLocalNameNullType (string type)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrls (type, null, "en"));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestResourceUrlsLocalNameNullLocale (string locale)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetResourceUrls ("jpg", null, locale));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestLocalizedStringNullKey (string key)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetLocalizedString (key, null, "tableName"));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestLocalizedStringNullTable (string tableName)
		{
			var main = CFBundle.GetMain ();
			Assert.Throws<ArgumentException> (() => main.GetLocalizedString ("key", null, tableName));
		}

		[Test]
		public void TestGetLocalizationsForPreferencesNullLocalArray ()
		{
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetLocalizationsForPreferences (null, new string [0]));
		}

		[Test]
		public void TestGetLocalizationsForPreferencesNullPrefArray ()
		{
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetLocalizationsForPreferences (new string [0], null));
		}

		[Test]
		public void TestGetInfoDictionaryNull ()
		{
			Assert.Throws<ArgumentNullException> (() => CFBundle.GetInfoDictionary (null));
		}

#if MONOMAC
		[Test]
		public void TestIsArchitectureLoadable ()
		{
			TestRuntime.AssertXcodeVersion (12, 2);

			var isX64Executable = global::System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == global::System.Runtime.InteropServices.Architecture.X64;
			var isArm64Executable = global::System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == global::System.Runtime.InteropServices.Architecture.Arm64;

			bool loadable_x86_64 = CFBundle.IsArchitectureLoadable (CFBundle.Architecture.X86_64);
			// Due to Rosetta, both x64 and arm64 executables are loadable on Apple Silicon.
			if (isX64Executable || isArm64Executable)
				Assert.IsTrue (loadable_x86_64, "x86_64 Expected => true");
			else
				Assert.IsFalse (loadable_x86_64, "x86_64 Expected => false");

			bool loadable_arm64 = CFBundle.IsArchitectureLoadable (CFBundle.Architecture.ARM64);
			if (isArm64Executable)
				Assert.IsTrue (loadable_arm64, "arm64 Expected => true");
			// Due to Rosetta, we can't determine whether ARM64 is loadable or not if we're an X64 executable ourselves.
		}

		[Test]
		public void TestIsExecutableLoadable ()
		{
			TestRuntime.AssertXcodeVersion (12, 2);

			var main = CFBundle.GetMain ();
			var loadableBundle = CFBundle.IsExecutableLoadable (main);
			Assert.IsTrue (loadableBundle, "loadableBundle");

			var loadableBundleUrl = CFBundle.IsExecutableLoadable (main.ExecutableUrl);
			Assert.IsTrue (loadableBundleUrl, "loadableBundleUrl");
		}
#endif
	}
}
