using System;
using System.IO;

using NUnit.Framework;

using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.TestImporter.Templates.Managed;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.TestImporter.Templates.Managed {
	public class InfoPlistGeneratorTests {

		[Test]
		public void GenerateCodeNullTemplateFile ()
		{
			Assert.Throws<ArgumentNullException> (() =>
				InfoPlistGenerator.GenerateCode (null, "Project Name"));
		}

		[Test]
		public void GenerateCodeNullProjectName ()
		{
			Assert.Throws<ArgumentNullException> (() => InfoPlistGenerator.GenerateCode ("Hello", null));
		}

		[Test]
		public async Task GenerateCode ()
		{
			const string projectName = "MyTest";
			var fakeTemplate = $"{InfoPlistGenerator.ApplicationNameReplacement}-{InfoPlistGenerator.IndentifierReplacement}";
			var tmpPath = Path.GetTempPath ();
			var templatePath = Path.Combine (tmpPath, Path.GetRandomFileName ());
			using (var file = new StreamWriter (templatePath, false)) {
				await file.WriteAsync (fakeTemplate);
			}

			var result = InfoPlistGenerator.GenerateCode (File.ReadAllText (templatePath), projectName);
			try {
				StringAssert.DoesNotContain (InfoPlistGenerator.ApplicationNameReplacement, result);
				StringAssert.DoesNotContain (InfoPlistGenerator.IndentifierReplacement, result);
				StringAssert.Contains (projectName, result);
			} finally {
				File.Delete (templatePath);
			}
		}
	}
}
