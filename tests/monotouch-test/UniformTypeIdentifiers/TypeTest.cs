using System;
using NUnit.Framework;

using Foundation;
using UniformTypeIdentifiers;

namespace MonoTouchFixtures.UniformTypeIdentifiers {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UTTypeTests {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
		}

		[Test]
		public void Archive ()
		{
			var a = UTTypes.Archive;
			Assert.False (a.Dynamic, "Dynamic");
			var z = UTTypes.Zip;
			Assert.True (z.IsSubtypeOf (a), "IsSubtypeOf");
			Assert.True (a.IsSupertypeOf (z), "IsSupertypeOf");
		}
	}
}
