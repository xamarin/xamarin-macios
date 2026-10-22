using NUnit.Framework;

namespace Xamarin.Tests {
	// These cases exercise adapter discovery and filtering, not Apple builds.
	[TestFixture]
	public class DotNetProjectTest {
		[TestCase ("plain")]
		[TestCase ("Xamarin.Tests.AppIconTest.FutureCase")]
		public void ProjectCase (string value)
		{
			Assert.That (value, Is.Not.Empty);
		}
	}

	[TestFixture]
	public class AppIconTest {
		[TestCase ("plain")]
		[TestCase ("Xamarin.Tests.DotNetProjectTest.FutureCase")]
		public void AppIconCase (string value)
		{
			Assert.That (value, Is.Not.Empty);
		}
	}

	[TestFixture]
	public class BundleStructureTest {
		[Test]
		public void BundleStructureCase ()
		{
			Assert.Pass ();
		}
	}

	[TestFixture]
	public class PostBuildTest {
		[Test]
		public void PostBuildCase ()
		{
			Assert.Pass ();
		}
	}

	[TestFixture]
	public class PrepareAssembliesTest {
		[Test]
		public void PrepareAssembliesCase ()
		{
			Assert.Pass ();
		}
	}

	[TestFixture]
	public class FutureFixture {
		[TestCase ("plain", "plain")]
		[TestCase ("Xamarin.Tests.DotNetProjectTest.FutureCase", "Xamarin.Tests.AppIconTest.FutureCase")]
		public void FutureCase (string first, string second)
		{
			Assert.That (first, Is.Not.Empty);
			Assert.That (second, Is.Not.Empty);
		}

		[Test]
		[Category ("Multiplatform")]
		public void MultiplePlatformsCase ()
		{
			Assert.Pass ();
		}

		[Test]
		[Category ("Windows")]
		public void WindowsCase ()
		{
			Assert.Pass ();
		}
	}
}
