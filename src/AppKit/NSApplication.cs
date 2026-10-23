//
// Copyright 2010, Novell, Inc.
// Copyright 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !__MACCATALYST__

using System.ComponentModel;
using System.Reflection;
using System.Threading;

#nullable enable

namespace AppKit {
	public partial class NSApplication : NSResponder {
		/// <summary>Controls whether calls to AppKit APIs are checked to ensure that they are made on the UI thread.</summary>
		/// <remarks>
		///   <para>Accessing AppKit APIs from a background thread can corrupt application state. These checks help identify such accesses.</para>
		///   <para>The initial value is controlled by the <c>CheckForIllegalCrossThreadCalls</c> build property, which defaults to <see langword="true" /> in debug builds and <see langword="false" /> in release builds.</para>
		/// </remarks>
		public static bool CheckForIllegalCrossThreadCalls;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public static bool CheckForEventAndDelegateMismatches = true;

		[DllImport (Constants.AppKitLibrary)]
		extern static int /* int */ NSApplicationMain (int /* int */ argc, IntPtr argv);

#if !XAMCORE_5_0
		/// <summary>This method does nothing.</summary>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static void Init ()
		{
			// No need anymore.
		}
#endif // !XAMCORE_5_0

		internal static void InitializeApplication ()
		{
			// The linker replaces the 'Runtime.CheckForIllegalCrossThreadCalls' getter with a constant value, so when the UI
			// thread checks are disabled the assignment below (and the 'CheckForIllegalCrossThreadCalls' field
			// itself, unless something else references it) is trimmed away.
			if (Runtime.CheckForIllegalCrossThreadCalls)
				CheckForIllegalCrossThreadCalls = true;

			SynchronizationContext.SetSynchronizationContext (new AppKitSynchronizationContext ());
		}

#if !XAMCORE_5_0
		/// <summary>This method does nothing.</summary>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static void InitDrawingBridge ()
		{
		}
#endif // !XAMCORE_5_0

		/// <param name="args">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
#if XAMCORE_5_0
		public static int Main (string [] args)
#else
		public static void Main (string [] args)
#endif
		{
			InitializeApplication ();

			var argsPtr = TransientString.AllocStringArray (args);
			try {
#if XAMCORE_5_0
				return NSApplicationMain (args.Length, argsPtr);
#else
				NSApplicationMain (args.Length, argsPtr);
#endif
			} finally {
				TransientString.FreeStringArray (argsPtr, args.Length);
			}
		}

		/// <summary>Checks that the current call is made on the UI thread.</summary>
		/// <exception cref="AppKitThreadAccessException">Thread checks are enabled and the current thread is not the UI thread.</exception>
		/// <remarks>This method does nothing when <see cref="CheckForIllegalCrossThreadCalls" /> is <see langword="false" />. Thread checks are enabled by default in debug builds and disabled by default in release builds.</remarks>
		public static void EnsureUIThread ()
		{
			Runtime.EnsureUIThread ();
		}

		/// <param name="del">To be added.</param>
		///         <param name="expectedType">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public static void EnsureEventAndDelegateAreNotMismatched (object del, Type expectedType)
		{
			if (NSApplication.CheckForEventAndDelegateMismatches && !(expectedType.IsAssignableFrom (del.GetType ())))
				throw new InvalidOperationException (string.Format ("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", del.GetType (), expectedType));
		}

		/// <param name="currentDelegateValue">To be added.</param>
		///         <param name="newDelegateValue">To be added.</param>
		///         <param name="internalDelegateType">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public static void EnsureDelegateAssignIsNotOverwritingInternalDelegate (object? currentDelegateValue, object? newDelegateValue, Type internalDelegateType)
		{
			if (NSApplication.CheckForEventAndDelegateMismatches && currentDelegateValue is not null && newDelegateValue is not null
				&& currentDelegateValue.GetType ().IsAssignableFrom (internalDelegateType)
				&& !newDelegateValue.GetType ().IsAssignableFrom (internalDelegateType))
				throw new InvalidOperationException (string.Format ("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", newDelegateValue.GetType (), internalDelegateType));
		}

		/// <param name="mask">To be added.</param>
		///         <param name="lastEvent">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void DiscardEvents (NSEventMask mask, NSEvent lastEvent)
		{
			DiscardEvents ((nuint) (ulong) mask, lastEvent);
		}

		// note: if needed override the protected Get|Set methods
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSApplicationActivationPolicy ActivationPolicy {
			get { return GetActivationPolicy (); }
			// ignore return value (bool)
			set { SetActivationPolicy (value); }
		}
	}
}
#endif // !__MACCATALYST__
