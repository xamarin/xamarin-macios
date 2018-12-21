using System;
using System.IO;

using Xunit;
using Xunit.Sdk;

using BCLTestImporter;

namespace BCLTestImporterTests {
	public class ApplicationOptionsTest : IDisposable {
		readonly string registerTypesTemplatePath;
		readonly string projectTemplatePath;
		readonly string plistTemplatePath;
		readonly string outputPath;
		readonly string monoCheckout;
		readonly Platform platform = Platform.iOS;

		static void WriteJunk (string path)
		{
			using (var file = new StreamWriter (path, false)) { 
				file.Write ("Data");
			}	
		}
		public ApplicationOptionsTest ()
		{
			var tmpPath = Path.GetTempPath ();
			registerTypesTemplatePath = Path.Combine (tmpPath, Path.GetRandomFileName());
			WriteJunk (registerTypesTemplatePath);
			projectTemplatePath = Path.Combine (tmpPath, Path.GetRandomFileName());
			WriteJunk (projectTemplatePath);
			plistTemplatePath = Path.Combine (tmpPath, Path.GetRandomFileName());
			WriteJunk (plistTemplatePath);
			outputPath = Path.Combine (tmpPath, Path.GetRandomFileName());
			monoCheckout = Path.Combine (tmpPath, Path.GetRandomFileName());
			var testDir = BCLTestAssemblyDefinition.GetTestDirectoryFromMonoPath (monoCheckout, platform);
			
			if (Directory.Exists (testDir)) return;
			
			// we need to create the mono dir and the test dir
			if (!Directory.Exists (testDir))
				Directory.CreateDirectory (testDir);
		}

		public void Dispose ()
		{
			if (registerTypesTemplatePath != null && File.Exists (registerTypesTemplatePath))
				File.Delete (registerTypesTemplatePath);
			if (projectTemplatePath != null && File.Exists (projectTemplatePath))
				File.Delete (projectTemplatePath);
			if (outputPath != null && File.Exists (outputPath))
				File.Delete (outputPath);
			if (monoCheckout != null && Directory.Exists (monoCheckout))
				Directory.Delete (monoCheckout, true);
		}
		
		[Fact]
		public void DefaultValues ()
		{
			var appOptions = new ApplicationOptions ();
			// flag values
			Assert.False (appOptions.ShouldShowHelp, "appOptions.ShouldShowHelp");
			Assert.False (appOptions.Verbose, "appOptions.Verbose");
			Assert.False (appOptions.ShowDict, "appOptions.ShowDict");
			Assert.False (appOptions.ListAssemblies, "appOptions.ListAssemblies");
			Assert.False (appOptions.GenerateProject, "appOptions.GenerateProject");
			Assert.False (appOptions.GenerateAllProjects, "appOptions.GenerateAllProjects");
			Assert.False (appOptions.GenerateTypeRegister, "appOptions.GenerateTypeRegister");
			Assert.False (appOptions.IsXUnit, "appOptions.IsXUnit");
			Assert.False (appOptions.Override, "appOptions.Override");
			Assert.False (appOptions.ClearAll, "appOptions.ClearAll");
			// string values
			Assert.Null (appOptions.MonoPath);
			Assert.Null (appOptions.Output);
			Assert.Null (appOptions.RegisterTypeTemplate);
			Assert.Null (appOptions.ProjectTemplate);
			Assert.Null (appOptions.Assembly);
			Assert.Null (appOptions.RegisterTypesPath);
			Assert.Null (appOptions.ProjectName);
			Assert.Null (appOptions.PlistTemplate);
			// lists
			Assert.Empty(appOptions.TestAssemblies);
		}

		[Fact]
		public void TypeRegisterMonoPathMissing ()
		{
			var appOptions = new ApplicationOptions {
				GenerateTypeRegister = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Output = outputPath,
				Override = true
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-type-register Mono checkout is missing.", errorMessage);
		}
		
		[Fact]
		public void TypeRegisterMonoNotValid ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateTypeRegister = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Output = outputPath,
				Override = true,
				MonoPath = Path.Combine (home, "foo"),
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Contains ("--generate-type-register Could not find mono checkout:", errorMessage);
		}

		[Fact]
		public void TypeRegisterTemplateMissing ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateTypeRegister = true,
				RegisterTypeTemplate = Path.Combine (home, "foo.in"),
				Output = outputPath,
				Override = true,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-type-register Template is missing.", errorMessage);
		}

