using System;
using System.Collections;
using NUnit.Framework;
using ObjCRuntime;
using Xamarin.Utils;

#nullable enable

namespace GeneratorTests {

	[TestFixture]
	[Parallelizable (ParallelScope.All)]
	public class AttributeFactoryTests {
		static void AssertAttributeCreation<T> (Func<PlatformName, int, int, string?, T> callback, PlatformName platform,
			int major, int minor, string? message = null) where T : AvailabilityBaseAttribute
		{
			var typeName = typeof (T).Name;
			var attr = callback (platform, major, minor, message) as T;
			Assert.IsNotNull (attr, $"{typeName} attribute type");
			Assert.AreEqual (platform, attr.Platform, $"{typeName} Platform");
			Assert.AreEqual (major, attr.Version.Major, $"{typeName} Major");
			Assert.AreEqual (minor, attr.Version.Minor, $"{typeName} Minor");
			Assert.AreEqual (message, attr.Message);
		}

		static void AssertAttributeCreationNotVersion<T> (Func<PlatformName, string?, T> callback, PlatformName platform,
			string? message = null) where T : AvailabilityBaseAttribute
		{
			var typeName = typeof (T).Name;
			var attr = callback (platform, message) as T;
			Assert.IsNotNull (attr, $"{typeName} attribute type");
			Assert.AreEqual (platform, attr.Platform, $"{typeName} Platform");
			Assert.AreEqual (message, attr.Message);
		}


		// simple tests, but we want to test it
		[TestCase (PlatformName.iOS, 13, 4, "message")]
		[TestCase (PlatformName.iOS, 12, 4, null)]
		public void CreateAttributeTest (PlatformName platform, int major, int minor, string? message)
		{
			// call several times with diff types
			AssertAttributeCreation (AttributeFactory.CreateNewAttribute<IntroducedAttribute>, platform, major, minor, message);
			AssertAttributeCreation (AttributeFactory.CreateNewAttribute<DeprecatedAttribute>, platform, major, minor, message);
			AssertAttributeCreation (AttributeFactory.CreateNewAttribute<ObsoletedAttribute>, platform, major, minor, message);
		}

		[TestCase (PlatformName.iOS, "message")]
		[TestCase (PlatformName.iOS, null)]
		public void CreateAttributeNoVersionTest (PlatformName platform, string? message)
		{
			// call several times with diff types
			AssertAttributeCreationNotVersion (AttributeFactory.CreateNewAttribute<IntroducedAttribute>, platform, message);
			AssertAttributeCreationNotVersion (AttributeFactory.CreateNewAttribute<UnavailableAttribute>, platform, message);
			AssertAttributeCreationNotVersion (AttributeFactory.CreateNewAttribute<DeprecatedAttribute>, platform, message);
			AssertAttributeCreationNotVersion (AttributeFactory.CreateNewAttribute<ObsoletedAttribute>, platform, message);
		}

		[TestCase (PlatformName.iOS)]
		[TestCase (PlatformName.MacCatalyst)]
		[TestCase (PlatformName.MacOSX)]
		[TestCase (PlatformName.TvOS)]
		public void CreateNoVersionSupportedAttributeTest (PlatformName platform)
			=> Assert.AreEqual (platform, AttributeFactory.CreateNoVersionSupportedAttribute (platform).Platform);

		[Test]
		public void CreateNoVersionSupportedAttributeWatchOSTest ()
			=> Assert.Throws<InvalidOperationException> (
				() => AttributeFactory.CreateNoVersionSupportedAttribute (PlatformName.WatchOS));

		[TestCase (PlatformName.iOS)]
		[TestCase (PlatformName.MacCatalyst)]
		[TestCase (PlatformName.MacOSX)]
		[TestCase (PlatformName.TvOS)]
		public void CreateUnsupportedAttributeTest (PlatformName platform)
			=> Assert.AreEqual (platform, AttributeFactory.CreateUnsupportedAttribute (platform).Platform);

		[TestCase (PlatformName.iOS)]
		[TestCase (PlatformName.MacCatalyst)]
		[TestCase (PlatformName.MacOSX)]
		[TestCase (PlatformName.TvOS)]
		public void CreateUnsupportedAttributeWatchOSTest (PlatformName platform)
			=> Assert.AreEqual (platform, AttributeFactory.CreateUnsupportedAttribute (platform).Platform);

