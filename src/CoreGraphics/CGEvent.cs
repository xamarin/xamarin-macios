/*
 * CGEvent.cs: bindings to the ApplicationServices framework's CoreGraphics CGEvent APIs
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
using System.Runtime.CompilerServices;
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
	[MacCatalyst (13,1)]
#endif
	public sealed class CGEvent : NativeObject {
#if !COREBUILD
		public delegate IntPtr CGEventTapCallback (IntPtr tapProxyEvent, CGEventType eventType, IntPtr eventRef, IntPtr userInfo);

		static ConditionalWeakTable<CFMachPort, TapData>? tap_table;
		static object tap_lock = new object ();

		class TapData : IDisposable {
			GCHandle handle;
			public TapData (CGEventTapCallback cb, IntPtr userInfo)
			{
				Callback = cb;
				UserInfo = userInfo;
				handle = GCHandle.Alloc (this, GCHandleType.Weak);
			}

			public void Dispose ()
			{
				if (handle.IsAllocated)
					handle.Free ();
				GC.SuppressFinalize (this);
			}

			~TapData ()
			{
				Dispose ();
			}

			public CGEventTapCallback Callback { get; private set; }
			public IntPtr UserInfo { get; private set; }
			public IntPtr Handle => GCHandle.ToIntPtr (handle);
		}

#if NET
		[UnmanagedCallersOnly]
		static IntPtr TapCallback (IntPtr tapProxyEvent, CGEventType eventType, IntPtr eventRef, IntPtr userInfo)
		{
			var gch = GCHandle.FromIntPtr (userInfo);
			var tapData = (TapData) gch.Target!;
			return tapData.Callback (tapProxyEvent, eventType, eventRef, tapData.UserInfo);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static unsafe IntPtr CGEventTapCreate (CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, delegate* unmanaged<IntPtr, CGEventType, IntPtr, IntPtr, IntPtr> cback, IntPtr data);
#else
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventTapCreate (CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data);
#endif

		static CFMachPort? CreateMachPortAndAddToTable (IntPtr machPort, TapData data)
		{
			if (machPort == IntPtr.Zero) {
				data.Dispose ();
				return null;
			}

			var rv = new CFMachPort (machPort, true);
			lock (tap_lock) {
				tap_table = tap_table ?? new ConditionalWeakTable<CFMachPort, TapData> ();
				tap_table.Add (rv, data);
			}
			return rv;
		}

		/// <summary>Create an event tap</summary>
		/// <return>A <see cref="T:CoreFoundation.CFMachPort" /> that represents the new tap, or null if the tap couldn't be created.</return>
		/// <remarks>Calling Dispose on the returned <see cref="T:CoreFoundation.CFMachPort" /> (or letting the GC collect it) will release the tap as well.</remarks>
		/// <param name="location">The location of the tap.</param>
		/// <param name="place">The placement of the tap in the list of active taps.</param>
		/// <param name="options">Any options for the new tap.</param>
		/// <param name="mask">A mask of the events to monitor.</param>
		/// <param name="cback">The callback the tap calls when there are events. The callback is called on the run loop the tap was added to.</param>
		/// <param name="data">Custom data that is passed as-is to the callback.</param>
		public static CFMachPort? CreateTap (CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data)
		{
			var tapData = new TapData (cback, data);
			IntPtr r;
#if NET
			unsafe {
				r = CGEventTapCreate (location, place, options, mask, &TapCallback, tapData.Handle);
			}
#else
			r = CGEventTapCreate (location, place, options, mask, cback, tapData.Handle);
#endif
			return CreateMachPortAndAddToTable (r, tapData);
		}

#if NET
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static unsafe IntPtr CGEventTapCreateForPSN (IntPtr processSerialNumer, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, delegate* unmanaged<IntPtr, CGEventType, IntPtr, IntPtr, IntPtr> cback, IntPtr data);
#else
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventTapCreateForPSN (IntPtr processSerialNumer, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data);
#endif

#if !XAMCORE_5_0
		[Obsolete ("The location parameter is not used. Consider using the overload without the location parameter.", false)]
		[System.ComponentModel.EditorBrowsable (System.ComponentModel.EditorBrowsableState.Never)]
		public static CFMachPort? CreateTap (IntPtr processSerialNumber, CGEventTapLocation location, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data)
		{
			return CreateTap (processSerialNumber, place, options, mask, cback, data);
		}
#endif

		/// <summary>Create an event tap monitoring the specified process serial number (psn)</summary>
		/// <return>A <see cref="T:CoreFoundation.CFMachPort" /> that represents the new tap, or null if the tap couldn't be created.</return>
		/// <remarks>Calling Dispose on the returned <see cref="T:CoreFoundation.CFMachPort" /> (or letting the GC collect it) will release the tap as well.</remarks>
		/// <param name="processSerialNumber">The process serial number (psn) to monitor</param>
		/// <param name="place">The placement of the tap in the list of active taps.</param>
		/// <param name="options">Any options for the new tap.</param>
		/// <param name="mask">A mask of the events to monitor.</param>
		/// <param name="cback">The callback the tap calls when there are events. The callback is called on the run loop the tap was added to.</param>
		/// <param name="data">Custom data that is passed as-is to the callback.</param>
		public static CFMachPort? CreateTap (IntPtr processSerialNumber, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback cback, IntPtr data)
		{
			var tapData = new TapData (cback, data);
			unsafe {
				var psnPtr = new IntPtr (&processSerialNumber);
#if NET
				var r = CGEventTapCreateForPSN (psnPtr, place, options, mask, &TapCallback, tapData.Handle);
#else
				var r = CGEventTapCreateForPSN (psnPtr, place, options, mask, cback, data);
#endif
				return CreateMachPortAndAddToTable (r, tapData);
			}
		}

#if NET
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static unsafe IntPtr CGEventTapCreateForPid (int pid, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, delegate* unmanaged<IntPtr, CGEventType, IntPtr, IntPtr, IntPtr> cback, IntPtr data);

		/// <summary>Create an event tap monitoring the specified process</summary>
		/// <return>A <see cref="T:CoreFoundation.CFMachPort" /> that represents the new tap, or null if the tap couldn't be created.</return>
		/// <remarks>Calling Dispose on the returned <see cref="T:CoreFoundation.CFMachPort" /> (or letting the GC collect it) will release the tap as well.</remarks>
		/// <param name="pid">The pid to monitor</param>
		/// <param name="place">The placement of the tap in the list of active taps.</param>
		/// <param name="options">Any options for the new tap.</param>
		/// <param name="mask">A mask of the events to monitor.</param>
		/// <param name="callback">The callback the tap calls when there are events. The callback is called on the run loop the tap was added to.</param>
		/// <param name="data">Custom data that is passed as-is to the callback.</param>
		public static CFMachPort? CreateTap (int pid, CGEventTapPlacement place, CGEventTapOptions options, CGEventMask mask, CGEventTapCallback callback, IntPtr data = default (IntPtr))
		{
			unsafe {
				var tapData = new TapData (callback, data);
				var r = CGEventTapCreateForPid (pid, place, options, mask, &TapCallback, tapData.Handle);
				return CreateMachPortAndAddToTable (r, tapData);
			}
		}
#endif

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateFromData (IntPtr allocator, IntPtr nsdataSource);

		static IntPtr Create (NSData source)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			return CGEventCreateFromData (IntPtr.Zero, source.Handle);
		}

		public CGEvent (NSData source)
			: base (Create (source), true)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreate (IntPtr eventSourceHandle);

		public CGEvent (CGEventSource? eventSource)
			: base (CGEventCreate (eventSource.GetHandle ()), true)
		{
		}

#if !NET
		public CGEvent (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGEvent (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateMouseEvent (IntPtr source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton);

		public CGEvent (CGEventSource? source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton)
			: base (CGEventCreateMouseEvent (source.GetHandle (), mouseType, mouseCursorPosition, mouseButton), true)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateKeyboardEvent (IntPtr source, ushort virtualKey, byte keyDown);

		public CGEvent (CGEventSource? source, ushort virtualKey, bool keyDown)
			: base (CGEventCreateKeyboardEvent (source.GetHandle (), virtualKey, keyDown.AsByte ()), true)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateScrollWheelEvent2 (IntPtr source, CGScrollEventUnit units, uint /* uint32_t */ wheelCount, int /* uint32_t */ wheel1, int /* uint32_t */ wheel2, int /* uint32_t */ wheel3);

		static IntPtr Create (CGEventSource? source, CGScrollEventUnit units, params int [] wheel)
		{
			IntPtr handle;
			IntPtr shandle = source.GetHandle ();

			switch (wheel.Length) {
			case 0:
				throw new ArgumentException ("At least one wheel must be provided");
			case 1:
				handle = CGEventCreateScrollWheelEvent2 (shandle, units, 1, wheel [0], 0, 0);
				break;
			case 2:
				handle = CGEventCreateScrollWheelEvent2 (shandle, units, 2, wheel [0], wheel [1], 0);
				break;
			case 3:
				handle = CGEventCreateScrollWheelEvent2 (shandle, units, 3, wheel [0], wheel [1], wheel [2]);
				break;
			default:
				throw new ArgumentException ("Only one to three wheels are supported on this constructor");
			}
			return handle;
		}

		/// <summary>Create a new scrolling event.</summary>
		/// <returns>A new scrolling event.</returns>
		/// <param name="source">An optional source for the new scrolling event.</param>
		/// <param name="units">The unit of measurement for the new scrolling event.</param>
		/// <param name="wheel">The wheel(s) the scrolling event refer to.</param>
		public CGEvent (CGEventSource? source, CGScrollEventUnit units, params int [] wheel)
			: base (Create (source, units, wheel), true)
		{
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateCopy (IntPtr handle);

		public CGEvent Copy ()
		{
			return new CGEvent (CGEventCreateCopy (Handle), true);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateData (IntPtr allocator, IntPtr handle);

		public NSData? ToData ()
		{
			return Runtime.GetNSObject<NSData> (CGEventCreateData (IntPtr.Zero, Handle));
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventCreateSourceFromEvent (IntPtr evthandle);

		public CGEventSource? CreateEventSource ()
		{
			var esh = CGEventCreateSourceFromEvent (Handle);
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
				return CGEventGetLocation (Handle);
			}
			set {
				CGEventSetLocation (Handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGPoint CGEventGetUnflippedLocation (IntPtr handle);

		public CGPoint UnflippedLocation {
			get {
				return CGEventGetUnflippedLocation (Handle);
			}
		}

		// Keep this public, as we want to avoid creating instances of the object
		// just to peek at the flags
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint = "CGEventGetFlags")]
		public extern static CGEventFlags GetFlags (IntPtr eventHandle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		internal extern static void CGEventSetFlags (IntPtr eventHandle, CGEventFlags flags);

		public CGEventFlags Flags {
			get {
				return GetFlags (Handle);
			}
			set {
				CGEventSetFlags (Handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint = "CGEventGetIntegerValueField")]
		extern static long GetLong (IntPtr eventHandle, CGEventField eventField);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint = "CGEventSetIntegerValueField")]
		extern static void SetLong (IntPtr eventHandle, CGEventField eventField, long value);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint = "CGEventGetDoubleValueField")]
		extern static double GetDouble (IntPtr eventHandle, CGEventField eventField);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint = "CGEventSetDoubleValueField")]
		extern static void SetDouble (IntPtr eventHandle, CGEventField eventField, double value);

		/// <summary>Get the 64-bit integer value of the specified event field.</summary>
		/// <param name="field">The field whose value to get.</param>
		/// <returns>The 64-bit integer value of the specified event field.</returns>
		public long GetLongValueField (CGEventField field)
		{
			return GetLong (Handle, field);
		}

		/// <summary>Get the double value of the specified event field.</summary>
		/// <param name="field">The field whose value to get.</param>
		/// <returns>The double value of the specified event field.</returns>
		public double GetDoubleValueField (CGEventField field)
		{
			return GetDouble (Handle, field);
		}

		/// <summary>Set a 64-bit integer value for the specified event field.</summary>
		/// <param name="field">The field whose value to set.</param>
		/// <param name="value">The value to set.</param>
		public void SetValueField (CGEventField field, long value)
		{
			SetLong (Handle, field, value);
		}

		/// <summary>Set a double value for the specified event field.</summary>
		/// <param name="field">The field whose value to set.</param>
		/// <param name="value">The value to set.</param>
		public void SetValueField (CGEventField field, double value)
		{
			SetDouble (Handle, field, value);
		}

		/// <summary>The mouse button event number.</summary>
		/// <remarks>Matching mouse down and mouse up events will have the same event number.</remarks>
		public long MouseEventNumber {
			get {
				return GetLong (Handle, CGEventField.MouseEventNumber);
			}
			set {
				SetLong (Handle, CGEventField.MouseEventNumber, value);
			}
		}

		/// <summary>The mouse button click state.</summary>
		/// <remarks>A value of 1 is a single click, a value of 2 is a double click, and so on.</remarks>
		public long MouseEventClickState {
			get {
				return GetLong (Handle, CGEventField.MouseEventClickState);
			}
			set {
				SetLong (Handle, CGEventField.MouseEventClickState, value);
			}
		}

		/// <summary>The mouse button pressure state, ranging from 0 (mouse being up) to 1.</summary>
		public double MouseEventPressure {
			get {
				return GetDouble (Handle, CGEventField.MouseEventPressure);
			}
			set {
				SetDouble (Handle, CGEventField.MouseEventPressure, value);
			}
		}

		/// <summary>The mouse button number.</summary>
		public long MouseEventButtonNumber {
			get {
				return GetLong (Handle, CGEventField.MouseEventButtonNumber);
			}
			set {
				SetLong (Handle, CGEventField.MouseEventButtonNumber, value);
			}
		}

		/// <summary>The horizontal delta since the last mouse movement event.</summary>
		public long MouseEventDeltaX {
			get {
				return GetLong (Handle, CGEventField.MouseEventDeltaX);
			}
			set {
				SetLong (Handle, CGEventField.MouseEventDeltaX, value);
			}
		}

		/// <summary>The vertical delta since the last mouse movement event.</summary>
		public long MouseEventDeltaY {
			get {
				return GetLong (Handle, CGEventField.MouseEventDeltaY);
			}
			set {
				SetLong (Handle, CGEventField.MouseEventDeltaY, value);
			}
		}

		/// <summary>A value indicating whether the event should be ignored by the Inkwell subsystem.</summary>
		public bool MouseEventInstantMouser {
			get {
				return GetLong (Handle, CGEventField.MouseEventInstantMouser) != 0;
			}
			set {
				SetLong (Handle, CGEventField.MouseEventInstantMouser, value ? 1 : 0);
			}
		}

		/// <summary>The mouse event subtype.</summary>
		public long MouseEventSubtype {
			get {
				return GetLong (Handle, CGEventField.MouseEventSubtype);
			}
			set {
				SetLong (Handle, CGEventField.MouseEventSubtype, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static IntPtr CGEventSetSource (IntPtr handle, IntPtr source);

		public void SetEventSource (CGEventSource eventSource)
		{
			if (eventSource is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (eventSource));
			CGEventSetSource (Handle, eventSource.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static CGEventType CGEventGetType (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSetType (IntPtr handle, CGEventType evtType);

		public CGEventType EventType {
			get {
				return CGEventGetType (Handle);
			}
			set {
				CGEventSetType (Handle, value);
			}
		}


		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static ulong CGEventGetTimestamp (IntPtr handle);

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventSetTimestamp (IntPtr handle, ulong timeStampp);

#if !NET
		[Obsolete ("Use 'Timestamp' instead.")]
		public ulong Timestampe {
			get {
				return CGEventGetTimestamp (Handle);
			}
			set {
				CGEventSetTimestamp (Handle, value);
			}
		}
#endif

		public ulong Timestamp {
			get {
				return CGEventGetTimestamp (Handle);
			}
			set {
				CGEventSetTimestamp (Handle, value);
			}
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventTapEnable (IntPtr machPort, byte enable);

		public static void TapEnable (CFMachPort machPort)
		{
			if (machPort is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (machPort));
			CGEventTapEnable (machPort.Handle, 1);
		}

		public static void TapDisable (CFMachPort machPort)
		{
			if (machPort is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (machPort));
			CGEventTapEnable (machPort.Handle, 0);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static byte CGEventTapIsEnabled (IntPtr machPort);

		public static bool IsTapEnabled (CFMachPort machPort)
		{
			if (machPort is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (machPort));
			return CGEventTapIsEnabled (machPort.Handle) != 0;
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static void CGEventKeyboardGetUnicodeString (IntPtr handle, nuint maxLen, nuint* actualLen, ushort* buffer);

		public unsafe string GetUnicodeString ()
		{
			const int bufferLength = 40;
			ushort* buffer = stackalloc ushort [bufferLength];
			nuint actual = 0;
			CGEventKeyboardGetUnicodeString (Handle, bufferLength, &actual, buffer);
			return new String ((char*) buffer, 0, (int) actual);
		}

#if NET
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static void CGEventKeyboardSetUnicodeString (IntPtr handle, nuint len, IntPtr buffer);
#else
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static void CGEventKeyboardSetUnicodeString (IntPtr handle, nuint len,  [MarshalAs(UnmanagedType.LPWStr)] string buffer);
#endif

		public void SetUnicodeString (string value)
		{
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
#if NET
			using var valueStr = new TransientString (value, TransientString.Encoding.Unicode);
			CGEventKeyboardSetUnicodeString (Handle, (nuint) value.Length, valueStr);
#else
			CGEventKeyboardSetUnicodeString (Handle, (nuint) value.Length, value);
#endif
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventTapPostEvent (IntPtr proxy, IntPtr evtHandle);

		public static void TapPostEven (IntPtr tapProxyEvent, CGEvent evt)
		{
			if (evt is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (evt));

			CGEventTapPostEvent (tapProxyEvent, evt.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventPost (CGEventTapLocation location, IntPtr handle);

		public static void Post (CGEvent evt, CGEventTapLocation location)
		{
			if (evt is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (evt));

			CGEventPost (location, evt.Handle);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventPostToPSN (IntPtr processSerialNumber, IntPtr handle);

		/// <summary>Post an event to a specific process</summary>
		/// <remarks>Deprecated, use <see cref="PostToPid(CGEvent,int)" /> instead.</remarks>
		public static void PostToPSN (CGEvent evt, IntPtr processSerialNumber)
		{
			if (evt is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (evt));

			CGEventPostToPSN (processSerialNumber, evt.Handle);
		}

		/// <summary>Post an event to a specific process</summary>
		/// <remarks>Deprecated, use <see cref="PostToPid(int)" /> instead.</remarks>
		public void PostToPSN (IntPtr processSerialNumber)
		{
			PostToPSN (this, processSerialNumber);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		extern static void CGEventPostToPid (int pid, IntPtr handle);

		/// <summary>Post an event to a specific process</summary>
		public static void PostToPid (CGEvent evt, int pid)
		{
			if (evt is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (evt));

			CGEventPostToPid (pid, evt.Handle);
		}

		/// <summary>Post an event to a specific process</summary>
		public void PostToPid (int pid)
		{
			PostToPid (this, pid);
		}

		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		unsafe extern static int /* CGError = int32_t */ CGGetEventTapList (
			uint /* uint32_t */ maxNumberOfTaps,
			CGEventTapInformation* tapList,
			uint* /* uint32_t* */ eventTapCount);

		public unsafe CGEventTapInformation []? GetEventTapList ()
		{
			uint count;
			if (CGGetEventTapList (0, null, &count) != 0)
				return null;
			var result = new CGEventTapInformation [count];
			fixed (CGEventTapInformation* p = result) {
				if (CGGetEventTapList (count, p, &count) != 0)
					return null;
			}
			return result;
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		static extern byte CGPreflightListenEventAccess ();

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static bool PreflightListenEventAccess () => CGPreflightListenEventAccess () != 0;

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary, EntryPoint = "CGRequestListenEventAccess")]
		static extern byte CGRequestListenEventAccess ();

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static bool RequestListenEventAccess () => CGRequestListenEventAccess () != 0;

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		static extern byte CGPreflightPostEventAccess ();

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static bool PreflightPostEventAccess () => CGPreflightPostEventAccess () != 0;

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ApplicationServicesCoreGraphicsLibrary)]
		static extern byte CGRequestPostEventAccess ();

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static bool RequestPostEventAccess () => CGRequestPostEventAccess () != 0;
#endif // !COREBUILD
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
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
#endif // !COREBUILD

}

#endif // MONOMAC
