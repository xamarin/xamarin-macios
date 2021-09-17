//
// Unit tests for StatusCodeError
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using Foundation;
using SystemConfiguration;
using NUnit.Framework;

namespace MonoTouchFixtures.SystemConfiguration {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StatusCodeErrorTest {

		[Test]
		public void InvalidStatusCode ()
		{
			var s = StatusCodeError.GetErrorDescription ((StatusCode) 1);
			// "Operation not permitted" (might be localized so we just check non-null)
			Assert.NotNull (s, "1");
			s = StatusCodeError.GetErrorDescription ((StatusCode) Int32.MinValue);
			// in previous version of xcode, if the error was not known you would get a null ptr, in Xcode 13 and later you
			// get a message stating that the error is not knwon.
			if (TestRuntime.CheckXcodeVersion (13, 0, 0)) {
				Assert.NotNull (s, "MinValue null");
				Assert.True (s.StartsWith ("Unknown error:"), "MinValue value");
			} else {
				Assert.Null (s, "MinValue");
			}
		}
	}
}

#endif // !__WATCHOS__
