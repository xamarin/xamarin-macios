#if __MACOS__
using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Foundation;

#nullable enable

namespace MonoTouchFixtures.Foundation {
	public class NSHostTest {

		public void EqualsNullAllowed ()
		{
			using var host = NSHost.FromAddress ("http://microsoft.com");
			Assert.False (host.Equals (null));
		}
	}
}
#endif
