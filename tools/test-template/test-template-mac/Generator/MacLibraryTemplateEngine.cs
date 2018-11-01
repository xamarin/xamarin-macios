using System;
namespace Xamarin.Tests.Templating
{
	public class MacLibraryTemplateEngine : TemplateEngineBase
	{
		public MacLibraryTemplateEngine (ProjectFlavor flavor, ProjectLanguage language = ProjectLanguage.CSharp) : base (new TemplateInfo (flavor, ProjectType.Library, language))
		{
		}

		public MacLibraryTemplateEngine (TemplateInfo info) : base (info)
		{
		}

		public string Generate (string outputDirectory, ProjectSubstitutions projectSubstitutions, FileSubstitutions fileSubstitutions)
		{
			FileTemplateEngine templateEngine = CreateEngine (outputDirectory);

			templateEngine.CopyFileWithSubstitutions (TemplateInfo.SourceName, Replacement.Create ("%CODE%", fileSubstitutions.TestCode));

			return templateEngine.CopyFileWithSubstitutions (TemplateInfo.ProjectName, GetStandardProjectReplacement (projectSubstitutions));
		}
	}
}
