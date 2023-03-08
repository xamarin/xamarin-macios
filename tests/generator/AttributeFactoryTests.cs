using System;
using NUnit.Framework;
using ObjCRuntime;

namespace GeneratorTests {

	[TestFixture]
	[Parallelizable (ParallelScope.All)]
	public class AttributeFactoryTests {

		void AssertAttributeCreation<T> (Func<PlatformName, int, int, string?, Attribute> callback, PlatformName platform,
			int major, int minor, string? message = null) where T : AvailabilityBaseAttribute
		{
			var attr = callback (platform, major, minor, message) as T;
			Assert.IsNotNull (attr, "attribute type");
			Assert.AreEqual (platform, attr.Platform, "Platform");
			Assert.AreEqual (major, attr.Version.Major, "Major");
			Assert.AreEqual (minor, attr.Version.Minor, "Minor");
			Assert.AreEqual (message, attr.Message);
		}


		[Test]
		public void CreateNoVersionSupportedAttributeTest ()
		{
			Assert.Fail("Not implemented.");
		}

		[Test]
		public void CreateUnsupportedAttributeTest ()
		{
			Assert.Fail("Not implemented.");
		}

		[Test]
		public void FindHighestIntroducedAttributesTest ()
		{
			Assert.Fail("Not implemented.");
		}

		[Test]
		public void CopyValidAttributesTest ()
		{
			Assert.Fail("Not implemented.");
		}
	}
}