		[Fact]
		public void TypeRegisterMissingOutput ()
		{
			var appOptions = new ApplicationOptions {
				GenerateTypeRegister = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = true,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-type-register output path must be provided.", errorMessage);
		}

		[Fact]
		public void TypeRegisterNotOverrideAndPresentFile ()
		{
			WriteJunk (outputPath);
			var appOptions = new ApplicationOptions {
				GenerateTypeRegister = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-type-register Output path already exists.", errorMessage);
		}

		[Fact]
		public void TypeRegisterNoTestAssemblies ()
		{
			var appOptions = new ApplicationOptions {
				GenerateTypeRegister = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-type-register test assemblies must be passed for code generation.", errorMessage);
		}

		[Fact]
		public void GenerateProjectMonoPathNotValid ()
		{
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = registerTypesTemplatePath,
				Output = outputPath,
				Override = true
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project Mono checkout is missing.", errorMessage);
		}

		[Fact]
		public void GenerateProjectPlatformNotValid ()
		{
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = registerTypesTemplatePath,
				Output = outputPath,
				Override = true
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project Mono checkout is missing.", errorMessage);
		}

		[Fact]
		public void GenerateProjectTemplateMissing ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = Path.Combine (home, "foo.in"),
				Output = outputPath,
				Override = true,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project Project template is missing.", errorMessage);
		}
		
		[Fact]
		public void GeneratePlistTemplateNoProvided ()
		{
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				Output = outputPath,
				Override = true,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project Plist template must be provided.", errorMessage);
		}
		
		[Fact]
		public void GeneratePlistTemplateMissing ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				PlistTemplate = Path.Combine (home, "foo.in"),
				Output = outputPath,
				Override = true,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project Plist template is missing.", errorMessage);
		}

		[Fact]
		public void GenerateProjectMissingOutput ()
		{
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				PlistTemplate = plistTemplatePath,
				Override = true,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project output path must be provided.", errorMessage);
		}

		[Fact]
		public void GenerateProjectNotOverrideAndPresentFile ()
		{
			WriteJunk (outputPath);
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				PlistTemplate = plistTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			appOptions.TestAssemblies.Add ("Foo.dll");
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project Output path already exists.", errorMessage);
		}

		[Fact]
		public void GenerateProjectNoTestAssemblies ()
		{
			var appOptions = new ApplicationOptions {
				GenerateProject = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				PlistTemplate = plistTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-project test assemblies must be passed for project generation.", errorMessage);
		}

		[Fact]
		public void GenerateAllProjectsMonoPathNotValid ()
		{
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Output = outputPath,
				Override = true
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Mono checkout is missing.", errorMessage);
		}

		[Fact]
		public void GenerateAllProjectsPlatformNotValid ()
		{
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectName = "Test",
				ProjectTemplate = projectTemplatePath,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Output = outputPath,
				Override = true
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Mono checkout is missing.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsMissingOutput ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectName = "Test",
				ProjectTemplate = home,
				PlistTemplate = home,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = true,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects output path must be provided.", errorMessage);
		}

		[Fact]
		public void GenerateAllProjectsOutputNotDir ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			WriteJunk (outputPath);
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectTemplate = home,
				PlistTemplate = home,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Output path must be an existing directory.", errorMessage);
		}


		[Fact]
		public void GenerateAllProjectsProjectTemplateNotProvided ()
		{
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Project template must be provided.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsProjectTemplateMissing ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				RegisterTypeTemplate = registerTypesTemplatePath,
				ProjectTemplate = Path.Combine (home, "foo.in"),
				PlistTemplate = plistTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Project template is missing.", errorMessage);
		}

		[Fact]
		public void GenerateAllProjectsRegisterTypeTemplateNotProvided ()
		{
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectTemplate = projectTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Register type template must be provided.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsRegisterTypeTemplateMissing ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectTemplate = projectTemplatePath,
				RegisterTypeTemplate = Path.Combine (home, "foo.in"),
				PlistTemplate = plistTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Register type template is missing.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsPlistTemplateNotProvided ()
		{
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectTemplate = projectTemplatePath,
				RegisterTypeTemplate = registerTypesTemplatePath,
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Plist template must be provided.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsPlistTemplateMissing ()
		{
			var home = Environment.GetEnvironmentVariable ("HOME");
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ProjectTemplate = home,
				RegisterTypeTemplate = registerTypesTemplatePath,
				PlistTemplate = Path.Combine (home, "foo.in"),
				Override = false,
				Output = outputPath,
				MonoPath = monoCheckout,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Plist template is missing.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsClearMissingOutput ()
		{
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ClearAll = true,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects output path must be provided.", errorMessage);
		}
		
		[Fact]
		public void GenerateAllProjectsOutputClearNotDir ()
		{
			WriteJunk (outputPath);
			var appOptions = new ApplicationOptions {
				GenerateAllProjects = true,
				ClearAll = true,
				Output = outputPath,
			};
			Assert.False (appOptions.OptionsAreValid (out var errorMessage));
			Assert.Equal ("--generate-all-projects Output path must be an existing directory.", errorMessage);
		}

	}
}