using System;

namespace Xamarin.Tests.Templating
{
	public class TemplateInfo
	{
		public ProjectLanguage Language { get; private set; }
		public ProjectFlavor Flavor { get; private set; }
		public ProjectType ProjectType { get; private set; }

		public TemplateInfo (ProjectFlavor flavor, ProjectType projectType, ProjectLanguage language)
		{
			Flavor = flavor;
			ProjectType = projectType;
			Language = language;
		}

		TemplateInfo (ProjectType projectType, ProjectLanguage language, string projectName)
		{
			ProjectType = projectType;
			Language = language;
			ProjectNameOverride = projectName;
		}

		TemplateInfo (string projectName, string sourceName)
		{
			ProjectNameOverride = projectName;
			SourceNameOverride = sourceName;
		}

		public static TemplateInfo FromCustomProject (ProjectType projectType, ProjectLanguage language, string projectName) => new TemplateInfo (projectType, language, projectName);
		public static TemplateInfo FromFiles (string projectName, string sourceName) => new TemplateInfo (projectName, sourceName);

		string ProjectNameOverride;
		public string ProjectName => ProjectNameOverride ?? DefaultProjectName;

		string SourceNameOverride;
		public string SourceName => SourceNameOverride ?? DefaultSourceName;

		string DefaultSourceName {
			get {
				switch (ProjectType) {
				case ProjectType.Library:
					switch (Language) {
					case ProjectLanguage.CSharp:
						return "MyClass.cs";
					case ProjectLanguage.FSharp:
						return "Component1.fs";
					}
					break;
				case ProjectType.App:
					switch (Language) {
					case ProjectLanguage.CSharp:
						return "Main.cs";
					case ProjectLanguage.FSharp:
						return "Main.fs";
					}
					break;
				}
				throw new NotImplementedException ();
			}
		}

		string DefaultProjectName {
			get {
				switch (Language) {
				case ProjectLanguage.CSharp:
					switch (Flavor) {
					case ProjectFlavor.FullXM:
						switch (ProjectType) {
						case ProjectType.App:
							return "XM45Example.csproj";
						case ProjectType.Binding:
							return "XM45Binding.csproj";
						case ProjectType.Library:
							return "XM45Library.csproj";
						}
						break;
					case ProjectFlavor.ModernXM:
						switch (ProjectType) {
						case ProjectType.App:
							return "UnifiedExample.csproj";
						case ProjectType.Binding:
							return "MobileBinding.csproj";
						case ProjectType.Library:
							return "UnifiedLibrary.csproj";
						}
						break;
					}
					break;
				case ProjectLanguage.FSharp:
					switch (Flavor) {
					case ProjectFlavor.FullXM:
						switch (ProjectType) {
						case ProjectType.App:
							return "FSharpXM45Example.fsproj";
						case ProjectType.Library:
							return "FSharpXM45Library.fsproj";
						}
						break;
					case ProjectFlavor.ModernXM:
						switch (ProjectType) {
						case ProjectType.App:
							return "FSharpUnifiedExample.fsproj";
						case ProjectType.Library:
							return "FSharpUnifiedLibrary.fsproj";
						}
						break;
					}
					break;
				}

				throw new NotImplementedException ();
			}
		}
	}
}
