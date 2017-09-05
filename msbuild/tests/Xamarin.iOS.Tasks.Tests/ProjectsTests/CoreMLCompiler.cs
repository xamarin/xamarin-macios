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

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MarsHabitatPricePredictor", Platform, "Debug");
		}
	}
}
