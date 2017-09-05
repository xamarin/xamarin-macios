using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;

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
			var mlmodelcDir = Path.Combine (AppBundlePath, modelName + ".mlmodelc");
			var mlmodelc = Path.Combine (mlmodelcDir, "coremldata.bin");
			var model0 = Path.Combine (mlmodelcDir, "model0", "coremldata.bin");
			var model1 = Path.Combine (mlmodelcDir, "model1", "coremldata.bin");

			Assert.IsTrue (File.Exists (mlmodelc), "{0} does not exist", mlmodelc);
			Assert.IsTrue (File.Exists (model0), "{0} does not exist", model0);
			Assert.IsTrue (File.Exists (model1), "{0} does not exist", model1);
		}

		[Test]
		public void RebuildTest ()
		{
			BuildProject ("MarsHabitatPricePredictor", Platform, "Debug", clean: true);

			AssertCompiledModelExists ("MarsHabitatPricer");

			Thread.Sleep (1000);

			// Rebuild w/ no changes
			BuildProject ("MarsHabitatPricePredictor", Platform, "Debug", clean: false);

			AssertCompiledModelExists ("MarsHabitatPricer");
		}
	}
}
