//
// Unit tests for NSObject
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012, 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Drawing;
using System.Reflection;
using System.Threading;

#if XAMCORE_2_0
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using Security;
#if MONOMAC
using AppKit;
using PlatformException = Foundation.ObjCException;
using UIView = AppKit.NSView;
#else
using UIKit;
using PlatformException=Foundation.MonoTouchException;
#endif
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using PlatformException=MonoTouch.Foundation.MonoTouchException;
using MonoTouch.Security;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSObjectTest {
		
		bool GetIsDirectBinding (NSObject obj)
		{
#if XAMCORE_2_0
			int flags = (byte) typeof (NSObject).GetField ("flags", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic).GetValue (obj);
			return (flags & 4) == 4;
#else
			return (bool) typeof (NSObject).GetField ("IsDirectBinding", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic).GetValue (obj);
#endif
		}
		
		class MyObject : NSObject {
			
			public bool GetIsDirectBinding ()
			{
				return this.IsDirectBinding;
			}
		}

		[Test]
		public void IsDirectBinding ()
		{
			using (var o1 = new NSObject ()) {
				Assert.True (GetIsDirectBinding (o1), "inside monotouch.dll");
			}
			using (var o2 = new MyObject ()) {
				Assert.False (o2.GetIsDirectBinding (), "outside monotouch.dll");
			}
		}

		[Test]
		public void SuperClass ()
		{
			Class c = new Class ("NSObject");
			Assert.That (c.Name, Is.EqualTo ("NSObject"), "Name");
			Assert.That (c.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			Assert.That (c.SuperClass, Is.EqualTo (IntPtr.Zero), "SuperClass");
		}

		[Test]
		public void FromObject_INativeObject ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=8458
			using (CGPath p = CGPath.FromRect (new RectangleF (1, 2, 3, 4))) {
				Assert.IsNotNull (NSObject.FromObject (p), "CGPath");
			}
			using (CGColor c = new CGColor (CGColorSpace.CreateDeviceRGB (), new nfloat [] { 0.1f, 0.2f, 0.3f, 1.0f })) {
				Assert.IsNotNull (NSObject.FromObject (c), "CGColor");
			}
			var hasSecAccessControl = TestRuntime.CheckXcodeVersion (6, 0);
#if __MACOS__
			if (!TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 10))
				hasSecAccessControl = false;
#endif
			if (hasSecAccessControl) {
				using (var sac = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly)) {
					Assert.IsNotNull (NSObject.FromObject (sac), "SecAccessControl");
				}
			}
		}

		[Test]
		public void FromObject_Handle ()
		{
			using (CGPath p = CGPath.FromRect (new RectangleF (1, 2, 3, 4))) {
				Assert.IsNotNull (NSObject.FromObject (p.Handle), "CGPath");
			}
			using (CGColor c = new CGColor (CGColorSpace.CreateDeviceRGB (), new nfloat [] { 0.1f, 0.2f, 0.3f, 1.0f })) {
				Assert.IsNotNull (NSObject.FromObject (c.Handle), "CGColor");
			}
		}

		[Test]
		public void FromObject_NativeTypes ()
		{
			// to avoid issues like https://github.com/mono/xwt/commit/9b110e848030d5f6a0319212fd21bac02efad2c1
			using (var nativeint = (NSNumber) NSObject.FromObject ((nint)(-42))) {
				Assert.That (nativeint.Int32Value, Is.EqualTo (-42), "nint");
			}
			using (var nativeuint = (NSNumber) NSObject.FromObject ((nuint)42)) {
				Assert.That (nativeuint.UInt32Value, Is.EqualTo (42), "nuint");
			}
			using (var nativefloat = (NSNumber) NSObject.FromObject ((nfloat)3.14)) {
				Assert.That (nativefloat.FloatValue, Is.EqualTo (3.14f), "nfloat");
			}
		}

		[Test]
		public void ValueForInvalidKeyTest ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=13243
			Assert.Throws<PlatformException> (() =>
			{
				using (var str = new NSString ("test")) {
					str.ValueForKey(str);
				}
			});
		}

		[Test]
		public void Copy ()
		{
			IntPtr nscopying = Runtime.GetProtocol ("NSCopying");
			Assert.That (nscopying, Is.Not.EqualTo (IntPtr.Zero), "NSCopying");

			IntPtr nsmutablecopying = Runtime.GetProtocol ("NSMutableCopying");
			Assert.That (nsmutablecopying, Is.Not.EqualTo (IntPtr.Zero), "NSMutableCopying");

			// NSObject does not conform to NSCopying
			using (var o = new NSObject ()) {
				Assert.False (o.ConformsToProtocol (nscopying), "NSObject/NSCopying");
				Assert.False (o.ConformsToProtocol (nsmutablecopying), "NSObject/NSMutableCopying");
			}

			// NSNumber conforms to NSCopying - but not NSMutableCopying
			using (var n = new NSNumber (-1)) {
				Assert.True (n.ConformsToProtocol (nscopying), "NSNumber/NSCopying");
				using (var xn = n.Copy ()) {
					Assert.NotNull (xn, "NSNumber/Copy/NotNull");
					Assert.AreSame (n, xn, "NSNumber/Copy/NotSame");
				}
				Assert.False (n.ConformsToProtocol (nsmutablecopying), "NSNumber/NSMutableCopying");
			}

			// NSMutableString conforms to NSCopying - but not NSMutableCopying
			using (var s = new NSMutableString (1)) {
				Assert.True (s.ConformsToProtocol (nscopying), "NSMutableString/NSCopying");
				using (var xs = s.Copy ()) {
					Assert.NotNull (xs, "NSMutableString/Copy/NotNull");
					Assert.AreNotSame (s, xs, "NSMutableString/Copy/NotSame");
				}
				Assert.True (s.ConformsToProtocol (nsmutablecopying), "NSMutableString/NSMutableCopying");
				using (var xs = s.MutableCopy ()) {
					Assert.NotNull (xs, "NSMutableString/MutableCopy/NotNull");
					Assert.AreNotSame (s, xs, "NSMutableString/MutableCopy/NotSame");
				}
			}
		}

		[Test]
		public void Encode ()
		{
			IntPtr nscoding = Runtime.GetProtocol ("NSCoding");
			Assert.That (nscoding, Is.Not.EqualTo (IntPtr.Zero), "NSCoding");

#if !XAMCORE_2_0
			// NSObject does not conform to NSCoding
			using (var o = new NSObject ()) {
				Assert.False (o.ConformsToProtocol (nscoding), "NSObject/NSCoding");
				using (var c = new NSCoder ()) {
					Assert.Throws<InvalidOperationException> (delegate {
						o.EncodeTo (c);
					}, "NSObject/!NSCoding");
				}
			}
#endif

			// NSNumber conforms to NSCoding
			using (var n = new NSNumber (-1)) {
				Assert.True (n.ConformsToProtocol (nscoding), "NSNumber/NSCoding");
				using (var d = new NSMutableData ())
				using (var a = new NSKeyedArchiver (d)) {
					n.EncodeTo (a);
					a.FinishEncoding ();
				}
			}
		}

