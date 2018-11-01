using System;
using System.IO;

namespace Xamarin.Tests.Templating
{
	public class MacAppTemplateEngine : TemplateEngineBase
	{
		public MacAppTemplateEngine (ProjectFlavor flavor, ProjectLanguage language = ProjectLanguage.CSharp) : base (new TemplateInfo (flavor, ProjectType.App, language))
		{
		}

		public MacAppTemplateEngine (TemplateInfo info) : base (info)
		{
		}

		public string Generate (string outputDirectory, ProjectSubstitutions projectSubstitutions, FileSubstitutions fileSubstitutions, PListSubstitutions plistReplacements = null, bool includeAssets = false)
		{
			plistReplacements = plistReplacements ?? PListSubstitutions.None;
			FileTemplateEngine templateEngine = CreateEngine (outputDirectory);

			if (includeAssets) {
				templateEngine.CopyDirectory ("Icons/Assets.xcassets");

				projectSubstitutions.ItemGroup += @"<ItemGroup>
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\Contents.json"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-128.png"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-128%402x.png"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-16.png"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-16%402x.png"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-256%402x.png"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-32.png"" />
    <ImageAsset Include=""Assets.xcassets\AppIcon.appiconset\AppIcon-32%402x.png"" />
    <ImageAsset Include=""Assets.xcassets\Contents.json"" />
  </ItemGroup>";

				// HACK - Should process using CopyFileWithSubstitutions
				plistReplacements.Replacements.Add ("</dict>", @"<key>XSAppIconAssets</key><string>Assets.xcassets/AppIcon.appiconset</string></dict>");
			}

			ReplacementGroup replacements = ReplacementGroup.Create (Replacement.Create ("%CODE%", fileSubstitutions.TestCode), Replacement.Create ("%DECL%", fileSubstitutions.TestDecl));
			templateEngine.CopyTextWithSubstitutions (GetAppMainSourceText (), TemplateInfo.SourceName, replacements);

			templateEngine.CopyFileWithSubstitutions ("Info-Unified.plist", plistReplacements.CreateReplacementAction (), "Info.plist");

			return templateEngine.CopyFileWithSubstitutions (TemplateInfo.ProjectName, GetStandardProjectReplacement (projectSubstitutions));
		}

		string GetAppMainSourceText ()
		{
			const string FSharpMainTemplate = @"
namespace FSharpUnifiedExample
open System
open AppKit

module main =
    %DECL%
 
    [<EntryPoint>]
    let main args =
        NSApplication.Init ()
        %CODE%
        0";

			const string MainTemplate = @"
using Foundation;
using AppKit;

namespace TestCase
{
	class MainClass
	{
		%DECL%

		static void Main (string[] args)
		{
			NSApplication.Init ();
			%CODE%
		}
	}
}";

			return TemplateInfo.Language == ProjectLanguage.FSharp ? FSharpMainTemplate : MainTemplate;
		}
	}
}