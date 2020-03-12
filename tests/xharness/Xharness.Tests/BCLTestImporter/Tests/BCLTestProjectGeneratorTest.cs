using System;
using System.IO;
using NUnit.Framework;
using Xharness.BCLTestImporter;

namespace Xharness.Tests.BCLTestImporter.Tests {
	
	public class BCLTestProjectGeneratorTest
	{
		readonly string outputdir;
		
		public BCLTestProjectGeneratorTest ()
		{
			outputdir = Path.GetTempPath ();
		}

		[Test]
		public void ConstructorNullOutputDir ()
		{
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator (null));
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator (null, ""));
		}
		
		[Test]
		public void ConstructorNullMonoDir () => 
			Assert.Throws<ArgumentNullException> (() => new BCLTestProjectGenerator ("", null));

		/*
		[TestCase ("iOSProject", Platform.iOS, "iOSProject.csproj")]
		[TestCase ("WatchOSProject", Platform.WatchOS, "WatchOSProject-watchos.csproj")]
		[TestCase ("TvOSProject", Platform.TvOS, "TvOSProject-tvos.csproj")]
		// TODO: [InlineData ("MacProject", Platform.MacOS, null)] not implemented yet
		public void GetProjectPath (string projectName, Platform platform, string expectedName)
		{
			// ignore the fact that all params are the same, we do not care
			var generator = new BCLTestProjectGenerator (outputdir);
			var path = generator.GetProjectPath (projectName, platform);	
			Assert.AreEqual (Path.Combine (generator.OutputDirectoryPath, expectedName), path);
		}
		
		[TestCase ("WatchApp", WatchAppType.App, "WatchApp-watchos-app.csproj")]
		[TestCase ("WatchExtension", WatchAppType.Extension, "WatchExtension-watchos-extension.csproj")]
		public void GetProjectPathWatchOS (string projectName, WatchAppType appType, string expectedName)
		{
			// ignore the fact that all params are the same, we do not care
			var generator = new BCLTestProjectGenerator (outputdir, outputdir, outputdir, outputdir, outputdir);
			var path = generator.GetProjectPath (projectName, appType);
			Assert.AreEqual (Path.Combine (generator.OutputDirectoryPath, expectedName), path);
		}

		[TestCase ("/usr/path", Platform.iOS, "Info.plist")]
		[TestCase ("/usr/second/path", Platform.TvOS, "Info-tv.plist")]
		[TestCase ("/usr/other/path", Platform.WatchOS, "Info-watchos.plist")]
		// TODO: [InlineData ("/usr/mac/path", Platform.MacOS)] not implemented yet
		public void GetPListPath (string rootDir, Platform platform, string expectedName)
		{
			var path = BCLTestProjectGenerator.GetPListPath (rootDir, platform);
			Assert.AreEqual (Path.Combine (rootDir, expectedName), path);
		}
		
		[TestCase ("/usr/bin", WatchAppType.App, "Info-watchos-app.plist")]
		[TestCase ("/usr/local", WatchAppType.Extension, "Info-watchos-extension.plist")]
		public void GetPListPathWatchOS  (string rootDir, WatchAppType  appType, string expectedName)
		{
			var path = BCLTestProjectGenerator.GetPListPath (rootDir, appType);
			Assert.AreEqual (Path.Combine (rootDir, expectedName), path);
		}
		
		[TestCase ("System.Xml.dll")]
		[TestCase ("MyAssembly.dll")]
		public void GetReferenceNodeNullHint (string assembly)
		{
			var expected = $"<Reference Include=\"{assembly}\" />";
			Assert.AreEqual (expected, BCLTestProjectGenerator.GetReferenceNode (assembly));
		}
		
		[TestCase ("System.Xml.dll", "my/path/to/the/dll")]
		[TestCase ("MyAssembly.dll", "thepath")]
		public void GetReferenceNode (string assembly, string hint)
		{
			var fixedHint = hint.Replace ("/", "\\");
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Reference Include=\"{assembly}\" >");
			sb.AppendLine ($"<HintPath>{fixedHint}</HintPath>");
			sb.AppendLine ("</Reference>");
			var expected = sb.ToString ();	
			Assert.AreEqual (expected, BCLTestProjectGenerator.GetReferenceNode (assembly, hint));
		}

		[TestCase ("my/path/to/the/dll")]
		[TestCase ("my/other/path/to/the/dll")]
		public void GetRegisterTypeNode (string registerPath)
		{
			var fixedPath = registerPath.Replace ("/", "\\");
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Compile Include=\"{registerPath}\">");
			sb.AppendLine ($"<Link>{Path.GetFileName (registerPath)}</Link>");
			sb.AppendLine ("</Compile>");
			var expected = sb.ToString ();
			Assert.AreEqual (expected, BCLTestProjectGenerator.GetRegisterTypeNode (registerPath));
		}

		[TestCase ("TestProject", "/my/project/plist/path")]
		[TestCase ("OtherProject", "\\test\\project\\plist")]
		public void GenerateWatchProject (string projectName, string plistPath)
		{
			var generator = new BCLTestProjectGenerator (outputdir, outputdir, outputdir, outputdir, outputdir);
			var template = $"{BCLTestProjectGenerator.NameKey} {BCLTestProjectGenerator.WatchOSTemplatePathKey} {BCLTestProjectGenerator.PlistKey} {BCLTestProjectGenerator.WatchOSCsporjAppKey}";
			var generatedProject = generator.GenerateWatchProject (projectName, template, plistPath);
			StringAssert.DoesNotContain (BCLTestProjectGenerator.NameKey, generatedProject);
			StringAssert.Contains (projectName, generatedProject);
			StringAssert.DoesNotContain (BCLTestProjectGenerator.WatchOSTemplatePathKey, generatedProject);
			StringAssert.DoesNotContain (BCLTestProjectGenerator.PlistKey, generatedProject);
			StringAssert.Contains (plistPath, generatedProject);
		}
		*/
	}
}