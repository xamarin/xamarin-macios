/*
 * CGEventSource.cs: bindings to the ApplicationServices framework's CoreGraphics CGEventSource API
 * 
 * Copyright 2013, 2014 Xamarin Inc
 * All Rights Reserved
 * 
 * Authors:
 *    Miguel de Icaza
 */

#if MONOMAC

using System;
using System.Runtime.InteropServices;
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreGraphics {
	public sealed class CGEventSource : IDisposable, INativeObject {
		IntPtr handle;

#region Lifecycle
		public CGEventSource (IntPtr handle) : this (handle, false)
		{
		}

		public CGEventSource (IntPtr handle, bool ownsHandle)
		{
			if (!ownsHandle)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		~CGEventSource ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}
#endregion
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventSourceCreate (CGEventSourceStateID stateID);
	
		public CGEventSource (CGEventSourceStateID stateID)
		{
			handle = CGEventSourceCreate (stateID);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static int /* CGEventSourceKeyboardType = uint32_t */ CGEventSourceGetKeyboardType (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetKeyboardType (IntPtr handle, int /* CGEventSourceKeyboardType = uint32_t */ keyboardType);

		public int KeyboardType  {
			get {
				return CGEventSourceGetKeyboardType (handle);
			}
			set {
				CGEventSourceSetKeyboardType (handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGEventSourceStateID CGEventSourceGetSourceStateID (IntPtr handle);
		
		public CGEventSourceStateID StateID {
			get {
				return CGEventSourceGetSourceStateID (handle);
			}
		}


		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static double CGEventSourceGetPixelsPerLine (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetPixelsPerLine (IntPtr handle, double value);
		
		public double PixelsPerLine {
			get {
				return CGEventSourceGetPixelsPerLine (handle);
			}
			set {
				CGEventSourceSetPixelsPerLine (handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceButtonState")]
		public extern static bool GetButtonState (CGEventSourceStateID stateID, CGMouseButton button);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventSourceKeyState")]
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
				return CGEventSourceGetUserData (handle);
			}
			set {
				CGEventSourceSetUserData (handle, value);
			}
		}


		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetLocalEventsFilterDuringSuppressionState (IntPtr handle, CGEventFilterMask filter, CGEventSuppressionState state);

		public void SetLocalEventsFilterDuringSupressionState (CGEventFilterMask filter, CGEventSuppressionState state)
		{
			CGEventSourceSetLocalEventsFilterDuringSuppressionState (handle, filter, state);
		}
		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGEventFilterMask CGEventSourceGetLocalEventsFilterDuringSuppressionState (IntPtr handle, CGEventSuppressionState state);

		public CGEventFilterMask GetLocalEventsFilterDuringSupressionState (CGEventSuppressionState state)
		{
			return CGEventSourceGetLocalEventsFilterDuringSuppressionState (handle, state);
		}
		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSourceSetLocalEventsSuppressionInterval (IntPtr handle, double seconds);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static double CGEventSourceGetLocalEventsSuppressionInterval (IntPtr handle);

		public double LocalEventsSupressionInterval {
			get {
				return CGEventSourceGetLocalEventsSuppressionInterval (handle);
			}
			set {
				CGEventSourceSetLocalEventsSuppressionInterval (handle, value);
			}
		}
		
	}
}

#endif // MONOMAC
