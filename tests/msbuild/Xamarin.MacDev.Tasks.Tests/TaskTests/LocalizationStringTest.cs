using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using Xamarin.Localization.MSBuild;
using System.ComponentModel;

namespace Xamarin.iOS.Tasks {
	[TestFixture]
	public class LocalizationStringTest : TestBase {
		[TestCase ("cs-CZ", "došlo k chybě: neznámý formát image")]
		[TestCase ("de-DE", "Unbekanntes Imageformat.")]
		[TestCase ("en-US", "Unknown image format.")]
		[TestCase ("es-ES", "formato de imagen desconocido.")]
		[TestCase ("fr-FR", "format d'image inconnu.")]
		[TestCase ("it-IT", "Formato immagine sconosciuto.")]
		[TestCase ("ja-JP", "の読み込みでエラーが発生しました: 画像の形式が不明です。")]
		[TestCase ("ko-KR", "을(를) 로드하는 동안 오류 발생: 알 수 없는 이미지 형식입니다.")]
		[TestCase ("pl-PL", "nieznany format obrazu.")]
		[TestCase ("pt-BR", "formato de imagem desconhecido.")]
		[TestCase ("ru-RU", "неизвестный формат изображения.")]
		[TestCase ("tr-TR", "yüklenirken hata oluştu: Görüntü biçimi bilinmiyor.")]
		[TestCase ("zh-CN", "时出错: 未知图像格式")]
		[TestCase ("zh-TW", "時發生錯誤: 未知的映像格式。")]
		public void AllSupportedTranslations (string culture, string errorMessage)
		{
			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo newCulture;
			try {
				newCulture = new CultureInfo (culture);
				Thread.CurrentThread.CurrentUICulture = newCulture;
				var task = CreateTask<CollectITunesArtwork> ();
				task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

				Assert.IsFalse (task.Execute (), "Execute failure");
				Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
				bool isTranslated = Engine.Logger.ErrorEvents[0].Message.Contains (errorMessage);
				Assert.IsTrue (isTranslated, culture + ": is not supported correctly. ");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}

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
			// insert which error code you'd like to test
			string errorCode = "E0007";
			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;

			try {
				Assert.IsFalse (string.IsNullOrEmpty (errorCode), "Error code is null or empty");
				string englishError = TranslateError ("en-US", errorCode);
				string newCultureError = TranslateError (culture, errorCode);

				Assert.AreNotEqual (englishError, newCultureError, $"\"{errorCode}\" is not translated in {culture}.");
			} catch (NullReferenceException){
				Assert.IsFalse (true, $"Error code \"{errorCode}\" was not found");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}

		private string TranslateError (string culture, string errorCode)
		{
			CultureInfo cultureInfo = new CultureInfo (culture);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			PropertyInfo propertyInfo = typeof (MSBStrings).GetProperty (errorCode);
			return (string) propertyInfo.GetValue (null, null);
		}

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
		public void AllErrorTranslation (string culture)
		{
			string errorCode = string.Empty;
			string errorList = string.Empty;
			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;
			string path = $"{Directory.GetCurrentDirectory ()}/TaskTests/LocalizationIgnore";

			List<string> commonIgnoreList = ReadFile ($"{path}/common-Translations.ignore");
			List<string> cultureIgnoreList = ReadFile ($"{path}/{culture}-Translations.ignore");

			foreach (var errorCodeInfo in typeof (MSBStrings).GetProperties ()) {
				try {
					errorCode = errorCodeInfo.Name.ToString ();
					if (errorCode == "ResourceManager" || errorCode == "Culture")
						continue;

					Assert.IsFalse (string.IsNullOrEmpty (errorCode), "Error code is null or empty");
					string englishError = TranslateError ("en-US", errorCode);
					string newCultureError = TranslateError (culture, errorCode);

					if (commonIgnoreList.Contains (errorCode)) {
						if (englishError != newCultureError)
							Assert.IsFalse (true, $"{errorCode} is translated. Remove {errorCode} from ~/LocalizationIgnore/common-Translations.ignore");
					} else if (cultureIgnoreList.Contains (errorCode)) {
						if (englishError != newCultureError)
							Assert.IsFalse (true, $"{errorCode} is translated. Remove {errorCode} from ~/LocalizationIgnore/{culture}-Translations.ignore");
					} else if (englishError == newCultureError)
						errorList += $"{errorCode} ";
				} finally {
					Thread.CurrentThread.CurrentUICulture = originalCulture;
				}
			}

			Assert.AreEqual (string.Empty, errorList, $"The following errors were not translated. Add them to ~/LocalizationIgnore/common-Translations.ignore or ~/LocaliztionIgnore/{culture}-Translations.ignore");
		}

		private List<string> ReadFile (string path)
		{
			List<string> ignoreList = new List<string> ();
			string temp;
			try {
				using (var sr = new StreamReader (path)) {
					while (!sr.EndOfStream){
						if (sr.Peek () == -1)
							break;
						temp = sr.ReadLine ();
						if (string.IsNullOrEmpty (temp) || temp.Trim ()[0] == '#') {
							continue;
						} else {
							ignoreList.Add (temp.Trim ());
						}
					}
				}
			} catch (IOException e) {
				Assert.IsTrue (false, $"Could not read file at: {path}");
			}
			return ignoreList;
		}
	}
}
