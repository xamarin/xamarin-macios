using System;
using System.IO;
using System.Text;
using Xunit;

using BCLTestImporter;

namespace BCLTestImporterTests {
	
	public class BCLTestProjectGeneratorTest
	{
		readonly string outputdir;
		
		public BCLTestProjectGeneratorTest ()
		{
			outputdir = Path.GetTempPath ();
		}

		[Fact]
		public void ConstructorNullOutputDir ()
		{
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator (null));
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator (null, "", "", "", ""));
		}
		
		[Fact]
		public void ConstructorNullMonoDir ()
		{
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator ("", null, "", "", ""));
		}
		
		[Fact]
		public void ConstructorNullProjectTemplatePath ()
		{
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator ("", "", null, "", ""));
		}
		
		[Fact]
		public void ConstructorNullRegisterPath ()
		{
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator ("", "", "", null, ""));
		}
		
		[Fact]
		public void ConstructorNullPlistTemplatePath ()
		{
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator ("", "", "", "", null));
		}
		
		[Theory]
		[InlineData ("iOSProject", Platform.iOS, "iOSProject.csproj")]
		[InlineData ("WatchOSProject", Platform.WatchOS, "WatchOSProject-watchos.csproj")]
		[InlineData ("TvOSProject", Platform.TvOS, "TvOSProject-tvos.csproj")]
		// TODO: [InlineData ("MacProject", Platform.MacOS, null)] not implemented yet
		public void GetProjectPath (string projectName, Platform platform, string expectedName)
		{
			// ignore the fact that all params are the same, we do not care
			var generator = new BCLTestProjectGenerator (outputdir, outputdir, outputdir, outputdir, outputdir);
			var path = generator.GetProjectPath (projectName, platform);	
			Assert.Equal (Path.Combine (generator.OutputDirectoryPath, expectedName), path);
		}
		
		[Theory]
		[InlineData ("WatchApp", BCLTestProjectGenerator.WatchAppType.App, "WatchApp-watchos-app.csproj")]
		[InlineData ("WatchExtension", BCLTestProjectGenerator.WatchAppType.Extension, "WatchExtension-watchos-extension.csproj")]
		public void GetProjectPathWatchOS (string projectName, BCLTestProjectGenerator.WatchAppType appType, string expectedName)
		{
			// ignore the fact that all params are the same, we do not care
			var generator = new BCLTestProjectGenerator (outputdir, outputdir, outputdir, outputdir, outputdir);
			var path = generator.GetProjectPath (projectName, appType);
			Assert.Equal (Path.Combine (generator.OutputDirectoryPath, expectedName), path);
		}

		[Theory]
		[InlineData ("/usr/path", Platform.iOS, "Info.plist")]
		[InlineData ("/usr/second/path", Platform.TvOS, "Info-tv.plist")]
		[InlineData ("/usr/other/path", Platform.WatchOS, "Info-watchos.plist")]
		// TODO: [InlineData ("/usr/mac/path", Platform.MacOS)] not implemented yet
		public void GetPListPath (string rootDir, Platform platform, string expectedName)
		{
			var path = BCLTestProjectGenerator.GetPListPath (rootDir, platform);
			Assert.Equal (Path.Combine (rootDir, expectedName), path);
		}
		
		[Theory]
		[InlineData ("/usr/bin", BCLTestProjectGenerator.WatchAppType.App, "Info-watchos-app.plist")]
		[InlineData ("/usr/local", BCLTestProjectGenerator.WatchAppType.Extension, "Info-watchos-extension.plist")]
		public void GetPListPathWatchOS  (string rootDir, BCLTestProjectGenerator.WatchAppType  appType, string expectedName)
		{
			var path = BCLTestProjectGenerator.GetPListPath (rootDir, appType);
			Assert.Equal (Path.Combine (rootDir, expectedName), path);
		}

		[Theory]
		[InlineData ("System.Xml.dll")]
		[InlineData ("MyAssembly.dll")]
		public void GetReferenceNodeNullHint (string assembly)
		{
			var expected = $"<Reference Include=\"{assembly}\" />";
			Assert.Equal (expected, BCLTestProjectGenerator.GetReferenceNode (assembly));
		}
		
		[Theory]
		[InlineData ("System.Xml.dll", "my/path/to/the/dll")]
		[InlineData ("MyAssembly.dll", "thepath")]
		public void GetReferenceNode (string assembly, string hint)
		{
			var fixedHint = hint.Replace ("/", "\\");
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Reference Include=\"{assembly}\" >");
			sb.AppendLine ($"<HintPath>{fixedHint}</HintPath>");
			sb.AppendLine ("</Reference>");
			var expected = sb.ToString ();	
			Assert.Equal (expected, BCLTestProjectGenerator.GetReferenceNode (assembly, hint));
		}

		[Theory]
		[InlineData ("my/path/to/the/dll")]
		[InlineData ("my/other/path/to/the/dll")]
		public void GetRegisterTypeNode (string registerPath)
		{
			var fixedPath = registerPath.Replace ("/", "\\");
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Compile Include=\"{fixedPath}\">");
			sb.AppendLine ($"<Link>{Path.GetFileName (registerPath)}</Link>");
			sb.AppendLine ("</Compile>");
			var expected = sb.ToString ();
			Assert.Equal (expected, BCLTestProjectGenerator.GetRegisterTypeNode (registerPath));
		}

		[Theory]
		[InlineData ("TestProject", "/my/project/plist/path")]
		[InlineData ("OtherProject", "\\test\\project\\plist")]
		public void GenerateWatchProject (string projectName, string plistPath)
		{
			var generator = new BCLTestProjectGenerator (outputdir, outputdir, outputdir, outputdir, outputdir);
			var template = $"{BCLTestProjectGenerator.NameKey} {BCLTestProjectGenerator.WatchOSTemplatePathKey} {BCLTestProjectGenerator.PlistKey} {BCLTestProjectGenerator.WatchOSCsporjAppKey}";
			var generatedProject = generator.GenerateWatchProject (projectName, template, plistPath);
			Assert.DoesNotContain (BCLTestProjectGenerator.NameKey, generatedProject);
			Assert.Contains (projectName, generatedProject);
			Assert.DoesNotContain (BCLTestProjectGenerator.WatchOSTemplatePathKey, generatedProject);
			Assert.DoesNotContain (BCLTestProjectGenerator.PlistKey, generatedProject);
			Assert.Contains (plistPath.Replace ("/", "\\"), generatedProject);
		}
	}
}