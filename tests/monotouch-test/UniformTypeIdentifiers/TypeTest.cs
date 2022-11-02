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
			// This test may fail in the simulator, if the architecture of the simulator isn't the native one (say running x86_64 on an M1 machine),
			// so just skip this test for the simulator.
			TestRuntime.AssertIfSimulatorThenARM64 ();

			var a = UTTypes.Archive;
			Assert.False (a.Dynamic, "Dynamic");
			var z = UTTypes.Zip;
			Assert.True (z.IsSubtypeOf (a), "IsSubtypeOf");
			Assert.True (a.IsSupertypeOf (z), "IsSupertypeOf");
		}
	}
}
