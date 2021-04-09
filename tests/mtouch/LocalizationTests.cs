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
			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;

			try {
				string englishError = TranslateError ("en-US");
				string newCultureError = TranslateError (culture);

				Assert.AreNotEqual (englishError, newCultureError, $"\"MT0003\" is not translated in {culture}.");
			} catch (NullReferenceException){
				Assert.IsFalse (true, $"Error code \"MT0003\" was not found");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}

		private string TranslateError (string culture)
		{
			CultureInfo cultureInfo = new CultureInfo (culture);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;


			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Abi = "invalid-arm";
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				var messages = mtouch.Messages.GetEnumerator ();
				messages.MoveNext ();
				return messages.Current.Message;
			}

			//PropertyInfo propertyInfo = typeof (Errors).GetProperty (errorCode);
			//return (string) propertyInfo.GetValue (null, null);
		}
	}
}

