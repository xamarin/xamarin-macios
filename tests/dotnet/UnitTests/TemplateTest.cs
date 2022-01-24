using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests {
	[TestFixture]
	public class TemplateTest {
		public struct TemplateInfo {
			public readonly string Platform;
			public readonly string Template;
			public readonly bool ValidateSuccessfulBuild;

			public TemplateInfo (string platform, string template, bool validateSuccessfulBuild = true)
			{
				Platform = platform;
				Template = template;
				ValidateSuccessfulBuild = validateSuccessfulBuild;
			}
		}

		public static TemplateInfo[] Templates = {
			new TemplateInfo ("iOS", "ios"),
			new TemplateInfo ("iOS", "ioslib"),
			new TemplateInfo ("iOS", "iosbinding", false), // Bindings can not build without a native library assigned
			new TemplateInfo ("tvOS", "tvos"),
			new TemplateInfo ("MacCatalyst", "maccatalyst"),
			new TemplateInfo ("MacCatalyst", "maccatalystbinding", false), // Bindings can not build without a native library assigned
			new TemplateInfo ("macOS", "macos"),
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
			Configuration.CopyDotNetSupportingFiles (tmpDir);
			var outputDir = Path.Combine (tmpDir, info.Template);
			DotNet.AssertNew (outputDir, info.Template);
			var csproj = Path.Combine (outputDir, info.Template + ".csproj");
			var rv = DotNet.AssertBuild (csproj);
			var warnings = BinLog.GetBuildLogWarnings (rv.BinLogPath).Select (v => v.Message);
			Assert.That (warnings, Is.Empty, $"Build warnings:\n\t{string.Join ("\n\t", warnings)}");
		}
	}
}
