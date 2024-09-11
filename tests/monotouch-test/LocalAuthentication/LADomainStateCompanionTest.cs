#if HAS_LOCALAUTHENTICATION
using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;
using LocalAuthentication;

namespace MonoTouchFixtures.LocalAuthentication {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LADomainStateCompanionTest {

		[Test]
		public void AvailableCompanionTypes ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var context = new LAContext ();
			Assert.IsNotNull (context.DomainState, "DomainState");
			Assert.IsNotNull (context.DomainState.Companion, "DomainState.Companion");
			Assert.IsNotNull (context.DomainState.Companion.WeakAvailableCompanionTypes, "DomainState.Companion.WeakAvailableCompanionTypes");
			Assert.That (context.DomainState.Companion.AvailableCompanionTypes, Is.EqualTo (LACompanionType.None).Or.EqualTo (LACompanionType.Watch), "DomainState.Companion.AvailableCompanionTypes");
		}
	}
}
#endif // HAS_LOCALAUTHENTICATION
