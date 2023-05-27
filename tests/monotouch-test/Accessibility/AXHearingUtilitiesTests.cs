//
// Copyright 2021 Microsoft Corp
//
// Authors:
//	Rachel Kang (rachelkang@microsoft.com)
//

#if !__TVOS__ && !MONOMAC

using System;
using Foundation;
using Accessibility;
using NUnit.Framework;

namespace MonoTouchFixtures.Accessibility {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AXHearingUtilitiesTests {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
		}

		[Test]
		public void GetHearingDeviceEar ()
		{
			Assert.That (AXHearingUtilities.GetMFiHearingDeviceStreamingEar (), Is.EqualTo (AXHearingDeviceEar.None).Or.EqualTo (AXHearingDeviceEar.Both), "default");
		}

		[Test]
		public void GetDoesSupportBidirectionalHearing ()
		{
			Assert.That (AXHearingUtilities.SupportsBidirectionalStreaming (), Is.EqualTo (false).Or.EqualTo (true), "GetDoesSupportBidirectionalHearing");
		}

		[Test]
		public void GetHearingDevicePairedUuids ()
		{
			NSUuid [] emptyArray = new NSUuid [0];
			Assert.That (AXHearingUtilities.GetMFiHearingDevicePairedUuids (), Is.EqualTo (emptyArray), "GetHearingDevicePairedUuids");
		}
	}
}
#endif
