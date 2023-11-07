using NUnit.Framework;
using System;

using ObjCRuntime;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MonoTouchFixtures.Foundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSOperatingSystemVersionTest {

		// IEqual Tests

		public static IEnumerable<(NSOperatingSystemVersion, NSOperatingSystemVersion, bool)> VersionCasesEqual {
			get {
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1), Result: true);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1), VersionTwo: new NSOperatingSystemVersion (major: (nint) 2), Result: false);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 1), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1), Result: false);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 0), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1), Result: true);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 1, patchVersion: (nint) 2), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), Result: false);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), Result: true);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 0), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2), Result: true);

			}
		}

		[Test, TestCaseSource ("VersionCasesEqual")]
		public void NSVersionEqual ((NSOperatingSystemVersion v1, NSOperatingSystemVersion v2, bool res) sample)
		{
			Assert.AreEqual (sample.v1 == sample.v2, sample.res, $"{sample.v1}=={sample.v2}");
			Assert.AreEqual (sample.v2 == sample.v1, sample.res, $"{sample.v2}=={sample.v1}");
			Assert.AreNotEqual (sample.v1 != sample.v2, sample.res, $"{sample.v1}!={sample.v2}");
		}

		// Object Equal Tests

		public static IEnumerable<(Object, bool)> VersionCasesObjectEqual {
			get {
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), Result: true);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 2), Result: false);
				yield return ("hello", Result: false);
				yield return (null, Result: false);
			}
		}

		[Test, TestCaseSource ("VersionCasesObjectEqual")]
		public void NSVersionObject ((Object obj, bool res) sample)
		{
			NSOperatingSystemVersion v1 = new NSOperatingSystemVersion ((nint) 1, (nint) 2, (nint) 3);
			Assert.AreEqual (v1.Equals (sample.obj), sample.res);
		}

		// Object Compare Tests

		public static IEnumerable<(Object, int)> VersionCasesObjectCompare {
			get {
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), Result: 0);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 2), Result: -1);
				yield return ("hello", Result: 1);
				yield return (null, Result: 1);
			}
		}

		[Test, TestCaseSource ("VersionCasesObjectCompare")]
		public void NSVersionCompareObject ((Object obj, int result) sample)
		{
			NSOperatingSystemVersion v1 = new NSOperatingSystemVersion ((nint) 1, (nint) 2, (nint) 3);
			Assert.AreEqual (v1.CompareTo (sample.obj), sample.result);
		}

		// Hash Tests

		public static IEnumerable<NSOperatingSystemVersion> VersionCasesHashCode {
			get {
				yield return (new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3));
				yield return (new NSOperatingSystemVersion (major: (nint) 2));
				yield return (new NSOperatingSystemVersion (major: (nint) 2, minor: (nint) 5));
				yield return (new NSOperatingSystemVersion (major: (nint) 2, minor: (nint) 9, patchVersion: (nint) 10));
			}
		}

		[Test, TestCaseSource ("VersionCasesHashCode")]
		public void NSVersionHashCode (NSOperatingSystemVersion version)
			=> Assert.AreEqual (version.GetHashCode (), version.GetHashCode ());


		// ToString Tests

		public static IEnumerable<(NSOperatingSystemVersion, string)> VersionCasesString {
			get {
				yield return (new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), "1.2.3");
				yield return (new NSOperatingSystemVersion (major: (nint) 2), "2.0.0");
				yield return (new NSOperatingSystemVersion (major: (nint) 2, minor: (nint) 5), "2.5.0");
				yield return (new NSOperatingSystemVersion (major: (nint) 2, minor: (nint) 9, patchVersion: (nint) 10), "2.9.10");
			}
		}

		[Test, TestCaseSource ("VersionCasesString")]
		public void NSVersionToString ((NSOperatingSystemVersion version, string versionStr) sample)
			=> Assert.AreEqual (sample.version.ToString (), sample.versionStr);


		// ICompare Tests

		public static IEnumerable<(NSOperatingSystemVersion, NSOperatingSystemVersion, int)> VersionCasesCompare {
			get {
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1), Result: 0);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1), VersionTwo: new NSOperatingSystemVersion (major: (nint) 2), Result: -1);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 1), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1), Result: 1);
				yield return (VersionOne: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 2), VersionTwo: new NSOperatingSystemVersion (major: (nint) 1, minor: (nint) 2, patchVersion: (nint) 3), Result: -1);
			}
		}

		[Test, TestCaseSource ("VersionCasesCompare")]
		public void NSVersionCompare ((NSOperatingSystemVersion v1, NSOperatingSystemVersion v2, int res) sample)
			=> Assert.AreEqual (sample.v1.CompareTo (sample.v2), sample.res);

	}
}
