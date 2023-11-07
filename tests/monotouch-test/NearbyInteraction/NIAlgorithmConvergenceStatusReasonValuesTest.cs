// Copyright 2022 Microsoft Corp.

#if IOS || WATCH || __MACCATALYST__

using System;
using Foundation;
using ObjCRuntime;
using NearbyInteraction;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.NearbyInteraction {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NIAlgorithmConvergenceStatusReasonValuesTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
		}

		[Test]
		public void GetConvergenceStatusReasonTest ()
		{
			var reason = NIAlgorithmConvergenceStatusReason.InsufficientHorizontalSweep;
			Assert.IsNotNull (NIAlgorithmConvergenceStatusReasonValues.GetConvergenceStatusReason (reason), "NIAlgorithmConvergenceStatusReason should not be null.");
		}
	}
}

#endif // IOS || WATCH || __MACCATALYST__