		class CloneCasesNoVersionClass : IEnumerable {
			public IEnumerator GetEnumerator ()
			{
				yield return new object [] {

					new IntroducedAttribute (PlatformName.iOS),
					PlatformName.TvOS,
				};
				yield return new object [] {
					new DeprecatedAttribute (PlatformName.MacCatalyst),
					PlatformName.iOS,
				};
				yield return new object [] {
					new ObsoletedAttribute(PlatformName.WatchOS),
					PlatformName.iOS
				};
				yield return new object [] {
					new UnavailableAttribute (PlatformName.MacOSX),
					PlatformName.MacCatalyst
				};
			}
		}

		[TestCaseSource (typeof (CloneCasesNoVersionClass))]
		public void CloneNoVersionTest (AvailabilityBaseAttribute attributeToClone, PlatformName targetPlatform)
		{
			var clone = AttributeFactory.CloneFromOtherPlatform (attributeToClone, targetPlatform);
			Assert.AreEqual (targetPlatform, clone.Platform, "platform");
			Assert.AreEqual (attributeToClone.Message, clone.Message, "message");
			Assert.AreEqual (attributeToClone.GetType (), clone.GetType (), "type");
		}

		class CloneCasesMinVersionClass : IEnumerable {
			public IEnumerator GetEnumerator ()
			{
				yield return new object [] {
					new IntroducedAttribute (PlatformName.iOS, 1, 0),
					PlatformName.TvOS,
				};
				yield return new object [] {
					new DeprecatedAttribute (PlatformName.MacCatalyst, 1, 0),
					PlatformName.iOS,
				};
				yield return new object [] {
					new ObsoletedAttribute(PlatformName.WatchOS, 1, 0),
					PlatformName.iOS
				};
			}
		}

		[TestCaseSource (typeof (CloneCasesMinVersionClass))]
		public void CloneMinVersion (AvailabilityBaseAttribute attributeToClone, PlatformName targetPlatform)
		{
			var clone = AttributeFactory.CloneFromOtherPlatform (attributeToClone, targetPlatform);
			Assert.AreEqual (targetPlatform, clone.Platform, "platform");
			Assert.AreEqual (attributeToClone.Message, clone.Message, "message");
			Assert.AreEqual (attributeToClone.GetType (), clone.GetType (), "type");
#if NET
			if (clone.AvailabilityKind == AvailabilityKind.Introduced) {
				Assert.Null (clone.Version, "Version");
			} else {
				Assert.AreEqual (Xamarin.SdkVersions.GetMinVersion (targetPlatform.AsApplePlatform ()), clone.Version, "Version");
			}
#else
			Assert.AreEqual (Xamarin.SdkVersions.GetMinVersion (targetPlatform.AsApplePlatform ()), clone.Version, "Version");
#endif
		}

		class CloneCasesBuildVersionClass : IEnumerable {
			public IEnumerator GetEnumerator ()
			{
				var tvOsMin = Xamarin.SdkVersions.GetMinVersion (ApplePlatform.TVOS);
				tvOsMin = new Version (tvOsMin.Major, tvOsMin.Minor, tvOsMin.Build + 3);
				var iOSMin = Xamarin.SdkVersions.GetMinVersion (ApplePlatform.iOS);
				iOSMin = new Version (iOSMin.Major, iOSMin.Minor, iOSMin.Build + 3);
				yield return new object [] {
					new IntroducedAttribute (PlatformName.iOS, tvOsMin.Major, tvOsMin.Minor, tvOsMin.Build),
					PlatformName.TvOS,
				};
				yield return new object [] {
					new DeprecatedAttribute (PlatformName.MacCatalyst, iOSMin.Major, iOSMin.Minor, iOSMin.Build),
					PlatformName.iOS,
				};
				yield return new object [] {
					new ObsoletedAttribute(PlatformName.WatchOS, iOSMin.Major, iOSMin.Minor, iOSMin.Build),
					PlatformName.iOS
				};
			}
		}

		[TestCaseSource (typeof (CloneCasesBuildVersionClass))]
		public void CloneBuildVersion (AvailabilityBaseAttribute attributeToClone, PlatformName targetPlatform)
		{
			var clone = AttributeFactory.CloneFromOtherPlatform (attributeToClone, targetPlatform);
			Assert.AreEqual (targetPlatform, clone.Platform, "platform");
			Assert.AreEqual (attributeToClone.Message, clone.Message, "message");
			Assert.AreEqual (attributeToClone.GetType (), clone.GetType (), "type");
			Assert.AreEqual (attributeToClone.Version, clone.Version);
		}

	}
}
