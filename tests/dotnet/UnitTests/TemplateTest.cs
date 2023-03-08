using System.Text.Json;

#nullable enable

namespace Xamarin.Tests {
	public enum TemplateLanguage {
		CSharp,
		FSharp,
		VisualBasic,
	}

	public static class TemplateLanguageExtensions {
		public static string AsString (this TemplateLanguage @this)
		{
			return @this switch {
				TemplateLanguage.CSharp => "csharp",
				TemplateLanguage.FSharp => "fsharp",
				TemplateLanguage.VisualBasic => "visualbasic",
				_ => throw new NotImplementedException ($"'{@this} is not implemented.'")
			};
		}

		public static string AsFileExtension (this TemplateLanguage @this)
		{
			return @this switch {
				TemplateLanguage.CSharp => "csproj",
				TemplateLanguage.FSharp => "fsproj",
				TemplateLanguage.VisualBasic => "vbproj",
				_ => throw new NotImplementedException ($"'{@this} is not implemented.'")
			};
		}

		public static string AsLanguageIdentifier (this TemplateLanguage @this)
		{
			return @this switch {
				TemplateLanguage.CSharp => "C#",
				TemplateLanguage.FSharp => "F#",
				TemplateLanguage.VisualBasic => "VB",
				_ => throw new NotImplementedException ($"'{@this} is not implemented.'")
			};
		}
	}

	[TestFixture]
	public class TemplateTest : TestBaseClass {
		public struct TemplateInfo {
			public readonly ApplePlatform Platform;
			public readonly TemplateLanguage? Language;
			public readonly string Template;
			public readonly bool Execute;
			public readonly TemplateType TemplateType;

			public TemplateInfo (ApplePlatform platform, string template, TemplateLanguage? language = null, bool execute = false)
			{
				Platform = platform;
				Language = language;
				Template = template;
				Execute = execute;
				TemplateType = ParseConfig (platform, language, template);
			}

			static TemplateType ParseConfig (ApplePlatform platform, TemplateLanguage? language, string template)
			{
				var languageDir = language?.AsString () ?? "";
				var jsonPath = Path.Combine (Configuration.SourceRoot, "dotnet", "Templates", $"Microsoft.{platform.AsString ()}.Templates", template, languageDir, ".template.config", "template.json");

				var options = new JsonSerializerOptions {
					PropertyNameCaseInsensitive = true,
					IncludeFields = true,
					AllowTrailingCommas = false, // for max compat
				};
				try {
					var json = JsonSerializer.Deserialize<TemplateConfig> (File.ReadAllText (jsonPath), options);
					var type = json?.Tags?.Type;
					return Enum.Parse<TemplateType> (type!, true);
				} catch (Exception e) {
					throw new Exception ($"Failed to parse {jsonPath}", e);
				}
			}

			public override string ToString ()
			{
				return TemplateWithLanguage;
			}

			public string TemplateWithLanguage {
				get => Language.HasValue ? $"{Template}-{Language.Value.AsString ()}" : Template;
			}
		}

		public enum TemplateType {
			Project,
			Item,
		}

