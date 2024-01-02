#if !TVOS && !WATCH
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
		Ok = 0,
		InvalidClient = -10830,
		InvalidPort = -10831,
		WrongEndpointType = -10832,
		NoConnection = -10833,
		UnknownEndpoint = -10834,
		UnknownProperty = -10835,
		WrongPropertyType = -10836,
		NoCurrentSetup = -10837,
		MessageSendErr = -10838,
		ServerStartErr = -10839,
		SetupFormatErr = -10840,
		WrongThread = -10841,
		ObjectNotFound = -10842,
		IDNotUnique = -10843,
		NotPermitted = -10844
	}

	[Flags]
	// SInt32 - MIDIServices.h
	enum MidiObjectType : int {
		Other = -1,
		Device, Entity, Source, Destination,
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

		public static nint DestinationCount {
			get {
				return MIDIGetNumberOfDestinations ();
			}
		}

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

		public static nint ExternalDeviceCount {
			get {
				return MIDIGetNumberOfExternalDevices ();
			}
		}

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

		internal static IntPtr kMIDIPropertyAdvanceScheduleTimeMuSec;
		internal static IntPtr kMIDIPropertyCanRoute;
		internal static IntPtr kMIDIPropertyConnectionUniqueID;
		internal static IntPtr kMIDIPropertyDeviceID;
		internal static IntPtr kMIDIPropertyDisplayName;
		internal static IntPtr kMIDIPropertyDriverDeviceEditorApp;
		internal static IntPtr kMIDIPropertyDriverOwner;
		internal static IntPtr kMIDIPropertyDriverVersion;
		internal static IntPtr kMIDIPropertyImage;
		internal static IntPtr kMIDIPropertyIsBroadcast;
		internal static IntPtr kMIDIPropertyIsDrumMachine;
		internal static IntPtr kMIDIPropertyIsEffectUnit;
		internal static IntPtr kMIDIPropertyIsEmbeddedEntity;
		internal static IntPtr kMIDIPropertyIsMixer;
		internal static IntPtr kMIDIPropertyIsSampler;
		internal static IntPtr kMIDIPropertyManufacturer;
		internal static IntPtr kMIDIPropertyMaxReceiveChannels;
		internal static IntPtr kMIDIPropertyMaxSysExSpeed;
		internal static IntPtr kMIDIPropertyMaxTransmitChannels;
		internal static IntPtr kMIDIPropertyModel;
		internal static IntPtr kMIDIPropertyName;
		internal static IntPtr kMIDIPropertyNameConfiguration;
		internal static IntPtr kMIDIPropertyOffline;
		internal static IntPtr kMIDIPropertyPanDisruptsStereo;
		internal static IntPtr kMIDIPropertyPrivate;
		internal static IntPtr kMIDIPropertyReceiveChannels;
		internal static IntPtr kMIDIPropertyReceivesBankSelectLSB;
		internal static IntPtr kMIDIPropertyReceivesBankSelectMSB;
		internal static IntPtr kMIDIPropertyReceivesClock;
		internal static IntPtr kMIDIPropertyReceivesMTC;
		internal static IntPtr kMIDIPropertyReceivesNotes;
		internal static IntPtr kMIDIPropertyReceivesProgramChanges;
		internal static IntPtr kMIDIPropertySingleRealtimeEntity;
		internal static IntPtr kMIDIPropertySupportsGeneralMIDI;
		internal static IntPtr kMIDIPropertySupportsMMC;
		internal static IntPtr kMIDIPropertySupportsShowControl;
		internal static IntPtr kMIDIPropertyTransmitChannels;
		internal static IntPtr kMIDIPropertyTransmitsBankSelectLSB;
		internal static IntPtr kMIDIPropertyTransmitsBankSelectMSB;
		internal static IntPtr kMIDIPropertyTransmitsClock;
		internal static IntPtr kMIDIPropertyTransmitsMTC;
		internal static IntPtr kMIDIPropertyTransmitsNotes;
		internal static IntPtr kMIDIPropertyTransmitsProgramChanges;
		internal static IntPtr kMIDIPropertyUniqueID;
		internal static IntPtr kMIDIDriverPropertyUsesSerial;
		internal static IntPtr kMIDIPropertyFactoryPatchNameFile;
		internal static IntPtr kMIDIPropertyUserPatchNameFile;
		internal static IntPtr kMIDIPropertyNameConfigurationDictionary;

		static MidiObject ()
		{
			var midiLibrary = Libraries.CoreMidi.Handle;
			kMIDIPropertyAdvanceScheduleTimeMuSec = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyAdvanceScheduleTimeMuSec");
			kMIDIPropertyCanRoute = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyCanRoute");
			kMIDIPropertyConnectionUniqueID = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyConnectionUniqueID");
			kMIDIPropertyDeviceID = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyDeviceID");
			kMIDIPropertyDisplayName = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyDisplayName");
			kMIDIPropertyDriverDeviceEditorApp = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyDriverDeviceEditorApp");
			kMIDIPropertyDriverOwner = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyDriverOwner");
			kMIDIPropertyDriverVersion = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyDriverVersion");
			kMIDIPropertyImage = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyImage");
			kMIDIPropertyIsBroadcast = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyIsBroadcast");
			kMIDIPropertyIsDrumMachine = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyIsDrumMachine");
			kMIDIPropertyIsEffectUnit = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyIsEffectUnit");
			kMIDIPropertyIsEmbeddedEntity = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyIsEmbeddedEntity");
			kMIDIPropertyIsMixer = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyIsMixer");
			kMIDIPropertyIsSampler = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyIsSampler");
			kMIDIPropertyManufacturer = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyManufacturer");
			kMIDIPropertyMaxReceiveChannels = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyMaxReceiveChannels");
			kMIDIPropertyMaxSysExSpeed = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyMaxSysExSpeed");
			kMIDIPropertyMaxTransmitChannels = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyMaxTransmitChannels");
			kMIDIPropertyModel = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyModel");
			kMIDIPropertyName = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyName");
			kMIDIPropertyNameConfiguration = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyNameConfiguration");
			kMIDIPropertyOffline = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyOffline");
			kMIDIPropertyPanDisruptsStereo = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyPanDisruptsStereo");
			kMIDIPropertyPrivate = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyPrivate");
			kMIDIPropertyReceiveChannels = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceiveChannels");
			kMIDIPropertyReceivesBankSelectLSB = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceivesBankSelectLSB");
			kMIDIPropertyReceivesBankSelectMSB = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceivesBankSelectMSB");
			kMIDIPropertyReceivesClock = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceivesClock");
			kMIDIPropertyReceivesMTC = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceivesMTC");
			kMIDIPropertyReceivesNotes = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceivesNotes");
			kMIDIPropertyReceivesProgramChanges = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyReceivesProgramChanges");
			kMIDIPropertySingleRealtimeEntity = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertySingleRealtimeEntity");
			kMIDIPropertySupportsGeneralMIDI = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertySupportsGeneralMIDI");
			kMIDIPropertySupportsMMC = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertySupportsMMC");
			kMIDIPropertySupportsShowControl = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertySupportsShowControl");
			kMIDIPropertyTransmitChannels = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitChannels");
			kMIDIPropertyTransmitsBankSelectLSB = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitsBankSelectLSB");
			kMIDIPropertyTransmitsBankSelectMSB = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitsBankSelectMSB");
			kMIDIPropertyTransmitsClock = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitsClock");
			kMIDIPropertyTransmitsMTC = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitsMTC");
			kMIDIPropertyTransmitsNotes = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitsNotes");
			kMIDIPropertyTransmitsProgramChanges = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyTransmitsProgramChanges");
			kMIDIPropertyUniqueID = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyUniqueID");
			kMIDIDriverPropertyUsesSerial = Dlfcn.GetIntPtr (midiLibrary, "kMIDIDriverPropertyUsesSerial");
			kMIDIPropertyFactoryPatchNameFile = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyFactoryPatchNameFile");
			kMIDIPropertyUserPatchNameFile = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyUserPatchNameFile");
			kMIDIPropertyNameConfigurationDictionary = Dlfcn.GetIntPtr (midiLibrary, "kMIDIPropertyNameConfigurationDictionary");
		}

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
		public long TimeStamp;
		IntPtr byteptr;
		byte [] bytes = Array.Empty<byte> ();
		int start;
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

		internal byte [] ByteArray {
			get { return bytes; }
		}

		internal IntPtr BytePointer {
			get { return byteptr; }
		}

#if !XAMCORE_5_0
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

		public MidiClient Client { get; private set; }
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

		public nint Destinations {
			get {
				return MIDIEntityGetNumberOfDestinations (handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint MIDIEntityGetNumberOfSources (MidiEntityRef entity);

		public nint Sources {
			get {
				return MIDIEntityGetNumberOfSources (handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIEntityGetDevice (MidiEntityRef handle, MidiDeviceRef* devRef);

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

		public int AdvanceScheduleTimeMuSec {
			get {
				return GetInt (kMIDIPropertyAdvanceScheduleTimeMuSec);
			}
			set {
				SetInt (kMIDIPropertyAdvanceScheduleTimeMuSec, value);
			}
		}

		public bool CanRoute {
			get {
				return GetInt (kMIDIPropertyCanRoute) != 0;
			}
			set {
				SetInt (kMIDIPropertyCanRoute, value ? 1 : 0);
			}
		}

		public int ConnectionUniqueIDInt {
			get {
				return GetInt (kMIDIPropertyConnectionUniqueID);
			}
			set {
				SetInt (kMIDIPropertyConnectionUniqueID, value);
			}
		}

		public NSData? ConnectionUniqueIDData {
			get {
				return GetData (kMIDIPropertyConnectionUniqueID);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetData (kMIDIPropertyConnectionUniqueID, value);
			}
		}

		public int DeviceID {
			get {
				return GetInt (kMIDIPropertyDeviceID);
			}
			set {
				SetInt (kMIDIPropertyDeviceID, value);
			}
		}

		public string? DisplayName {
			get {
				return GetString (kMIDIPropertyDisplayName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDisplayName, value);
			}
		}

		public string? DriverOwner {
			get {
				return GetString (kMIDIPropertyDriverOwner);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDriverOwner, value);
			}
		}

		public int DriverVersion {
			get {
				return GetInt (kMIDIPropertyDriverVersion);
			}
			set {
				SetInt (kMIDIPropertyDriverVersion, value);
			}
		}

		public bool IsBroadcast {
			get {
				return GetInt (kMIDIPropertyIsBroadcast) != 0;
			}
			set {
				SetInt (kMIDIPropertyIsBroadcast, value ? 1 : 0);
			}
		}

		public bool IsDrumMachine {
			get {
				return GetInt (kMIDIPropertyIsDrumMachine) != 0;
			}
		}

		public bool IsEffectUnit {
			get {
				return GetInt (kMIDIPropertyIsEffectUnit) != 0;
			}
		}

		public bool IsEmbeddedEntity {
			get {
				return GetInt (kMIDIPropertyIsEmbeddedEntity) != 0;
			}
		}

		public bool IsMixer {
			get {
				return GetInt (kMIDIPropertyIsMixer) != 0;
			}
		}

		public bool IsSampler {
			get {
				return GetInt (kMIDIPropertyIsSampler) != 0;
			}
		}

		public int MaxReceiveChannels {
			get {
				return GetInt (kMIDIPropertyMaxReceiveChannels);
			}
			//set {
			//SetInt (kMIDIPropertyMaxReceiveChannels, value);
			//}
		}

		public int MaxSysExSpeed {
			get {
				try {
					return GetInt (kMIDIPropertyMaxSysExSpeed);
				} catch {
					// Some endpoints do not support this property
					// return the MIDI 1.0 default in those cases.
					return 3125;
				}
			}
			set {
				SetInt (kMIDIPropertyMaxSysExSpeed, value);
			}
		}

		public int MaxTransmitChannels {
			get {
				return GetInt (kMIDIPropertyMaxTransmitChannels);
			}
			set {
				SetInt (kMIDIPropertyMaxTransmitChannels, value);
			}
		}

		public string? Model {
			get {
				return GetString (kMIDIPropertyModel);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyModel, value);
			}
		}

		public string? Name {
			get {
				return GetString (kMIDIPropertyName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyName, value);
			}
		}

		public NSDictionary? NameConfiguration {
			get {
				return GetDictionary (kMIDIPropertyNameConfiguration);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetDictionary (kMIDIPropertyNameConfiguration, value);
			}
		}

		public bool Offline {
			get {
				return GetInt (kMIDIPropertyOffline) != 0;
			}
			set {
				SetInt (kMIDIPropertyOffline, value ? 1 : 0);
			}
		}

		public bool PanDisruptsStereo {
			get {
				return GetInt (kMIDIPropertyPanDisruptsStereo) != 0;
			}
			set {
				SetInt (kMIDIPropertyPanDisruptsStereo, value ? 1 : 0);
			}
		}

		public bool Private {
			get {
				return GetInt (kMIDIPropertyPrivate) != 0;
			}
			set {
				SetInt (kMIDIPropertyPrivate, value ? 1 : 0);
			}
		}

		public bool ReceivesBankSelectLSB {
			get {
				return GetInt (kMIDIPropertyReceivesBankSelectLSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesBankSelectLSB, value ? 1 : 0);
			}
		}

		public bool ReceivesBankSelectMSB {
			get {
				return GetInt (kMIDIPropertyReceivesBankSelectMSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesBankSelectMSB, value ? 1 : 0);
			}
		}

		public bool ReceivesClock {
			get {
				return GetInt (kMIDIPropertyReceivesClock) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesClock, value ? 1 : 0);
			}
		}

		public bool ReceivesMTC {
			get {
				return GetInt (kMIDIPropertyReceivesMTC) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesMTC, value ? 1 : 0);
			}
		}

		public bool ReceivesNotes {
			get {
				return GetInt (kMIDIPropertyReceivesNotes) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesNotes, value ? 1 : 0);
			}
		}

		public bool ReceivesProgramChanges {
			get {
				return GetInt (kMIDIPropertyReceivesProgramChanges) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesProgramChanges, value ? 1 : 0);
			}
		}

		public bool SupportsGeneralMidi {
			get {
				return GetInt (kMIDIPropertySupportsGeneralMIDI) != 0;
			}
			set {
				SetInt (kMIDIPropertySupportsGeneralMIDI, value ? 1 : 0);
			}
		}

		public bool SupportsMMC {
			get {
				return GetInt (kMIDIPropertySupportsMMC) != 0;
			}
			set {
				SetInt (kMIDIPropertySupportsMMC, value ? 1 : 0);
			}
		}

		public bool SupportsShowControl {
			get {
				return GetInt (kMIDIPropertySupportsShowControl) != 0;
			}
			set {
				SetInt (kMIDIPropertySupportsShowControl, value ? 1 : 0);
			}
		}

		public bool TransmitsBankSelectLSB {
			get {
				return GetInt (kMIDIPropertyTransmitsBankSelectLSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsBankSelectLSB, value ? 1 : 0);
			}
		}

		public bool TransmitsBankSelectMSB {
			get {
				return GetInt (kMIDIPropertyTransmitsBankSelectMSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsBankSelectMSB, value ? 1 : 0);
			}
		}

		public bool TransmitsClock {
			get {
				return GetInt (kMIDIPropertyTransmitsClock) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsClock, value ? 1 : 0);
			}
		}

		public bool TransmitsMTC {
			get {
				return GetInt (kMIDIPropertyTransmitsMTC) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsMTC, value ? 1 : 0);
			}
		}

		public bool TransmitsNotes {
			get {
				return GetInt (kMIDIPropertyTransmitsNotes) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsNotes, value ? 1 : 0);
			}
		}

		public bool TransmitsProgramChanges {
			get {
				return GetInt (kMIDIPropertyTransmitsProgramChanges) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsProgramChanges, value ? 1 : 0);
			}
		}
