using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness.Targets {
	public abstract class Target {
		public IHarness Harness;

		protected XmlDocument inputProject;
		protected string OriginalInfoPListInclude { get; private set; }
		string outputType;
		string bundleIdentifier;
		string targetDirectory;

		public string Name { get; protected set; }
		public string ProjectGuid { get; protected set; }
		public string ProjectPath { get; protected set; }

		public string ProjectFileExtension { get { return IsFSharp ? "fsproj" : "csproj"; } }
		public bool IsFSharp { get { return (ProjectPath ?? TemplateProjectPath).EndsWith (".fsproj", StringComparison.Ordinal); } }

		public string TemplateProjectPath { get; set; }

		bool? is_dotnet_project;
		public bool IsDotNetProject { get { return is_dotnet_project ?? (is_dotnet_project = inputProject.IsDotNetProject ()).Value; } }
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
		public virtual string ExtraLinkerDefsSuffix { get { return Suffix; } }
		protected virtual string ProjectTypeGuids { get { throw new NotImplementedException (); } }
		protected virtual string BindingsProjectTypeGuids { get { throw new NotImplementedException (); } }
		protected virtual string TargetFrameworkIdentifier { get { throw new NotImplementedException (); } }
		protected virtual string Imports { get { throw new NotImplementedException (); } }
		protected virtual string BindingsImports { get { throw new NotImplementedException (); } }
		public virtual bool IsMultiArchitecture { get { return false; } }
		public virtual string SimulatorArchitectures { get { throw new NotImplementedException (); } }
		public virtual string DeviceArchitectures { get { throw new NotImplementedException (); } }
		protected virtual string GetMinimumOSVersion (string templateMinimumOSVersion) { throw new NotImplementedException (); }
		protected virtual int [] UIDeviceFamily { get { throw new NotImplementedException (); } }
		protected virtual string AdditionalDefines { get { return string.Empty; } }
		protected virtual string RemoveDefines { get { return string.Empty; } }
		public virtual string Platform { get { throw new NotImplementedException (); } }

		public bool ShouldSkipProjectGeneration { get; set; }
		public virtual bool ShouldSetTargetFrameworkIdentifier { get { return true; } }
		public virtual string DefaultAssemblyReference { get { return "Xamarin.iOS"; } }
		public virtual IEnumerable<string> ReferenceToRemove { get { return Enumerable.Empty<string> (); } }
		public virtual Dictionary<string, string> NewPropertiesToAdd { get { return new Dictionary<string, string> (); } }
		public virtual HashSet<string> PropertiesToRemove { get { return null; } }

		public const string FSharpGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
		public const string CSharpGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		public string DotNetTfm => Harness.DOTNET_TFM;

		public string LanguageGuid { get { return IsFSharp ? FSharpGuid : CSharpGuid; } }

		public abstract string DotNetSdk { get; }
		public abstract string RuntimeIdentifier { get; }
		public abstract DevicePlatform ApplePlatform { get; }
		public abstract string TargetFramework { get; }
		public abstract string TargetFrameworkForNuGet { get; }

		public static string ProjectsDir { get { return "generated-projects"; } }
		protected string GetTargetSpecificDir (string customSuffix = null)
		{
			string rv;
			if (string.IsNullOrEmpty (customSuffix)) {
				rv = Suffix;
			} else {
				rv = Suffix + "-" + customSuffix;
			}
			if (IsDotNetProject)
				rv += "-dotnet";
			return rv.TrimStart ('-');
		}

		public virtual string PlatformString {
			get {
				return ApplePlatform.AsString ();
			}
		}

		protected virtual bool FixProjectReference (string include, string subdir, string suffix, out string fixed_include)
		{
			var fn = Path.GetFileName (include);

			switch (fn) {
			case "Touch.Client-iOS.dotnet.csproj":
			case "Touch.Client-iOS.csproj":
				var dir = Path.GetDirectoryName (include);
				var parentDir = Path.GetFileName (dir);
				if (parentDir == "iOS")
					dir = Path.Combine (Path.GetDirectoryName (dir), PlatformString);
				fixed_include = Path.Combine (dir, fn.Replace ("-iOS", "-" + PlatformString));
				break;
			default:
				include = include.Replace (".csproj", suffix + ".csproj");
				include = include.Replace (".fsproj", suffix + ".fsproj");

				if (!string.IsNullOrEmpty (subdir))
					include = Path.Combine (Path.GetDirectoryName (include), subdir, Path.GetFileName (include));

				fixed_include = include;
				break;
			}

			return true;
		}

		protected virtual void ProcessDotNetProject ()
		{
			inputProject.SetSdk (DotNetSdk);
			inputProject.SetRuntimeIdentifier (RuntimeIdentifier);
			inputProject.FixProjectReferences (Path.Combine (ProjectsDir, GetTargetSpecificDir ()), Suffix, FixProjectReference);
			inputProject.SetNode ("TargetFramework", TargetFramework);
			var fixedAssetTargetFallback = inputProject.GetAssetTargetFallback ()?.Replace ("xamarinios10", TargetFrameworkForNuGet);
			if (fixedAssetTargetFallback is not null)
				inputProject.SetAssetTargetFallback (fixedAssetTargetFallback);
			inputProject.ResolveAllPaths (TemplateProjectPath);
		}

		protected virtual void ProcessProject ()
		{
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
			if (removedProperties is not null) {
				foreach (var p in removedProperties)
					inputProject.RemoveNode (p, false);
			}

			inputProject.FixProjectReferences (Path.Combine (ProjectsDir, GetTargetSpecificDir ()), Suffix, FixProjectReference);
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
			inputProject.Save (ProjectPath, Harness);

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
			inputProject.ResolveAllPaths (TemplateProjectPath);
			inputProject.Save (ProjectPath, Harness);

			ProjectGuid = inputProject.GetProjectGuid ();
		}

		protected virtual void CreateDotNetProject ()
		{
			ProcessDotNetProject ();
			inputProject.Save (ProjectPath, Harness);
			UpdateInfoPList ();
		}

		protected virtual void ExecuteInternal ()
		{
			if (IsDotNetProject) {
				CreateDotNetProject ();
				return;
			}
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
			targetDirectory = Path.GetDirectoryName (TemplateProjectPath);
			CalculateName ();

			var templateName = Path.GetFileName (TemplateProjectPath);
			if (templateName.EndsWith (".template", StringComparison.OrdinalIgnoreCase))
				templateName = Path.GetFileNameWithoutExtension (templateName);
			templateName = Path.GetFileNameWithoutExtension (templateName);

			if (templateName.Equals ("mono-native-mac"))
				templateName = "mono-native";

			if (!ShouldSkipProjectGeneration) {
				inputProject = new XmlDocument ();
				inputProject.LoadWithoutNetworkAccess (TemplateProjectPath);
				OriginalInfoPListInclude = inputProject.GetInfoPListInclude ()?.Replace ('\\', '/');

				ProjectPath = Path.Combine (targetDirectory, ProjectsDir, GetTargetSpecificDir (), templateName + ProjectFileSuffix + "." + ProjectFileExtension);

				outputType = inputProject.GetOutputType ();

				if (inputProject.IsDotNetProject ()) {
					IsBindingProject = string.Equals (inputProject.GetIsBindingProject (), "true", StringComparison.OrdinalIgnoreCase);
				} else {
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
				}

				ExecuteInternal ();
			} else {
				ProjectPath = TemplateProjectPath;
			}
		}

		public virtual IEnumerable<RelatedProject> GetRelatedProjects ()
		{
			return null;
		}
	}

	public class RelatedProject {
		public string ProjectPath;
		public string Guid;
	}
}
