using System;
using System.IO;

using Xunit;
using Xunit.Sdk;

using BCLTestImporter;

namespace BCLTestImporterTests {
	public class BCLTestInfoPlistGeneratorTest {

		[Fact]
		public async void GenerateCodeNullTemplateFile ()
		{
			await Assert.ThrowsAsync<ArgumentNullException> (() => 
				BCLTestInfoPlistGenerator.GenerateCodeAsync (null, "Project Name"));
		}

		[Fact]
		public async void GenerateCodeNullProjectName ()
		{
			await Assert.ThrowsAsync <ArgumentNullException> (() =>
				BCLTestInfoPlistGenerator.GenerateCodeAsync ("A/path", null));
		}

		[Fact]
		public async void GenerateCode ()
		{
			const string projectName = "MyTest";
			var fakeTemplate = $"{BCLTestInfoPlistGenerator.ApplicationNameReplacement}-{BCLTestInfoPlistGenerator.IndentifierReplacement}";
			var tmpPath = Path.GetTempPath ();
			var templatePath = Path.Combine (tmpPath, Path.GetRandomFileName());
			using (var file = new StreamWriter (templatePath, false)) { 
				await file.WriteAsync (fakeTemplate);
			}

			var result = await BCLTestInfoPlistGenerator.GenerateCodeAsync (templatePath, projectName);
			try {
				Assert.DoesNotContain (BCLTestInfoPlistGenerator.ApplicationNameReplacement, result);
				Assert.DoesNotContain (BCLTestInfoPlistGenerator.IndentifierReplacement, result);
				Assert.Contains (projectName, result);
			}
			finally {
				File.Delete (templatePath);
			}
		}
	}
}
