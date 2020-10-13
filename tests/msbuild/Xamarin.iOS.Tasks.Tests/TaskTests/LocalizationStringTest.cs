using System.IO;
using System.Reflection;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

namespace Xamarin.iOS.Tasks {
	[TestFixture]
	public class LocalizationStringTest : TestBase {
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
		public void SpecificErrorTranslation (string culture)
		{
			var task = CreateTask<LocalizationString> ();

			// You can change this value to the Error you want to test
			task.MSBErrorCode = "E7069";

			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo englishUSCulture;
			CultureInfo newCulture;
			try {
				englishUSCulture = new CultureInfo ("en-US");
				Thread.CurrentThread.CurrentUICulture = englishUSCulture;
				Assert.IsFalse (task.Execute (), "Execute failure");
				Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
				string englishError = Engine.Logger.ErrorEvents[0].Message;
				Assert.AreNotEqual (englishError, "Error code not found", "Error code not found.");

				newCulture = new CultureInfo (culture);
				Thread.CurrentThread.CurrentUICulture = newCulture;
				Assert.IsFalse (task.Execute (), "Execute failure");
				Assert.AreEqual (2, Engine.Logger.ErrorEvents.Count, "ErrorCount");
				string newCultureError = Engine.Logger.ErrorEvents[1].Message;

				Assert.AreNotEqual (englishError, newCultureError, $"\"{task.MSBErrorCode}\" is not translated in {culture}.");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}
	}
}
