using System;
using System.IO;
using System.Diagnostics;
using Xamarin.Tests;

using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Reflection;
using Xamarin.Bundler;

namespace Xamarin.Tests
{
	[TestFixture]
	public class LocalizationTests
	{
		[TestCase ("cs-CZ")]
		[TestCase ("de-DE")]
		[TestCase ("en-US")]
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
		public void CompareErrorCodeMT0015 (string culture)
		{
			string errorCode = "MT0015";
			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;

			try {
				string englishError = TranslateError ("en-US", errorCode);
				string newCultureError = TranslateError (culture, errorCode);

				Assert.AreNotEqual (englishError, newCultureError, $"\"{errorCode}\" is not translated in {culture}.");
			} catch (NullReferenceException){
				Assert.IsFalse (true, $"Error code \"{errorCode}\" was not found");
			} catch (Exception e) {

			} finally {
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}

		private string TranslateError (string culture, string errorCode)
		{
			CultureInfo cultureInfo = new CultureInfo (culture);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;

			var type = typeof (MachO).Assembly.GetType ("Xamarin.Bundler.Errors");
			PropertyInfo propertyInfo = type.GetProperty (errorCode, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
			var value = (string) propertyInfo.GetValue (null, null);
			var value2 = (string) propertyInfo.GetValue (null, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, null, null);

			return value;
			//PropertyInfo propertyInfo = typeof (Errors).GetProperty (errorCode);
			//return (string) propertyInfo.GetValue (null, null);
		}
	}
}

