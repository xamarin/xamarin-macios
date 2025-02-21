#if !TVOS
//
// MidiServices.cs: Implementation of the MidiObject base class and its derivates
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012 Xamarin Inc
//
// TODO:
//   * Each MidiObject should be added to a static hashtable so we can always
//     obtain objects that have already been created from the handle, and avoid
//     having two managed objects referencing the same unmanaged object.
//
//     Currently a few lookup functions end up creating objects that might have
//     already been surfaced (new MidiEndpoint (handle) for example)
//
// MIDISendSysex
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

#nullable enable

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

using MidiObjectRef = System.Int32;
using MidiClientRef = System.Int32;
using MidiDeviceRef = System.Int32;
using MidiDeviceListRef = System.Int32;
using MidiPortRef = System.Int32;
using MidiEndpointRef = System.Int32;
using MidiEntityRef = System.Int32;

namespace CoreMidi {

	// anonymous enum - MIDIServices.h
	public enum MidiError : int {
		/// <summary>To be added.</summary>
		Ok = 0,
		/// <summary>To be added.</summary>
		InvalidClient = -10830,
		/// <summary>To be added.</summary>
		InvalidPort = -10831,
		/// <summary>To be added.</summary>
		WrongEndpointType = -10832,
		/// <summary>To be added.</summary>
		NoConnection = -10833,
		/// <summary>To be added.</summary>
		UnknownEndpoint = -10834,
		/// <summary>To be added.</summary>
		UnknownProperty = -10835,
		/// <summary>To be added.</summary>
		WrongPropertyType = -10836,
		/// <summary>To be added.</summary>
		NoCurrentSetup = -10837,
		/// <summary>To be added.</summary>
		MessageSendErr = -10838,
		/// <summary>To be added.</summary>
		ServerStartErr = -10839,
		/// <summary>To be added.</summary>
		SetupFormatErr = -10840,
		/// <summary>To be added.</summary>
		WrongThread = -10841,
		/// <summary>To be added.</summary>
		ObjectNotFound = -10842,
		/// <summary>To be added.</summary>
		IDNotUnique = -10843,
		/// <summary>To be added.</summary>
		NotPermitted = -10844,
	}

	[Flags]
	// SInt32 - MIDIServices.h
	enum MidiObjectType : int {
		Other = -1,
		Device,
		Entity,
		Source,
		Destination,
		ExternalMask = 0x10,
		ExternalDevice = ExternalMask | Device,
		ExternalEntity = ExternalMask | Entity,
		ExternalSource = ExternalMask | Source,
		ExternalDestination = ExternalMask | Destination,
	}

	public static partial class Midi {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		extern static void MIDIRestart ();

		public static void Restart ()
		{
			MIDIRestart ();
		}

		/// <summary>The number of MIDI destinations.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public static nint DestinationCount {
			get {
				return MIDIGetNumberOfDestinations ();
			}
		}

		/// <summary>Returns the number of sources in the system.</summary>
		///         <value>
		///         </value>
		///         <remarks>Use the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:CoreMidi.MidiEndpoint.GetSource(int)&amp;scope=Xamarin" title="T:CoreMidi.MidiEndpoint.GetSource(int)">T:CoreMidi.MidiEndpoint.GetSource(int)</a></format> to fetch a specific source (represented by a MidiEndpoint).</remarks>
		public static nint SourceCount {
			get {
				return MIDIGetNumberOfSources ();
			}
		}
		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint /* ItemCount = unsigned long */ MIDIGetNumberOfDestinations ();

		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint /* ItemCount = unsigned long */ MIDIGetNumberOfSources ();

		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint /* ItemCount = unsigned long */ MIDIGetNumberOfExternalDevices ();

		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint /* ItemCount = unsigned long */ MIDIGetNumberOfDevices ();

		/// <summary>The number of external devices connected to this system.</summary>
		///         <value>
		///         </value>
		///         <remarks>You can retrieve a specific MIDI device by using the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:CoreMidi.Midi.GetExternalDevice(int)&amp;scope=Xamarin" title="T:CoreMidi.Midi.GetExternalDevice(int)">T:CoreMidi.Midi.GetExternalDevice(int)</a></format>.</remarks>
		public static nint ExternalDeviceCount {
			get {
				return MIDIGetNumberOfExternalDevices ();
			}
		}

		/// <summary>The number of MIDI devices in the system (do not confused with external connected devices).</summary>
		///         <value>
		///         </value>
		///         <remarks>You can retrieve a specific MIDI device by using the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:CoreMidi.Midi.GetDevice(int)&amp;scope=Xamarin" title="T:CoreMidi.Midi.GetDevice(int)">T:CoreMidi.Midi.GetDevice(int)</a></format>.</remarks>
		public static nint DeviceCount {
			get {
				return MIDIGetNumberOfDevices ();
			}
		}
		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiDeviceRef MIDIGetExternalDevice (nint /* ItemCount = unsigned long */ item);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiDeviceRef MIDIGetDevice (nint /* ItemCount = unsigned long */ item);

