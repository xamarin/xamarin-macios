using NUnit.Framework;

using Xamarin.Bundler;
using Xamarin.Utils;

[SetUpFixture]
public class AssemblyInitialization {
	[OneTimeSetUp]
	public void Setup ()
	{
		ErrorHelper.Platform = ApplePlatform.iOS;
	}
}
