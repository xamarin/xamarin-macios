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
			Assert.Null (s, "MinValue");
		}
	}
}

#endif // !__WATCHOS__
