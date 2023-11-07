//
// Unit tests for CFRunLoop
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

using System;
using Foundation;
using CoreFoundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RunLoopTest {

		[Test]
		public void AllModes ()
		{
			Assert.That (CFRunLoop.Main.AllModes, Is.Not.Empty, "AllModes");
		}

		[Test]
		public void CurrentMode ()
		{
			Assert.DoesNotThrow (() => { GC.KeepAlive (CFRunLoop.Main.CurrentMode); }, "CurrentMode");
		}

		[Test]
		public void RunInMode ()
		{
			var loop = CFRunLoop.Main;
			Assert.DoesNotThrow (() => { loop.RunInMode ((string) loop.AllModes [0], 0.01, false); }, "RunInMode (string, false)");
			Assert.DoesNotThrow (() => { loop.RunInMode ((string) loop.AllModes [0], 0.01, true); }, "RunInMode (string, true)");
			Assert.DoesNotThrow (() => { loop.RunInMode ((NSString) loop.AllModes [0], 0.01, false); }, "RunInMode (NSString, false)");
			Assert.DoesNotThrow (() => { loop.RunInMode ((NSString) loop.AllModes [0], 0.01, true); }, "RunInMode (NSString, true)");
		}
	}
}
