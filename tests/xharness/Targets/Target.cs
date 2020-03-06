using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xharness.Utilities;

namespace Xharness.Targets
{
	public abstract class Target
	{
		public Harness Harness;

		protected XmlDocument inputProject;

		public string Name { get; protected set; }
		public string ProjectGuid { get; protected set; }
		public string ProjectPath { get; protected set; }

		public string ProjectFileExtension => IsFSharp ? "fsproj" : "csproj";
		public bool IsFSharp => (ProjectPath ?? TemplateProjectPath).EndsWith (".fsproj", StringComparison.Ordinal);

		public string TemplateProjectPath { get; set; }

		public string OutputType { get; private set; }
		public string TargetDirectory { get; private set; }
		public bool IsLibrary => OutputType == "Library";
		public bool IsExe => OutputType == "Exe";
		public bool IsBindingProject { get; private set; }
		public bool IsBCLProject => ProjectPath.Contains ("bcl-test");
		public bool IsNUnitProject { get; set; }

		public string BundleIdentifier { get; protected set; }

		public virtual string Suffix => throw new NotImplementedException ();
		public virtual string MakefileWhereSuffix => string.Empty;
		public virtual string ProjectFileSuffix => Suffix;
		public virtual string ExtraLinkerDefsSuffix => Suffix;
		protected virtual string ProjectTypeGuids => throw new NotImplementedException ();
		protected virtual string BindingsProjectTypeGuids => throw new NotImplementedException ();
		protected virtual string TargetFrameworkIdentifier => throw new NotImplementedException ();
		protected virtual string Imports => throw new NotImplementedException ();
		protected virtual string BindingsImports => throw new NotImplementedException ();
		protected virtual bool SupportsBitcode => false;
		public virtual bool IsMultiArchitecture => false;
		public virtual string SimulatorArchitectures => throw new NotImplementedException ();
		public virtual string DeviceArchitectures => throw new NotImplementedException ();
		protected virtual string GetMinimumOSVersion (string templateMinimumOSVersion) { throw new NotImplementedException (); }
		protected virtual int [] UIDeviceFamily => throw new NotImplementedException ();
		protected virtual string AdditionalDefines => string.Empty;
		protected virtual string RemoveDefines => string.Empty;
		public virtual string Platform => throw new NotImplementedException ();

		public bool ShouldSkipProjectGeneration { get; set; }
		public virtual bool ShouldSetTargetFrameworkIdentifier => true;
		public virtual string DefaultAssemblyReference => "Xamarin.iOS";
		public virtual IEnumerable<string> ReferenceToRemove => Enumerable.Empty<string> ();
		public virtual Dictionary<string, string> NewPropertiesToAdd => new Dictionary<string, string> ();
		public virtual HashSet<string> PropertiesToRemove => null;

		public const string FSharpGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
		public const string CSharpGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

		public string LanguageGuid => IsFSharp ? FSharpGuid : CSharpGuid;

		protected virtual bool FixProjectReference (string name, out string fixed_name)
		{
			fixed_name = null;
			return true;
		}

		protected virtual void ProcessProject ()
		{
			if (SupportsBitcode) {
				inputProject.CloneConfiguration ("iPhone", "Release", "Release-bitcode");
				inputProject.AddExtraMtouchArgs ("--bitcode:full", "iPhone", "Release-bitcode");
				inputProject.SetMtouchUseLlvm (true, "iPhone", "Release-bitcode");
			}

			if (!IsMultiArchitecture && IsExe) {
				inputProject.DeleteConfiguration ("iPhone", "Debug32");
				inputProject.DeleteConfiguration ("iPhone", "Debug64");
			}

			inputProject.SetOutputPath ("bin\\$(Platform)\\$(Configuration)" + Suffix);
			inputProject.SetIntermediateOutputPath ("obj\\$(Platform)\\$(Configuration)" + Suffix);

			if (ShouldSetTargetFrameworkIdentifier)
				inputProject.SetTargetFrameworkIdentifier (TargetFrameworkIdentifier);

			inputProject.SetAssemblyReference (DefaultAssemblyReference, TargetFrameworkIdentifier);

			foreach (var r in ReferenceToRemove)
				inputProject.RemoveReferences (r);

			var newProperties = NewPropertiesToAdd;
			foreach (var k in newProperties.Keys)
				inputProject.SetTopLevelPropertyGroupValue (k, newProperties [k]);

			var removedProperties = PropertiesToRemove;
			if (removedProperties != null) {
				foreach (var p in removedProperties)
					inputProject.RemoveNode (p, false);
			}

			inputProject.FixProjectReferences (Suffix, FixProjectReference);
			inputProject.SetAssemblyReference ("OpenTK", "OpenTK-1.0");
			inputProject.SetProjectTypeGuids (IsBindingProject ? BindingsProjectTypeGuids : ProjectTypeGuids);
			inputProject.SetImport ("$(MSBuildExtensionsPath)\\Xamarin\\" + (IsBindingProject ? BindingsImports : Imports));
			inputProject.FixTestLibrariesReferences (Platform);
			if (!string.IsNullOrEmpty (AdditionalDefines))
				inputProject.AddAdditionalDefines (AdditionalDefines);
			if (!string.IsNullOrEmpty (RemoveDefines))
				inputProject.RemoveDefines (RemoveDefines);
		}

