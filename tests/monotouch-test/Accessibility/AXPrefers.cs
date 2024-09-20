//
// Copyright 2024 Microsoft Corp
//

using System;
using Foundation;
using Accessibility;
using NUnit.Framework;

namespace MonoTouchFixtures.Accessibility {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AXPrefersTests {
		[Test]
		public void HorizontalTextEnabled ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);
			Assert.That (AXPrefers.HorizontalTextEnabled, Is.EqualTo (true).Or.EqualTo (false), "HorizontalTextEnabled");
		}

#if NET
		[Test]
		public void NonBlinkingTextInsertionIndicator ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			Assert.That (AXPrefers.NonBlinkingTextInsertionIndicator, Is.EqualTo (true).Or.EqualTo (false), "NonBlinkingTextInsertionIndicator");
		}
#endif
	}
}
