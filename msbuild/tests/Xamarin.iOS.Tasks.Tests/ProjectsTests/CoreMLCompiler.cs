using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class CoreMLCompiler : ProjectTest
	{
		public CoreMLCompiler (string platform) : base (platform)
		{
		}

		void AssertCompiledModelExists (string modelName)
		{
			var expected = new string[] { "coremldata.bin", "model.espresso.net", "model.espresso.shape", "model.espresso.weights", "model/coremldata.bin", "neural_network_optionals/coremldata.bin" };
			var mlmodelc = Path.Combine (AppBundlePath, modelName + ".mlmodelc");

			Assert.IsTrue (Directory.Exists (mlmodelc));

			var files = new HashSet<string> (Directory.EnumerateFiles (mlmodelc, "*.*", SearchOption.AllDirectories));

			Assert.AreEqual (expected.Length, files.Count);

			foreach (var name in expected)
				Assert.IsTrue (files.Contains (Path.Combine (mlmodelc, name)), "{0} not found", name);
		}

		[Test]
		public void RebuildTest ()
		{
			BuildProject ("MyCoreMLApp", Platform, "Debug", clean: true);

			AssertCompiledModelExists ("SqueezeNet");

			Thread.Sleep (1000);

			// Rebuild w/ no changes
			BuildProject ("MyCoreMLApp", Platform, "Debug", clean: false);

			AssertCompiledModelExists ("SqueezeNet");
		}
	}
}
