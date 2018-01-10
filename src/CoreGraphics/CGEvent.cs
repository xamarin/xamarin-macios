/*
 * CGEvent.cs: bindings to the ApplicationServices framework's CoreGraphics CGEvent APIs
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
	public sealed class CGEvent : IDisposable, INativeObject {
		public delegate IntPtr CGEventTapCallback (IntPtr tapProxyEvent, CGEventType eventType, IntPtr eventRef, IntPtr userInfo);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
#if !XAMCORE_2_0 // Accidently exposed this to public, can't break API
		public
#endif
		extern static IntPtr CGEventTapCreate (CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data);

		public static CFMachPort CreateTap (CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data)
		{
			var r = CGEventTapCreate (location, place, options, mask, cback, data);
			if (r == IntPtr.Zero)
				return null;
			return new CFMachPort (r);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventTapCreateForPSN (IntPtr processSerialNumer, CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data);
		
		public static CFMachPort CreateTap (IntPtr processSerialNumber, CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data)
		{
			var r = CGEventTapCreateForPSN (processSerialNumber, location, place, options, mask, cback, data);
			if (r == IntPtr.Zero)
				return null;
			return new CFMachPort (r);
		}
		
		IntPtr handle;

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateFromData (IntPtr allocator, IntPtr nsdataSource);

		public CGEvent (NSData source) 
		{
			if (source == null)
				throw new ArgumentNullException ("source");

			handle = CGEventCreateFromData (IntPtr.Zero, source.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreate (IntPtr eventSourceHandle);
		
		public CGEvent (CGEventSource eventSource)
		{
			handle = CGEventCreate (eventSource == null ? IntPtr.Zero : eventSource.Handle);
		}

		public CGEvent (IntPtr handle) : this (handle, false)
		{
		}

		internal CGEvent (IntPtr handle, bool ownsHandle)
		{
			if (!ownsHandle)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateMouseEvent(IntPtr source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton);
			
		public CGEvent (CGEventSource source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton)
		{
			handle = CGEventCreateMouseEvent (source == null ? IntPtr.Zero : source.Handle, mouseType, mouseCursorPosition, mouseButton);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateKeyboardEvent (IntPtr source, ushort virtualKey, bool keyDown);

		public CGEvent (CGEventSource source, ushort virtualKey, bool keyDown)
		{
			handle = CGEventCreateKeyboardEvent (source == null ? IntPtr.Zero : source.Handle, virtualKey, keyDown);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateScrollWheelEvent (IntPtr source, CGScrollEventUnit units, uint /* uint32_t */ wheelCount, int /* uint32_t */ wheel1);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateScrollWheelEvent (IntPtr source, CGScrollEventUnit units, uint /* uint32_t */ wheelCount, int /* uint32_t */ wheel1, int /* uint32_t */ wheel2);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateScrollWheelEvent (IntPtr source, CGScrollEventUnit units, uint /* uint32_t */ wheelCount, int /* uint32_t */ wheel1, int /* uint32_t */ wheel2, int /* uint32_t */ wheel3);

		public CGEvent (CGEventSource source, CGScrollEventUnit units, params int []  wheel)
		{
			IntPtr shandle = source == null ? IntPtr.Zero : source.Handle;
			
			switch (wheel.Length){
			case 0:
				throw new ArgumentException ("At least one wheel must be provided");
			case 1:
				handle = CGEventCreateScrollWheelEvent (shandle, units, 1, wheel [0]);
				break;
			case 2:
				handle = CGEventCreateScrollWheelEvent (shandle, units, 2, wheel [0], wheel [1]);
				break;
			case 3:
				handle = CGEventCreateScrollWheelEvent (shandle, units, 3, wheel [0], wheel [1], wheel [2]);
				break;
			default:
				throw new ArgumentException ("Only one to three wheels are supported on this constructor");
			}
		}
	
		~CGEvent ()
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

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateCopy (IntPtr handle);

		public CGEvent Copy ()
		{
			return new CGEvent (CGEventCreateCopy (handle), true);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateData (IntPtr allocator, IntPtr handle);

		public NSData ToData ()
		{
			return new NSData (CGEventCreateData (IntPtr.Zero, handle));
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateSourceFromEvent (IntPtr evthandle);

		public CGEventSource CreateEventSource ()
		{
			var esh = CGEventCreateSourceFromEvent (handle);
			if (esh == IntPtr.Zero)
				return null;
			return new CGEventSource (esh, true);
		}
       
		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGPoint CGEventGetLocation (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSetLocation (IntPtr handle, CGPoint location);

		public CGPoint Location {
			get {
				return CGEventGetLocation (handle);
			}
			set {
				CGEventSetLocation (handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGPoint CGEventGetUnflippedLocation (IntPtr handle);

		public CGPoint UnflippedLocation {
			get {
				return CGEventGetUnflippedLocation (handle);
			}
		}

		// Keep this public, as we want to avoid creating instances of the object
		// just to peek at the flags
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventGetFlags")]
		public extern static CGEventFlags GetFlags (IntPtr eventHandle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		internal extern static void CGEventSetFlags (IntPtr eventHandle, CGEventFlags flags);

		public CGEventFlags Flags {
			get {
				return GetFlags (handle);
			}
			set {
				CGEventSetFlags (handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventGetIntegerValueField")]
		internal extern static long GetLong (IntPtr eventHandle, CGEventField eventField);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint="CGEventGetDoubleValueField")]
		internal extern static double GetDouble (IntPtr eventHandle, CGEventField eventField);

		internal long GetLong (CGEventField eventField)
		{
			return GetLong (handle, (CGEventField) eventField);
		}

		public long MouseEventNumber {
			get {
				return GetLong (handle, CGEventField.MouseEventNumber);
			}
		}

		public long MouseEventClickState {
			get {
				return GetLong (handle, CGEventField.MouseEventClickState);
			}
		}

		public double MouseEventPressure {
			get {
				return GetDouble (handle, CGEventField.MouseEventPressure);
			}
		}

		public long MouseEventButtonNumber {
			get {
				return GetLong (handle, CGEventField.MouseEventButtonNumber);
			}
		}

		public long MouseEventDeltaX {
			get {
				return GetLong (handle, CGEventField.MouseEventDeltaX);
			}
		}

		public long MouseEventDeltaY {
			get {
				return GetLong (handle, CGEventField.MouseEventDeltaY);
			}
		}

		public bool MouseEventInstantMouser {
			get {
				return GetLong (handle, CGEventField.MouseEventButtonNumber) != 0;
			}
		}

		public long MouseEventSubtype {
			get {
				return GetLong (handle, CGEventField.MouseEventSubtype);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventSetSource (IntPtr handle, IntPtr source);
       
		public void SetEventSource (CGEventSource eventSource)
		{
			if (eventSource == null)
				throw new ArgumentNullException ("eventSource");
			CGEventSetSource (handle, eventSource.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGEventType  CGEventGetType (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSetType (IntPtr handle, CGEventType evtType);
		
		public CGEventType EventType {
			get {
				return CGEventGetType (handle);
			}
			set {
				CGEventSetType (handle, value);
			}
		}


		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static ulong CGEventGetTimestamp (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSetTimestamp (IntPtr handle, ulong timeStampp);
		
#if !XAMCORE_4_0
		[Obsolete ("Use 'Timestamp' instead.")]
		public ulong Timestampe {
			get {
				return CGEventGetTimestamp (handle);
			}
			set {
				CGEventSetTimestamp (handle, value);
			}
		}
#endif

		public ulong Timestamp {
			get {
				return CGEventGetTimestamp (handle);
			}
			set {
				CGEventSetTimestamp (handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventTapEnable (IntPtr machPort, bool enable);

		public static void TapEnable (CFMachPort machPort)
		{
			if (machPort == null)
				throw new ArgumentNullException ("machPort");
			CGEventTapEnable (machPort.Handle, true);
		}

		public static void TapDisable (CFMachPort machPort)
		{
			if (machPort == null)
				throw new ArgumentNullException ("machPort");
			CGEventTapEnable (machPort.Handle, false);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static bool CGEventTapIsEnabled (IntPtr machPort);

		public static bool IsTapEnabled (CFMachPort machPort)
		{
			if (machPort == null)
				throw new ArgumentNullException ("machPort");
			return CGEventTapIsEnabled (machPort.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static void CGEventKeyboardGetUnicodeString (IntPtr handle, nuint maxLen, out nuint actualLen, char *buffer);

		public unsafe string GetUnicodeString ()
		{
			char *buffer = stackalloc char [40];
		        nuint actual;
			
			CGEventKeyboardGetUnicodeString (handle, 40, out actual, buffer);
			return new String (buffer, 0, (int) actual);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static void CGEventKeyboardSetUnicodeString (IntPtr handle, nuint len,  [MarshalAs(UnmanagedType.LPWStr)] string buffer);

		public unsafe void SetUnicodeString (string value)
		{
			CGEventKeyboardSetUnicodeString (handle, (nuint) value.Length, value);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventTapPostEvent (IntPtr proxy, IntPtr evtHandle);

		public static void TapPostEven (IntPtr tapProxyEvent, CGEvent evt)
		{
			if (evt == null)
				throw new ArgumentNullException ("evt");
			
			CGEventTapPostEvent (tapProxyEvent, evt.Handle);
		}
		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventPost (CGEventTapLocation location, IntPtr handle);

		public static void Post (CGEvent evt, CGEventTapLocation location)
		{
			if (evt == null)
				throw new ArgumentNullException ("evt");
			
			CGEventPost (location, evt.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventPostToPSN (IntPtr processSerialNumber, IntPtr handle);

		public static void PostToPSN (CGEvent evt, IntPtr processSerialNumber)
		{
			if (evt == null)
				throw new ArgumentNullException ("evt");
			
			CGEventPostToPSN (processSerialNumber, evt.Handle);
		}

		
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static int /* CGError = int32_t */ CGGetEventTapList (
			uint /* uint32_t */ maxNumberOfTaps,
			CGEventTapInformation *tapList,
			out uint /* uint32_t* */ eventTapCount);

		public unsafe CGEventTapInformation [] GetEventTapList ()
		{
			uint count;
			if (CGGetEventTapList (0, null, out count) != 0)
				return null;
			var result = new CGEventTapInformation [count];
			fixed (CGEventTapInformation *p = &result [0]){
				if (CGGetEventTapList (count, p, out count) != 0)
					return null;
			}
			return result;
		}

	}

	public struct CGEventTapInformation {
		public uint /* uint32_t */ EventTapID;
		public CGEventTapLocation TapPoint;
		public CGEventTapOptions Options;
		public CGEventMask EventsOfInterest;
		public int /* pid_t = int */ TappingProcess;
		public int /* pid_t = int */ ProcessBeingTapped;
		public bool /* bool */ Enabled;
		public float /* float */ MinUsecLatency;
		public float /* float */ AvgUsecLatency;
		public float /* float */ MaxUsecLatency;
	};

}

#endif // MONOMAC
