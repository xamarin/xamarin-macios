using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace xharness
{
	public abstract class Target
	{
		public Harness Harness;

		protected XmlDocument inputProject;
		string outputType;
		string bundleIdentifier;
		string targetDirectory;

		public string Name { get; protected set; }
		public string ProjectGuid { get; protected set; }
		public string ProjectPath { get; protected set; }

		public string ProjectFileExtension { get { return IsFSharp ? "fsproj" : "csproj"; } }
		public bool IsFSharp { get { return (ProjectPath ?? TemplateProjectPath).EndsWith (".fsproj", StringComparison.Ordinal); } }

		public string TemplateProjectPath { get; set; }

		public string OutputType { get { return outputType; } }
		public string TargetDirectory { get { return targetDirectory; } }
		public bool IsLibrary { get { return outputType == "Library"; } }
		public bool IsExe { get { return outputType == "Exe"; } }
		public bool IsBindingProject { get; private set; }
		public bool IsBCLProject { get { return ProjectPath.Contains ("bcl-test"); } }
		public bool IsNUnitProject { get; set; }

		public string BundleIdentifier { get { return bundleIdentifier; } protected set { bundleIdentifier = value; } }

		public virtual string Suffix { get { throw new NotImplementedException (); } }
		public virtual string MakefileWhereSuffix { get { return string.Empty; } }
		public virtual string ProjectFileSuffix { get { return Suffix; } }
		public virtual string ExtraLinkerDefsSuffix {  get { return Suffix; } }
		protected virtual string ProjectTypeGuids { get { throw new NotImplementedException (); } }
		protected virtual string BindingsProjectTypeGuids { get { throw new NotImplementedException (); } }
		protected virtual string TargetFrameworkIdentifier { get { throw new NotImplementedException (); } }
		protected virtual string Imports { get { throw new NotImplementedException (); } }
		protected virtual string BindingsImports { get { throw new NotImplementedException (); } }
		protected virtual bool SupportsBitcode { get { return false; } }
		public virtual bool IsMultiArchitecture { get { return false; } }	
		public virtual string SimulatorArchitectures { get { throw new NotImplementedException (); } }
		public virtual string DeviceArchitectures { get { throw new NotImplementedException (); } }
		protected virtual string GetMinimumOSVersion (string templateMinimumOSVersion) { throw new NotImplementedException (); }
		protected virtual int[] UIDeviceFamily { get { throw new NotImplementedException (); } }
		protected virtual string AdditionalDefines { get { return string.Empty; } }
		public virtual string Platform { get { throw new NotImplementedException (); } }

		public virtual bool ShouldSkipProjectGeneration { get { return false; } } // Only generate makefile to invoke hard coded csproj
		public virtual bool ShouldSetTargetFrameworkIdentifier { get { return true; } }
		public virtual string DefaultAssemblyReference { get { return "Xamarin.iOS"; } }
		public virtual IEnumerable<string> ReferenceToRemove { get { return Enumerable.Empty<string> (); } }
		public virtual Dictionary<string, string> NewPropertiesToAdd { get { return new Dictionary<string, string> (); } }

		public const string FSharpGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
		public const string CSharpGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

		public string LanguageGuid { get { return IsFSharp ? FSharpGuid : CSharpGuid; } }

		protected virtual bool FixProjectReference (string name)
		{
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
				inputProject.SetTopLevelPropertyGroupValue (k, newProperties[k]);	

			inputProject.FixProjectReferences (Suffix, FixProjectReference);
			inputProject.SetAssemblyReference ("OpenTK", "OpenTK-1.0");
			inputProject.SetProjectTypeGuids (IsBindingProject ? BindingsProjectTypeGuids : ProjectTypeGuids);
			inputProject.SetImport ("$(MSBuildExtensionsPath)\\Xamarin\\" + (IsBindingProject ? BindingsImports : Imports));
			inputProject.FixTestLibrariesReferences (Platform);
			if (!string.IsNullOrEmpty (AdditionalDefines))
				inputProject.AddAdditionalDefines (AdditionalDefines);
		}

		protected void CreateExecutableProject ()
		{
			ProcessProject ();
			if (Harness.Mac) {
				ProjectGuid = "{" + Harness.NewStableGuid ().ToString ().ToUpper () + "}";
				inputProject.SetProjectGuid (ProjectGuid);
			} else {
				inputProject.FixArchitectures (SimulatorArchitectures, DeviceArchitectures);
				inputProject.FixInfoPListInclude (Suffix);
				inputProject.SetExtraLinkerDefs ("extra-linker-defs" + ExtraLinkerDefsSuffix + ".xml");
			}
			Harness.Save (inputProject, ProjectPath);

			if (!Harness.Mac) {
				ProjectGuid = inputProject.GetProjectGuid ();

				XmlDocument info_plist = new XmlDocument ();
				var target_info_plist = Path.Combine (TargetDirectory, "Info" + Suffix + ".plist");
				info_plist.LoadWithoutNetworkAccess (Path.Combine (TargetDirectory, "Info.plist"));
				BundleIdentifier = info_plist.GetCFBundleIdentifier ();
				info_plist.SetMinimumOSVersion (GetMinimumOSVersion (info_plist.GetMinimumOSVersion ()));
				info_plist.SetUIDeviceFamily (UIDeviceFamily);
				Harness.Save (info_plist, target_info_plist);
			}
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
			Name = Path.GetFileName (targetDirectory);
			if (string.Equals (Name, "ios", StringComparison.OrdinalIgnoreCase) || string.Equals (Name, "mac", StringComparison.OrdinalIgnoreCase))
				Name = Path.GetFileName (Path.GetDirectoryName (targetDirectory));
		}

		public void Execute ()
		{
			targetDirectory = Path.GetDirectoryName(TemplateProjectPath);
			CalculateName ();

			var templateName = Path.GetFileName (TemplateProjectPath);
			if (templateName.EndsWith (".template", StringComparison.OrdinalIgnoreCase))
				templateName = Path.GetFileNameWithoutExtension (templateName);
			templateName = Path.GetFileNameWithoutExtension (templateName);

			if (templateName.Equals ("mono-native-mac"))
				templateName = "mono-native";

			ProjectPath = Path.Combine (targetDirectory, templateName + ProjectFileSuffix + "." + ProjectFileExtension);

			if (!ShouldSkipProjectGeneration)
			{
				inputProject = new XmlDocument ();
				inputProject.LoadWithoutNetworkAccess (TemplateProjectPath);
	
				outputType = inputProject.GetOutputType ();
	
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

