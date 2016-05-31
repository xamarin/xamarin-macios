//
// Unit tests for CFNotificationCenter
//
// Authors:
//	Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Net;

#if XAMCORE_2_0
using Foundation;
using CoreFoundation;
#else
#if MONOMAC
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
#endif
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CFNotificationCenterTest
	{
		[Test]
		public void TestObservers ()
		{
			var target = new NSObject ();
			var d = CFNotificationCenter.Local;
			int count = 0;
			int count2 = 0;
			CFNotificationObserverToken o2 = null;
			var o1 = d.AddObserver ("hello", target, (x,dd)=>{
				count++;	
//				Console.WriteLine ("Here");

				if (count == 1)
					o2 = d.AddObserver ("hello", target, (y,ee)=> {
//						Console.WriteLine ("There");
						count2++;
					});
			});
			d.PostNotification ("hello", target, null, deliverImmediately:true);
			Assert.AreEqual (1, count);
			d.PostNotification ("hello", target, null, deliverImmediately:true);
			Assert.AreEqual (2, count);
			Assert.AreEqual (1, count2);

			// Remove the first observer, count should not be updated
			d.RemoveObserver (o1);
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (2, count2);

			// Remove the last observer, there should be no change in count
			d.RemoveObserver (o2);
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (2, count);
			Assert.AreEqual (2, count2);

			// Test removing all observers
			count = 0;
			o1 = d.AddObserver ("hello", target, (x,dd)=>{
				count++;	
				Console.WriteLine ("Here");
			});
			o2 = d.AddObserver ("hello", target, (y,ee)=> {count++;});
			d.RemoveEveryObserver ();
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (0, count);

			// Test removing from a callback
			count = 0;
			o2 = d.AddObserver ("hello", target, (y,ee)=> {count++; d.RemoveObserver (o2); });
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (1, count);
			d.PostNotification ("hello", target, null);
			Assert.AreEqual (1, count);
		}

	}
}

