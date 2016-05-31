//
// Unit tests CFHTTPMessage
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Net;
#if XAMCORE_4_0
using CFNetwork;
#elif XAMCORE_2_0
using CoreServices;
#endif
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.CoreServices;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreServices {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HttpMessageTest {

		[Test]
		public void CreateEmptyTrue ()
		{
			using (var m = CFHTTPMessage.CreateEmpty (true)) {
				Assert.That (m.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.False (m.IsHeaderComplete, "IsHeaderComplete");
				Assert.True (m.IsRequest, "IsRequest");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusCode; }, "ResponseStatusCode");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusLine; }, "ResponseStatusLine");
				Assert.That (m.Version.ToString (), Is.EqualTo ("1.1"), "Version");
			}
		}

		[Test]
		public void CreateEmptyFalse ()
		{
			using (var m = CFHTTPMessage.CreateEmpty (false)) {
				Assert.That (m.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.False (m.IsHeaderComplete, "IsHeaderComplete");
				Assert.False (m.IsRequest, "IsRequest");
				Assert.That (m.ResponseStatusCode, Is.EqualTo (HttpStatusCode.OK), "ResponseStatusCode");
				Assert.That (m.ResponseStatusLine, Is.Empty, "ResponseStatusLine");
				Assert.That (m.Version.ToString (), Is.EqualTo ("1.1"), "Version");
			}
		}

		[Test]
		public void CreateRequest10 ()
		{
			using (var m = CFHTTPMessage.CreateRequest (new Uri ("http://www.xamarin.com"), "GET", new Version (1, 0))) {
				Assert.That (m.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.False (m.IsHeaderComplete, "IsHeaderComplete");
				Assert.True (m.IsRequest, "IsRequest");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusCode; }, "ResponseStatusCode");
				Assert.Throws<InvalidOperationException> (delegate { var x = m.ResponseStatusLine; }, "ResponseStatusLine");
				Assert.That (m.Version.ToString (), Is.EqualTo ("1.0"), "Version");
			}
		}
	}
}

#endif // !__WATCHOS__
