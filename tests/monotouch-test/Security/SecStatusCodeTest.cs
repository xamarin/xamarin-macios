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

using Foundation;
using Security;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SecStatusCodeTest {

		[Test]
		[Culture ("en")]
		public void ErrorDescriptionTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 3);

			var desc = SecStatusCode.Success.GetStatusDescription ();
			Assert.NotNull (desc, $"{nameof (desc)} not null");

			var noErr = "No error.";
			Assert.That (desc, Is.EqualTo (noErr), $"{nameof (desc)} == {noErr}");
		}
	}
}
