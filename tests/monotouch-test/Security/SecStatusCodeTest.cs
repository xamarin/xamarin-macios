// 
// SecStatusCodeTest.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
// 
// Copyright 2018 Xamarin Inc.
//

using System;
using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.Security;
#endif

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SecStatusCodeTest {

		[Test]
		public void ErrorDescriptionTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 3);

			var desc = SecStatusCode.Success.GetStatusDescription ();
			Assert.NotNull (desc, $"{nameof (desc)} not null");

			// This value generated from objc enum documentation and very unlikely to change.
			var noErr = "No error.";
			Assert.That (desc, Is.EqualTo (noErr), $"{nameof (desc)} == {noErr}");
		}
	}
}