		public static TemplateInfo [] Templates = {
			/* project templates */
			new TemplateInfo (ApplePlatform.iOS, "ios", TemplateLanguage.CSharp),
			new TemplateInfo (ApplePlatform.iOS, "ios", TemplateLanguage.FSharp),
			new TemplateInfo (ApplePlatform.iOS, "ios", TemplateLanguage.VisualBasic),
			new TemplateInfo (ApplePlatform.iOS, "ios-tabbed"),
			new TemplateInfo (ApplePlatform.iOS, "ioslib", TemplateLanguage.CSharp),
			new TemplateInfo (ApplePlatform.iOS, "ioslib", TemplateLanguage.VisualBasic),
			new TemplateInfo (ApplePlatform.iOS, "iosbinding", TemplateLanguage.CSharp),

			new TemplateInfo (ApplePlatform.TVOS, "tvos", TemplateLanguage.CSharp),
			new TemplateInfo (ApplePlatform.TVOS, "tvos", TemplateLanguage.VisualBasic),
			new TemplateInfo (ApplePlatform.TVOS, "tvoslib", TemplateLanguage.CSharp),
			new TemplateInfo (ApplePlatform.TVOS, "tvoslib", TemplateLanguage.VisualBasic),
			new TemplateInfo (ApplePlatform.TVOS, "tvosbinding", TemplateLanguage.CSharp),

			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst", TemplateLanguage.CSharp, execute: true),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst", TemplateLanguage.VisualBasic, execute: true),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalystlib", TemplateLanguage.CSharp),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalystlib", TemplateLanguage.VisualBasic),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalystbinding", TemplateLanguage.CSharp),

			new TemplateInfo (ApplePlatform.MacOSX, "macos", TemplateLanguage.CSharp, execute: true),
			new TemplateInfo (ApplePlatform.MacOSX, "macos", TemplateLanguage.VisualBasic, execute: true),
			new TemplateInfo (ApplePlatform.MacOSX, "macoslib", TemplateLanguage.CSharp),
			new TemplateInfo (ApplePlatform.MacOSX, "macoslib", TemplateLanguage.VisualBasic),
			new TemplateInfo (ApplePlatform.MacOSX, "macosbinding", TemplateLanguage.CSharp),

			/* item templates */
			new TemplateInfo (ApplePlatform.iOS, "ios-controller"),
			new TemplateInfo (ApplePlatform.iOS, "ios-storyboard"),
			new TemplateInfo (ApplePlatform.iOS, "ios-view"),
			new TemplateInfo (ApplePlatform.iOS, "ios-viewcontroller"),

