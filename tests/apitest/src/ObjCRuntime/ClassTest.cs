//
// Unit tests for Class
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoMacFixtures.ObjCRuntime {
	
	[TestFixture]
	public class ClassTest {
		[Test]
		public void ThrowOnMissingNativeClassTest ()
		{
			bool saved = Class.ThrowOnInitFailure;
			
			Class.ThrowOnInitFailure = true;
			try {
				new InexistentClass ();
				Assert.Fail ("a");
			} catch {
				// OK
			} finally {
				Class.ThrowOnInitFailure = saved;
			}
		}
		
		[Register ("Inexistent", true)]
		public class InexistentClass : NSObject {
			public override IntPtr ClassHandle {
				get {
					return Class.GetHandle (GetType ().Name);
				}
			}
		}
	}
}
