#if !__WATCHOS__
using System;
using System.Threading;
using Network;
using CoreFoundation;
using Foundation;

using NUnit.Framework;


namespace monotouchtest.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWPrivacyContextTest {
		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13,0);
		}

		[Test]
		public void FlushCacheTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void DisableLoggingTest ()
		{
			Assert.Fail ("Not implemented");
		}

		[Test]
		public void RequireEncryptedNameResolutionTest ()
		{
			Assert.Fail ("Not implemented");
		}
	}
}
#endif
