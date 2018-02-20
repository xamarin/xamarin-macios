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
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Foundation;
using ObjCRuntime;

namespace AppKit {
	public partial class NSApplication : NSResponder {
		public static bool CheckForIllegalCrossThreadCalls = true;
		public static bool CheckForEventAndDelegateMismatches = true;
		public static bool IgnoreMissingAssembliesDuringRegistration = false;

		private static Thread mainThread;

		static NSApplication () {
			class_ptr = Class.GetHandle ("NSApplication");
		}

		[DllImport (Constants.AppKitLibrary)]
		extern static int /* int */ NSApplicationMain (int /* int */ argc, string [] argv);

		static bool initialized;

		[Preserve]
		public static void Init ()
		{
			if (initialized) {
				throw new InvalidOperationException ("Init has already be be invoked; it can only be invoke once");
			}

			Runtime.EnsureInitialized ();

			initialized = true;

			Runtime.RegisterAssemblies ();

			// Runtime hosts embedding MonoMac may use a different sync context 
			// and call NSApplicationMain externally prior to this Init, so only
			// initialize the context if it hasn't been set externally. Alternatively,
			// AppKitSynchronizationContext could be made public.
			if(SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext (new AppKitSynchronizationContext ());

			// Establish the main thread at the time of Init to support hosts
			// that don't call Main.
			NSApplication.mainThread = Thread.CurrentThread;

			// Launcher sets this to work around https://bugzilla.xamarin.com/show_bug.cgi?id=45279
			// But can affect child xbuild processes, so unset
			Environment.SetEnvironmentVariable ("MONO_CFG_DIR", "");

			// TODO:
			//   Install hook to register dynamically loaded assemblies
		}

		public static void InitDrawingBridge ()
		{
			FieldInfo UseCocoaDrawableField = Type.GetType ("System.Drawing.GDIPlus, System.Drawing").GetField ("UseCocoaDrawable", BindingFlags.Static | BindingFlags.Public);
			FieldInfo UseCarbonDrawableField = Type.GetType ("System.Drawing.GDIPlus, System.Drawing").GetField ("UseCarbonDrawable", BindingFlags.Static | BindingFlags.Public);

			UseCocoaDrawableField.SetValue (null, true);
			UseCarbonDrawableField.SetValue (null, false);
		}

		public static void Main (string [] args)
		{
			// Switch to an AppKitSynchronizationContext if Main is invoked
			if(SynchronizationContext.Current == null || !typeof(AppKitSynchronizationContext).IsAssignableFrom(SynchronizationContext.Current.GetType()))
				SynchronizationContext.SetSynchronizationContext (new AppKitSynchronizationContext ());

			// Init where this is set the first time is generally paired
			// with a call to Main, but this guarantees the right thread.
			NSApplication.mainThread = Thread.CurrentThread;

			NSApplicationMain (args.Length, args);
		}

		public static void EnsureUIThread()
		{
			if (NSApplication.CheckForIllegalCrossThreadCalls && NSApplication.mainThread != Thread.CurrentThread)
				throw new AppKitThreadAccessException();
		}

		public static void EnsureEventAndDelegateAreNotMismatched (object del, Type expectedType)
		{
			if (NSApplication.CheckForEventAndDelegateMismatches && !(expectedType.IsAssignableFrom (del.GetType ())))
				throw new InvalidOperationException (string.Format("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", del.GetType (), expectedType));
		}

		public static void EnsureDelegateAssignIsNotOverwritingInternalDelegate (object currentDelegateValue, object newDelegateValue, Type internalDelegateType)
		{
			if (NSApplication.CheckForEventAndDelegateMismatches && currentDelegateValue != null && newDelegateValue != null
				&& currentDelegateValue.GetType().IsAssignableFrom (internalDelegateType)
				&& !newDelegateValue.GetType().IsAssignableFrom (internalDelegateType))
				throw new InvalidOperationException (string.Format("Event registration is overwriting existing delegate. Either just use events or your own delegate: {0} {1}", newDelegateValue.GetType(), internalDelegateType));
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the 'NextEvent (nuint, NSDate, [NSRunLoopMode|NSString], bool)' overloads instead.")]
		public NSEvent NextEvent (NSEventMask mask, NSDate expiration, string mode, bool deqFlag)
		{
			// NSEventMask must be casted to nuint to preserve the NSEventMask.Any special value
			// on 64 bit systems.
			return NextEvent ((nuint)(ulong) mask, expiration, mode, deqFlag);
		}
#endif

		public void DiscardEvents (NSEventMask mask, NSEvent lastEvent)
		{
			DiscardEvents ((nuint)(ulong) mask, lastEvent);
		}

		[Obsolete ("This method does nothing.")]
		public static void RestoreWindow (string identifier, Foundation.NSCoder state, NSWindowCompletionHandler onCompletion)
		{
		}

		// note: if needed override the protected Get|Set methods
		public NSApplicationActivationPolicy ActivationPolicy { 
			get { return GetActivationPolicy (); }
			// ignore return value (bool)
			set { SetActivationPolicy (value); }
		}
	}
}
