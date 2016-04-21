//
// UIApplication.cs: Extensions to UIApplication
//
// Authors:
//   Geoff Norton
//
// Copyright 2009, Novell, Inc.
// Copyrigh 2014, Xamarin Inc.
//

using System;
using System.Threading;
using XamCore.ObjCRuntime;
using System.Runtime.InteropServices;
using XamCore.Foundation;

namespace XamCore.UIKit {
	public class UIKitThreadAccessException : Exception {
		public UIKitThreadAccessException () : base ("UIKit Consistency error: you are calling a UIKit method that can only be invoked from the UI thread.")
		{
		}
	}
	
#if WATCH
	// There's no UIApplication on the watch, but we use the class extensively in bindings (EnsureUIThread, etc)
	// so we include it as an internal type.
	internal
#else
	public
#endif
	partial class UIApplication
#if !WATCH 
	: UIResponder
#endif
	{
		static Thread mainThread;
		public static bool CheckForIllegalCrossThreadCalls = true;
		public static bool CheckForEventAndDelegateMismatches = true;

#if !WATCH
		// We link with __Internal here so that this function is interposable from third-party native libraries.
		// See: https://github.com/xamarin/MicrosoftInTune/issues/3 for an example.
		[DllImport (/*Constants.UIKitLibrary*/ "__Internal")]
		extern static int UIApplicationMain (int argc, /* char[]* */ string [] argv, /* NSString* */ IntPtr principalClassName, /* NSString* */ IntPtr delegateClassName);
#endif

		// called from NSExtension.Initialize (so other, future stuff, can be added if needed)
		// NOTE: must be called from the main thread, e.g. for extensions
		internal static void Initialize ()
		{
			if (mainThread != null)
				return;
			
			SynchronizationContext.SetSynchronizationContext (new UIKitSynchronizationContext ());
			mainThread = Thread.CurrentThread;
		}
		
#if !WATCH
		public static void Main (string [] args, string principalClassName, string delegateClassName)
		{
			IntPtr principal = principalClassName != null ? new NSString (principalClassName).Handle : IntPtr.Zero;
			IntPtr delegatec = delegateClassName != null ? new NSString (delegateClassName).Handle : IntPtr.Zero;
			Main (args, principal, delegatec);
		}
		
		public static void Main (string [] args, Type principalClass, Type delegateClass)
		{
			Main (args, principalClass == null ? null : new Class (principalClass).Name, delegateClass == null ? null : new Class (delegateClass).Name);
		}

		public static void Main (string [] args)
		{
			Main (args, IntPtr.Zero, IntPtr.Zero);
		}

		static void Main (string [] args, IntPtr principal, IntPtr @delegate)
		{
			Initialize ();
			UIApplicationMain (args.Length, args, principal, @delegate);
		}
#endif

		public static void EnsureUIThread ()
		{
			// note: some extensions, like keyboards, won't call Main (and set mainThread)
			// FIXME: do better than disabling the feature
			if (CheckForIllegalCrossThreadCalls && (mainThread != null) && (mainThread != Thread.CurrentThread))
				throw new UIKitThreadAccessException ();
		}

		internal static void EnsureEventAndDelegateAreNotMismatched (object del, Type expectedType)
		{
			if (CheckForEventAndDelegateMismatches && !(expectedType.IsAssignableFrom (del.GetType ())))
				throw new InvalidOperationException (string.Format("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", del.GetType (), expectedType));
		}

		internal static void EnsureDelegateAssignIsNotOverwritingInternalDelegate (object currentDelegateValue, object newDelegateValue, Type internalDelegateType)
		{
			if (UIApplication.CheckForEventAndDelegateMismatches && currentDelegateValue != null && newDelegateValue != null
				&& currentDelegateValue.GetType().IsAssignableFrom (internalDelegateType)
				&& !newDelegateValue.GetType().IsAssignableFrom (internalDelegateType))
				throw new InvalidOperationException (string.Format("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", newDelegateValue.GetType(), internalDelegateType));
		}
	}

#if !WATCH
	internal partial class _UIContentSizeCategory {
		public static UIContentSizeCategory ToEnum (NSString key)
		{
			if (key == null)
				return UIContentSizeCategory.Medium;
			
			var kh = key.Handle;
			
			if (kh == ExtraSmall.Handle)
				return UIContentSizeCategory.ExtraSmall;
			if (kh == Small.Handle)
				return UIContentSizeCategory.Small;
			if (kh == Large.Handle)
				return UIContentSizeCategory.Large;
			if (kh == ExtraLarge.Handle)
				return UIContentSizeCategory.ExtraLarge;
			if (kh == ExtraExtraLarge.Handle)
				return UIContentSizeCategory.ExtraExtraLarge;
			if (kh == ExtraExtraExtraLarge.Handle)
				return UIContentSizeCategory.ExtraExtraExtraLarge;

			// Do not compare to Medium, it is the fallback
			return UIContentSizeCategory.Medium;
		}
	}
	
	public partial class UIContentSizeCategoryChangedEventArgs {
		public UIContentSizeCategory NewValue {
			get {
				return _UIContentSizeCategory.ToEnum (WeakNewValue);
			}
		}
	}
#endif
}