#endif // !COREBUILD
	}

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

		public nint EntityCount {
			get {
				return MIDIDeviceGetNumberOfEntities (handle);
			}
		}

		public string? Image {
			get {
				return GetString (kMIDIPropertyImage);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyImage, value);
			}
		}

		public string? DriverDeviceEditorApp {
			get {
				return GetString (kMIDIPropertyDriverDeviceEditorApp);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDriverDeviceEditorApp, value);
			}
		}

		public int SingleRealtimeEntity {
			get {
				return GetInt (kMIDIPropertySingleRealtimeEntity);
			}
			set {
				SetInt (kMIDIPropertySingleRealtimeEntity, value);
			}
		}

		public int UniqueID {
			get {
				return GetInt (kMIDIPropertyUniqueID);
			}
			set {
				SetInt (kMIDIPropertyUniqueID, value);
			}
		}

		public bool UsesSerial {
			get {
				return GetInt (kMIDIDriverPropertyUsesSerial) != 0;
			}
			set {
				SetInt (kMIDIDriverPropertyUsesSerial, value ? 1 : 0);
			}
		}

		public string? FactoryPatchNameFile {
			get {
				return GetString (kMIDIPropertyFactoryPatchNameFile);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyFactoryPatchNameFile, value);
			}
		}

		public string? UserPatchNameFile {
			get {
				return GetString (kMIDIPropertyUserPatchNameFile);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyUserPatchNameFile, value);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
#endif
		public string? NameConfigurationDictionary {
			get {
				return GetString (kMIDIPropertyNameConfigurationDictionary);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyNameConfigurationDictionary, value);
			}
		}

		public int AdvanceScheduleTimeMuSec {
			get {
				return GetInt (kMIDIPropertyAdvanceScheduleTimeMuSec);
			}
			set {
				SetInt (kMIDIPropertyAdvanceScheduleTimeMuSec, value);
			}
		}

		public bool CanRoute {
			get {
				return GetInt (kMIDIPropertyCanRoute) != 0;
			}
			set {
				SetInt (kMIDIPropertyCanRoute, value ? 1 : 0);
			}
		}

		public int ConnectionUniqueIDInt {
			get {
				return GetInt (kMIDIPropertyConnectionUniqueID);
			}
			set {
				SetInt (kMIDIPropertyConnectionUniqueID, value);
			}
		}

		public NSData? ConnectionUniqueIDData {
			get {
				return GetData (kMIDIPropertyConnectionUniqueID);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetData (kMIDIPropertyConnectionUniqueID, value);
			}
		}

		public int DeviceID {
			get {
				return GetInt (kMIDIPropertyDeviceID);
			}
			set {
				SetInt (kMIDIPropertyDeviceID, value);
			}
		}

		public string? DisplayName {
			get {
				return GetString (kMIDIPropertyDisplayName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDisplayName, value);
			}
		}

		public string? DriverOwner {
			get {
				return GetString (kMIDIPropertyDriverOwner);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDriverOwner, value);
			}
		}

		public int DriverVersion {
			get {
				return GetInt (kMIDIPropertyDriverVersion);
			}
			set {
				SetInt (kMIDIPropertyDriverVersion, value);
			}
		}

		public bool IsDrumMachine {
			get {
				return GetInt (kMIDIPropertyIsDrumMachine) != 0;
			}
		}

		public bool IsEffectUnit {
			get {
				return GetInt (kMIDIPropertyIsEffectUnit) != 0;
			}
		}

		public bool IsEmbeddedEntity {
			get {
				return GetInt (kMIDIPropertyIsEmbeddedEntity) != 0;
			}
		}

		public bool IsMixer {
			get {
				return GetInt (kMIDIPropertyIsMixer) != 0;
			}
		}

		public bool IsSampler {
			get {
				return GetInt (kMIDIPropertyIsSampler) != 0;
			}
		}

		public string? Manufacturer {
			get {
				return GetString (kMIDIPropertyManufacturer);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyManufacturer, value);
			}
		}

		public int MaxReceiveChannels {
			get {
				return GetInt (kMIDIPropertyMaxReceiveChannels);
			}
			//set {
			//SetInt (kMIDIPropertyMaxReceiveChannels, value);
			//}
		}

		public int MaxSysExSpeed {
			get {
				try {
					return GetInt (kMIDIPropertyMaxSysExSpeed);
				} catch {
					// Some endpoints do not support this property
					// return the MIDI 1.0 default in those cases.
					return 3125;
				}
			}
			set {
				SetInt (kMIDIPropertyMaxSysExSpeed, value);
			}
		}

		public int MaxTransmitChannels {
			get {
				return GetInt (kMIDIPropertyMaxTransmitChannels);
			}
			set {
				SetInt (kMIDIPropertyMaxTransmitChannels, value);
			}
		}

		public string? Model {
			get {
				return GetString (kMIDIPropertyModel);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyModel, value);
			}
		}

		public string? Name {
			get {
				return GetString (kMIDIPropertyName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyName, value);
			}
		}

		public NSDictionary? NameConfiguration {
			get {
				return GetDictionary (kMIDIPropertyNameConfiguration);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetDictionary (kMIDIPropertyNameConfiguration, value);
			}
		}

		public bool Offline {
			get {
				return GetInt (kMIDIPropertyOffline) != 0;
			}
			set {
				SetInt (kMIDIPropertyOffline, value ? 1 : 0);
			}
		}

		public bool PanDisruptsStereo {
			get {
				return GetInt (kMIDIPropertyPanDisruptsStereo) != 0;
			}
			set {
				SetInt (kMIDIPropertyPanDisruptsStereo, value ? 1 : 0);
			}
		}

		public bool Private {
			get {
				return GetInt (kMIDIPropertyPrivate) != 0;
			}
			set {
				SetInt (kMIDIPropertyPrivate, value ? 1 : 0);
			}
		}

		public bool ReceivesBankSelectLSB {
			get {
				return GetInt (kMIDIPropertyReceivesBankSelectLSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesBankSelectLSB, value ? 1 : 0);
			}
		}

		public bool ReceivesBankSelectMSB {
			get {
				return GetInt (kMIDIPropertyReceivesBankSelectMSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesBankSelectMSB, value ? 1 : 0);
			}
		}

		public bool ReceivesClock {
			get {
				return GetInt (kMIDIPropertyReceivesClock) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesClock, value ? 1 : 0);
			}
		}

		public bool ReceivesMTC {
			get {
				return GetInt (kMIDIPropertyReceivesMTC) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesMTC, value ? 1 : 0);
			}
		}

		public bool ReceivesNotes {
			get {
				return GetInt (kMIDIPropertyReceivesNotes) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesNotes, value ? 1 : 0);
			}
		}

		public bool ReceivesProgramChanges {
			get {
				return GetInt (kMIDIPropertyReceivesProgramChanges) != 0;
			}
			set {
				SetInt (kMIDIPropertyReceivesProgramChanges, value ? 1 : 0);
			}
		}

		public bool SupportsGeneralMidi {
			get {
				return GetInt (kMIDIPropertySupportsGeneralMIDI) != 0;
			}
			set {
				SetInt (kMIDIPropertySupportsGeneralMIDI, value ? 1 : 0);
			}
		}

		public bool SupportsMMC {
			get {
				return GetInt (kMIDIPropertySupportsMMC) != 0;
			}
			set {
				SetInt (kMIDIPropertySupportsMMC, value ? 1 : 0);
			}
		}

		public bool SupportsShowControl {
			get {
				return GetInt (kMIDIPropertySupportsShowControl) != 0;
			}
			set {
				SetInt (kMIDIPropertySupportsShowControl, value ? 1 : 0);
			}
		}

		public bool TransmitsBankSelectLSB {
			get {
				return GetInt (kMIDIPropertyTransmitsBankSelectLSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsBankSelectLSB, value ? 1 : 0);
			}
		}

		public bool TransmitsBankSelectMSB {
			get {
				return GetInt (kMIDIPropertyTransmitsBankSelectMSB) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsBankSelectMSB, value ? 1 : 0);
			}
		}

		public bool TransmitsClock {
			get {
				return GetInt (kMIDIPropertyTransmitsClock) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsClock, value ? 1 : 0);
			}
		}

		public bool TransmitsMTC {
			get {
				return GetInt (kMIDIPropertyTransmitsMTC) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsMTC, value ? 1 : 0);
			}
		}

		public bool TransmitsNotes {
			get {
				return GetInt (kMIDIPropertyTransmitsNotes) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsNotes, value ? 1 : 0);
			}
		}

		public bool TransmitsProgramChanges {
			get {
				return GetInt (kMIDIPropertyTransmitsProgramChanges) != 0;
			}
			set {
				SetInt (kMIDIPropertyTransmitsProgramChanges, value ? 1 : 0);
			}
		}

		internal MidiDevice (MidiDeviceRef handle) : base (handle)
		{
		}

		internal MidiDevice (MidiDeviceRef handle, bool owns) : base (handle, owns)
		{
		}
