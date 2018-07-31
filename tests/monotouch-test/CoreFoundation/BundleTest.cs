//
// Copyright 2015 Xamarin Inc
//
using System;
#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BundleTest {
#if __WATCHOS__
		const string ExpectedAppName = "monotouchtest.appex";
#elif MONOMAC
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
  						String.Format("Found bundle with null url and id {0}", b.Identifier));
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
			foreach (CFBundle b in bundles) {
				var id = b.Identifier;
				if (!String.IsNullOrEmpty (id)) {
					var otherBundle = CFBundle.Get (id);
					Assert.AreEqual (b.Info.Type, otherBundle.Info.Type,
  							 String.Format("Found bundle with diff type and id {0}", id));
					var bPath = (string) ((NSString) b.Url.Path).ResolveSymlinksInPath ();
					var otherPath = (string) ((NSString) otherBundle.Url.Path).ResolveSymlinksInPath ();
					Assert.AreEqual (bPath, otherPath,
  							 String.Format("Found bundle with diff url and id {0}", id));
				}
			}
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestGetBundleIdNull (string id)
		{
			var bundle = CFBundle.Get (id);
		}

		[Test]
		public void TestGetMain ()
		{
			var main = CFBundle.GetMain ();
#if __WATCHOS__
			var expectedBundleId = "com.xamarin.monotouch-test-watch.watchkitapp.watchkitextension";
#elif MONOMAC
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
			Assert.That(main.BuiltInPlugInsUrl.ToString (), Contains.Substring ("PlugIns/"));
		}

		[Test]
		public void TestExecutableUrl ()
		{
			var main = CFBundle.GetMain ();
#if MONOMAC
			Assert.That (main.ExecutableUrl.ToString (), Contains.Substring (ExpectedAppName + "/Contents/MacOS/xammac_tests"));
#else
			Assert.That(main.ExecutableUrl.ToString (), Contains.Substring (ExpectedAppName + "/monotouchtest"));
#endif
		}

		[Test]
		public void TestPrivateFrameworksUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That(main.PrivateFrameworksUrl.ToString (), Contains.Substring ("Frameworks/"));
		}

		[Test]
		public void TestResourcesDirectoryUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That(main.ResourcesDirectoryUrl.ToString (), Contains.Substring (ExpectedAppName + "/"));
		}

		[Test]
		public void TestSharedFrameworksUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That(main.SharedFrameworksUrl.ToString (), Contains.Substring ("SharedFrameworks/"));
		}

		[Test]
		public void TestSharedSupportUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That(main.SharedSupportUrl.ToString (), Contains.Substring ("SharedSupport/"));
		}

		[Test]
		public void TestSupportFilesDirectoryUrl ()
		{
			var main = CFBundle.GetMain ();
			Assert.That(main.SupportFilesDirectoryUrl.ToString (), Contains.Substring (ExpectedAppName + "/"));
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
			Assert.That(main.Url.ToString (), Contains.Substring (ExpectedAppName + "/"));
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
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestLocalizationsNull ()
		{
			var localizations = CFBundle.GetLocalizations (null);
		}

		[Test]
		public void TestPreferredLocalizations ()
		{
			var preferred = new string [] {"en", "es"};
			var used = CFBundle.GetPreferredLocalizations (preferred);
			Assert.IsTrue (used.Length > 0);
			Assert.That (used, Has.Exactly (1).EqualTo ("en"));
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestPreferredLocalizationsNull ()
		{
			var used = CFBundle.GetPreferredLocalizations (null);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestAuxiliaryExecutableUrlNull (string executable)
		{
			var main = CFBundle.GetMain ();
			main.GetAuxiliaryExecutableUrl (executable);
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
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlNullName (string resourceName)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrl (resourceName, "type", null);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrl ("resourceName", resourceType, null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestStaticResourceUrlNull ()
		{
			NSUrl url = null;
			CFBundle.GetResourceUrl (url, "resourceName", "resourceType", null);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestStaticResourceUrlNullName (string resourceName)
		{
			var main = CFBundle.GetMain ();
			CFBundle.GetResourceUrl (main.Url, resourceName, "resourceType", null);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestStaticResourceUrlNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			CFBundle.GetResourceUrl (main.Url, "resourceName", resourceType, null);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlsNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrls (resourceType, null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestStaticResourceUrlsNullType ()
		{
			NSUrl url = null;
			CFBundle.GetResourceUrls (url, "resourceType", null);
		}
		
		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestStaticResourceUrlsNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			CFBundle.GetResourceUrls (main.Url, resourceType, null);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlLocalNameNullName (string resourceName)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrl (resourceName, "resourceType", null, "en");
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlLocalNameNullType (string resourceType)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrl ("resourceName", resourceType, null, "en");
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlLocalNameNullLocale (string locale)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrl ("resourceName", "resourceType", null, locale);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlsLocalNameNullType (string type)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrls (type, null, "en");
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestResourceUrlsLocalNameNullLocale (string locale)
		{
			var main = CFBundle.GetMain ();
			main.GetResourceUrls ("jpg", null, locale);
		}

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestLocalizedStringNullKey (string key)
		{
			var main = CFBundle.GetMain ();
			main.GetLocalizedString (key, null, "tableName");
		} 

		[TestCase ("")]
		[TestCase (null)]
		[ExpectedException (typeof (ArgumentException))]
		public void TestLocalizedStringNullTable (string tableName)
		{
			var main = CFBundle.GetMain ();
			main.GetLocalizedString ("key", null, tableName);
		} 

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestGetLocalizationsForPreferencesNullLocalArray ()
		{
			CFBundle.GetLocalizationsForPreferences (null, new string [0]);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestGetLocalizationsForPreferencesNullPrefArray ()
		{
			CFBundle.GetLocalizationsForPreferences (new string [0], null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestGetInfoDictionaryNull ()
		{
			CFBundle.GetInfoDictionary (null);
		}
	}
}
