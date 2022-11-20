using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xamarin.Tests {
	[TestFixture]
	public class TemplateTest : TestBaseClass {
		public enum TemplateLanguage {
			Undefined,
			CSharp,
			FSharp
		}

		public struct TemplateInfo {
			public readonly ApplePlatform Platform;
			public readonly TemplateLanguage Language;
			public readonly string Template;
			public readonly bool Execute;
			public readonly TemplateType TemplateType;

			public TemplateInfo (ApplePlatform platform, TemplateLanguage language, string template, bool execute = false)
			{
				Platform = platform;
				Language = language;
				Template = template;
				Execute = execute;
				TemplateType = ParseConfig (Platform, language, template);
			}

			static TemplateType ParseConfig (ApplePlatform platform, TemplateLanguage language, string template)
			{
				// read the template's configuration to figure out if it's a project template, and if not, skip it
				var dir = Path.Combine (Configuration.SourceRoot, "dotnet", "Templates", $"Microsoft.{platform.AsString ()}.Templates");
				var rootPath = Path.Combine (dir, template);

				switch (language) {
				case TemplateLanguage.Undefined:
					break;
				case TemplateLanguage.CSharp:
					rootPath = Path.Combine (rootPath, "csharp");
					break;
				case TemplateLanguage.FSharp:
					rootPath = Path.Combine (rootPath, "fsharp");
					break;
				}

				var jsonPath = Path.Combine (rootPath, ".template.config", "template.json");

				var options = new JsonSerializerOptions {
					PropertyNameCaseInsensitive = true,
					IncludeFields = true,
					AllowTrailingCommas = false, // for max compat
				};
				try {
					var json = JsonSerializer.Deserialize<TemplateConfig> (File.ReadAllText (jsonPath), options);
					var type = json.Tags.Type;
					return Enum.Parse<TemplateType> (type, true);
				} catch (Exception e) {
					throw new Exception ($"Failed to parse {jsonPath}", e);
				}
			}

			public override string ToString ()
			{
				return Template;
			}
		}

		public enum TemplateType {
			Project,
			Item,
		}

		public static TemplateInfo [] Templates = {
			/* project templates */
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.CSharp, "ios"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.FSharp, "ios"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "ios-tabbed"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "ioslib"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "iosbinding"),

			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvos"),
			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvoslib"),
			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvosbinding"),

			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalyst", execute: true),
			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalystlib"),
			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalystbinding"),

			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macos", execute: true),
			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macoslib"),
			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macosbinding"),

			/* item templates */
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "ios-controller"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "ios-storyboard"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "ios-view"),
			new TemplateInfo (ApplePlatform.iOS, TemplateLanguage.Undefined, "ios-viewcontroller"),

			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvos-controller"),
			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvos-storyboard"),
			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvos-view"),
			new TemplateInfo (ApplePlatform.TVOS, TemplateLanguage.Undefined, "tvos-viewcontroller"),

			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalyst-controller"),
			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalyst-storyboard"),
			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalyst-view"),
			new TemplateInfo (ApplePlatform.MacCatalyst, TemplateLanguage.Undefined, "maccatalyst-viewcontroller"),

			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macos-controller"),
			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macos-storyboard"),
			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macos-view"),
			new TemplateInfo (ApplePlatform.MacOSX, TemplateLanguage.Undefined, "macos-viewcontroller"),
		};

		public static TemplateInfo [] GetProjectTemplates ()
		{
			return Templates.Where (v => v.TemplateType == TemplateType.Project).ToArray ();
		}

		public class TemplateConfig {
			public string Name;
			public string ShortName;
			public TemplateConfigTag Tags;
		}

		public class TemplateConfigTag {
			public string Language;
			public string Type;
		}

		[Test]
		public void AreAllTemplatesListed ()
		{
			var allListedTemplates = Templates.Select (v => v.Template).ToArray ();
			var allTemplates = new List<string> ();
			foreach (var platform in Enum.GetValues<ApplePlatform> ()) {
				var dir = Path.Combine (Configuration.SourceRoot, "dotnet", "Templates", $"Microsoft.{platform.AsString ()}.Templates");
				if (!Directory.Exists (dir))
					continue;

				var templateDirectories = Directory.GetDirectories (dir);
				var options = new JsonSerializerOptions {
					PropertyNameCaseInsensitive = true,
					IncludeFields = true,
				};

				// read the template's configuration to figure out if it's a project template, and if not, skip it
				foreach (var templateDir in templateDirectories) {
					var jsonPath = Path.Combine (templateDir, ".template.config", "template.json");
					Assert.That (jsonPath, Does.Exist, "Config file must exist");
					if (!File.Exists (jsonPath))
						continue;

					allTemplates.Add (Path.GetFileName (templateDir));
				}
			}
			Assert.That (allListedTemplates, Is.EquivalentTo (allTemplates), "The listed templates here and the templates on disk don't match");
		}

		[Test]
		[TestCaseSource (nameof (GetProjectTemplates))]
		public void CreateAndBuildProjectTemplate (TemplateInfo info)
		{
			Configuration.IgnoreIfIgnoredPlatform (info.Platform);
			var tmpDir = Cache.CreateTemporaryDirectory ();
			var outputDir = Path.Combine (tmpDir, info.Template);
			DotNet.AssertNew (outputDir, info.Template);
			var csproj = Path.Combine (outputDir, info.Template + ".csproj");
			var rv = DotNet.AssertBuild (csproj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			if (info.Execute) {
				var platform = info.Platform;
				var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);

				Assert.IsTrue (CanExecute (info.Platform, runtimeIdentifiers), "Must be executable to execute!");

				// First add some code to exit the template if it launches successfully.
				var mainFile = Path.Combine (outputDir, "Main.cs");
				var mainContents = File.ReadAllText (mainFile);
				var exitSampleWithSuccess = @"NSTimer.CreateScheduledTimer (1, (v) => {
	Console.WriteLine (Environment.GetEnvironmentVariable (""MAGIC_WORD""));
	Environment.Exit (0);
			});
			";
				var modifiedMainContents = mainContents.Replace ("// This is the main entry point of the application.", exitSampleWithSuccess);
				Assert.AreNotEqual (modifiedMainContents, mainContents, "Failed to modify the main content");
				File.WriteAllText (mainFile, modifiedMainContents);

				// Build the sample
				rv = DotNet.AssertBuild (csproj);

				// There should still not be any warnings
				warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
				Assert.That (warnings, Is.Empty, $"Build warnings (2):\n\t{string.Join ("\n\t", warnings)}");

				var appPath = GetAppPath (csproj, platform, runtimeIdentifiers);
				var appExecutable = GetNativeExecutable (platform, appPath);
				ExecuteWithMagicWordAndAssert (appExecutable);
			}
		}

		[Test]
		[TestCase (ApplePlatform.iOS)]
		[TestCase (ApplePlatform.TVOS)]
		[TestCase (ApplePlatform.MacCatalyst)]
		[TestCase (ApplePlatform.MacOSX)]
		public void CreateAndBuildItemTemplates (ApplePlatform platform)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);

			// We create a new project from the basic project template, and then we add all the
			// item templates for the given platforms. This is faster than testing the item templates
			// one-by-one.

			var info = Templates.Single (v => string.Equals (v.Template, platform.AsString (), StringComparison.OrdinalIgnoreCase));
			var itemTemplates = Templates.Where (v => v.TemplateType == TemplateType.Item && v.Platform == platform);

			var tmpDir = Cache.CreateTemporaryDirectory ();
			var outputDir = Path.Combine (tmpDir, info.Template);
			DotNet.AssertNew (outputDir, info.Template);

			foreach (var item in itemTemplates)
				DotNet.AssertNew (outputDir, item.Template, "item_" + item.Template);

			var csproj = Path.Combine (outputDir, info.Template + ".csproj");
			var rv = DotNet.AssertBuild (csproj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");

			if (info.Execute) {
				var runtimeIdentifiers = GetDefaultRuntimeIdentifier (platform);

				Assert.IsTrue (CanExecute (info.Platform, runtimeIdentifiers), "Must be executable to execute!");

				// First add some code to exit the template if it launches successfully.
				var mainFile = Path.Combine (outputDir, "Main.cs");
				var mainContents = File.ReadAllText (mainFile);
				var exitSampleWithSuccess = @"NSTimer.CreateScheduledTimer (1, (v) => {
	Console.WriteLine (Environment.GetEnvironmentVariable (""MAGIC_WORD""));
	Environment.Exit (0);
			});
			";
				var modifiedMainContents = mainContents.Replace ("// This is the main entry point of the application.", exitSampleWithSuccess);
				Assert.AreNotEqual (modifiedMainContents, mainContents, "Failed to modify the main content");
				File.WriteAllText (mainFile, modifiedMainContents);

				// Build the sample
				rv = DotNet.AssertBuild (csproj);

				// There should still not be any warnings
				warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
				Assert.That (warnings, Is.Empty, $"Build warnings (2):\n\t{string.Join ("\n\t", warnings)}");

				var appPath = GetAppPath (csproj, platform, runtimeIdentifiers);
				var appExecutable = GetNativeExecutable (platform, appPath);
				ExecuteWithMagicWordAndAssert (appExecutable);
			}
		}
	}
}
