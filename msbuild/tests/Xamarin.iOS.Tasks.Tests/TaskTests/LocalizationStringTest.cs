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
			CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
			string locale = currentCulture.Name;

			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			bool isTranslated = TranslationAvailable (locale, Engine.Logger.ErrorEvents [0].Message);
			Assert.IsTrue (isTranslated, "Your current locale is not supported: " + locale + ".");
		}

		[TestCase ("cs-CZ")]
		[TestCase ("de-DE")]
		[TestCase ("en-US")]
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
		public void AllSupportedTranslations (string culture)
		{
			CultureInfo originalCulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo newCulture;
			try {
				newCulture = new CultureInfo (culture);
				Thread.CurrentThread.CurrentUICulture = newCulture;
			}
			catch (System.Globalization.CultureNotFoundException) {
				Assert.IsTrue (false, culture + ": is not a valid culture. ");
			}

			var task = CreateTask<CollectITunesArtwork> ();
			task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

			Assert.IsFalse (task.Execute (), "Execute failure");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
			bool isTranslated = TranslationAvailable (culture, Engine.Logger.ErrorEvents[0].Message);
			Assert.IsTrue (isTranslated, culture + ": is not supported correctly. ");

			Thread.CurrentThread.CurrentUICulture = originalCulture;
		}

		public bool TranslationAvailable (string locale, string respectiveErrorMessage) {
			var translatedMessages = new Dictionary<string, string> (){
				["cs-CZ"] = "došlo k chybě: neznámý formát image",
				["de-DE"] =  "Unbekanntes Imageformat.",
				["en-US"] =  "Unknown image format.",
				["es-ES"] =  "formato de imagen desconocido.",
				["fr-FR"] =  "format d'image inconnu.",
				["it-IT"] =  "Formato immagine sconosciuto.",
				["ja-JP"] =  "の読み込みでエラーが発生しました: 画像の形式が不明です。",
				["ko-KR"] =  "을(를) 로드하는 동안 오류 발생: 알 수 없는 이미지 형식입니다.",
				["pl-PL"] =  "nieznany format obrazu.",
				["pt-BR"] =  "formato de imagem desconhecido.",
				["ru-RU"] =  "неизвестный формат изображения.",
				["tr-TR"] =  "yüklenirken hata oluştu: Görüntü biçimi bilinmiyor.",
				["zh-CN"] =  "时出错: 未知图像格式",
				["zh-TW"] =  "時發生錯誤: 未知的映像格式。",
			};

			if (translatedMessages.ContainsKey (locale) && respectiveErrorMessage.Contains (translatedMessages[locale]))
				return true;
			else
				return false;
		}
	}
}
