using System;
using System.Collections.Generic;
using System.IO;

namespace Xamarin.Tests.Templating
{
	public enum ProjectLanguage { None, CSharp, FSharp }
	public enum ProjectPlatform { None, macOS }
	public enum ProjectFlavor { None, ModernXM, FullXM }
	public enum ProjectType { None, Library, App, Binding }

	public class ProjectSubstitutions
	{
		public string CSProjConfig { get; set; } = "";
		public string References { get; set; } = "";
		public string ReferencesBeforePlatform { get; set; } = "";
		public string ItemGroup { get; set; } = "";
		public string TargetFrameworkVersion { get; set; } = "";
		public string AssemblyNameOverride { get; set; } = null;

		public Tuple<string, string> CustomProjectReplacement { get; set; } = null;
	}

	public class FileSubstitutions
	{
		public string ApiDefinition { get; set; } = "";
		public string StructsAndEnums { get; set; } = "";

		public string TestCode { get; set; } = "";
		public string TestDecl { get; set; } = "";
	}

	public class PListSubstitutions
	{
		public static PListSubstitutions None => new PListSubstitutions ();

		public Dictionary<string, string> Replacements { get; set; } = new Dictionary<string, string> ();

		public ReplacementGroup CreateReplacementAction ()
		{
			ReplacementGroup group = new ReplacementGroup ();
			foreach (var item in Replacements) 
				group.Append (Replacement.Create (item.Key, item.Value));
			return group;
		}
	}

	public abstract class TemplateEngineBase
	{
		protected TemplateInfo TemplateInfo;

		protected TemplateEngineBase (TemplateInfo info)
		{
			TemplateInfo = info;
		}

		protected ReplacementGroup GetStandardProjectReplacement (ProjectSubstitutions config)
		{
			ReplacementGroup group = ReplacementGroup.Create (
				Replacement.Create ("%CODE%", config.CSProjConfig),
				Replacement.Create ("%REFERENCES%", config.References),
				Replacement.Create ("%REFERENCES_BEFORE_PLATFORM%", config.ReferencesBeforePlatform),
				Replacement.Create ("%NAME%", config.AssemblyNameOverride ?? Path.GetFileNameWithoutExtension (TemplateInfo.ProjectName)),
				Replacement.Create ("%ITEMGROUP%", config.ItemGroup),
				Replacement.Create ("%TARGET_FRAMEWORK_VERSION%", config.TargetFrameworkVersion));

			if (config.CustomProjectReplacement != null)
				group.Append (Replacement.Create (config.CustomProjectReplacement.Item1, config.CustomProjectReplacement.Item2));
			return group;
		}

		protected virtual FileTemplateEngine CreateEngine (string outputDirectory) => new FileTemplateEngine (DirectoryFinder.FindSourceDirectory (), outputDirectory);
	}
}
