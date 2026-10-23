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

using System.ComponentModel;
using System.Threading;
using CoreFoundation;

#nullable enable

namespace UIKit {
	/// <include file="../../docs/api/UIKit/UIKitThreadAccessException.xml" path="/Documentation/Docs[@DocId='T:UIKit.UIKitThreadAccessException']/*" />
	public class UIKitThreadAccessException : Exception {
		/// <summary>Initializes a new instance of the <see cref="UIKitThreadAccessException" /> class.</summary>
		public UIKitThreadAccessException () : base ("UIKit Consistency error: you are calling a UIKit method that can only be invoked from the UI thread.")
		{
		}
	}

	public partial class UIApplication
	: UIResponder {
		/// <summary>Controls whether calls to UIKit APIs are checked to ensure that they are made on the UI thread.</summary>
		/// <remarks>
		///   <para>Accessing UIKit APIs from a background thread can corrupt application state. These checks help identify such accesses.</para>
		///   <para>The initial value is controlled by the <c>CheckForIllegalCrossThreadCalls</c> build property, which defaults to <see langword="true" /> in debug builds and <see langword="false" /> in release builds.</para>
		/// </remarks>
		public static bool CheckForIllegalCrossThreadCalls;
		/// <summary>If <see langword="true" />, the system will try to diagnose potential mistakes where events and delegate-object overrides are in conflict.</summary>
		public static bool CheckForEventAndDelegateMismatches = true;

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

		// called from NSExtension.Initialize (so other, future stuff, can be added if needed)
		// NOTE: must be called from the main thread, e.g. for extensions
		internal static void InitializeApplication ()
		{
			// The linker replaces the 'Runtime.CheckForIllegalCrossThreadCalls' getter with a constant value, so when the UI
			// thread checks are disabled the assignment below (and the 'CheckForIllegalCrossThreadCalls' field
			// itself, unless something else references it) is trimmed away.
			if (Runtime.CheckForIllegalCrossThreadCalls)
				CheckForIllegalCrossThreadCalls = true;

			SynchronizationContext.SetSynchronizationContext (new UIKitSynchronizationContext ());
		}

		/// <include file="../../docs/api/UIKit/UIApplication.xml" path="/Documentation/Docs[@DocId='M:UIKit.UIApplication.Main(System.String[],System.String,System.String)']/*" />
		[Obsolete ("Use the overload with 'Type' instead of 'String' parameters for type safety.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#if XAMCORE_5_0
		public static int Main (string []? args, string? principalClassName, string? delegateClassName)
#else
		public static void Main (string []? args, string? principalClassName, string? delegateClassName)
#endif
		{
			using var p = new TransientCFString (principalClassName);
			using var d = new TransientCFString (delegateClassName);
			InitializeApplication ();
#if XAMCORE_5_0
			return UIApplicationMain (args?.Length ?? 0, args, p, d);
#else
			UIApplicationMain (args?.Length ?? 0, args, p, d);
#endif
		}

		/// <include file="../../docs/api/UIKit/UIApplication.xml" path="/Documentation/Docs[@DocId='M:UIKit.UIApplication.Main(System.String[],System.Type,System.Type)']/*" />
#if XAMCORE_5_0
		public static int Main (string []? args, Type? principalClass, Type? delegateClass)
#else
		public static void Main (string []? args, Type? principalClass, Type? delegateClass)
#endif
		{
			using var p = new TransientCFString (principalClass is null ? null : new Class (principalClass).Name);
			using var d = new TransientCFString (delegateClass is null ? null : new Class (delegateClass).Name);
			InitializeApplication ();
#if XAMCORE_5_0
			return UIApplicationMain (args?.Length ?? 0, args, p, d);
#else
			UIApplicationMain (args?.Length ?? 0, args, p, d);
#endif
		}

		/// <param name="args">Command line parameters from the Main program.</param>
		///         <summary>Launches the main application loop with the given command line parameters.</summary>
		///         <remarks>This launches the main application loop, assumes that the main application class is UIApplication, and uses the UIApplicationDelegate instance specified in the main NIB file for this program.</remarks>
#if XAMCORE_5_0
		public static int Main (string []? args)
#else
		public static void Main (string []? args)
#endif
		{
			InitializeApplication ();
#if XAMCORE_5_0
			return UIApplicationMain (args?.Length ?? 0, args, IntPtr.Zero, IntPtr.Zero);
#else
			UIApplicationMain (args?.Length ?? 0, args, IntPtr.Zero, IntPtr.Zero);
#endif
		}

		/// <summary>Checks that the current call is made on the UI thread.</summary>
		/// <exception cref="UIKitThreadAccessException">Thread checks are enabled and the current thread is not the UI thread.</exception>
		/// <remarks>This method does nothing when <see cref="CheckForIllegalCrossThreadCalls" /> is <see langword="false" />. Thread checks are enabled by default in debug builds and disabled by default in release builds.</remarks>
		public static void EnsureUIThread ()
		{
			Runtime.EnsureUIThread ();
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

	/// <summary>Provides data for the <see cref="UIContentSizeCategory" /> changed event.</summary>
	public partial class UIContentSizeCategoryChangedEventArgs {
		/// <summary>The new size of the content, e.g., the new font size, in points.</summary>
		public UIContentSizeCategory NewValue {
			get {
				return UIContentSizeCategoryExtensions.GetValue (WeakNewValue);
			}
		}
	}
}
