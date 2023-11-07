#if !__WATCHOS__
using System;
using System.Threading;
using Network;
using CoreFoundation;
using Foundation;

using NUnit.Framework;

#nullable enable

namespace MonoTouchTest.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWPrivacyContextTest {
		NWPrivacyContext? context;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			context = NWPrivacyContext.Default;
		}

		[TearDown]
		public void TearDown ()
			=> context?.Dispose ();

		[Test]
		public void FlushCacheTest ()
			=> Assert.DoesNotThrow (() => context.FlushCache ());

		[Test]
		public void DisableLoggingTest ()
			=> Assert.DoesNotThrow (() => context.DisableLogging ());

		[Test]
		public void RequireEncryptedNameResolutionTest ()
			=> Assert.DoesNotThrow (() => context.RequireEncryptedNameResolution (true, null));
	}
}
#endif
