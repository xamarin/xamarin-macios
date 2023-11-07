// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ApplicationTest {

		[Test]
		public void BackgroundTaskInvalid ()
		{
			Assert.That (UIApplication.BackgroundTaskInvalid.ToString () == "0", "#1");
			Assert.That (UIApplication.BackgroundTaskInvalid == 0, "#2");
		}

#if !__TVOS__
		[Test]
		public void SetKeepAliveTimeout_Null ()
		{
			Assert.False (UIApplication.SharedApplication.SetKeepAliveTimeout (600, null), "SetKeepAliveTimeout");
		}
#endif

		[Test]
		public void BeginBackgroundTask_Null ()
		{
			var taskid = UIApplication.SharedApplication.BeginBackgroundTask (null);
			Assert.That (taskid, Is.Not.EqualTo (UIApplication.BackgroundTaskInvalid), "BeginBackgroundTask");
			UIApplication.SharedApplication.EndBackgroundTask (taskid);
		}

		[Test]
		public void MinimumKeepAliveTimeout ()
		{
			// NSTimeInternal (double) not NSString
			Assert.That (UIApplication.MinimumKeepAliveTimeout, Is.EqualTo (600.0), "MinimumKeepAliveTimeout");
		}

		[Test]
		public void SendAction_Null ()
		{
			UIApplication.SharedApplication.SendAction (new Selector ("someSelector"), null, null, null);
		}
	}
}

#endif // !__WATCHOS__