#if !XAMCORE_2_0
		[Test]
		public void CtorNSCoder ()
		{
			// NSObject does NOT conform but (funnily enough) does not have a .ctor(NSCoder) like every other
			// type that subclass it. We'll use NSKeyedUnarchiver as it also does not conform to NSCoding
			using (var c = new NSCoder ()) {
				Assert.Throws<InvalidOperationException> (delegate {
					new NSKeyedUnarchiver (c);
				}, "NSObject/!NSCoding");
			}
		}
#endif

		[Test]
		public void Equality ()
		{
			using (var o1 = new NSObject ())
			using (var o2 = new NSObject ()) {
				Assert.False (o1.Equals ((object)null), "Equals(object) null");
				Assert.False (o1.Equals ((object)o2), "Equals(object) 1-2");
				Assert.False (o2.Equals ((object)o1), "Equals(object) 2-1");

				Assert.False (o1.Equals (3), "Equals(object) 1-3");

				Assert.False (o1.Equals ((NSObject)null), "Equals(NSObject) null");
				Assert.False (o1.Equals ((NSObject)o2), "Equals(NSObject) 1-2");
				Assert.False (o2.Equals ((NSObject)o1), "Equals(NSObject) 2-1");

				// on a more positive note...
				Assert.True (o1.Equals ((object)o1), "Equals(object) 1-1");
				Assert.True (o2.Equals ((NSObject)o2), "Equals(NSObject) 2-2");
			}
		}

		class NSOverrideEqualObject : NSObject {

			public NSOverrideEqualObject (bool throwEquals)
			{
				Throw = throwEquals;
			}

			bool Throw { get; set; }

			public bool Direct {
				get { return IsDirectBinding; }
			}

			public override bool Equals (object obj)
			{
				if (Throw)
					throw new NotFiniteNumberException ();
				return base.Equals (obj);
			}

			public override int GetHashCode ()
			{
				return 42;
			}
		}

		[Test]
		public void SubclassEquality ()
		{
			using (var o1 = new NSObject ())
			using (var o2 = new NSOverrideEqualObject (true))
			using (var o3 = new NSOverrideEqualObject (false)) {
				// true, same object
				Assert.True (o1.Equals (o1), "direct - direct / same");
				Assert.True (o3.Equals (o3), "indirect - indirect / same");

				// false, good since there's state in o2 and o3 that does not exists in o1 (direct / native only)
				Assert.False (o1.Equals (o2), "direct - indirect");
				Assert.False (o3.Equals (o1), "indirect - direct");

				// default is false, which is good since the managed state (Throw) differs between o2 and o3
				Assert.False (o3.Equals (o2), "indirect - indirect");

				// throws (as implemented above)
				Assert.Throws<NotFiniteNumberException> (() => { o2.Equals ((object) o1); }, "Equals(object) 2-1");

				// throws (as IEquatable<NSObject>.Equals calls _overriden_ Equals
				Assert.Throws<NotFiniteNumberException> (() => { o2.Equals ((NSObject) o1); }, "Equals(NSObject) 2-1");
			}
		}

