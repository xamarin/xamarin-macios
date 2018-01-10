// Copyright 2014-2015 Xamarin Inc. All rights reserved.

#if WATCH || IOS

using System;
using System.Reflection;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {

#if !COREBUILD
	public partial class WKInterfaceController {

		public void PushController (string name, string context)
		{
			using (var ns = context == null ? null : new NSString (context)) {
				PushController (name, (NSObject) ns);
			}
		}

		public void PresentController (string name, string context)
		{
			using (var ns = context == null ? null : new NSString (context)) {
				PresentController (name, (NSObject) ns);
			}
		}

		public void PresentController (string [] names, string [] contexts)
		{
			NSObject[] array = null;
			try {
				if (contexts != null) {
					array = new NSObject [contexts.Length];
					for (int i = 0; i < array.Length; i++)
						array [i] = new NSString (contexts [i]);
				}
				PresentController (names, array);
			}
			finally {
				if (array != null) {
					foreach (var ns in array)
						ns.Dispose ();
				}
			}
		}

		static int counter = 0;

		static Selector GetUniqueSelector ()
		{
			return new Selector ("xamarinAddMenuItem" + counter++.ToString ());
		}

		MethodInfo GetMethodInfo (Action action)
		{
			var del = action as Delegate;
			if (del == null)
				throw new ArgumentNullException ("action");
			var met = del.Method;
			// <quote>The method must be defined on the current interface controller object.</quote>
			if (met.DeclaringType != GetType ())
				throw new ArgumentException ("Action delegate must declared in the same type.");
			// note that an anonymous delegate that does not use any instance member can be defined as static
			// e.g. Console.WriteLine ("coucou");
			if (met.IsStatic)
				throw new ArgumentException ("Action delegate must declared an instance (not static) method.");
			return met;
		}

		public void AddMenuItem (UIImage image, string title, Action action)
		{
			var mi = GetMethodInfo (action);
			var sel = GetUniqueSelector ();
			AddMenuItem (image, title, sel);
			Runtime.ConnectMethod (mi, sel);
		}

		public void AddMenuItem (string imageName, string title, Action action)
		{
			var mi = GetMethodInfo (action);
			var sel = GetUniqueSelector ();
			AddMenuItem (imageName, title, sel);
			Runtime.ConnectMethod (mi, sel);
		}

		public void AddMenuItem (WKMenuItemIcon itemIcon, string title, Action action)
		{
			var mi = GetMethodInfo (action);
			var sel = GetUniqueSelector ();
			AddMenuItem (itemIcon, title, sel);
			Runtime.ConnectMethod (mi, sel);
		}
	}
#endif

	public class WKPresentMediaPlayerResult {
#if !COREBUILD
		public WKPresentMediaPlayerResult (bool didPlayToEnd, double /* NSTimeInterval */ endTime)
		{
			DidPlayToEnd = didPlayToEnd;
			EndTime = endTime;
		}

		public bool DidPlayToEnd { get; set; }

		public double EndTime { get; set; }
#endif
	}
}

#endif // WATCH || IOS
