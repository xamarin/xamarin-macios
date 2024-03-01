/*
 * CGEventSource.cs: bindings to the ApplicationServices framework's CoreGraphics CGEventSource API
 * 
 * Copyright 2013, 2014 Xamarin Inc
 * All Rights Reserved
 * 
 * Authors:
 *    Miguel de Icaza
 */

#nullable enable

#if MONOMAC || __MACCATALYST__

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	public sealed class CGEventSource : NativeObject {
#if !NET
		public CGEventSource (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
#if NET
		internal CGEventSource (NativeHandle handle, bool owns)
#else
		public CGEventSource (NativeHandle handle, bool owns)
#endif
			: base (handle, owns)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventSourceCreate (CGEventSourceStateID stateID);
	
		public CGEventSource (CGEventSourceStateID stateID)
			: base (CGEventSourceCreate (stateID), true)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static int /* CGEventSourceKeyboardType = uint32_t */ CGEventSourceGetKeyboardType (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetKeyboardType (IntPtr handle, int /* CGEventSourceKeyboardType = uint32_t */ keyboardType);

		public int KeyboardType  {
			get {
				return CGEventSourceGetKeyboardType (Handle);
			}
			set {
				CGEventSourceSetKeyboardType (Handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGEventSourceStateID CGEventSourceGetSourceStateID (IntPtr handle);
		
		public CGEventSourceStateID StateID {
			get {
				return CGEventSourceGetSourceStateID (Handle);
			}
		}


		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static double CGEventSourceGetPixelsPerLine (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetPixelsPerLine (IntPtr handle, double value);
		
		public double PixelsPerLine {
			get {
				return CGEventSourceGetPixelsPerLine (Handle);
			}
			set {
				CGEventSourceSetPixelsPerLine (Handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceButtonState")]
		[return: MarshalAs (UnmanagedType.I1)]
		public extern static bool GetButtonState (CGEventSourceStateID stateID, CGMouseButton button);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceKeyState")]
		[return: MarshalAs (UnmanagedType.I1)]
		public extern static bool GetKeyState (CGEventSourceStateID stateID, ushort keycode);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceFlagsState")]
		public extern static  CGEventFlags GetFlagsState (CGEventSourceStateID stateID);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceSecondsSinceLastEventType")]
		public extern static double GetSecondsSinceLastEventType (CGEventSourceStateID stateID, CGEventType eventType);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceCounterForEventType")]
		public extern static uint /* uint32_t */ GetCounterForEventType (CGEventSourceStateID stateID, CGEventType eventType);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetUserData (IntPtr handle, long data);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static long CGEventSourceGetUserData (IntPtr handle);

		public long UserData {
			get {
				return CGEventSourceGetUserData (Handle);
			}
			set {
				CGEventSourceSetUserData (Handle, value);
			}
		}


		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetLocalEventsFilterDuringSuppressionState (IntPtr handle, CGEventFilterMask filter, CGEventSuppressionState state);

		public void SetLocalEventsFilterDuringSupressionState (CGEventFilterMask filter, CGEventSuppressionState state)
		{
			CGEventSourceSetLocalEventsFilterDuringSuppressionState (Handle, filter, state);
		}
		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGEventFilterMask CGEventSourceGetLocalEventsFilterDuringSuppressionState (IntPtr handle, CGEventSuppressionState state);

		public CGEventFilterMask GetLocalEventsFilterDuringSupressionState (CGEventSuppressionState state)
		{
			return CGEventSourceGetLocalEventsFilterDuringSuppressionState (Handle, state);
		}
		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetLocalEventsSuppressionInterval (IntPtr handle, double seconds);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static double CGEventSourceGetLocalEventsSuppressionInterval (IntPtr handle);

		public double LocalEventsSupressionInterval {
			get {
				return CGEventSourceGetLocalEventsSuppressionInterval (Handle);
			}
			set {
				CGEventSourceSetLocalEventsSuppressionInterval (Handle, value);
			}
		}
		
	}
}

#endif // MONOMAC
