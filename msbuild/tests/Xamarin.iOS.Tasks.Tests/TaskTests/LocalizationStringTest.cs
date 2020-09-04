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

		[Test]
		public void OperatingSystemTranslations ()
		{

			CultureInfo culture1 = CultureInfo.CurrentCulture;
			string locale = culture1.Name;

			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			bool isTranslated = TranslationAvailable (locale, Engine.Logger.ErrorEvents [0].Message);
			Assert.IsTrue (isTranslated, "Your current locale is not supported: " + locale + ".");
		}

		[Test]
		public void AllSupportedTranslations ()
		{
			string reportString = string.Empty;
			int count = 0;
			var cultures = new List<string> () {
				"cs-CZ",
				"de-DE",
				"en-US",
				"es-ES",
				"fr-FR",
				"it-IT",
				"ja-JP",
				"ko-KR",
				"pl-PL",
				"pt-BR",
				"ru-RU",
				"tr-TR",
				"zh-CN",
				"zh-TW",
			};

			foreach (string culture in cultures) {
				CultureInfo newCulture;
				try {
					newCulture = new CultureInfo (culture);		
				}
				catch (System.Globalization.CultureNotFoundException) {
					reportString += culture + ": is not a valid culture. ";
					continue;
				}

				Thread.CurrentThread.CurrentUICulture = newCulture;

				var task = CreateTask<CollectITunesArtwork> ();
				task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

				Assert.IsFalse (task.Execute (), "Execute failure");
				Assert.AreEqual (count+1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
				bool isTranslated = TranslationAvailable (culture, Engine.Logger.ErrorEvents[count].Message);
				count++; 

				if (!isTranslated)
					reportString += culture + ": is not supported correctly. ";
			}
			Assert.AreEqual (string.Empty, reportString, "Some locales were not translated properly: " + reportString);
		}

		public bool TranslationAvailable (string locale, string respectiveErrorMessage) {
			bool isTranslated = false;
			switch (locale) {
			case "cs-CZ": 
				isTranslated = respectiveErrorMessage.Contains ("došlo k chybě: neznámý formát image");
				break;
			case "de-DE": 
				isTranslated = respectiveErrorMessage.Contains ("Unbekanntes Imageformat.");
				break;
			case "en-US": 
				isTranslated = respectiveErrorMessage.Contains ("Unknown image format.");
				break;
			case "es-ES": 
				isTranslated = respectiveErrorMessage.Contains ("formato de imagen desconocido.");
				break;
			case "fr-FR": 
				isTranslated = respectiveErrorMessage.Contains ("format d'image inconnu.");
				break;
			case "it-IT": 
				isTranslated = respectiveErrorMessage.Contains ("Formato immagine sconosciuto.");
				break;
			case "ja-JP": 
				isTranslated = respectiveErrorMessage.Contains ("の読み込みでエラーが発生しました: 画像の形式が不明です。");
				break;
			case "ko-KR": 
				isTranslated = respectiveErrorMessage.Contains ("을(를) 로드하는 동안 오류 발생: 알 수 없는 이미지 형식입니다.");
				break;
			case "pl-PL": 
				isTranslated = respectiveErrorMessage.Contains ("nieznany format obrazu.");
				break;
			case "pt-BR": 
				isTranslated = respectiveErrorMessage.Contains ("formato de imagem desconhecido.");
				break;
			case "ru-RU": 
				isTranslated = respectiveErrorMessage.Contains ("неизвестный формат изображения.");
				break;
			case "tr-TR": 
				isTranslated = respectiveErrorMessage.Contains ("yüklenirken hata oluştu: Görüntü biçimi bilinmiyor.");
				break;
			case "zh-CN": 
				isTranslated = respectiveErrorMessage.Contains ("时出错: 未知图像格式");
				break;
			case "zh-TW": 
				isTranslated = respectiveErrorMessage.Contains ("時發生錯誤: 未知的映像格式。");
				break;
			}
			return isTranslated;
		}
	}
}
