using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xamarin.Tests {
	[TestFixture]
	public class TemplateTest : TestBaseClass {
		public struct TemplateInfo {
			public readonly ApplePlatform Platform;
			public readonly string Template;
			public readonly bool Execute;
			public readonly TemplateType TemplateType;

			public TemplateInfo (ApplePlatform platform, string template, bool execute = false)
			{
				Platform = platform;
				Template = template;
				Execute = execute;
				TemplateType = ParseConfig (Platform, template);
			}

			static TemplateType ParseConfig (ApplePlatform platform, string template)
			{
				// read the template's configuration to figure out if it's a project template, and if not, skip it
				var dir = Path.Combine (Configuration.SourceRoot, "dotnet", "Templates", $"Microsoft.{platform.AsString ()}.Templates");
				var jsonPath = Path.Combine (dir, template, ".template.config", "template.json");
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
			new TemplateInfo (ApplePlatform.iOS, "ios"),
			new TemplateInfo (ApplePlatform.iOS, "ios-tabbed"),
			new TemplateInfo (ApplePlatform.iOS, "ioslib"),
			new TemplateInfo (ApplePlatform.iOS, "iosbinding"),

			new TemplateInfo (ApplePlatform.TVOS, "tvos"),
			new TemplateInfo (ApplePlatform.TVOS, "tvoslib"),
			new TemplateInfo (ApplePlatform.TVOS, "tvosbinding"),

			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst", execute: true),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalystlib"),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalystbinding"),

			new TemplateInfo (ApplePlatform.MacOSX, "macos", execute: true),
			new TemplateInfo (ApplePlatform.MacOSX, "macoslib"),
			new TemplateInfo (ApplePlatform.MacOSX, "macosbinding"),

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