#endif // !COREBUILD
	}

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

		public int AdvanceScheduleTimeMuSec {
			get {
				return GetInt (kMIDIPropertyAdvanceScheduleTimeMuSec);
			}
			set {
				SetInt (kMIDIPropertyAdvanceScheduleTimeMuSec, value);
			}
		}

		public int ConnectionUniqueIDInt {
			get {
				return GetInt (kMIDIPropertyConnectionUniqueID);
			}
			set {
				SetInt (kMIDIPropertyConnectionUniqueID, value);
			}
		}

		public NSData? ConnectionUniqueIDData {
			get {
				return GetData (kMIDIPropertyConnectionUniqueID);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetData (kMIDIPropertyConnectionUniqueID, value);
			}
		}

		public string? DisplayName {
			get {
				return GetString (kMIDIPropertyDisplayName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDisplayName, value);
			}
		}

		public string? DriverOwner {
			get {
				return GetString (kMIDIPropertyDriverOwner);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyDriverOwner, value);
			}
		}

		public int DriverVersion {
			get {
				return GetInt (kMIDIPropertyDriverVersion);
			}
			set {
				SetInt (kMIDIPropertyDriverVersion, value);
			}
		}

		public bool IsBroadcast {
			get {
				return GetInt (kMIDIPropertyIsBroadcast) != 0;
			}
			set {
				SetInt (kMIDIPropertyIsBroadcast, value ? 1 : 0);
			}
		}

		public string? Manufacturer {
			get {
				return GetString (kMIDIPropertyManufacturer);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyManufacturer, value);
			}
		}

		public int MaxSysExSpeed {
			get {
				try {
					return GetInt (kMIDIPropertyMaxSysExSpeed);
				} catch {
					// Some endpoints do not support this property
					// return the MIDI 1.0 default in those cases.
					return 3125;
				}
			}
			set {
				SetInt (kMIDIPropertyMaxSysExSpeed, value);
			}
		}

		public string? Name {
			get {
				return GetString (kMIDIPropertyName);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetString (kMIDIPropertyName, value);
			}
		}

		public NSDictionary? NameConfiguration {
			get {
				return GetDictionary (kMIDIPropertyNameConfiguration);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetDictionary (kMIDIPropertyNameConfiguration, value);
			}
		}

		public bool Offline {
			get {
				return GetInt (kMIDIPropertyOffline) != 0;
			}
			set {
				SetInt (kMIDIPropertyOffline, value ? 1 : 0);
			}
		}

		public bool Private {
			get {
				return GetInt (kMIDIPropertyPrivate) != 0;
			}
			set {
				SetInt (kMIDIPropertyPrivate, value ? 1 : 0);
			}
		}

		public int ReceiveChannels {
			get {
				return GetInt (kMIDIPropertyReceiveChannels);
			}
			set {
				SetInt (kMIDIPropertyReceiveChannels, value);
			}
		}

		public int TransmitChannels {
			get {
				return GetInt (kMIDIPropertyTransmitChannels);
			}
			set {
				SetInt (kMIDIPropertyTransmitChannels, value);
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
		public MidiObject? Parent { get; private set; }
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
		public MidiObject? MidiObject { get; private set; }
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
		public MidiDevice Device { get; set; }
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

		public IntPtr PacketListRaw {
			get {
				return packetList;
			}
		}

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