		protected void CreateExecutableProject ()
		{
			ProcessProject ();
			PostProcessExecutableProject ();
			Harness.Save (inputProject, ProjectPath);

			UpdateInfoPList ();
		}

		protected virtual void PostProcessExecutableProject ()
		{
		}

		protected virtual void UpdateInfoPList ()
		{
		}

		protected void CreateLibraryProject ()
		{
			ProcessProject ();
			Harness.Save (inputProject, ProjectPath);

			ProjectGuid = inputProject.GetProjectGuid ();
		}

		protected virtual void ExecuteInternal ()
		{
			switch (OutputType) {
			case "Exe":
				CreateExecutableProject ();
				break;
			case "Library":
				CreateLibraryProject ();
				break;
			default:
				throw new Exception (string.Format ("Unknown OutputType: {0}", OutputType));
			}
		}

		protected virtual void CalculateName ()
		{
			Name = Path.GetFileName (TargetDirectory);
			if (string.Equals (Name, "ios", StringComparison.OrdinalIgnoreCase) || string.Equals (Name, "mac", StringComparison.OrdinalIgnoreCase))
				Name = Path.GetFileName (Path.GetDirectoryName (TargetDirectory));
		}

		public void Execute ()
		{
			TargetDirectory = Path.GetDirectoryName (TemplateProjectPath);
			CalculateName ();

			var templateName = Path.GetFileName (TemplateProjectPath);
			if (templateName.EndsWith (".template", StringComparison.OrdinalIgnoreCase))
				templateName = Path.GetFileNameWithoutExtension (templateName);
			templateName = Path.GetFileNameWithoutExtension (templateName);

			if (templateName.Equals ("mono-native-mac"))
				templateName = "mono-native";

			ProjectPath = Path.Combine (TargetDirectory, templateName + ProjectFileSuffix + "." + ProjectFileExtension);

			if (!ShouldSkipProjectGeneration) {
				inputProject = new XmlDocument ();
				inputProject.LoadWithoutNetworkAccess (TemplateProjectPath);

				OutputType = inputProject.GetOutputType ();

				switch (inputProject.GetImport ()) {
				case "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.CSharp.targets":
				case "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.FSharp.targets":
				case "$(MSBuildExtensionsPath)\\Xamarin\\Mac\\Xamarin.Mac.CSharp.targets":
				case "$(MSBuildExtensionsPath":
				case "$(MSBuildBinPath)\\Microsoft.CSharp.targets":
					IsBindingProject = false;
					break;
				case "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.ObjCBinding.CSharp.targets":
				case "$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.ObjCBinding.FSharp.targets":
				case "$(MSBuildExtensionsPath)\\Xamarin\\Mac\\Xamarin.Mac.ObjcBinding.CSharp":
					IsBindingProject = true;
					break;
				default:
					throw new Exception (string.Format ("Unknown Imports: {0} in {1}", inputProject.GetImport (), TemplateProjectPath));
				}

				ExecuteInternal ();
			}
		}

		public virtual IEnumerable<RelatedProject> GetRelatedProjects ()
		{
			return null;
		}
	}

	public class RelatedProject
	{
		public string ProjectPath;
		public string Guid;
	}
}

