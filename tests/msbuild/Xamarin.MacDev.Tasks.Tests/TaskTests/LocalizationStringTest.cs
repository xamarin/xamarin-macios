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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xamarin.Tests;

namespace Xamarin.MacDev.Tasks {
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
			CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo newCulture;
			try {
				newCulture = new CultureInfo (culture);
				Thread.CurrentThread.CurrentUICulture = newCulture;
				var task = CreateTask<CollectITunesArtwork> ();
				task.ITunesArtwork = new TaskItem [] { new TaskItem (Assembly.GetExecutingAssembly ().Location) };

				Assert.IsFalse (task.Execute (), "Execute failure");
				Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "ErrorCount");
				bool isTranslated = Engine.Logger.ErrorEvents [0].Message.Contains (errorMessage);
				Assert.IsTrue (isTranslated, $"Should contain \"{errorMessage}\", but instead has value: \"{Engine.Logger.ErrorEvents [0].Message}\"");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalUICulture;
				Thread.CurrentThread.CurrentCulture = originalCulture;
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
			CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

			try {
				Assert.IsFalse (string.IsNullOrEmpty (errorCode), "Error code is null or empty");
				string englishError = TranslateError ("en-US", errorCode);
				string newCultureError = TranslateError (culture, errorCode);

				Assert.AreNotEqual (englishError, newCultureError, $"\"{errorCode}\" is not translated in {culture}.");
			} catch (NullReferenceException) {
				Assert.Fail ($"Error code \"{errorCode}\" was not found");
			} finally {
				Thread.CurrentThread.CurrentUICulture = originalUICulture;
				Thread.CurrentThread.CurrentCulture = originalCulture;
			}
		}

		private string TranslateError (string culture, string errorCode)
		{
			CultureInfo cultureInfo = new CultureInfo (culture);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			PropertyInfo propertyInfo = typeof (MSBStrings).GetProperty (errorCode);
			return (string) propertyInfo.GetValue (null, null);
		}

		readonly string [] ignoreList = {
			"ResourceManager",
			"Culture",
		};

		[Test]
		public void UpdatedResources ()
		{
			var resxPath = Path.Combine (Configuration.RootPath, "msbuild", "Xamarin.Localization.MSBuild", "MSBStrings.resx");
			var xml = XDocument.Load (resxPath);
			var resxNames = xml.Root.Descendants ().Where (n => n.Name == "data").Select (n => n.Attribute ("name").Value);
			var resxHashSet = new HashSet<string> (resxNames);
			var resourceNames = typeof (MSBStrings).GetProperties ().Select (s => s.Name);
			var resourceHashSet = new HashSet<string> (resourceNames);

			var errorsNotInResources = string.Join (" ", resxHashSet.Where (n => !resourceHashSet.Contains (n) && !ignoreList.Contains (n)));
			var errorsNotInResx = string.Join (" ", resourceHashSet.Where (n => !resxHashSet.Contains (n) && !ignoreList.Contains (n)));

			Assert.IsEmpty (errorsNotInResources, $"The following error(s) were found in MSBStrings.resx but not through the MSBStrings resource. Try to recompile the msbuild project and then the test project\n{errorsNotInResources}");
			Assert.IsEmpty (errorsNotInResx, $"The following error(s) were found in the MSBStrings resource but not in MSBStrings.resx. Try to recompile the msbuild project and then the test project\n{errorsNotInResx}");
		}
	}
}
