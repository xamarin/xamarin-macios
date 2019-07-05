using System;
using System.Security.Cryptography;

using Foundation;

using NUnit.Framework;

namespace LinkAnyTest {
	// This test is included in both the LinkAll and LinkSdk projects.
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LinkAnyTest {
		[Test]
		public void AES ()
		{
			Assert.NotNull (Aes.Create (), "AES");
		}
	}
}