			new TemplateInfo (ApplePlatform.TVOS, "tvos-controller"),
			new TemplateInfo (ApplePlatform.TVOS, "tvos-storyboard"),
			new TemplateInfo (ApplePlatform.TVOS, "tvos-view"),
			new TemplateInfo (ApplePlatform.TVOS, "tvos-viewcontroller"),

			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst-controller"),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst-storyboard"),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst-view"),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst-viewcontroller"),

			new TemplateInfo (ApplePlatform.MacOSX, "macos-controller"),
			new TemplateInfo (ApplePlatform.MacOSX, "macos-storyboard"),
			new TemplateInfo (ApplePlatform.MacOSX, "macos-view"),
			new TemplateInfo (ApplePlatform.MacOSX, "macos-viewcontroller"),
		};

		public static TemplateInfo [] GetProjectTemplates ()
		{
			return Templates.Where (v => v.TemplateType == TemplateType.Project).ToArray ();
		}

		public class TemplateConfig {
			public string? Name;
			public string? ShortName;
			public TemplateConfigTag? Tags;
		}

		public class TemplateConfigTag {
			public string? Language;
			public string? Type;
		}

		[Test]
		public void AreAllTemplatesListed ()
		{
			var allListedTemplates = Templates.Select (v => v.TemplateWithLanguage).ToArray ();
			var allTemplates = new List<string> ();

			foreach (var platform in Enum.GetValues<ApplePlatform> ()) {
				var dir = Path.Combine (Configuration.SourceRoot, "dotnet", "Templates", $"Microsoft.{platform.AsString ()}.Templates");
				if (!Directory.Exists (dir))
					continue;

				var templateDirectories = Directory.GetDirectories (dir);

				// read the template's configuration to figure out if it's a project template, and if not, skip it
				foreach (var template in templateDirectories) {
					var templateDir = Path.Combine (dir, template);
					var hasFoundLangTemplate = false;

					foreach (var language in Enum.GetValues<TemplateLanguage> ()) {
						var langDir = Path.Combine (templateDir, language.AsString ());
						if (!Directory.Exists (langDir))
							continue;

						var jsonPath = Path.Combine (langDir, ".template.config", "template.json");
						Assert.That (jsonPath, Does.Exist, "Config file must exist");
						if (!File.Exists (jsonPath))
							continue;

						allTemplates.Add ($"{Path.GetFileName (template)}-{language.AsString ()}");
						hasFoundLangTemplate = true;
					}

					if (!hasFoundLangTemplate) {
						var rootJsonPath = Path.Combine (templateDir, ".template.config", "template.json");
						Assert.That (rootJsonPath, Does.Exist, "Config file must exist");
						if (!File.Exists (rootJsonPath))
							continue;

						allTemplates.Add (Path.GetFileName (template));
					}
				}
			}
			Assert.That (allListedTemplates, Is.EquivalentTo (allTemplates), "The listed templates here and the templates on disk don't match");
		}

		[Test]
		[TestCaseSource (nameof (GetProjectTemplates))]
		public void CreateAndBuildProjectTemplate (TemplateInfo info)
		{
			Configuration.IgnoreIfIgnoredPlatform (info.Platform);

			var language = info.Language ?? TemplateLanguage.CSharp;

			var tmpDir = Cache.CreateTemporaryDirectory ();
			var outputDir = Path.Combine (tmpDir, info.Template);
			DotNet.AssertNew (outputDir, info.Template, language: language.AsLanguageIdentifier ());
			var proj = Path.Combine (outputDir, $"{info.Template}.{language.AsFileExtension ()}");
			var rv = DotNet.AssertBuild (proj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			if (info.Execute) {
				var platform = info.Platform;
				var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);

				Assert.IsTrue (CanExecute (info.Platform, runtimeIdentifiers), "Must be executable to execute!");

				// First add some code to exit the template if it launches successfully.
				InsertCodeToExitAppAfterLaunch (language, outputDir);

				// Build the sample
				rv = DotNet.AssertBuild (proj);

				// There should still not be any warnings
				warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
				Assert.That (warnings, Is.Empty, $"Build warnings (2):\n\t{string.Join ("\n\t", warnings)}");

				var appPath = GetAppPath (proj, platform, runtimeIdentifiers);
				var appExecutable = GetNativeExecutable (platform, appPath);
				ExecuteWithMagicWordAndAssert (appExecutable);
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS, TemplateLanguage.CSharp)]
		[TestCase (ApplePlatform.iOS, TemplateLanguage.FSharp)]
		[TestCase (ApplePlatform.TVOS, TemplateLanguage.CSharp)]
		[TestCase (ApplePlatform.MacCatalyst, TemplateLanguage.CSharp)]
		[TestCase (ApplePlatform.MacOSX, TemplateLanguage.CSharp)]
		public void CreateAndBuildItemTemplates (ApplePlatform platform, TemplateLanguage language)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			// We create a new project from the basic project template, and then we add all the
			// item templates for the given platforms. This is faster than testing the item templates
			// one-by-one.

			bool IsMatching (TemplateLanguage lang, TemplateLanguage? value)
			{
				if (lang == TemplateLanguage.CSharp)
					return value is null or TemplateLanguage.CSharp;

				return lang == value;
			}

			var info = Templates.Single (v => string.Equals (v.Template, platform.AsString (), StringComparison.OrdinalIgnoreCase) && IsMatching (language, v.Language));
			var itemTemplates = Templates.Where (v => v.TemplateType == TemplateType.Item && v.Platform == platform && IsMatching (language, v.Language));

			var tmpDir = Cache.CreateTemporaryDirectory ();
			var outputDir = Path.Combine (tmpDir, info.Template);
			DotNet.AssertNew (outputDir, info.Template, language: language.AsLanguageIdentifier ());

			foreach (var item in itemTemplates)
				DotNet.AssertNew (outputDir, item.Template, "item_" + item.Template, language: language.AsLanguageIdentifier ());

			var proj = Path.Combine (outputDir, $"{info.Template}.{language.AsFileExtension ()}");
			var rv = DotNet.AssertBuild (proj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			if (info.Execute) {
				var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);

				Assert.IsTrue (CanExecute (info.Platform, runtimeIdentifiers), "Must be executable to execute!");

				// First add some code to exit the template if it launches successfully.
				InsertCodeToExitAppAfterLaunch (language, outputDir);

				// Build the sample
				rv = DotNet.AssertBuild (proj);

				// There should still not be any warnings
				warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
				Assert.That (warnings, Is.Empty, $"Build warnings (2):\n\t{string.Join ("\n\t", warnings)}");

				var appPath = GetAppPath (proj, platform, runtimeIdentifiers);
				var appExecutable = GetNativeExecutable (platform, appPath);
				ExecuteWithMagicWordAndAssert (appExecutable);
			}
		}

		static void InsertCodeToExitAppAfterLaunch (TemplateLanguage language, string outputDir)
		{
			switch (language) {
			case TemplateLanguage.CSharp:
				InsertCSharpCodeToExitAppAfterLaunch (outputDir);
				break;
			case TemplateLanguage.FSharp:
				InsertFSharpCodeToExitAppAfterLaunch (outputDir);
				break;
			case TemplateLanguage.VisualBasic:
				InsertVBCodeToExitAppAfterLaunch (outputDir);
				break;
			default:
				throw new NotImplementedException ($"'Inserting {language} code is not implemented.'");
			}
		}

		static void InsertCSharpCodeToExitAppAfterLaunch (string outputDir)
		{
			var mainFile = Path.Combine (outputDir, "Main.cs");
			var mainContents = File.ReadAllText (mainFile);
			var exitSampleWithSuccess = @"NSTimer.CreateScheduledTimer (1, (v) => {
Console.WriteLine (Environment.GetEnvironmentVariable (""MAGIC_WORD""));
Environment.Exit (0);
		});
		";
			var modifiedMainContents =
				mainContents.Replace ("// This is the main entry point of the application.",
					exitSampleWithSuccess);
			Assert.AreNotEqual (modifiedMainContents, mainContents, "Failed to modify the main content");
			File.WriteAllText (mainFile, modifiedMainContents);
		}

		static void InsertFSharpCodeToExitAppAfterLaunch (string outputDir)
		{
			var mainFile = Path.Combine (outputDir, "Main.fs");
			var mainContents = File.ReadAllText (mainFile);

			// Attention: The indentation is important in F#, so to avoid having to deal with that, put everything on a single line.
			var exitSampleWithSuccess =
				@"Foundation.NSTimer.CreateScheduledTimer(1, fun _ -> System.Console.WriteLine(System.Environment.GetEnvironmentVariable(""MAGIC_WORD"")); System.Environment.Exit(0)) |> ignore";

			var modifiedMainContents =
				mainContents.Replace ("// This is the main entry point of the application.",
					exitSampleWithSuccess);
			Assert.AreNotEqual (modifiedMainContents, mainContents, "Failed to modify the main content");
			File.WriteAllText (mainFile, modifiedMainContents);
		}

		static void InsertVBCodeToExitAppAfterLaunch (string outputDir)
		{
			var mainFile = Path.Combine (outputDir, "Main.vb");
			var mainContents = File.ReadAllText (mainFile);
			var exitSampleWithSuccess = @"Foundation.NSTimer.CreateScheduledTimer (1,
Sub (ByVal v)
	Console.WriteLine (Environment.GetEnvironmentVariable (""MAGIC_WORD""))
	Environment.Exit (0)
End Sub
	)
		";
			var modifiedMainContents =
				mainContents.Replace ("' This is the main entry point of the application.",
					exitSampleWithSuccess);
			Assert.AreNotEqual (modifiedMainContents, mainContents, "Failed to modify the main content");
			File.WriteAllText (mainFile, modifiedMainContents);
		}
	}
}
