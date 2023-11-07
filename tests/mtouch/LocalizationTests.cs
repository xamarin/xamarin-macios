using System;
using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Xamarin.Bundler;

namespace Xamarin.Tests {
	[TestFixture]
	public class LocalizationTests {
		[TestCase ("cs-CZ")]
		[TestCase ("de-DE")]
		[TestCase ("es-ES")]
		[TestCase ("fr-FR")]
		[TestCase ("it-IT")]
		[TestCase ("ja-JP")]
		[TestCase ("ko-KR")]
		[TestCase ("pl-PL")]
		[TestCase ("pt-BR")]
		[TestCase ("ru-RU")]
		[TestCase ("tr-TR")]
		[TestCase ("zh-CN")]
		[TestCase ("zh-TW")]
		[Ignore ("OneLocBuild will return proper translated resx files.")]
		public void TestSpecificErrorCode (string culture)
		{
			var errorCode = "MT0015";
			var originalUICulture = Thread.CurrentThread.CurrentUICulture;
			var originalCulture = Thread.CurrentThread.CurrentCulture;

			try {
				var englishError = TranslateError ("en-US", errorCode);
				var newCultureError = TranslateError (culture, errorCode);
				Assert.AreNotEqual (englishError, newCultureError, $"\"{errorCode}\" is not translated in {culture}.");
			} catch (NullReferenceException) {
				Assert.Fail ($"Error code \"{errorCode}\" was not found");
			} catch (AssertionException) {
				throw;
			} catch (Exception e) {
				Assert.Fail ($"There was an issue obtaining the {culture} translation for {errorCode}. {e.Message}");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalUICulture;
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		private string TranslateError (string culture, string errorCode)
		{
			CultureInfo cultureInfo = new CultureInfo (culture);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;

			var resourceManager = new ResourceManager ("Errors.mtouch", typeof (MachO).Assembly);
			return resourceManager.GetString (errorCode, cultureInfo);
		}

		readonly string [] ignoredProperties = {
			"ResourceManager",
			"Culture",
			"_default",
			"default",
		};

		[TestCase ("cs-CZ")]
		[TestCase ("de-DE")]
		[TestCase ("es-ES")]
		[TestCase ("fr-FR")]
		[TestCase ("it-IT")]
		[TestCase ("ja-JP")]
		[TestCase ("ko-KR")]
		[TestCase ("pl-PL")]
		[TestCase ("pt-BR")]
		[TestCase ("ru-RU")]
		[TestCase ("tr-TR")]
		[TestCase ("zh-CN")]
		[TestCase ("zh-TW")]
		[Ignore ("OneLocBuild will return proper translated resx files.")]
		public void AllErrorTranslation (string culture)
		{
			var errorList = new StringBuilder ();

			var originalUICulture = Thread.CurrentThread.CurrentUICulture;
			var originalCulture = Thread.CurrentThread.CurrentCulture;

			foreach (var errorCodeInfo in typeof (Errors).GetProperties ()) {
				try {
					var errorCode = errorCodeInfo.Name;
					if (ignoredProperties.Contains (errorCode))
						continue;
					string englishError = TranslateError ("en-US", errorCode);
					string newCultureError = TranslateError (culture, errorCode);

					if (englishError == newCultureError)
						errorList.Append ($"{errorCode} ");
				} finally {
					Thread.CurrentThread.CurrentUICulture = originalUICulture;
					Thread.CurrentThread.CurrentCulture = originalCulture;
				}
			}
			Assert.IsEmpty (errorList.ToString (), $"The following errors were not translated:");
		}

		[Test]
		public void UpdatedResources ()
		{
			var resxPath = Path.Combine (Configuration.RootPath, "tools", "mtouch", "Errors.resx");
			var xml = XDocument.Load (resxPath);
			var resxNames = xml.Root.Descendants ().Where (n => n.Name == "data").Select (n => n.Attribute ("name").Value);
			var resxHashSet = new HashSet<string> (resxNames);

			var resourceNames = typeof (Errors).GetProperties ().Select (s => s.Name);
			var resourceHashSet = new HashSet<string> (resourceNames);

			var errorsNotInResources = string.Join (" ", resxHashSet.Where (n => !resourceHashSet.Contains (n) && !ignoredProperties.Contains (n)));
			var errorsNotInResx = string.Join (" ", resourceHashSet.Where (n => !resxHashSet.Contains (n) && !ignoredProperties.Contains (n)));

			Assert.IsEmpty (errorsNotInResources, $"The following error(s) were found in Errors.resx but not through the mtouch resources. Try to recompile the mtouch project and then the test project\n{errorsNotInResources}");
			Assert.IsEmpty (errorsNotInResx, $"The following error(s) were found in the mtouch resources but not in Errors.resx. Try to recompile the mtouch project and then the test project\n{errorsNotInResx}");
		}
	}
}
