//
// Copyright 2015 Xamarin Inc
//
using System.IO;
using System.Linq;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BundleTest {
#if APP_EXTENSION
		const string ExpectedAppName = "monotouchtest.appex";
		const string ExpectedBundleExtension = ".appex";
		const string ExpectedBundleId = "com.xamarin.monotouch-test.AudioUnitExtension";
#else
		const string ExpectedAppName = "monotouchtest.app";
		const string ExpectedBundleExtension = ".app";
		const string ExpectedBundleId = "com.xamarin.monotouch-test";
#endif
		const string ExpectedExecutableName = "monotouchtest";

		[Test]
		public void TestGetAll ()
		{
			var bundles = CFBundle.GetAll ();
			Assert.That (bundles.Length > 0, Is.True);
			foreach (CFBundle b in bundles) {
				Assert.That (String.IsNullOrEmpty (b.Url.ToString ()), Is.False, String.Format ("Found bundle with null url and id {0}", b.Identifier));
			}
		}

		[Test]
		public void TestGetBundleIdMissing ()
		{
			var bundle = CFBundle.Get ("????");
			Assert.That (bundle, Is.Null);
		}

		[Test]
		public void TestGetBundleId ()
		{
			// grab all bundles and make sure we do get the correct ones using their id
			var bundles = CFBundle.GetAll ();
			Assert.That (bundles.Length > 0, Is.True);

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
					Assert.That (otherBundle.Info.Type, Is.EqualTo (b.Info.Type), String.Format ("Found bundle with diff type and id {0}", id));
					var bPath = (string) ((NSString) b.Url.Path).ResolveSymlinksInPath ();
					var list = dict [id];
					Assert.That (list, Does.Contain (bPath), $"None of the bundles for {id} matches the path {bPath}");
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
			Assert.That (main.Identifier, Is.EqualTo (ExpectedBundleId));
			Assert.That (main.HasLoadedExecutable, Is.True);
		}

		[Test]
		public void TestMainBundleExtension ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (Path.GetExtension ((string) main.Url.Path), Is.EqualTo (ExpectedBundleExtension));
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
#if __MACCATALYST__ || __MACOS__
			var executableRelativePath = Path.Combine (ExpectedAppName, "Contents", "MacOS", ExpectedExecutableName);
#else
			var executableRelativePath = Path.Combine (ExpectedAppName, ExpectedExecutableName);
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
			Assert.That (main.Architectures.Length > 0, Is.True);
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
			Assert.That (String.IsNullOrEmpty (main.DevelopmentRegion), Is.True);
		}

		[Test]
		public void TestLocalizations ()
		{
			var main = CFBundle.GetMain ();
			var localizations = CFBundle.GetLocalizations (main.Url).OrderBy (v => v).ToArray ();
			var expected = new string [] {
				"Base", "en-AU", "en-UK", "es", "es-AR", "es-ES"
			}.OrderBy (v => v).ToArray ();
			Assert.That (string.Join (";", localizations), Is.EqualTo (string.Join (";", expected)), "Localizations");
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
			Assert.That (used.Length > 0, Is.True);
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
			Assert.That (url, Is.Null);
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

		[Test]
		public void TestGetInfoDictionary ()
		{
			var main = CFBundle.GetMain ();
			Assert.That (main.Url, Is.Not.Null, "Url");
			var dict = CFBundle.GetInfoDictionary (main.Url);
			Assert.That (dict, Is.Not.Null, "GetInfoDictionary");
			Assert.That (dict.Count, Is.GreaterThan ((nuint) 0), "Count");
		}

		[Test]
		public void GetLocalizedString ()
		{
			var main = CFBundle.GetMain ();

			Assert.Multiple (() => {
				Assert.Throws<ArgumentException> (() => main.GetLocalizedString (null, "value", "table"), "Key E1");
				Assert.Throws<ArgumentException> (() => main.GetLocalizedString ("", "value", "table"), "Key E2");

				var defaultValue = "default";
				var preferred = NSBundle.MainBundle.PreferredLocalizations [0];
				string key;
				string expectedValue;
				string tableName;
				string s;

				// default localization
				foreach (var tn in new string [] { "Localizable", null, "" }) {
					tableName = tn;
					key = "GoodMorning";
					expectedValue = "?";
					switch (preferred) {
					case "en-AU":
						expectedValue = "G'day mate";
						break;
					case "en-UK":
						expectedValue = "Wakey, wakey, eggs and bakey";
						break;
					case "es":
						expectedValue = "Buenas";
						break;
					case "es-AR":
						expectedValue = "Buen día";
						break;
					case "es-ES":
						expectedValue = "Buenos días";
						break;
					default:
						expectedValue = $"Unexpected preferred language ({preferred}), probably missing localizations.";
						break;
					}
					s = main.GetLocalizedString (key, defaultValue, tableName);
					Assert.That (s, Is.EqualTo (expectedValue), $"{tableName}/{key}");
				}

				// no matching table, so default value
				foreach (var tn in new string [] { "Base", "AnythingElse" }) {
					tableName = tn;
					key = "GoodMorning";
					expectedValue = "default";
					s = main.GetLocalizedString (key, defaultValue, tableName);
					Assert.That (s, Is.EqualTo (expectedValue), $"{tableName}/{key}");
				}

				tableName = "CustomTable";
				key = "Local Animal";
				expectedValue = "?";
				switch (preferred) {
				case "en-AU":
					expectedValue = "Quokka";
					break;
				case "en-UK":
					expectedValue = "Tiger of the Highlands";
					break;
				case "es":
					expectedValue = "Ocelote";
					break;
				case "es-AR":
					expectedValue = "Pato vapor cabeza blanca";
					break;
				case "es-ES":
					expectedValue = "Lince ibérico";
					break;
				default:
					expectedValue = $"Unexpected preferred language ({preferred}), probably missing localizations.";
					break;
				}
				s = main.GetLocalizedString (key, defaultValue, tableName);
				Assert.That (s, Is.EqualTo (expectedValue), key);
			});
		}

		[Test]
		public void GetLocalizedStringWithLanguages ()
		{
			TestRuntime.AssertXcodeVersion (16, 3);
			var main = CFBundle.GetMain ();

			Assert.Multiple (() => {
				Assert.Throws<ArgumentException> (() => main.GetLocalizedString (null, "value", "table", Array.Empty<string> ()), "Key E1");
				Assert.Throws<ArgumentException> (() => main.GetLocalizedString ("", "value", "table", Array.Empty<string> ()), "Key E2");

				Assert.Throws<ArgumentNullException> (() => main.GetLocalizedString ("key", "value", "table", (string []) null), "Localizations E1");

				var defaultValue = "default";
				string tableName;
				string key;
				string s;

				tableName = "CustomTable";
				key = "Local Animal";
				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { });
				Assert.That (s, Is.EqualTo ("Tiger of the Highlands"), $"{tableName}/{key}:[]");

				// There's no en-US translation, so the en-UK one is picked instead
				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-US" });
				Assert.That (s, Is.EqualTo ("Tiger of the Highlands"), $"{tableName}/{key}:en-US");

				// There's no de-DE translation, so the en-UK one is picked instead
				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "de-DE" });
				Assert.That (s, Is.EqualTo ("Tiger of the Highlands"), $"{tableName}/{key}:en-US");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-AU" });
				Assert.That (s, Is.EqualTo ("Quokka"), $"{tableName}/{key}:en-AU");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-UK" });
				Assert.That (s, Is.EqualTo ("Tiger of the Highlands"), $"{tableName}/{key}:en-UK");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-ES" });
				Assert.That (s, Is.EqualTo ("Lince ibérico"), $"{tableName}/{key}:es-ES");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-AR" });
				Assert.That (s, Is.EqualTo ("Pato vapor cabeza blanca"), $"{tableName}/{key}:es-AR");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es" });
				Assert.That (s, Is.EqualTo ("Ocelote"), $"{tableName}/{key}:es");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-MX" });
				Assert.That (s, Is.EqualTo ("Ocelote"), $"{tableName}/{key}:es-MX");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-AR", "es-ES" });
				Assert.That (s, Is.EqualTo ("Pato vapor cabeza blanca"), $"{tableName}/{key}:es-AR;es-ES");

				s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-ES", "es-AR" });
				Assert.That (s, Is.EqualTo ("Lince ibérico"), $"{tableName}/{key}:es-ES;es-AR");

				foreach (var tn in new string [] { "Localizable", null, "" }) {
					tableName = tn;
					key = "GoodMorning";
					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { });
					Assert.That (s, Is.EqualTo ("Wakey, wakey, eggs and bakey"), $"{tableName}/{key}:[]");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-CA" });
					Assert.That (s, Is.EqualTo ("Wakey, wakey, eggs and bakey"), $"{tableName}/{key}:en-CA");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-US" });
					Assert.That (s, Is.EqualTo ("Wakey, wakey, eggs and bakey"), $"{tableName}/{key}:en-US");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-AU" });
					Assert.That (s, Is.EqualTo ("G'day mate"), $"{tableName}/{key}:en-AU");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-UK" });
					Assert.That (s, Is.EqualTo ("Wakey, wakey, eggs and bakey"), $"{tableName}/{key}:en-UK");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-ES" });
					Assert.That (s, Is.EqualTo ("Buenos días"), $"{tableName}/{key}:es-ES");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-AR" });
					Assert.That (s, Is.EqualTo ("Buen día"), $"{tableName}/{key}:es-AR");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es" });
					Assert.That (s, Is.EqualTo ("Buenas"), $"{tableName}/{key}:es");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-MX" });
					Assert.That (s, Is.EqualTo ("Buenas"), $"{tableName}/{key}:es-MX");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-AR", "es-ES" });
					Assert.That (s, Is.EqualTo ("Buen día"), $"{tableName}/{key}:es-AR;es-ES");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-ES", "es-AR" });
					Assert.That (s, Is.EqualTo ("Buenos días"), $"{tableName}/{key}:es-ES;es-AR");
				}

				foreach (var tn in new string [] { "Base", "AnythingElse" }) {
					tableName = tn;
					key = "GoodMorning";
					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:[]");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-CA" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:en-CA");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-US" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:en-US");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-AU" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:en-AU");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "en-UK" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:en-UK");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-ES" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:es-ES");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-AR" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:es-AR");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:es");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-MX" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:es-MX");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-AR", "es-ES" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:es-AR;es-ES");

					s = main.GetLocalizedString (key, defaultValue, tableName, new string [] { "es-ES", "es-AR" });
					Assert.That (s, Is.EqualTo (defaultValue), $"{tableName}/{key}:es-ES;es-AR");
				}
			});
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
				Assert.That (loadable_x86_64, Is.True, "x86_64 Expected => true");
			else
				Assert.That (loadable_x86_64, Is.False, "x86_64 Expected => false");

			bool loadable_arm64 = CFBundle.IsArchitectureLoadable (CFBundle.Architecture.ARM64);
			if (isArm64Executable)
				Assert.That (loadable_arm64, Is.True, "arm64 Expected => true");
			// Due to Rosetta, we can't determine whether ARM64 is loadable or not if we're an X64 executable ourselves.
		}

		[Test]
		public void TestIsExecutableLoadable ()
		{
			TestRuntime.AssertXcodeVersion (12, 2);

			var main = CFBundle.GetMain ();
			var loadableBundle = CFBundle.IsExecutableLoadable (main);
			Assert.That (loadableBundle, Is.True, "loadableBundle");

			var loadableBundleUrl = CFBundle.IsExecutableLoadable (main.ExecutableUrl);
			Assert.That (loadableBundleUrl, Is.True, "loadableBundleUrl");
		}
#endif
	}
}
