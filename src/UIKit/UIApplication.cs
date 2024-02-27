//
// UIApplication.cs: Extensions to UIApplication
//
// Authors:
//   Geoff Norton
//
// Copyright 2009, Novell, Inc.
// Copyright 2014, Xamarin Inc.
// Copyright 2019 Microsoft Corporation.
//

using System;
using System.ComponentModel;
using System.Threading;
using ObjCRuntime;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace UIKit {
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
		static Thread? mainThread;
		public static bool CheckForIllegalCrossThreadCalls = true;
		public static bool CheckForEventAndDelegateMismatches = true;

#if !WATCH
		// We link with __Internal here so that this function is interposable from third-party native libraries.
		// See: https://github.com/xamarin/MicrosoftInTune/issues/3 for an example.
		[DllImport ("__Internal")]
		unsafe extern static int xamarin_UIApplicationMain (int argc, /* char[]* */ IntPtr argv, /* NSString* */ IntPtr principalClassName, /* NSString* */ IntPtr delegateClassName, IntPtr* gchandle);

		static int UIApplicationMain (int argc, /* char[]* */ string []? argv, /* NSString* */ IntPtr principalClassName, /* NSString* */ IntPtr delegateClassName)
		{
			var strArr = TransientString.AllocStringArray (argv);
			IntPtr gchandle;
			int rv;
			unsafe {
				rv = xamarin_UIApplicationMain (argc, strArr, principalClassName, delegateClassName, &gchandle);
			}
			TransientString.FreeStringArray (strArr, argv?.Length ?? 0);
			Runtime.ThrowException (gchandle);
			return rv;
		}
#endif

		// called from NSExtension.Initialize (so other, future stuff, can be added if needed)
		// NOTE: must be called from the main thread, e.g. for extensions
		internal static void Initialize ()
		{
			if (mainThread is not null)
				return;

			SynchronizationContext.SetSynchronizationContext (new UIKitSynchronizationContext ());
			mainThread = Thread.CurrentThread;
		}

#if !WATCH
		[Obsolete ("Use the overload with 'Type' instead of 'String' parameters for type safety.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static void Main (string []? args, string? principalClassName, string? delegateClassName)
		{
			using var p = new TransientCFString (principalClassName);
			using var d = new TransientCFString (delegateClassName);
			Initialize ();
			UIApplicationMain (args?.Length ?? 0, args, p, d);
		}

		public static void Main (string []? args, Type? principalClass, Type? delegateClass)
		{
			using var p = new TransientCFString (principalClass is null ? null : new Class (principalClass).Name);
			using var d = new TransientCFString (delegateClass is null ? null : new Class (delegateClass).Name);
			Initialize ();
			UIApplicationMain (args?.Length ?? 0, args, p, d);
		}

		public static void Main (string []? args)
		{
			Initialize ();
			UIApplicationMain (args?.Length ?? 0, args, IntPtr.Zero, IntPtr.Zero);
		}
#endif

		public static void EnsureUIThread ()
		{
			// note: some extensions, like keyboards, won't call Main (and set mainThread)
			// FIXME: do better than disabling the feature
			if (CheckForIllegalCrossThreadCalls && (mainThread is not null) && (mainThread != Thread.CurrentThread))
				throw new UIKitThreadAccessException ();
		}

		internal static void EnsureEventAndDelegateAreNotMismatched (object del, Type expectedType)
		{
			if (CheckForEventAndDelegateMismatches && !(expectedType.IsAssignableFrom (del.GetType ())))
				throw new InvalidOperationException (string.Format ("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", del.GetType (), expectedType));
		}

		internal static void EnsureDelegateAssignIsNotOverwritingInternalDelegate (object? currentDelegateValue, object? newDelegateValue, Type internalDelegateType)
		{
			if (UIApplication.CheckForEventAndDelegateMismatches && currentDelegateValue is not null && newDelegateValue is not null
				&& currentDelegateValue.GetType ().IsAssignableFrom (internalDelegateType)
				&& !newDelegateValue.GetType ().IsAssignableFrom (internalDelegateType))
				throw new InvalidOperationException (string.Format ("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", newDelegateValue.GetType (), internalDelegateType));
		}
	}

#if !WATCH
	public partial class UIContentSizeCategoryChangedEventArgs {
		public UIContentSizeCategory NewValue {
			get {
				return UIContentSizeCategoryExtensions.GetValue (WeakNewValue);
			}
		}
	}
#endif
}
