using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class Issue3199Test {

		[Test]
		// FIXME: Replace by actual MSBuild test when we transition the tests from xbuild to MSBuild: https://github.com/xamarin/xamarin-macios/issues/3810
		public void NetstandardPackageReferenceTest ()
		{
			// The error only happens when *building* for device, when we try to AOT a reference assembly.
			TestRuntime.AssertDevice ();

			var dt = new KeyValuePair<DateTime, decimal> [2];
			// The assert is pretty irrelevent but a test without an assert isn't a real test :P
			// What matter here is that the call requires `System.Runtime.CompilerServices.Unsafe` which comes from a NugetPackage added via
			// the PackageReference mechanism. Without the fix to https://github.com/xamarin/xamarin-macios/issues/3199 we get a build error for monotouch-test.
			Assert.DoesNotThrow (() => Unsafe.As<KeyValuePair<DateTime, decimal>, byte> (ref dt [0]), "Should not throw");
		}
	}
}
