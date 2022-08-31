using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xamarin.Tests {
	[TestFixture]
	public class TemplateTest : TestBaseClass {
		public struct TemplateInfo {
			public readonly ApplePlatform Platform;
			public readonly string Template;
			public readonly bool ValidateSuccessfulBuild;
			public readonly bool Execute;

			public TemplateInfo (ApplePlatform platform, string template, bool validateSuccessfulBuild = true, bool execute = false)
			{
				Platform = platform;
				Template = template;
				ValidateSuccessfulBuild = validateSuccessfulBuild;
				Execute = execute;
			}

			public override string ToString ()
			{
				return $"Platform: {Platform} Template: {Template} ValidateSuccessfulBuild: {ValidateSuccessfulBuild} Execute: {Execute}";
			}
		}

		public static TemplateInfo[] Templates = {
			new TemplateInfo (ApplePlatform.iOS, "ios"),
			new TemplateInfo (ApplePlatform.iOS, "ios-tabbed"),
			new TemplateInfo (ApplePlatform.iOS, "ioslib"),
			new TemplateInfo (ApplePlatform.iOS, "iosbinding", false), // Bindings can not build without a native library assigned
			new TemplateInfo (ApplePlatform.TVOS, "tvos"),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalyst", execute: true),
			new TemplateInfo (ApplePlatform.MacCatalyst, "maccatalystbinding", false), // Bindings can not build without a native library assigned
			new TemplateInfo (ApplePlatform.MacOSX, "macos", execute: true),
		};

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
					if (!File.Exists (jsonPath))
						continue;
					var json = JsonSerializer.Deserialize<TemplateConfig> (File.ReadAllText (jsonPath), options);
					if (json.Tags.Type != "project")
						continue;

					allTemplates.Add (json.ShortName);
				}
			}
			Assert.That (allListedTemplates, Is.EquivalentTo (allTemplates), "The listed templates here and the templates on disk don't match");
		}

		[Test]
		[TestCaseSource (nameof (Templates))]
		public void CreateAndBuildTemplate (TemplateInfo info)
		{
			if (!info.ValidateSuccessfulBuild) {
				return;
			}

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
	}
}
