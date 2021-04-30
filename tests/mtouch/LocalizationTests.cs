using System;
using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Collections.Generic;

namespace Xamarin.Tests
{
	[TestFixture]
	public class LocalizationTests
	{
		[TestCase ("cs-CZ")]
		[TestCase ("de-DE")]
		[TestCase ("es-ES")]
		[TestCase ("fr-FR")]
		[TestCase ("it-IT")]
		[TestCase ("ja-JP")]
		[TestCase ("ko-KR")]
		[TestCase ("nl")]
		[TestCase ("pt-BR")]
		[TestCase ("pt-PT")]
		[TestCase ("ru-RU")]
		[TestCase ("sv")]
		[TestCase ("tr-TR")]
		[TestCase ("zh-CN")]
		[TestCase ("zh-TW")]
		public void TestSpecificErrorCode (string culture)
		{
			var errorCode = "MT0015";
			var originalUICulture = Thread.CurrentThread.CurrentUICulture;
			var originalCulture = Thread.CurrentThread.CurrentCulture;

			try {
				var englishError = TranslateError ("en-US", errorCode);
				var newCultureError = TranslateError (culture, errorCode);
				Assert.AreNotEqual (englishError, newCultureError, $"\"{errorCode}\" is not translated in {culture}.");
			} catch (NullReferenceException){
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

			var resourceManager = new ResourceManager ("mtouch.Errors", typeof (MachO).Assembly);
			return resourceManager.GetString (errorCode, cultureInfo);
		}

		List<string> IgnoredProperties = new List<string> () {
			"ResourceManager",
			"Culture",
			"_default",
		};

		[TestCase ("cs-CZ")]
		[TestCase ("de-DE")]
		[TestCase ("es-ES")]
		[TestCase ("fr-FR")]
		[TestCase ("it-IT")]
		[TestCase ("ja-JP")]
		[TestCase ("ko-KR")]
		[TestCase ("nl")]
		[TestCase ("pt-BR")]
		[TestCase ("pt-PT")]
		[TestCase ("ru-RU")]
		[TestCase ("sv")]
		[TestCase ("tr-TR")]
		[TestCase ("zh-CN")]
		[TestCase ("zh-TW")]
		public void AllErrorTranslation (string culture)
		{
			var errorList = new StringBuilder ();

			var originalUICulture = Thread.CurrentThread.CurrentUICulture;
			var originalCulture = Thread.CurrentThread.CurrentCulture;

			// since the Xamarin.Bundler.Errors type is inaccessible, we go through MachO to access it
			var errorsAssembly = typeof (MachO).Assembly.GetType ("Xamarin.Bundler.Errors");
			var props = errorsAssembly.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

			foreach (var errorCodeInfo in props) {
				try {
					var errorCode = errorCodeInfo.Name;
					if (IgnoredProperties.Contains(errorCode))
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
	}
}

