#if !__WATCHOS__
using System;
using System.Threading;
using Foundation;
using Network;
using ObjCRuntime;
using CoreFoundation;

using NUnit.Framework;
using NUnit.Framework.Internal;
using UIKit;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWResolutionReportTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13,0);
		}

		[Test]
		public void SourceTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void MillisecondsTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void EndpointCountTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void SuccessfulEndpointTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void PreferredEndpointTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void ProtocolTest ()
		{
			Assert.Fail ("Not implemented");
		}
	}
}
#endif