#if !__WATCHOS__ // FIXME: this test can probably be fixed to run on WatchOS by testing something other than UIView
		[Test]
		public void ObserverTest ()
		{
			bool observed = false;
			using (var o = new UIView ()) {
				using (var observer = o.AddObserver ("frame", NSKeyValueObservingOptions.OldNew, change => {
					var old = ((NSValue) change.OldValue).CGRectValue;
					var @new = ((NSValue) change.NewValue).CGRectValue;
					Assert.AreEqual ("{X=0,Y=0,Width=0,Height=0}", old.ToString (), "#old");
					Assert.AreEqual ("{X=0,Y=0,Width=123,Height=234}", @new.ToString (), "#new");
					observed = true;
				})) {
					o.Frame = new RectangleF (0, 0, 123, 234);
				}
			}
			Assert.IsTrue (observed, "observed");
		}
#endif // !__WATCHOS__

		[Test]
		[Timeout (5000)]
		public void InvokeTest ()
		{
			var evt = new ManualResetEvent (false);
			using (var obj = new NSObject ())
				obj.Invoke (() => evt.Set (), .2);
			while (!evt.WaitOne (1))
				NSRunLoop.Current.RunUntil (NSRunLoopMode.Default, NSDate.Now.AddSeconds (1));

			Assert.True (evt.WaitOne (1), "Our invoke was not fired?");
		}
	}
}