		public static MidiDevice? GetDevice (nint deviceIndex)
		{
			var h = MIDIGetDevice (deviceIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (h);
		}

		public static MidiDevice? GetExternalDevice (nint deviceIndex)
		{
			var h = MIDIGetExternalDevice (deviceIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (h);
		}
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiObject
#if !COREBUILD
	: IDisposable
#endif
	{
#if !COREBUILD
		internal const MidiObjectRef InvalidRef = 0;
		internal MidiObjectRef handle;
		internal bool owns;

		/// <summary>Handle (pointer) to the unmanaged object representation.</summary>
		///         <value>A pointer</value>
		///         <remarks>This IntPtr is a handle to the underlying unmanaged representation for this object.</remarks>
		public MidiObjectRef Handle {
			get { return handle; }
		}

		internal MidiObjectRef MidiHandle {
			get { return handle; }
		}

		internal MidiObject ()
		{
			owns = true;
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetIntegerProperty (MidiObjectRef obj, IntPtr str, int* /* SInt32 */ ret);
		internal int GetInt (IntPtr property)
		{
			int val, code;

			unsafe {
				code = MIDIObjectGetIntegerProperty (handle, property, &val);
			}
			if (code == 0)
				return val;
			throw new MidiException ((MidiError) code);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIObjectSetIntegerProperty (MidiObjectRef obj, IntPtr str, int /* SInt32 */ val);
		internal void SetInt (IntPtr property, int value)
		{
			MIDIObjectSetIntegerProperty (handle, property, value);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */  MIDIObjectGetDictionaryProperty (MidiObjectRef obj, IntPtr str, IntPtr* dict);
		internal NSDictionary? GetDictionary (IntPtr property)
		{
			IntPtr val;
			int code;

			unsafe {
				code = MIDIObjectGetDictionaryProperty (handle, property, &val);
			}
			if (code == 0) {
				var dict = Runtime.GetNSObject (val) as NSDictionary;
				if (val != IntPtr.Zero)
					CFObject.CFRelease (val);
				return dict;
			}
			throw new MidiException ((MidiError) code);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIObjectSetDictionaryProperty (MidiObjectRef obj, IntPtr str, IntPtr dict);
		internal void SetDictionary (IntPtr property, NSDictionary dict)
		{
			MIDIObjectSetDictionaryProperty (handle, property, dict.Handle);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetDataProperty (MidiObjectRef obj, IntPtr str, IntPtr* data);

		public NSData? GetData (IntPtr property)
		{
			IntPtr val;
			int code;

			unsafe {
				code = MIDIObjectGetDataProperty (handle, property, &val);
			}
			if (code == 0) {
				var data = Runtime.GetNSObject (val) as NSData;
				if (val != IntPtr.Zero)
					CFObject.CFRelease (val);
				return data;
			}
			throw new MidiException ((MidiError) code);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIObjectSetDataProperty (MidiObjectRef obj, IntPtr str, IntPtr data);

		public void SetData (IntPtr property, NSData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			MIDIObjectSetDataProperty (handle, property, data.Handle);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetStringProperty (MidiObjectRef obj, IntPtr str, IntPtr* data);

		public string? GetString (IntPtr property)
		{
			IntPtr val;
			int code;

			unsafe {
				code = MIDIObjectGetStringProperty (handle, property, &val);
			}
			if (code == 0) {
				var ret = CFString.FromHandle (val);
				if (val != IntPtr.Zero)
					CFObject.CFRelease (val);
				return ret;
			}
			return null;
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIObjectSetStringProperty (MidiObjectRef obj, IntPtr str, IntPtr nstr);

		public void SetString (IntPtr property, string value)
		{
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			using (var nsval = new NSString (value)) {
				MIDIObjectSetDictionaryProperty (handle, property, nsval.Handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiError /* OSStatus = SInt32 */ MIDIObjectRemoveProperty (MidiObjectRef obj, IntPtr str);
		public MidiError RemoveProperty (string property)
		{
			using (var nsstr = new NSString (property)) {
				return MIDIObjectRemoveProperty (handle, nsstr.Handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetProperties (MidiObjectRef obj, IntPtr* dict, byte deep);

		public NSDictionary? GetDictionaryProperties (bool deep)
		{
			IntPtr val;
			unsafe {
				if (MIDIObjectGetProperties (handle, &val, deep ? (byte) 1 : (byte) 0) != 0 || val == IntPtr.Zero)
					return null;
			}
			var value = Runtime.GetNSObject (val) as NSDictionary;
			CFObject.CFRelease (val);
			return value;
		}

		public MidiObject (MidiObjectRef handle)
		: this (handle, true)
		{
		}

		internal MidiObject (MidiObjectRef handle, bool owns)
		{
			if (handle == MidiObject.InvalidRef)
				throw new Exception ("Invalid parameters to context creation");

			this.handle = handle;
			this.owns = owns;
		}

		~MidiObject ()
		{
			Dispose (false);
		}

		// Default implementation, not all Midi* objects have a native dispose mechanism,
		internal virtual void DisposeHandle ()
		{
			handle = MidiObject.InvalidRef;
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			DisposeHandle ();
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static MidiError /* OSStatus = SInt32 */ MIDIObjectFindByUniqueID (int /* MIDIUniqueID = SInt32 */ uniqueId, MidiObjectRef* obj, MidiObjectType* objectType);

		static internal MidiObject? MidiObjectFromType (MidiObjectType type, MidiObjectRef handle)
		{
			if (handle == MidiObject.InvalidRef)
				return null;

			if (type == MidiObjectType.Other)
				return new MidiObject (handle, false);

			switch (type) {
			case MidiObjectType.ExternalDevice:
			case MidiObjectType.Device:
				return new MidiDevice (handle, false);
			case MidiObjectType.ExternalEntity:
			case MidiObjectType.Entity:
				return new MidiEntity (handle, false);
			case MidiObjectType.ExternalSource:
			case MidiObjectType.Source:
				return new MidiEndpoint (handle, false);
			case MidiObjectType.ExternalDestination:
			case MidiObjectType.Destination:
				return new MidiEndpoint (handle, false);
			default:
				throw new Exception ("Unknown MidiObjectType " + (int) type);
			}
		}

		static public MidiError FindByUniqueId (int uniqueId, out MidiObject? result)
		{
			MidiObjectRef handle;
			MidiObjectType type;
			MidiError code;

			unsafe {
				code = MIDIObjectFindByUniqueID (uniqueId, &handle, &type);
			}
			result = null;
			if (code != MidiError.Ok)
				return code;

			result = MidiObjectFromType (type, handle);
			return code;
		}
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiException : Exception {
		internal MidiException (MidiError code) : base (code == MidiError.NotPermitted ? "NotPermitted, does your app Info.plist include the 'audio' key in the UIBackgroundModes section?" : code.ToString ())
		{
			ErrorCode = code;
		}

		/// <summary>Contains the underlying MIDI error code.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiError ErrorCode { get; private set; }
	}

	delegate void MidiNotifyProc (IntPtr message, IntPtr context);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiClient : MidiObject {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
#if NET
		unsafe extern static int /* OSStatus = SInt32 */ MIDIClientCreate (IntPtr str, delegate* unmanaged<IntPtr, IntPtr, void> callback, IntPtr context, MidiObjectRef* handle);
#else
		unsafe extern static int /* OSStatus = SInt32 */ MIDIClientCreate (IntPtr str, MidiNotifyProc callback, IntPtr context, MidiObjectRef* handle);
#endif

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIClientDispose (MidiObjectRef handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDISourceCreate (MidiObjectRef handle, IntPtr name, MidiObjectRef* endpoint);

		GCHandle gch;

		internal override void DisposeHandle ()
		{
			if (handle != MidiObject.InvalidRef) {
				if (owns)
					MIDIClientDispose (handle);
				handle = MidiObject.InvalidRef;
			}
			if (gch.IsAllocated)
				gch.Free ();
		}

		public MidiClient (string name)
		{
			using (var nsstr = new NSString (name)) {
				gch = GCHandle.Alloc (this);
				int code = 0;
				MidiObjectRef tempHandle;
				unsafe {
#if NET
					code = MIDIClientCreate (nsstr.Handle, &ClientCallback, GCHandle.ToIntPtr (gch), &tempHandle);
#else
					code = MIDIClientCreate (nsstr.Handle, static_MidiNotifyProc, GCHandle.ToIntPtr (gch), &tempHandle);
#endif
				}
				handle = tempHandle;
				if (code != 0) {
					gch.Free ();
					handle = MidiObject.InvalidRef;
					throw new MidiException ((MidiError) code);
				}
				Name = name;
			}
		}
		/// <summary>Name of this MidiClient.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public string Name { get; private set; }

		public override string ToString ()
		{
			return Name;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		public MidiEndpoint? CreateVirtualSource (string name, out MidiError statusCode)
		{
			using (var nsstr = new NSString (name)) {
				MidiObjectRef ret;
				int code;
				unsafe {
					code = MIDISourceCreate (handle, nsstr.Handle, &ret);
				}
				if (code != 0) {
					statusCode = (MidiError) code;
					return null;
				}
				statusCode = MidiError.Ok;
				return new MidiEndpoint (ret, true);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		public MidiEndpoint? CreateVirtualDestination (string name, out MidiError status)
		{
			var m = new MidiEndpoint (this, name, out status);
			if (status == MidiError.Ok)
				return m;

			m.Dispose ();
			return null;
		}

		public MidiPort CreateInputPort (string name)
		{
			return new MidiPort (this, name, true);
		}

		public MidiPort CreateOutputPort (string name)
		{
			return new MidiPort (this, name, false);
		}

		public event EventHandler? SetupChanged;
		public event EventHandler<ObjectAddedOrRemovedEventArgs>? ObjectAdded;
		public event EventHandler<ObjectAddedOrRemovedEventArgs>? ObjectRemoved;
		public event EventHandler<ObjectPropertyChangedEventArgs>? PropertyChanged;
		public event EventHandler? ThruConnectionsChanged;
		public event EventHandler? SerialPortOwnerChanged;
		public event EventHandler<IOErrorEventArgs>? IOError;
#if !NET
		static MidiNotifyProc? _static_MidiNotifyProc;
		static MidiNotifyProc static_MidiNotifyProc {
			get {
				if (_static_MidiNotifyProc is null)
					_static_MidiNotifyProc = new MidiNotifyProc (ClientCallback);
				return _static_MidiNotifyProc;
			}
		}
#endif
#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (MidiNotifyProc))]
#endif
#endif // if NET
		static void ClientCallback (IntPtr message, IntPtr context)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);
			MidiClient? client = gch.Target as MidiClient;

			var id = (MidiNotificationMessageId) Marshal.ReadInt32 (message); // SInt32
			switch (id) {
			case MidiNotificationMessageId.SetupChanged:
				var esc = client?.SetupChanged;
				if (esc is not null)
					esc (client, EventArgs.Empty);
				break;
			case MidiNotificationMessageId.ObjectAdded:
				var eoa = client?.ObjectAdded;
				if (eoa is not null) {
					var data = Marshal.PtrToStructure<MidiObjectAddRemoveNotification> (message)!;
					eoa (client, new ObjectAddedOrRemovedEventArgs (MidiObjectFromType (data.ParentType, data.Parent),
											MidiObjectFromType (data.ChildType, data.Child)));
				}
				break;
			case MidiNotificationMessageId.ObjectRemoved:
				var eor = client?.ObjectRemoved;
				if (eor is not null) {
					var data = Marshal.PtrToStructure<MidiObjectAddRemoveNotification> (message)!;
					eor (client, new ObjectAddedOrRemovedEventArgs (MidiObjectFromType (data.ParentType, data.Parent),
											MidiObjectFromType (data.ChildType, data.Child)));
				}
				break;
			case MidiNotificationMessageId.PropertyChanged:
				var epc = client?.PropertyChanged;
				if (epc is not null) {
					var data = Marshal.PtrToStructure<MidiObjectPropertyChangeNotification> (message)!;
					epc (client, new ObjectPropertyChangedEventArgs (
							 MidiObjectFromType (data.ObjectType, data.ObjectHandle), CFString.FromHandle (data.PropertyName)));
				}
				break;
			case MidiNotificationMessageId.ThruConnectionsChanged:
				var e = client?.ThruConnectionsChanged;
				if (e is not null)
					e (client, EventArgs.Empty);
				break;
			case MidiNotificationMessageId.SerialPortOwnerChanged:
				e = client?.SerialPortOwnerChanged;
				if (e is not null)
					e (client, EventArgs.Empty);
				break;
			case MidiNotificationMessageId.IOError:
				var eio = client?.IOError;
				if (eio is not null) {
					var data = Marshal.PtrToStructure<MidiIOErrorNotification> (message)!;
					eio (client, new IOErrorEventArgs (new MidiDevice (data.DeviceRef), data.ErrorCode));
				}
				break;
			default:
				Debug.WriteLine (String.Format ("Unknown message received: {0}", id));
				break;
			}
		}

		protected override void Dispose (bool disposing)
		{
			SetupChanged = null;
			ObjectAdded = null;
			ObjectRemoved = null;
			PropertyChanged = null;
			ThruConnectionsChanged = null;
			SerialPortOwnerChanged = null;
			IOError = null;
			base.Dispose (disposing);
		}

		// sizeof (MIDIObjectAddRemoveNotification): 24 (on x86_64), 
		[StructLayout (LayoutKind.Sequential)]
		struct MidiObjectAddRemoveNotification {
			public MidiNotificationMessageId id;
			public int MessageSize; // UInt32
			public MidiObjectRef Parent;
			public MidiObjectType ParentType;
			public MidiObjectRef Child;
			public MidiObjectType ChildType;
		}

		[StructLayout (LayoutKind.Sequential)]
		struct MidiObjectPropertyChangeNotification {
			public MidiNotificationMessageId id;
			public int MessageSize; // UInt32
			public MidiObjectRef ObjectHandle;
			public MidiObjectType ObjectType;
			public IntPtr PropertyName; // CFStringRef
		}

		[StructLayout (LayoutKind.Sequential)]
		struct MidiIOErrorNotification {
			public MidiNotificationMessageId id;
			public int MessageSize; // UInt32
			public MidiDeviceRef DeviceRef;
			public int ErrorCode; // OSStatus = SInt32
		}
#endif // !COREBUILD
	}

	//
	// We do not pack this structure since we do not really actually marshal it,
	// we manually encode it and decode it using Marshal.{Read|Write}
	//
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiPacket
#if !COREBUILD
		: IDisposable
#endif
	{
#if !COREBUILD
		/// <summary>Time for the event, use zero to mean now.</summary>
		///         <remarks>This is the time used by the host, this is measured using the Unix mach_absolute_time() result value.</remarks>
		public long TimeStamp;
		IntPtr byteptr;
		byte []? bytes;
		int start;
		/// <summary>The number of bytes in the Bytes array</summary>
		///         <remarks>
		///         </remarks>
		public ushort Length;

		public MidiPacket (long timestamp, ushort length, IntPtr bytes)
		{
			TimeStamp = timestamp;
			Length = length;
			byteptr = bytes;
		}

		public MidiPacket (long timestamp, byte [] bytes) : this (timestamp, bytes, 0, bytes.Length, false)
		{
		}

		public MidiPacket (long timestamp, byte [] bytes, int start, int len) : this (timestamp, bytes, start, len, true)
		{
		}

		MidiPacket (long timestamp, byte [] bytes, int start, int length, bool check)
		{
			if (bytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));
			if (length > UInt16.MaxValue)
				throw new ArgumentException ("length is bigger than 64k");

			if (check) {
				if (start < 0 || start >= bytes.Length)
					throw new ArgumentException ("range is not within bytes");
				if (start + length > bytes.Length)
					throw new ArgumentException ("range is not within bytes");
			}

			TimeStamp = timestamp;
			Length = (ushort) length;
			this.start = start;
			this.bytes = bytes;
		}

		~MidiPacket ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			// make sure we don't leave pointers to potentially freed memory.
			byteptr = IntPtr.Zero;
		}

		/// <summary>Returns the 'bytes' array passed to the <see cref="MidiPacket(long,byte[])" /> constructor or the <see cref="MidiPacket(long,byte[],int,int)" /> constructor.</summary>
		public byte []? ByteArray {
			get { return bytes; }
		}

		/// <summary>Returns the 'bytes' pointer passed to the <see cref="MidiPacket(long,ushort,IntPtr)" /> constructor.</summary>
		public IntPtr BytePointer {
			get { return byteptr; }
		}

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property may return a pointer to a managed object, and this pointer is never safe to use. Use ByteArray or BytePointer instead.")]
		public IntPtr Bytes {
			get {
				if (bytes is null || bytes.Length < 1)
					return byteptr;
				unsafe {
					fixed (byte* p = &bytes [start]) {
						return (IntPtr) p;
					}
				}
			}
		}
#endif

		internal static int GetPacketLength (int payload_length)
		{
			/*
			 * IMPORTANT:
			 *     On Intel and PowerPC, MIDIPacket is unaligned.
			 *     On ARM/ARM64, MIDIPacket must be 4-byte aligned.
			 */
			var aligned = Runtime.IsARM64CallingConvention;
#if !MONOMAC && !__MACCATALYST__
			aligned |= ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.DEVICE;
#endif
			if (aligned) {
				// align to 4 bytes.
				payload_length += 10;
				return ((payload_length + 3) & (~3));
			}
			return 10 + payload_length;
		}

		// length: upon return the total amount of memory read.
		internal static MidiPacket ReadPacket (IntPtr ptr, out int length)
		{
			/*
			 * struct MIDIPacket {
			 *     MIDITimeStamp timeStamp; // UInt64
			 *     UInt16        length;
			 *     Byte          data[];
			 * }
			 * 
			 * IMPORTANT:
			 *     On Intel and PowerPC, MIDIPacket is unaligned.
			 *     On ARM, MIDIPacket must be 4-byte aligned.
			 */

			var timestamp = Marshal.ReadInt64 (ptr);
			var plen = (ushort) Marshal.ReadInt16 (ptr, 8);

			length = GetPacketLength (plen);

			return new MidiPacket (timestamp, plen, ptr + 10);
		}

		internal static MidiPacket [] ReadPacketList (IntPtr packetList)
		{
			/*
			 * struct MIDIPacketList {
			 * 	  UInt32 numPackets;
			 *    MIDIPacket packet[]
			 * }
			 */

			int npackets = Marshal.ReadInt32 (packetList);
			var packets = new MidiPacket [npackets];
			packetList += 4;
			for (int i = 0; i < npackets; i++) {
				int plen;
				packets [i] = MidiPacket.ReadPacket (packetList, out plen);
				packetList += plen;
			}
			return packets;
		}

		internal unsafe static IntPtr CreatePacketList (MidiPacket [] packets)
		{
			// calculate the total size of the data.
			int size = 4;
			for (int i = 0; i < packets.Length; i++)
				size += GetPacketLength (packets [i].Length);

			// write it out
			IntPtr buffer = Marshal.AllocHGlobal (size);
			Marshal.WriteInt32 (buffer, 0, packets.Length);
			int dest = 4;
			for (int i = 0; i < packets.Length; i++) {
				var packet = packets [i];
				var packet_size = packet.Length;
				Marshal.WriteInt64 (buffer, dest, packet.TimeStamp);
				dest += 8;
				Marshal.WriteInt16 (buffer, dest, (short) packet_size);
				dest += 2;
				if (packet.ByteArray is null) {
					Buffer.MemoryCopy ((void*) packet.BytePointer, (void*) (buffer + dest), packet_size, packet_size);
				} else {
					Marshal.Copy (packet.ByteArray, packet.start, buffer + dest, packet_size);
				}
				dest += GetPacketLength (packet_size) - 10;
			}
			return buffer;
		}
#endif
	}

	delegate void MidiReadProc (IntPtr packetList, IntPtr context, IntPtr srcPtr);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiPort : MidiObject {
#if !COREBUILD

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
#if NET
		[DllImport (Constants.CoreMidiLibrary)]
		extern unsafe static int /* OSStatus = SInt32 */ MIDIInputPortCreate (MidiClientRef client, IntPtr /* CFStringRef */ portName, delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> readProc, IntPtr context, MidiPortRef* midiPort);
#else
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIInputPortCreate (MidiClientRef client, IntPtr /* CFStringRef */ portName, MidiReadProc readProc, IntPtr context, MidiPortRef* midiPort);
#endif
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIOutputPortCreate (MidiClientRef client, IntPtr /* CFStringRef */ portName, MidiPortRef* midiPort);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIPortDispose (MidiPortRef port);

		GCHandle gch;
		bool input;

		internal MidiPort (MidiClient client, string portName, bool input)
		{
			using (var nsstr = new NSString (portName)) {
				GCHandle gch = GCHandle.Alloc (this);
				int code;

				MidiPortRef tempHandle;
				if (input) {
					unsafe {
#if NET
						code = MIDIInputPortCreate (client.handle, nsstr.Handle, &Read, GCHandle.ToIntPtr (gch), &tempHandle);
#else
						code = MIDIInputPortCreate (client.handle, nsstr.Handle, static_MidiReadProc, GCHandle.ToIntPtr (gch), &tempHandle);
#endif
					}
				} else {
					unsafe {
						code = MIDIOutputPortCreate (client.handle, nsstr.Handle, &tempHandle);
					}
				}
				handle = tempHandle;

				if (code != 0) {
					gch.Free ();
					handle = MidiObject.InvalidRef;
					throw new MidiException ((MidiError) code);
				}
				Client = client;
				PortName = portName;
				this.input = input;
			}
		}

		/// <summary>The MidiClient that created this port.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiClient Client { get; private set; }
		/// <summary>The port name specified when the port was created</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public string PortName { get; private set; }

		internal override void DisposeHandle ()
		{
			if (handle != MidiObject.InvalidRef) {
				if (owns)
					MIDIPortDispose (handle);
				handle = MidiObject.InvalidRef;
			}
			if (gch.IsAllocated)
				gch.Free ();
		}

		protected override void Dispose (bool disposing)
		{
			MessageReceived = null;
			base.Dispose (disposing);
		}

		public event EventHandler<MidiPacketsEventArgs>? MessageReceived;

#if !NET
		static MidiReadProc? _static_MidiReadProc;
		static MidiReadProc static_MidiReadProc {
			get {
				if (_static_MidiReadProc is null)
					_static_MidiReadProc = new MidiReadProc (Read);
				return _static_MidiReadProc;
			}
		}
#endif
#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (MidiReadProc))]
#endif
#endif // if NET
		static void Read (IntPtr packetList, IntPtr context, IntPtr srcPtr)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);

			if (gch.Target is MidiPort port) {
				var e = port.MessageReceived;
				if (e is not null) {
					using (var args = new MidiPacketsEventArgs (packetList))
						e (port, args);
				}
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIPortConnectSource (MidiPortRef port, MidiEndpointRef endpoint, IntPtr /* void* */context);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */MIDIPortDisconnectSource (MidiPortRef port, MidiEndpointRef endpoint);

		public MidiError ConnectSource (MidiEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			return (MidiError) MIDIPortConnectSource (handle, endpoint.handle, GCHandle.ToIntPtr (gch));
		}

		public MidiError Disconnect (MidiEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			return (MidiError) MIDIPortDisconnectSource (handle, endpoint.handle);
		}

		public override string ToString ()
		{
			return (input ? "[input:" : "[output:") + Client + ":" + PortName + "]";
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiError /* OSStatus = SInt32 */ MIDISend (MidiPortRef port, MidiEndpointRef endpoint, IntPtr packets);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		public MidiError Send (MidiEndpoint endpoint, MidiPacket [] packets)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			if (packets is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packets));
			var p = MidiPacket.CreatePacketList (packets);
			var code = MIDISend (handle, endpoint.handle, p);
			Marshal.FreeHGlobal (p);
			return code;
		}
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiEntity : MidiObject {
#if !COREBUILD
		internal MidiEntity (MidiEntityRef handle) : base (handle)
		{
		}

		internal MidiEntity (MidiEntityRef handle, bool owns) : base (handle, owns)
		{
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiEndpointRef MIDIEntityGetDestination (MidiEntityRef entity, nint idx);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiEndpointRef MIDIEntityGetSource (MidiEntityRef entity, nint idx);

		public MidiEndpoint? GetDestination (nint idx)
		{
			var dest = MIDIEntityGetDestination (handle, idx);
			if (dest == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (dest, false);
		}

		public MidiEndpoint? GetSource (nint idx)
		{
			var dest = MIDIEntityGetSource (handle, idx);
			if (dest == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (dest, false);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint MIDIEntityGetNumberOfDestinations (MidiEntityRef entity);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nint Destinations {
			get {
				return MIDIEntityGetNumberOfDestinations (handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint MIDIEntityGetNumberOfSources (MidiEntityRef entity);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nint Sources {
			get {
				return MIDIEntityGetNumberOfSources (handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIEntityGetDevice (MidiEntityRef handle, MidiDeviceRef* devRef);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public MidiDevice? Device {
			get {
				MidiEntityRef res;
				unsafe {
					if (MIDIEntityGetDevice (handle, &res) == 0)
						return new MidiDevice (res);
				}
				return null;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int AdvanceScheduleTimeMuSec {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyAdvanceScheduleTimeMuSec);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyAdvanceScheduleTimeMuSec, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool CanRoute {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyCanRoute) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyCanRoute, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int ConnectionUniqueIDInt {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSData? ConnectionUniqueIDData {
			get {
				return GetData (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetData (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int DeviceID {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyDeviceID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyDeviceID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DisplayName {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDisplayName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDisplayName, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DriverOwner {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDriverOwner);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDriverOwner, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int DriverVersion {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyDriverVersion);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyDriverVersion, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsBroadcast {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsBroadcast) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyIsBroadcast, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsDrumMachine {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsDrumMachine) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsEffectUnit {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsEffectUnit) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsEmbeddedEntity {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsEmbeddedEntity) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsMixer {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsMixer) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsSampler {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsSampler) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxReceiveChannels {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxReceiveChannels);
			}
			//set {
			//SetInt (MidiPropertyExtensions.kMIDIPropertyMaxReceiveChannels, value);
			//}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxSysExSpeed {
			get {
				try {
					return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxSysExSpeed);
				} catch {
					// Some endpoints do not support this property
					// return the MIDI 1.0 default in those cases.
					return 3125;
				}
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyMaxSysExSpeed, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxTransmitChannels {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxTransmitChannels);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyMaxTransmitChannels, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Model {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyModel);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyModel, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Name {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyName, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary? NameConfiguration {
			get {
				return GetDictionary (MidiPropertyExtensions.kMIDIPropertyNameConfiguration);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetDictionary (MidiPropertyExtensions.kMIDIPropertyNameConfiguration, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Offline {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyOffline) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyOffline, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool PanDisruptsStereo {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyPanDisruptsStereo) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyPanDisruptsStereo, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Private {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyPrivate) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyPrivate, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesBankSelectLSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectLSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectLSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesBankSelectMSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectMSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectMSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesClock {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesClock) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesClock, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesMTC {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesMTC) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesMTC, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesNotes {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesNotes) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesNotes, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesProgramChanges {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesProgramChanges) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesProgramChanges, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool SupportsGeneralMidi {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySupportsGeneralMIDI) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySupportsGeneralMIDI, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool SupportsMMC {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySupportsMMC) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySupportsMMC, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool SupportsShowControl {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySupportsShowControl) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySupportsShowControl, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsBankSelectLSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectLSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectLSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsBankSelectMSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectMSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectMSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsClock {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsClock) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsClock, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsMTC {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsMTC) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsMTC, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsNotes {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsNotes) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsNotes, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsProgramChanges {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsProgramChanges) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsProgramChanges, value ? 1 : 0);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, iOS (14, 0), MacCatalyst (14, 0)]
#endif
		public MidiProtocolId ProtocolId {
			get {
				return (MidiProtocolId) GetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID, (int) value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		public ushort UmpActiveGroupBitmap {
			get {
				return (ushort) GetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap, (int) value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		public bool UmpCanTransmitGroupless {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless) == 1;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless, value ? 1 : 0);
			}
		}
#endif // !COREBUILD
	} // MidiEntity

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiDevice : MidiObject {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint MIDIDeviceGetNumberOfEntities (MidiDeviceRef handle);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiEntityRef MIDIDeviceGetEntity (MidiDeviceRef handle, nint item);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		[DllImport (Constants.CoreMidiLibrary)]
		extern static int MIDIDeviceAddEntity (MidiDeviceRef device, /* CFString */ IntPtr name, byte embedded, nuint numSourceEndpoints, nuint numDestinationEndpoints, MidiEntityRef newEntity);

		public MidiEntity? GetEntity (nint entityIndex)
		{
			if (handle == MidiObject.InvalidRef)
				throw new ObjectDisposedException ("handle");
			var h = MIDIDeviceGetEntity (handle, entityIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiEntity (h);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
#endif
		public int Add (string name, bool embedded, nuint numSourceEndpoints, nuint numDestinationEndpoints, MidiEntity newEntity)
		{
			if (handle == MidiObject.InvalidRef)
				throw new ObjectDisposedException ("handle");
			using (NSString nsName = new NSString (name)) {
				return MIDIDeviceAddEntity (handle, nsName.Handle, embedded ? (byte) 1 : (byte) 0, numSourceEndpoints, numDestinationEndpoints, newEntity.Handle);
			}
		}

		/// <summary>Returns the number of MIDI entities in this device.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public nint EntityCount {
			get {
				return MIDIDeviceGetNumberOfEntities (handle);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Image {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyImage);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyImage, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DriverDeviceEditorApp {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDriverDeviceEditorApp);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDriverDeviceEditorApp, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int SingleRealtimeEntity {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySingleRealtimeEntity);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySingleRealtimeEntity, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int UniqueID {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyUniqueID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool UsesSerial {
			get {
				var kMIDIDriverPropertyUsesSerial = Dlfcn.GetIntPtr (Libraries.CoreMidi.Handle, "kMIDIDriverPropertyUsesSerial");
				return GetInt (kMIDIDriverPropertyUsesSerial) != 0;
			}
			set {
				var kMIDIDriverPropertyUsesSerial = Dlfcn.GetIntPtr (Libraries.CoreMidi.Handle, "kMIDIDriverPropertyUsesSerial");
				SetInt (kMIDIDriverPropertyUsesSerial, value ? 1 : 0);
			}
		}

#if !XAMCORE_5_0 || __MACOS__
#if NET
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
#if !__MACOS__
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This API does not do anything on this platform.")]
#endif
		public string? FactoryPatchNameFile {
			get {
#if __MACOS__
				return GetString (MidiPropertyExtensions.kMIDIPropertyFactoryPatchNameFile);
#else
				return null;
#endif
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
#if __MACOS__
				SetString (MidiPropertyExtensions.kMIDIPropertyFactoryPatchNameFile, value);
#endif
			}
		}
#endif // !XAMCORE_5_0 || __MACOS__

#if !XAMCORE_5_0 || __MACOS__
#if NET
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#endif
#if !__MACOS__
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This API does not do anything on this platform.")]
#endif
		public string? UserPatchNameFile {
			get {
#if __MACOS__
				return GetString (MidiPropertyExtensions.kMIDIPropertyUserPatchNameFile);
#else
				return null;
#endif
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

#if __MACOS__
				SetString (MidiPropertyExtensions.kMIDIPropertyUserPatchNameFile, value);
#endif
			}
		}
#endif // !XAMCORE_5_0 || __MACOS__

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
#endif
		public string? NameConfigurationDictionary {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyNameConfigurationDictionary);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyNameConfigurationDictionary, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int AdvanceScheduleTimeMuSec {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyAdvanceScheduleTimeMuSec);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyAdvanceScheduleTimeMuSec, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool CanRoute {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyCanRoute) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyCanRoute, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int ConnectionUniqueIDInt {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSData? ConnectionUniqueIDData {
			get {
				return GetData (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetData (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int DeviceID {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyDeviceID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyDeviceID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DisplayName {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDisplayName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDisplayName, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DriverOwner {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDriverOwner);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDriverOwner, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int DriverVersion {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyDriverVersion);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyDriverVersion, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsDrumMachine {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsDrumMachine) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsEffectUnit {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsEffectUnit) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsEmbeddedEntity {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsEmbeddedEntity) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsMixer {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsMixer) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsSampler {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsSampler) != 0;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Manufacturer {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyManufacturer);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyManufacturer, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxReceiveChannels {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxReceiveChannels);
			}
			//set {
			//SetInt (MidiPropertyExtensions.kMIDIPropertyMaxReceiveChannels, value);
			//}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxSysExSpeed {
			get {
				try {
					return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxSysExSpeed);
				} catch {
					// Some endpoints do not support this property
					// return the MIDI 1.0 default in those cases.
					return 3125;
				}
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyMaxSysExSpeed, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxTransmitChannels {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxTransmitChannels);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyMaxTransmitChannels, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Model {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyModel);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyModel, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Name {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyName, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary? NameConfiguration {
			get {
				return GetDictionary (MidiPropertyExtensions.kMIDIPropertyNameConfiguration);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetDictionary (MidiPropertyExtensions.kMIDIPropertyNameConfiguration, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Offline {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyOffline) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyOffline, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool PanDisruptsStereo {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyPanDisruptsStereo) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyPanDisruptsStereo, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Private {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyPrivate) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyPrivate, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesBankSelectLSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectLSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectLSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesBankSelectMSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectMSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesBankSelectMSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesClock {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesClock) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesClock, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesMTC {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesMTC) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesMTC, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesNotes {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesNotes) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesNotes, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ReceivesProgramChanges {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceivesProgramChanges) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceivesProgramChanges, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool SupportsGeneralMidi {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySupportsGeneralMIDI) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySupportsGeneralMIDI, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool SupportsMMC {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySupportsMMC) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySupportsMMC, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool SupportsShowControl {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertySupportsShowControl) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertySupportsShowControl, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsBankSelectLSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectLSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectLSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsBankSelectMSB {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectMSB) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsBankSelectMSB, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsClock {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsClock) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsClock, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsMTC {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsMTC) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsMTC, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsNotes {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsNotes) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsNotes, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool TransmitsProgramChanges {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsProgramChanges) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitsProgramChanges, value ? 1 : 0);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, iOS (14, 0), MacCatalyst (14, 0)]
#endif
		public MidiProtocolId ProtocolId {
			get {
				return (MidiProtocolId) GetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID, (int) value);
			}
		}

		internal MidiDevice (MidiDeviceRef handle) : base (handle)
		{
		}

		internal MidiDevice (MidiDeviceRef handle, bool owns) : base (handle, owns)
		{
		}
#endif // !COREBUILD
	} // MidiDevice

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiDeviceList : MidiObject {

#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		static extern nuint MIDIDeviceListGetNumberOfDevices (MidiDeviceListRef devList);

		[DllImport (Constants.CoreMidiLibrary)]
		static extern MidiDeviceRef MIDIDeviceListGetDevice (MidiDeviceListRef devList, nuint index);

		[DllImport (Constants.CoreMidiLibrary)]
		static extern int MIDIDeviceListAddDevice (MidiDeviceListRef devList, MidiDeviceRef dev);

		[DllImport (Constants.CoreMidiLibrary)]
		static extern int MIDIDeviceListDispose (MidiDeviceListRef devList);

		internal MidiDeviceList (MidiDeviceListRef handle) : base (handle)
		{
		}

		internal MidiDeviceList (MidiDeviceListRef handle, bool owns) : base (handle, owns)
		{
		}

		public nuint GetNumberOfDevices ()
		{
			if (handle == MidiObject.InvalidRef)
				throw new ObjectDisposedException ("handle");
			return MIDIDeviceListGetNumberOfDevices (handle);
		}

		public MidiDevice? Get (nuint index)
		{
			if (handle == MidiObject.InvalidRef)
				throw new ObjectDisposedException ("handle");
			var h = MIDIDeviceListGetDevice (handle, index);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (h);
		}

		public int Add (MidiDevice device)
		{
			if (handle == MidiObject.InvalidRef)
				throw new ObjectDisposedException ("handle");
			return MIDIDeviceListAddDevice (handle, device.Handle);
		}

		internal override void DisposeHandle ()
		{
			if (handle != MidiObject.InvalidRef) {
				if (owns)
					MIDIDeviceListDispose (handle);
				handle = MidiObject.InvalidRef;
			}
		}

#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiEndpoint : MidiObject {
#if !COREBUILD
		GCHandle gch;

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIEndpointDispose (MidiEndpointRef handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
#if NET
		[DllImport (Constants.CoreMidiLibrary)]
		extern unsafe static MidiError /* OSStatus = SInt32 */ MIDIDestinationCreate (MidiClientRef client, IntPtr /* CFStringRef */ name, delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> readProc, IntPtr context, MidiEndpointRef* midiEndpoint);
#else
		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiError /* OSStatus = SInt32 */ MIDIDestinationCreate (MidiClientRef client, IntPtr /* CFStringRef */ name, MidiReadProc readProc, IntPtr context, out MidiEndpointRef midiEndpoint);
#endif

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIFlushOutput (MidiEndpointRef handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiError /* OSStatus = SInt32 */ MIDIReceived (MidiEndpointRef handle, IntPtr /* MIDIPacketList* */ packetList);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiEndpointRef MIDIGetSource (nint sourceIndex);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiEndpointRef MIDIGetDestination (nint destinationIndex);

		internal override void DisposeHandle ()
		{
			if (handle != MidiObject.InvalidRef) {
				if (owns)
					MIDIEndpointDispose (handle);
				handle = MidiObject.InvalidRef;
			}
			if (gch.IsAllocated)
				gch.Free ();
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string EndpointName { get; private set; }

		internal MidiEndpoint (MidiEndpointRef handle) : base (handle, false)
		{
			EndpointName = "Endpoint from Lookup";
		}

		internal MidiEndpoint (MidiEndpointRef handle, bool owns) : base (handle, owns)
		{
			EndpointName = "Endpoint from Lookup";
		}

		internal MidiEndpoint (MidiEndpointRef handle, string endpointName, bool owns) : base (handle, owns)
		{
			EndpointName = endpointName;
		}

		public static MidiEndpoint? GetSource (nint sourceIndex)
		{
			var h = MIDIGetSource (sourceIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (h, "Source" + sourceIndex, false);
		}

		public static MidiEndpoint? GetDestination (nint destinationIndex)
		{
			var h = MIDIGetDestination (destinationIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (h, "Destination" + destinationIndex, false);
		}

		internal MidiEndpoint (MidiClient client, string name, out MidiError code)
		{
			using (var nsstr = new NSString (name)) {
				GCHandle gch = GCHandle.Alloc (this);
#if NET
				unsafe {
					MidiEndpointRef tempHandle;
					code = MIDIDestinationCreate (client.handle, nsstr.Handle, &Read, GCHandle.ToIntPtr (gch), &tempHandle);
					handle = tempHandle;
				}
#else
				code = MIDIDestinationCreate (client.handle, nsstr.Handle, static_MidiReadProc, GCHandle.ToIntPtr (gch), out handle);
#endif
				EndpointName = name;
			}
		}

		protected override void Dispose (bool disposing)
		{
			MessageReceived = null;
			base.Dispose (disposing);
		}

		public event EventHandler<MidiPacketsEventArgs>? MessageReceived;
#if !NET
		static MidiReadProc? _static_MidiReadProc;
		static MidiReadProc static_MidiReadProc {
			get {
				if (_static_MidiReadProc is null)
					_static_MidiReadProc = new MidiReadProc (Read);
				return _static_MidiReadProc;
			}
		}
#endif

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (MidiReadProc))]
#endif
#endif // if NET
		static void Read (IntPtr packetList, IntPtr context, IntPtr srcPtr)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);

			if (gch.Target is MidiEndpoint port) {
				var e = port.MessageReceived;
				if (e is not null)
					e (port, new MidiPacketsEventArgs (packetList));
			}
		}

		public void FlushOutput ()
		{
			MIDIFlushOutput (handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
#else
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
#endif
		public MidiError Received (MidiPacket [] packets)
		{
			if (packets is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packets));

			var block = MidiPacket.CreatePacketList (packets);
			var code = MIDIReceived (handle, block);
			Marshal.FreeHGlobal (block);
			return code;
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIEndpointGetEntity (MidiEndpointRef endpoint, MidiEntityRef* entity);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public MidiEntity? Entity {
			get {
				MidiEntityRef entity;
				int code;
				unsafe {
					code = MIDIEndpointGetEntity (handle, &entity);
				}
				if (code == 0)
					return new MidiEntity (entity);
				return null;
			}
		}

		/// <summary>Determines if the Endpoint represents a network session.</summary>
		///         <value>
		///         </value>
		///         <remarks>This probes for the apple.midirtp.session property in the endpoint properties.</remarks>
		public bool IsNetworkSession {
			get {
				using (var dict = GetDictionaryProperties (true)) {
					if (dict is null)
						return false;

					using (var key = new NSString ("apple.midirtp.session"))
						return dict.ContainsKey (key);
				}
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int AdvanceScheduleTimeMuSec {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyAdvanceScheduleTimeMuSec);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyAdvanceScheduleTimeMuSec, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int ConnectionUniqueIDInt {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSData? ConnectionUniqueIDData {
			get {
				return GetData (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetData (MidiPropertyExtensions.kMIDIPropertyConnectionUniqueID, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DisplayName {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDisplayName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDisplayName, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DriverOwner {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyDriverOwner);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyDriverOwner, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int DriverVersion {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyDriverVersion);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyDriverVersion, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsBroadcast {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyIsBroadcast) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyIsBroadcast, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Manufacturer {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyManufacturer);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyManufacturer, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaxSysExSpeed {
			get {
				try {
					return GetInt (MidiPropertyExtensions.kMIDIPropertyMaxSysExSpeed);
				} catch {
					// Some endpoints do not support this property
					// return the MIDI 1.0 default in those cases.
					return 3125;
				}
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyMaxSysExSpeed, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Name {
			get {
				return GetString (MidiPropertyExtensions.kMIDIPropertyName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (MidiPropertyExtensions.kMIDIPropertyName, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary? NameConfiguration {
			get {
				return GetDictionary (MidiPropertyExtensions.kMIDIPropertyNameConfiguration);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetDictionary (MidiPropertyExtensions.kMIDIPropertyNameConfiguration, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Offline {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyOffline) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyOffline, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Private {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyPrivate) != 0;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyPrivate, value ? 1 : 0);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int ReceiveChannels {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyReceiveChannels);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyReceiveChannels, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int TransmitChannels {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyTransmitChannels);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyTransmitChannels, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, iOS (14, 0), MacCatalyst (14, 0)]
#endif
		public MidiProtocolId ProtocolId {
			get {
				return (MidiProtocolId) GetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID, (int) value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		public ushort UmpActiveGroupBitmap {
			get {
				return (ushort) GetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap, (int) value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		public bool UmpCanTransmitGroupless {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless) == 1;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless, value ? 1 : 0);
			}
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public int AssociatedEndpoint {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyAssociatedEndpoint);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyAssociatedEndpoint, value);
			}
		}
		// MidiEndpoint 
#endif // !COREBUILD
	}

	// SInt32 - MIDIServices.h
	enum MidiNotificationMessageId : int {
		SetupChanged = 1,
		ObjectAdded,
		ObjectRemoved,
		PropertyChanged,
		ThruConnectionsChanged,
		SerialPortOwnerChanged,
		IOError,
#if !MONOMAC
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		InternalStart = 0x1000,
#endif
	}

	//
	// The notification EventArgs
	//
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class ObjectAddedOrRemovedEventArgs : EventArgs {
		public ObjectAddedOrRemovedEventArgs (MidiObject? parent, MidiObject? child)
		{
			Parent = parent;
			Child = child;
		}
		/// <summary>The parent object for the added or removed object.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiObject? Parent { get; private set; }
		/// <summary>The child that was added or removed.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiObject? Child { get; private set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class ObjectPropertyChangedEventArgs : EventArgs {
		public ObjectPropertyChangedEventArgs (MidiObject? midiObject, string? propertyName)
		{
			MidiObject = midiObject;
			PropertyName = propertyName;
		}
		/// <summary>The MIDI object whose property has changed.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiObject? MidiObject { get; private set; }
		/// <summary>The name of the MIDI property that changed.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public string? PropertyName { get; private set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class IOErrorEventArgs : EventArgs {
		public IOErrorEventArgs (MidiDevice device, int errorCode)
		{
			Device = device;
			ErrorCode = errorCode;
		}
		/// <summary>Device that raised the error.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiDevice Device { get; set; }
		/// <summary>OSStatus error code.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public int ErrorCode { get; set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiPacketsEventArgs : EventArgs
#if !COREBUILD
		, IDisposable
#endif
		{
#if !COREBUILD
		IntPtr packetList;
		MidiPacket []? list;

		internal MidiPacketsEventArgs (IntPtr packetList)
		{
			this.packetList = packetList;
		}

		~MidiPacketsEventArgs ()
		{
			Dispose (false);
		}

		/// <summary>Low-level pointer to the packet list, use Packets instead.</summary>
		///         <value>
		///         </value>
		///         <remarks>This is provided in case you want to decode the value
		///         using a native library or some other existing MIDI parsing
		///         code.  Otherwise you can consume the strongly typed Packets
		///         property.</remarks>
		public IntPtr PacketListRaw {
			get {
				return packetList;
			}
		}

		/// <summary>Contains the individual MIDI packets</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public MidiPacket [] Packets {
			get {
				if (list is null)
					list = MidiPacket.ReadPacketList (packetList);
				return list;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			// The list of packets may have pointers into packetList, make sure
			// we invalidate those pointers.
			if (list is not null) {
				foreach (var packet in list)
					packet.Dispose ();
			}
		}
#endif // !COREBUILD
	}
}
#endif
