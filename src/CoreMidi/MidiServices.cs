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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using ObjCRuntime;
using CoreFoundation;

using MidiObjectRef = System.Int32;
using MidiClientRef = System.Int32;
using MidiDeviceRef = System.Int32;
using MidiDeviceListRef = System.Int32;
using MidiPortRef = System.Int32;
using MidiEndpointRef = System.Int32;
using MidiEntityRef = System.Int32;

namespace CoreMidi {

#if !TVOS
	// anonymous enum - MIDIServices.h
	/// <summary>Errors raised by the CoreMIDI stack.</summary>
	///     <remarks>
	///     </remarks>
	public enum MidiError : int {
		/// <summary>The operation completed successfully.</summary>
		Ok = 0,
		/// <summary>An invalid <see cref="MidiClient" /> was passed.</summary>
		InvalidClient = -10830,
		/// <summary>An invalid <see cref="MidiPort" /> was passed.</summary>
		InvalidPort = -10831,
		/// <summary>A source endpoint was passed to a function expecting a destination, or vice versa.</summary>
		WrongEndpointType = -10832,
		/// <summary>Attempt to close a non-existent connection.</summary>
		NoConnection = -10833,
		/// <summary>An invalid, unknown <see cref="MidiEndpoint" /> was passed.</summary>
		UnknownEndpoint = -10834,
		/// <summary>An attempt to query a property not set on the object.</summary>
		UnknownProperty = -10835,
		/// <summary>An attempt to set a property with a value of the wrong type (e.g., setting a string as an integer).</summary>
		WrongPropertyType = -10836,
		/// <summary>Internal error; there is no current MIDI setup.</summary>
		NoCurrentSetup = -10837,
		/// <summary>An error occurred while sending a MIDI message.</summary>
		MessageSendErr = -10838,
		/// <summary>Unable to start the MIDI server.</summary>
		ServerStartErr = -10839,
		/// <summary>An error occurred while reading the MIDI setup from persistent storage.</summary>
		SetupFormatErr = -10840,
		/// <summary>A driver is calling a non-I/O function from the server's MIDI I/O thread.</summary>
		WrongThread = -10841,
		/// <summary>The specified object does not exist.</summary>
		ObjectNotFound = -10842,
		/// <summary>The specified unique ID is not unique.</summary>
		IDNotUnique = -10843,
		/// <summary>The app does not have permission to use MIDI. Ensure your Info.plist includes the 'audio' key in UIBackgroundModes.</summary>
		NotPermitted = -10844,
	}
#endif // TVOS

	[Flags]
	// SInt32 - MIDIServices.h
	[NativeName ("MIDIObjectType")]
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

#if !TVOS
	public static partial class Midi {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		extern static void MIDIRestart ();

		/// <summary>Restarts the MIDI Subsystem.</summary>
		///         <remarks>This stops the MIDI subsystems and forces it to be reinitialized.</remarks>
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
		///         <remarks>Use the <see cref="CoreMidi.MidiEndpoint.GetSource(nint)" /> to fetch a specific source (represented by a MidiEndpoint).</remarks>
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
		///         <remarks>You can retrieve a specific MIDI device by using the <see cref="CoreMidi.Midi.GetExternalDevice(nint)" />.</remarks>
		public static nint ExternalDeviceCount {
			get {
				return MIDIGetNumberOfExternalDevices ();
			}
		}

		/// <summary>The number of MIDI devices in the system (do not confused with external connected devices).</summary>
		///         <value>
		///         </value>
		///         <remarks>You can retrieve a specific MIDI device by using the <see cref="CoreMidi.Midi.GetDevice(nint)" />.</remarks>
		public static nint DeviceCount {
			get {
				return MIDIGetNumberOfDevices ();
			}
		}
		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiDeviceRef MIDIGetExternalDevice (nint /* ItemCount = unsigned long */ item);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiDeviceRef MIDIGetDevice (nint /* ItemCount = unsigned long */ item);

		/// <param name="deviceIndex">The device index.</param>
		/// <summary>Returns an object representing the specified MIDI device.</summary>
		/// <returns>An instance of MidiDevice, or null on error.</returns>
		/// <remarks>
		///         </remarks>
		public static MidiDevice? GetDevice (nint deviceIndex)
		{
			var h = MIDIGetDevice (deviceIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (h);
		}

		/// <param name="deviceIndex">The external MIDI device index.</param>
		/// <summary>Returns an object representing the specified external MIDI device.</summary>
		/// <returns>An instance of MidiDevice, or null on error.</returns>
		/// <remarks>
		///         </remarks>
		public static MidiDevice? GetExternalDevice (nint deviceIndex)
		{
			var h = MIDIGetExternalDevice (deviceIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (h);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIExternalDeviceCreate (IntPtr /* CFStringRef */ name, IntPtr /* CFStringRef */ manufacturer, IntPtr /* CFStringRef */ model, MidiDeviceRef* outDevice);

		/// <summary>Create a new external MIDI device.</summary>
		/// <param name="name">The name for the new device.</param>
		/// <param name="manufacturer">The manufacturer for the new device.</param>
		/// <param name="model">The model for the new device.</param>
		/// <param name="status">A status code that describes the result of creating the new external device. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created external <see cref="MidiDevice" /> instance, null otherwise.</returns>
		public static MidiDevice? CreateExternalDevice (string name, string manufacturer, string model, out MidiError status)
		{
			using var namePtr = new TransientCFString (name);
			using var manufacturerPtr = new TransientCFString (manufacturer);
			using var modelPtr = new TransientCFString (model);

			var handle = default (MidiDeviceRef);
			unsafe {
				status = (MidiError) MIDIExternalDeviceCreate (namePtr, manufacturerPtr, modelPtr, &handle);
			}
			if (status != MidiError.Ok)
				return null;
			return new MidiDevice (handle);
		}
#endif // !COREBUILD
	}

	/// <summary>This class consists of functions that customize the global state of the MIDI system.</summary>
	public static class MidiSetup {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISetupAddDevice (MidiDeviceRef device);

		/// <summary>Add a device to the current MIDI setup.</summary>
		/// <param name="device">The device to add to the current MIDI setup.</param>
		/// <returns>A status code that describes the result of the operation. This will be <see cref="MidiError.Ok" /> in case of success.</returns>
		public static MidiError AddDevice (MidiDevice device)
		{
			return (MidiError) MIDISetupAddDevice (device.GetCheckedHandle ());
		}


		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISetupRemoveDevice (MidiDeviceRef device);

		/// <summary>Remove a device from the current MIDI setup.</summary>
		/// <param name="device">The device to remove from the current MIDI setup.</param>
		/// <returns>A status code that describes the result of the operation. This will be <see cref="MidiError.Ok" /> in case of success.</returns>
		public static MidiError RemoveDevice (MidiDevice device)
		{
			return (MidiError) MIDISetupRemoveDevice (device.GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISetupAddExternalDevice (MidiDeviceRef device);

		/// <summary>Add an external device to the current MIDI setup.</summary>
		/// <param name="device">The external device to add to the current MIDI setup.</param>
		/// <returns>A status code that describes the result of the operation. This will be <see cref="MidiError.Ok" /> in case of success.</returns>
		public static MidiError AddExternalDevice (MidiDevice device)
		{
			return (MidiError) MIDISetupAddExternalDevice (device.GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISetupRemoveExternalDevice (MidiDeviceRef device);

		/// <summary>Remove a device from the current MIDI setup.</summary>
		/// <param name="device">The device to remove from the current MIDI setup.</param>
		/// <returns>A status code that describes the result of the operation. This will be <see cref="MidiError.Ok" /> in case of success.</returns>
		public static MidiError RemoveExternalDevice (MidiDevice device)
		{
			return (MidiError) MIDISetupRemoveExternalDevice (device.GetCheckedHandle ());
		}
#endif // !COREBUILD
	}

	/// <summary>Base class for the <see cref="CoreMidi.MidiClient" />, <see cref="CoreMidi.MidiPort" />, <see cref="CoreMidi.MidiEntity" />, <see cref="CoreMidi.MidiDevice" /> and <see cref="CoreMidi.MidiEndpoint" /> classes.</summary>
	///     <remarks>
	///
	///       Provides the shared properties for the various Midi classes.
	///
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/CoreMidiSample/">CoreMidiSample</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
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

		internal MidiObjectRef GetCheckedHandle ()
		{
			if (handle == MidiObject.InvalidRef)
				throw new ObjectDisposedException ("handle");

			return handle;
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
			GC.KeepAlive (dict);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetDataProperty (MidiObjectRef obj, IntPtr str, IntPtr* data);

		/// <param name="property">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="property">To be added.</param>
		///         <param name="data">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetData (IntPtr property, NSData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			MIDIObjectSetDataProperty (handle, property, data.Handle);
			GC.KeepAlive (data);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetStringProperty (MidiObjectRef obj, IntPtr str, IntPtr* data);

		/// <param name="property">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="property">To be added.</param>
		///         <param name="value">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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
		/// <param name="property">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public MidiError RemoveProperty (string property)
		{
			using (var nsstr = new NSString (property)) {
				return MIDIObjectRemoveProperty (handle, nsstr.Handle);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIObjectGetProperties (MidiObjectRef obj, IntPtr* dict, byte deep);

		/// <param name="deep">Whether this should query properties of nested objects need to be included.</param>
		///         <summary>Returns the object properties as a dictionary.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="handle">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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

		/// <summary>Releases the resources used by the MidiObject object.</summary>
		///         <remarks>
		///           <para>The Dispose method releases the resources used by the MidiObject class.</para>
		///           <para>Calling the Dispose method when the application is finished using the MidiObject ensures that all external resources used by this managed object are released as soon as possible.  Once developers have invoked the Dispose method, the object is no longer useful and developers should no longer make any calls to it.  For more information on releasing resources see ``Cleaning up Unmananaged Resources'' at https://msdn.microsoft.com/en-us/library/498928w2.aspx</para>
		///         </remarks>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <include file="../../docs/api/CoreMidi/MidiObject.xml" path="/Documentation/Docs[@DocId='M:CoreMidi.MidiObject.Dispose(System.Boolean)']/*" />
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

		/// <param name="uniqueId">To be added.</param>
		///         <param name="result">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

	/// <summary>Exception raised by Midi methods.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class MidiException : Exception {
		internal MidiException (MidiError code) : base (code == MidiError.NotPermitted ? "NotPermitted, does your app Info.plist include the 'audio' key in the UIBackgroundModes section?" : code.ToString ())
		{
			ErrorCode = code;
		}

		/// <summary>Contains the underlying MIDI error code.</summary>
		/// <returns>The <see cref="MidiError" /> error code that caused this exception.</returns>
		public MidiError ErrorCode { get; private set; }
	}

	delegate void MidiNotifyProc (IntPtr message, IntPtr context);

	/// <include file="../../docs/api/CoreMidi/MidiClient.xml" path="/Documentation/Docs[@DocId='T:CoreMidi.MidiClient']/*" />
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class MidiClient : MidiObject {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIClientCreate (IntPtr str, delegate* unmanaged<IntPtr, IntPtr, void> callback, IntPtr context, MidiObjectRef* handle);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIClientDispose (MidiObjectRef handle);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0", "Call 'MIDISourceCreateWithProtocol' instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Call 'MIDISourceCreateWithProtocol' instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Call 'MIDISourceCreateWithProtocol' instead.")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDISourceCreate (MidiObjectRef handle, IntPtr name, MidiEndpointRef* endpoint);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISourceCreateWithProtocol (MidiClientRef client, IntPtr /* CFStringRef */ name, MidiProtocolId protocol, MidiEndpointRef* outSrc);

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

		/// <param name="name">Name for this client</param>
		///         <summary>Creates a new MidiClient.</summary>
		///         <remarks>
		///         </remarks>
		public MidiClient (string name)
		{
			using (var nsstr = new NSString (name)) {
				gch = GCHandle.Alloc (this);
				int code = 0;
				MidiObjectRef tempHandle;
				unsafe {
					code = MIDIClientCreate (nsstr.Handle, &ClientCallback, GCHandle.ToIntPtr (gch), &tempHandle);
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

		/// <summary>To be added.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public override string ToString ()
		{
			return Name;
		}

		/// <summary>Create a virtual source for this client.</summary>
		/// <param name="name">The name for the virtual source.</param>
		/// <param name="statusCode">A status code that describes the result of this operation. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created <see cref="MidiEndpoint" /> if successful, otherwise null.</returns>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0", "Call 'CreateVirtualSource (string, MidiProtocolId, out MidiError)' instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Call 'CreateVirtualSource (string, MidiProtocolId, out MidiError)' instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Call 'CreateVirtualSource (string, MidiProtocolId, out MidiError)' instead.")]
		public MidiEndpoint? CreateVirtualSource (string name, out MidiError statusCode)
		{
			using var namePtr = new TransientCFString (name);
			var endpointHandle = default (MidiEndpointRef);
			unsafe {
				statusCode = (MidiError) MIDISourceCreate (GetCheckedHandle (), namePtr, &endpointHandle);
			}
			if (statusCode != MidiError.Ok || endpointHandle == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (endpointHandle, true);
		}

		/// <summary>Create a virtual source for this client.</summary>
		/// <param name="name">The name for the virtual source.</param>
		/// <param name="protocol">The MIDI protocol for the data this source will produce.</param>
		/// <param name="status">A status code that describes the result of this operation. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created <see cref="MidiEndpoint" /> if successful, otherwise null.</returns>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public MidiEndpoint? CreateVirtualSource (string name, MidiProtocolId protocol, out MidiError status)
		{
			using var namePtr = new TransientCFString (name);
			var handle = default (MidiEndpointRef);
			unsafe {
				status = (MidiError) MIDISourceCreateWithProtocol (GetCheckedHandle (), namePtr, protocol, &handle);
			}
			if (status != MidiError.Ok || handle == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (handle, true);
		}

		/// <summary>Create a virtual destination for this client.</summary>
		/// <param name="name">The name for the virtual destination.</param>
		/// <param name="status">A status code that describes the result of this operation. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created <see cref="MidiEndpoint" /> if successful, otherwise null.</returns>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0", "Call the other 'CreateVirtualDestination' overload instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Call the other 'CreateVirtualDestination' overload instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Call the other 'CreateVirtualDestination' overload instead.")]
		public MidiEndpoint? CreateVirtualDestination (string name, out MidiError status)
		{
			var m = new MidiEndpoint (this, name, out status);
			if (status == MidiError.Ok)
				return m;

			m.Dispose ();
			return null;
		}

		/// <summary>A callback that receives incoming MIDI data on a virtual destination or input port created with a MIDI protocol.</summary>
		/// <param name="eventList">A pointer to the native <c>MIDIEventList</c> containing the incoming MIDI message(s). Use <see cref="MidiEventList(IntPtr)" /> to wrap it.</param>
		/// <param name="srcConnRefCon">A reference constant identifying the source connection, if any (passed to <see cref="MidiPort.ConnectSource(CoreMidi.MidiEndpoint)" />).</param>
		public delegate void MidiReceiveBlock (IntPtr eventList, IntPtr srcConnRefCon);

		/// <summary>Create a virtual destination for this client.</summary>
		/// <param name="name">The name for the virtual destination.</param>
		/// <param name="protocol">The MIDI protocol for the data this destination will receive.</param>
		/// <param name="readBlock">The callback that will be called when the destination receives MIDI data.</param>
		/// <param name="status">A status code that describes the result of this operation. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created <see cref="MidiEndpoint" /> if successful, otherwise null.</returns>
		/// <remarks>The <paramref name="readBlock" /> callback receives two pointers: the first is a pointer to the <c>MIDIEventList</c>, and the second is a pointer to the source <c>MIDIEndpointRef</c>. Use <see cref="MidiEventList(IntPtr)" /> to wrap the event list pointer.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe MidiEndpoint? CreateVirtualDestination (string name, MidiProtocolId protocol, MidiReceiveBlock readBlock, out MidiError status)
		{
			if (readBlock is null)
				throw new ArgumentNullException (nameof (readBlock));

			using var namePtr = new TransientCFString (name);
			var handle = default (MidiEndpointRef);
			delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &ReceiveBlockTrampoline;
			using var block = new BlockLiteral (trampoline, readBlock, typeof (MidiClient), nameof (ReceiveBlockTrampoline));
			var blockPtr = (BlockLiteral*) &block;
			status = (MidiError) MIDIDestinationCreateWithProtocol (GetCheckedHandle (), namePtr, protocol, &handle, blockPtr);
			if (status != MidiError.Ok || handle == MidiObject.InvalidRef)
				return null;
			return new MidiEndpoint (handle, name, true);
		}

		[UnmanagedCallersOnly]
		static unsafe void ReceiveBlockTrampoline (IntPtr block, IntPtr eventList, IntPtr srcConnRefCon)
		{
			var del = BlockLiteral.GetTarget<MidiReceiveBlock> (block);
			del?.Invoke (eventList, srcConnRefCon);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIDestinationCreateWithProtocol (MidiClientRef client, IntPtr /* CFStringRef */ name, MidiProtocolId protocol, MidiEndpointRef* outSrc, BlockLiteral* /* MIDIReceiveBlock */ readBlock);

		/// <param name="name">name for the input port.</param>
		///         <summary>Creates a new MIDI input port.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>The input port can be used to read events from MIDI, the returned MidiPort raises an event to notify you of new data available.</remarks>
		public MidiPort CreateInputPort (string name)
		{
			return new MidiPort (this, name, true);
		}

		/// <param name="name">name for the output port.</param>
		///         <summary>Creates a new MIDI output port.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>You can use an output MIDI port to transmit MIDI packets.</remarks>
		public MidiPort CreateOutputPort (string name)
		{
			return new MidiPort (this, name, false);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIInputPortCreateWithProtocol (
			MidiClientRef client,
			IntPtr /* CFStringRef */ name,
			MidiProtocolId protocol,
			MidiPortRef* outPort,
			BlockLiteral* /* MIDIReceiveBlock */ receiveBlock);

		/// <summary>Create a input port for this client.</summary>
		/// <param name="name">The name for the port.</param>
		/// <param name="protocol">The MIDI protocol for the data this port will receive.</param>
		/// <param name="readBlock">The callback that will be called when the port receives MIDI data.</param>
		/// <param name="status">A status code that describes the result of this operation. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created <see cref="MidiPort" /> if successful, otherwise null.</returns>
		/// <remarks>The <paramref name="readBlock" /> callback receives two pointers: the first is a pointer to the <c>MIDIEventList</c>, and the second is a pointer to the source <c>MIDIEndpointRef</c>. Use <see cref="MidiEventList(IntPtr)" /> to wrap the event list pointer.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public unsafe MidiPort? CreateInputPort (string name, MidiProtocolId protocol, MidiReceiveBlock readBlock, out MidiError status)
		{
			if (readBlock is null)
				throw new ArgumentNullException (nameof (readBlock));

			using var namePtr = new TransientCFString (name);
			var handle = default (MidiPortRef);
			delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &ReceiveBlockTrampoline;
			using var block = new BlockLiteral (trampoline, readBlock, typeof (MidiClient), nameof (ReceiveBlockTrampoline));
			var blockPtr = (BlockLiteral*) &block;
			status = (MidiError) MIDIInputPortCreateWithProtocol (GetCheckedHandle (), namePtr, protocol, &handle, blockPtr);
			if (status != MidiError.Ok || handle == MidiObject.InvalidRef)
				return null;
			return new MidiPort (handle, true, this, name, true);
		}

		/// <summary>Raised when the MIDI system's configuration changes, for example when devices are added, removed, or their properties change. The notification is coalesced and sent once after a series of changes.</summary>
		public event EventHandler? SetupChanged;
		/// <summary>Raised when an object (such as a device, entity, or endpoint) is added to the MIDI system.</summary>
		public event EventHandler<ObjectAddedOrRemovedEventArgs>? ObjectAdded;
		/// <summary>Raised when an object (such as a device, entity, or endpoint) is removed from the MIDI system.</summary>
		public event EventHandler<ObjectAddedOrRemovedEventArgs>? ObjectRemoved;
		/// <summary>Raised when a property of an object in the MIDI system changes.</summary>
		public event EventHandler<ObjectPropertyChangedEventArgs>? PropertyChanged;
		/// <summary>Raised when a MIDI thru connection in the system is created or destroyed.</summary>
		public event EventHandler? ThruConnectionsChanged;
		/// <summary>Raised when the owner of a serial port changes.</summary>
		public event EventHandler? SerialPortOwnerChanged;
		/// <summary>Raised when an I/O error occurs on a MIDI driver, for example when a device is disconnected.</summary>
		public event EventHandler<IOErrorEventArgs>? IOError;

		[UnmanagedCallersOnly]
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

		/// <inheritdoc />
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
	/// <summary>Encapsulates a series of MIDI events.</summary>
	///     <remarks>
	///       <para>
	/// 	When you consume a MidiPacket (because some data was received)
	/// 	you would use the Bytes property to get access to the
	/// 	underlying Midi data.  The actual number of valid bytes is
	/// 	stored in the Length property and you should not read beyond
	/// 	that point.
	///
	///       </para>
	///       <para>
	/// 	When you produce MidiPackets, you can either create MidiPacket
	/// 	instances by providing both an IntPtr and a Length parameter
	/// 	to your own buffers, or you can provide a byte array as well
	/// 	as a range within the array that determines where the Midi
	/// 	data is stored.
	///
	///       </para>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/CoreMidiSample/">CoreMidiSample</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
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

		/// <param name="timestamp">To be added.</param>
		///         <param name="length">To be added.</param>
		///         <param name="bytes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public MidiPacket (long timestamp, ushort length, IntPtr bytes)
		{
			TimeStamp = timestamp;
			Length = length;
			byteptr = bytes;
		}

		/// <summary>Create a new <see cref="MidiPacket" /> with the specified timestamp and MIDI data.</summary>
		/// <param name="timestamp">The timestamp for the packet.</param>
		/// <param name="bytes">The MIDI data for the packet.</param>
		public MidiPacket (long timestamp, byte [] bytes) : this (timestamp, bytes, 0, bytes is null ? 0 : bytes.Length, false)
		{
		}

		/// <summary>Create a new <see cref="MidiPacket" /> with the specified timestamp and a range of MIDI data.</summary>
		/// <param name="timestamp">The timestamp for the packet.</param>
		/// <param name="bytes">The byte array containing the MIDI data.</param>
		/// <param name="start">The starting index in <paramref name="bytes" /> for the MIDI data.</param>
		/// <param name="len">The number of bytes to include from <paramref name="bytes" />.</param>
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

		/// <summary>Releases the resources used by the MidiPacket object.</summary>
		///         <remarks>
		///           <para>The Dispose method releases the resources used by the MidiPacket class.</para>
		///           <para>Calling the Dispose method when the application is finished using the MidiPacket ensures that all external resources used by this managed object are released as soon as possible.  Once developers have invoked the Dispose method, the object is no longer useful and developers should no longer make any calls to it.  For more information on releasing resources see ``Cleaning up Unmananaged Resources'' at https://msdn.microsoft.com/en-us/library/498928w2.aspx</para>
		///         </remarks>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <include file="../../docs/api/CoreMidi/MidiPacket.xml" path="/Documentation/Docs[@DocId='M:CoreMidi.MidiPacket.Dispose(System.Boolean)']/*" />
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

	/// <summary>Input and Output ports.</summary>
	///     <remarks>
	///
	///       The input and output port objects are created by calling the
	///       <see cref="CoreMidi.MidiClient.CreateInputPort(System.String)" /> or <see cref="CoreMidi.MidiClient.CreateOutputPort(System.String)" /> methods.
	///
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/CoreMidiSample/">CoreMidiSample</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class MidiPort : MidiObject {
#if !COREBUILD
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
		[DllImport (Constants.CoreMidiLibrary)]
		extern unsafe static int /* OSStatus = SInt32 */ MIDIInputPortCreate (MidiClientRef client, IntPtr /* CFStringRef */ portName, delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> readProc, IntPtr context, MidiPortRef* midiPort);

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static int /* OSStatus = SInt32 */ MIDIOutputPortCreate (MidiClientRef client, IntPtr /* CFStringRef */ portName, MidiPortRef* midiPort);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIPortDispose (MidiPortRef port);

		GCHandle gch;
		bool input;

		internal MidiPort (MidiPortRef handle, bool owns, MidiClient client, string portName, bool input)
			: base (handle, owns)
		{
			Client = client;
			PortName = portName;
			this.input = input;
		}

		internal MidiPort (MidiClient client, string portName, bool input)
		{
			using (var nsstr = new NSString (portName)) {
				if (input)
					gch = GCHandle.Alloc (this);
				int code;

				MidiPortRef tempHandle;
				NativeHandle nsstrHandle = nsstr.Handle;
				if (input) {
					unsafe {
						code = MIDIInputPortCreate (client.handle, nsstrHandle, &Read, GCHandle.ToIntPtr (gch), &tempHandle);
					}
				} else {
					unsafe {
						code = MIDIOutputPortCreate (client.handle, nsstrHandle, &tempHandle);
					}
				}
				handle = tempHandle;

				if (code != 0) {
					if (gch.IsAllocated)
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

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			MessageReceived = null;
			base.Dispose (disposing);
		}

		/// <summary>Raised when MIDI messages are received on this input port from a connected source endpoint.</summary>
		/// <remarks>The received packets are provided in the event arguments. This event is only raised for input ports that have been connected to a source with <see cref="CoreMidi.MidiPort.ConnectSource(CoreMidi.MidiEndpoint)" />.</remarks>
		public event EventHandler<MidiPacketsEventArgs>? MessageReceived;

		[UnmanagedCallersOnly]
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

		/// <param name="endpoint">
		///         </param>
		///         <summary>Connects an input port to an endpoint.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>This method can be called multiple times to connect various endpoints to this port.</remarks>
		public MidiError ConnectSource (MidiEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			MidiError result = (MidiError) MIDIPortConnectSource (handle, endpoint.handle, GCHandle.ToIntPtr (gch));
			GC.KeepAlive (endpoint);
			return result;
		}

		/// <param name="endpoint">
		///         </param>
		///         <summary>Disconnects the port from the specified endpoint.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public MidiError Disconnect (MidiEndpoint endpoint)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			MidiError result = (MidiError) MIDIPortDisconnectSource (handle, endpoint.handle);
			GC.KeepAlive (endpoint);
			return result;
		}

		/// <summary>Returns a human-readable description of this port.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public override string ToString ()
		{
			return (input ? "[input:" : "[output:") + Client + ":" + PortName + "]";
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiError /* OSStatus = SInt32 */ MIDISend (MidiPortRef port, MidiEndpointRef endpoint, IntPtr packets);

		/// <param name="endpoint">Endpoint to send packets to.</param>
		///         <param name="packets">The packets to send to the endpoint.</param>
		///         <summary>Sends a set of MidiPackets to the specified endpoint.</summary>
		///         <returns>A status code</returns>
		///         <remarks>
		///         </remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
		public MidiError Send (MidiEndpoint endpoint, MidiPacket [] packets)
		{
			if (endpoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (endpoint));
			if (packets is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packets));
			var p = MidiPacket.CreatePacketList (packets);
			var code = MIDISend (handle, endpoint.handle, p);
			GC.KeepAlive (endpoint);
			Marshal.FreeHGlobal (p);
			return code;
		}
#endif // !COREBUILD
	}

	/// <summary>A <see cref="CoreMidi.MidiObject" /> that represents a sub-component of a <see cref="CoreMidi.MidiDevice" />.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
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

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public MidiProtocolId ProtocolId {
			get {
				return (MidiProtocolId) GetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID, (int) value);
			}
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public ushort UmpActiveGroupBitmap {
			get {
				return (ushort) GetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap, (int) value);
			}
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public bool UmpCanTransmitGroupless {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless) == 1;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless, value ? 1 : 0);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		extern static OSStatus MIDIEntityAddOrRemoveEndpoints (MidiEntityRef entity, nuint numSourceEndpoints, nuint numDestinationEndpoints);

		/// <summary>Sets the number of endpoints for this entity.</summary>
		/// <param name="numberOfSourceEndpoints">The number of source endpoints this entity will have.</param>
		/// <param name="numberOfDestinationEndpoints">The number of destination endpoints this entity will have.</param>
		/// <returns><see cref="MidiError.Ok" /> if successful, an error code otherwise.</returns>
		public MidiError AddOrRemoveEndpoints (nuint numberOfSourceEndpoints, nuint numberOfDestinationEndpoints)
		{
			return (MidiError) MIDIEntityAddOrRemoveEndpoints (GetCheckedHandle (), numberOfSourceEndpoints, numberOfDestinationEndpoints);
		}

#endif // !COREBUILD
	} // MidiEntity

	/// <summary>Represents a MIDI device (typically they represent a hardware device, but virtual devices also exist).   Devices can contain one or more entities.</summary>
	///     <remarks>
	///       <para>
	/// 	A single MIDI hardware device contains one or more entities.
	/// 	For example a single box could contain two independent MIDI tone
	/// 	generators, or a generator and a keyboard.
	///       </para>
	///       <para>
	/// 	To obtain a MidiDevice, use the <see cref="CoreMidi.Midi.GetDevice(nint)" /> or the <see cref="CoreMidi.Midi.GetExternalDevice(nint)" /> methods.
	///
	///       </para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class MidiDevice : MidiObject {
#if !COREBUILD
		[DllImport (Constants.CoreMidiLibrary)]
		extern static nint MIDIDeviceGetNumberOfEntities (MidiDeviceRef handle);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static MidiEntityRef MIDIDeviceGetEntity (MidiDeviceRef handle, nint item);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIDeviceNewEntity (MidiDeviceRef device, /* CFString */ IntPtr name, /* MIDIProtocolId */ MidiProtocolId protocol, byte embedded, /* ItemCount */ nuint numSourceEndpoints, /* ItemCount */ nuint numDestinationEndpoints, MidiEntityRef* newEntity);

		/// <summary>Create and add a new entity to an external device.</summary>
		/// <param name="name">The name for the new entity.</param>
		/// <param name="protocol">The protocol in use for the new entity.</param>
		/// <param name="embedded">Whether the new entity is inside the device (true), or if it consists of external connectors (false).</param>
		/// <param name="numberOfSourceEndpoints">The number of source endpoints in the new entity.</param>
		/// <param name="numberOfDestinationEndpoints">The number of destination endpoints in the new entity.</param>
		/// <param name="status">A status code that describes the result of creating the new entity. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created entity in case of success, null otherwise. In case of failure, <paramref name="status" /> will contain an error code.</returns>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public MidiEntity? CreateEntity (string name, MidiProtocolId protocol, bool embedded, nuint numberOfSourceEndpoints, nuint numberOfDestinationEndpoints, out MidiError status)
		{
			using var namePtr = new TransientCFString (name);
			var handle = default (MidiEntityRef);
			unsafe {
				status = (MidiError) MIDIDeviceNewEntity (GetCheckedHandle (), namePtr, protocol, embedded.AsByte (), numberOfSourceEndpoints, numberOfDestinationEndpoints, &handle);
			}
			if (status != MidiError.Ok || handle == MidiObject.InvalidRef)
				return null;
			return new MidiEntity (handle);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0", "Call 'CreateEntity' instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Call 'CreateEntity' instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Call 'CreateEntity' instead.")]
		[DllImport (Constants.CoreMidiLibrary)]
		extern static int MIDIDeviceAddEntity (MidiDeviceRef device, /* CFString */ IntPtr name, byte embedded, nuint numSourceEndpoints, nuint numDestinationEndpoints, MidiEntityRef newEntity);

		public MidiEntity? GetEntity (nint entityIndex)
		{
			var h = MIDIDeviceGetEntity (GetCheckedHandle (), entityIndex);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiEntity (h);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0", "Call 'CreateEntity' instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Call 'CreateEntity' instead.")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Call 'CreateEntity' instead.")]
		public int Add (string name, bool embedded, nuint numSourceEndpoints, nuint numDestinationEndpoints, MidiEntity newEntity)
		{
			using (NSString nsName = new NSString (name)) {
				var rv = MIDIDeviceAddEntity (GetCheckedHandle (), nsName.Handle, embedded ? (byte) 1 : (byte) 0, numSourceEndpoints, numDestinationEndpoints, newEntity.Handle);
				GC.KeepAlive (newEntity);
				return rv;
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreMidiLibrary)]
		extern static OSStatus MIDIDeviceRemoveEntity (MidiDeviceRef device, MidiEntityRef entity);

		/// <summary>Remove the specified entity from this device.</summary>
		/// <param name="entity">The entity to remove.</param>
		/// <returns><see cref="MidiError.Ok" /> if successful, an error code otherwise.</returns>
		public MidiError Remove (MidiEntity entity)
		{
			return (MidiError) MIDIDeviceRemoveEntity (GetCheckedHandle (), entity.GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIDeviceCreate (IntPtr /* MIDIDriverRef _nullable */ owner, IntPtr /* CFStringRef */ name, IntPtr /* CFStringRef */ manufacturer, IntPtr /* CFStringRef */ model, MidiDeviceRef* outDevice);

		/// <summary>Create a new external device corresponding to a specific piece of hardware.</summary>
		/// <param name="name">The name for the new device.</param>
		/// <param name="manufacturer">The manufacturer for the new device.</param>
		/// <param name="model">The model for the new device.</param>
		/// <param name="status">A status code that describes the result of creating the new device. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>A newly created <see cref="MidiDevice" /> instance, null otherwise.</returns>
		public static MidiDevice? Create (string name, string manufacturer, string model, out MidiError status)
		{
			var handle = default (MidiDeviceRef);
			using var namePtr = new TransientCFString (name);
			using var manufacturerPtr = new TransientCFString (manufacturer);
			using var modelPtr = new TransientCFString (model);

			unsafe {
				status = (MidiError) MIDIDeviceCreate (IntPtr.Zero, namePtr, manufacturerPtr, modelPtr, &handle);
			}

			if (status != MidiError.Ok || handle == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (handle);
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

		// kMIDIDriverPropertyUsesSerial doesn't exist on iOS or Mac Catalyst, but was accidentally exposed.
#if __MACOS__ || ((__MACCATALYST__ || __IOS__) && !XAMCORE_5_0)
		/// <summary>A value indicating whether the driver uses serial ports.</summary>
#if __MACCATALYST__ || __IOS__
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property only works on macOS.")]
#endif
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public bool UsesSerial {
			get {
#if __MACCATALYST__ || __IOS__
				return false;
#else
				return GetInt (MidiDriverPropertyExtensions.kMIDIDriverPropertyUsesSerial) != 0;
#endif
			}
			set {
#if !__MACCATALYST__ && !__IOS__
				SetInt (MidiDriverPropertyExtensions.kMIDIDriverPropertyUsesSerial, value ? 1 : 0);
#endif
			}
		}
#endif // __MACOS__ || ((__MACCATALYST__ || __IOS__) && !XAMCORE_5_0)

#if !XAMCORE_5_0 || __MACOS__
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
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
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
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

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
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

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
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

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
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

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public nuint GetNumberOfDevices ()
		{
			return MIDIDeviceListGetNumberOfDevices (GetCheckedHandle ());
		}

		public MidiDevice? Get (nuint index)
		{
			var h = MIDIDeviceListGetDevice (GetCheckedHandle (), index);
			if (h == MidiObject.InvalidRef)
				return null;
			return new MidiDevice (h);
		}

		/// <param name="device">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public int Add (MidiDevice device)
		{
			var rv = MIDIDeviceListAddDevice (GetCheckedHandle (), device.Handle);
			GC.KeepAlive (device);
			return rv;
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

	/// <summary>Endpoints represent an individual source or destination on the MIDI stream.</summary>
	///     <remarks>Physical endpoints are owned by <see cref="CoreMidi.MidiEntity" /> objects, virtual endpoints do not have an owning entity.</remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/CoreMidiSample/">CoreMidiSample</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class MidiEndpoint : MidiObject {
#if !COREBUILD
		GCHandle gch;

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIEndpointDispose (MidiEndpointRef handle);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
		[DllImport (Constants.CoreMidiLibrary)]
		extern unsafe static MidiError /* OSStatus = SInt32 */ MIDIDestinationCreate (MidiClientRef client, IntPtr /* CFStringRef */ name, delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> readProc, IntPtr context, MidiEndpointRef* midiEndpoint);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static int /* OSStatus = SInt32 */ MIDIFlushOutput (MidiEndpointRef handle);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
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

		internal MidiEndpoint (MidiClient client, string name, out MidiError status)
		{
			EndpointName = name;

			using var namePtr = new TransientCFString (name);
			var handle = default (MidiEndpointRef);
			gch = GCHandle.Alloc (this);
			unsafe {
				status = (MidiError) MIDIDestinationCreate (client.GetCheckedHandle (), namePtr, &Read, GCHandle.ToIntPtr (gch), &handle);
			}
			if (status != MidiError.Ok || handle == MidiObject.InvalidRef) {
				if (handle != MidiObject.InvalidRef)
					MIDIEndpointDispose (handle);
				gch.Free ();
				return;
			}
			this.handle = handle;
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			MessageReceived = null;
			base.Dispose (disposing);
		}

		/// <summary>Raised when MIDI messages are received on this virtual destination endpoint.</summary>
		/// <remarks>The received packets are provided in the event arguments. This event is raised for endpoints created as virtual destinations that other clients send MIDI to.</remarks>
		public event EventHandler<MidiPacketsEventArgs>? MessageReceived;

		[UnmanagedCallersOnly]
		static void Read (IntPtr packetList, IntPtr context, IntPtr srcPtr)
		{
			GCHandle gch = GCHandle.FromIntPtr (context);

			if (gch.Target is MidiEndpoint port) {
				var e = port.MessageReceived;
				if (e is not null)
					e (port, new MidiPacketsEventArgs (packetList));
			}
		}

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void FlushOutput ()
		{
			MIDIFlushOutput (handle);
		}

		/// <param name="packets">Packets received.</param>
		///         <summary>Broadcasts the packets to the client input ports which are connected to this source</summary>
		///         <returns>Status code of the operation</returns>
		///         <remarks>
		///         </remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos11.0")]
		[ObsoletedOSPlatform ("ios14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0")]
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

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public MidiProtocolId ProtocolId {
			get {
				return (MidiProtocolId) GetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyProtocolID, (int) value);
			}
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public ushort UmpActiveGroupBitmap {
			get {
				return (ushort) GetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPActiveGroupBitmap, (int) value);
			}
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public bool UmpCanTransmitGroupless {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless) == 1;
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyUMPCanTransmitGroupless, value ? 1 : 0);
			}
		}

		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[UnsupportedOSPlatform ("tvos")]
		public int AssociatedEndpoint {
			get {
				return GetInt (MidiPropertyExtensions.kMIDIPropertyAssociatedEndpoint);
			}
			set {
				SetInt (MidiPropertyExtensions.kMIDIPropertyAssociatedEndpoint, value);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIEndpointGetRefCons (MidiEndpointRef endpoint, IntPtr* ref1, IntPtr* ref2);

		[DllImport (Constants.CoreMidiLibrary)]
		extern static OSStatus MIDIEndpointSetRefCons (MidiEndpointRef endpoint, IntPtr ref1, IntPtr ref2);

		/// <summary>Get the refcons for this endpoint.</summary>
		/// <param name="ref1">Returns the first refcon if successful.</param>
		/// <param name="ref2">Returns the second refcon if successful.</param>
		/// <returns><see cref="MidiError.Ok" /> if successful, an error code otherwise.</returns>
		public MidiError GetRefCons (out IntPtr ref1, out IntPtr ref2)
		{
			ref1 = IntPtr.Zero;
			ref2 = IntPtr.Zero;
			unsafe {
				fixed (IntPtr* ref1Ptr = &ref1)
				fixed (IntPtr* ref2Ptr = &ref2) {
					return (MidiError) MIDIEndpointGetRefCons (GetCheckedHandle (), ref1Ptr, ref2Ptr);
				}
			}
		}

		/// <summary>Set the refcons for this endpoint.</summary>
		/// <param name="ref1">The first refcon.</param>
		/// <param name="ref2">The second refcon.</param>
		/// <returns><see cref="MidiError.Ok" /> if successful, an error code otherwise.</returns>
		public MidiError SetRefCons (IntPtr ref1, IntPtr ref2)
		{
			unsafe {
				return (MidiError) MIDIEndpointSetRefCons (GetCheckedHandle (), ref1, ref2);
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISendSysex (MidiSysexSendRequest* request);

		/// <summary>Asynchronously sends a single system-exclusive event.</summary>
		/// <param name="data">The data to send.</param>
		/// <param name="cancellationToken">An optional cancellation token that can be used to cancel the request.</param>
		/// <returns>A <see cref="MidiError" /> value for the request. This will be <see cref="MidiError.Ok" /> if the request was successful, an error code otherwise.</returns>
		public unsafe Task<MidiError> SendSysexAsync (byte [] data, CancellationToken? cancellationToken = null)
		{
			if (data is null)
				ThrowHelper.ThrowArgumentNullException (nameof (data));

			var tcs = new TaskCompletionSource<MidiError> (TaskCreationOptions.RunContinuationsAsynchronously);
			var request = new SysexRequest (this, data, tcs);
			MidiError rv;
			try {
				rv = (MidiError) MIDISendSysex (request.GetSysexRequestStruct (cancellationToken));
			} catch {
				request.Dispose ();
				throw;
			}
			if (rv != MidiError.Ok) {
				request.Dispose ();
				tcs.TrySetResult (rv);
			}

			return tcs.Task;
		}

		[DllImport (Constants.CoreMidiLibrary)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		unsafe extern static OSStatus MIDISendUMPSysex (MidiSysexSendRequestUmp* request);

		/// <summary>Asynchronously sends a single UMP system-exclusive event.</summary>
		/// <param name="data">The data to send.</param>
		/// <param name="cancellationToken">An optional cancellation token that can be used to cancel the request.</param>
		/// <returns>A <see cref="MidiError" /> value for the request. This will be <see cref="MidiError.Ok" /> if the request was successful, an error code otherwise.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public unsafe Task<MidiError> SendSysexUmpAsync (uint [] data, CancellationToken? cancellationToken = null)
		{
			if (data is null)
				ThrowHelper.ThrowArgumentNullException (nameof (data));

			var tcs = new TaskCompletionSource<MidiError> (TaskCreationOptions.RunContinuationsAsynchronously);
			var request = new SysexRequest (this, data, tcs);
			MidiError rv;
			try {
				rv = (MidiError) MIDISendUMPSysex (request.GetSysexUmpRequestStruct (cancellationToken));
			} catch {
				request.Dispose ();
				throw;
			}
			if (rv != MidiError.Ok) {
				request.Dispose ();
				tcs.TrySetResult (rv);
			}

			return tcs.Task;
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDISendUMPSysex8 (MidiSysexSendRequestUmp* request);

		/// <summary>Asynchronously sends a single 8-bit system-exclusive event.</summary>
		/// <param name="data">The data to send.</param>
		/// <param name="cancellationToken">An optional cancellation token that can be used to cancel the request.</param>
		/// <returns>A <see cref="MidiError" /> value for the request. This will be <see cref="MidiError.Ok" /> if the request was successful, an error code otherwise.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public unsafe Task<MidiError> SendSysexUmp8Async (uint [] data, CancellationToken? cancellationToken = null)
		{
			if (data is null)
				ThrowHelper.ThrowArgumentNullException (nameof (data));

			var tcs = new TaskCompletionSource<MidiError> (TaskCreationOptions.RunContinuationsAsynchronously);
			var request = new SysexRequest (this, data, tcs);
			MidiError rv;
			try {
				rv = (MidiError) MIDISendUMPSysex8 (request.GetSysexUmpRequestStruct (cancellationToken));
			} catch {
				request.Dispose ();
				throw;
			}
			if (rv != MidiError.Ok) {
				request.Dispose ();
				tcs.TrySetResult (rv);
			}

			return tcs.Task;
		}

		class SysexRequest : IDisposable {
			readonly object structPointerLock = new ();
			IntPtr structPointer;
			MidiEndpoint endpoint;
			byte []? byteData;
			uint []? uintData;
			GCHandle dataHandle;
			GCHandle thisHandle;
			TaskCompletionSource<MidiError> onCompletion;
			CancellationTokenRegistration? cancellationTokenRegistration;
			volatile bool wasCancelled;

			public SysexRequest (MidiEndpoint endpoint, byte [] data, TaskCompletionSource<MidiError> onCompletion)
			{
				this.endpoint = endpoint;
				this.byteData = data;
				this.onCompletion = onCompletion;

				unsafe {
					structPointer = Marshal.AllocHGlobal (sizeof (MidiSysexSendRequest));
					new Span<byte> ((void*) structPointer, sizeof (MidiSysexSendRequest)).Clear ();
				}
				dataHandle = GCHandle.Alloc (byteData, GCHandleType.Pinned);
				thisHandle = GCHandle.Alloc (this);
			}

			public SysexRequest (MidiEndpoint endpoint, uint [] data, TaskCompletionSource<MidiError> onCompletion)
			{
				this.endpoint = endpoint;
				this.uintData = data;
				this.onCompletion = onCompletion;

				unsafe {
					structPointer = Marshal.AllocHGlobal (sizeof (MidiSysexSendRequestUmp));
					new Span<byte> ((void*) structPointer, sizeof (MidiSysexSendRequestUmp)).Clear ();
				}
				dataHandle = GCHandle.Alloc (uintData, GCHandleType.Pinned);
				thisHandle = GCHandle.Alloc (this);
			}

			public unsafe MidiSysexSendRequest* GetSysexRequestStruct (CancellationToken? cancellationToken)
			{
				if (byteData is null)
					throw new InvalidOperationException ($"No byte[] data specified.");

				var rv = (MidiSysexSendRequest*) structPointer;

				rv->Destination = endpoint.GetCheckedHandle ();
				rv->Data = dataHandle.AddrOfPinnedObject ();
				rv->BytesToSend = (uint) byteData.Length;
				rv->CompletionProcedure = &SysexCompletion;
				rv->Context = GCHandle.ToIntPtr (thisHandle);

				cancellationTokenRegistration = cancellationToken?.Register (SysexCancellationRequest);

				return rv;
			}

			public unsafe MidiSysexSendRequestUmp* GetSysexUmpRequestStruct (CancellationToken? cancellationToken)
			{
				if (uintData is null)
					throw new InvalidOperationException ($"No uint[] data specified.");

				var rv = (MidiSysexSendRequestUmp*) structPointer;

				rv->Destination = endpoint.GetCheckedHandle ();
				rv->Words = dataHandle.AddrOfPinnedObject ();
				rv->WordsToSend = (uint) uintData.Length;
				rv->CompletionProcedure = &UmpSysexCompletion;
				rv->Context = GCHandle.ToIntPtr (thisHandle);

				cancellationTokenRegistration = cancellationToken?.Register (UmpSysexCancellationRequest);

				return rv;
			}

			void OnCompleted ()
			{
				if (wasCancelled) {
					onCompletion.TrySetCanceled ();
				} else {
					onCompletion.TrySetResult (MidiError.Ok);
				}
				Dispose ();
			}

			[UnmanagedCallersOnly]
			unsafe static void SysexCompletion (MidiSysexSendRequest* request)
			{
				var obj = (SysexRequest?) GCHandle.FromIntPtr (request->Context).Target;
				obj?.OnCompleted ();
			}

			[UnmanagedCallersOnly]
			unsafe static void UmpSysexCompletion (MidiSysexSendRequestUmp* request)
			{
				var obj = (SysexRequest?) GCHandle.FromIntPtr (request->Context).Target;
				obj?.OnCompleted ();
			}

			unsafe void SysexCancellationRequest ()
			{
				wasCancelled = true;
				lock (structPointerLock) {
					var rv = (MidiSysexSendRequest*) structPointer;
					if (rv is null)
						return;
					rv->Complete = true;
				}
			}

			unsafe void UmpSysexCancellationRequest ()
			{
				wasCancelled = true;
				lock (structPointerLock) {
					var rv = (MidiSysexSendRequestUmp*) structPointer;
					if (rv is null)
						return;
					rv->Complete = true;
				}
			}

			public void Dispose ()
			{
				cancellationTokenRegistration?.Dispose ();
				cancellationTokenRegistration = null;
				lock (structPointerLock) {
					var ptr = structPointer;
					structPointer = IntPtr.Zero;
					if (ptr != IntPtr.Zero)
						Marshal.FreeHGlobal (ptr);
				}
				if (dataHandle.IsAllocated)
					dataHandle.Free ();
				if (thisHandle.IsAllocated)
					thisHandle.Free ();
				GC.SuppressFinalize (this);
			}

			~SysexRequest ()
			{
				Dispose ();
			}
		}

		// MidiEndpoint 
#endif // !COREBUILD
	}
#endif // TVOS


	// SInt32 - MIDIServices.h
	[NativeName ("MIDINotificationMessageID")]
	enum MidiNotificationMessageId : int {
		SetupChanged = 1,
		ObjectAdded,
		ObjectRemoved,
		PropertyChanged,
		ThruConnectionsChanged,
		SerialPortOwnerChanged,
		IOError,
#if !MONOMAC
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		InternalStart = 0x1000,
#endif
	}

#if !TVOS
	//
	// The notification EventArgs
	//
	/// <summary>Provides data for the <see cref="CoreMidi.MidiClient.ObjectAdded" /> and <see cref="CoreMidi.MidiClient.ObjectRemoved" /> events.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class ObjectAddedOrRemovedEventArgs : EventArgs {
		/// <param name="parent">
		///         </param>
		///         <param name="child">
		///         </param>
		///         <summary>Initializes a new instance of the ObjectAddedOrRemovedEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
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

	/// <summary>Provides data for the <see cref="CoreMidi.MidiClient.PropertyChanged" /> event.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class ObjectPropertyChangedEventArgs : EventArgs {
		/// <param name="midiObject">The MIDI object whose property has changed.</param>
		///         <param name="propertyName">The name of the MIDI property that changed.</param>
		///         <summary>Initializes a new instance of the ObjectPropertyChangedEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
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

	/// <summary>Provides data for the <see cref="CoreMidi.MidiClient.IOError" /> event.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	public class IOErrorEventArgs : EventArgs {
		/// <param name="device">The device that triggered the error.</param>
		///         <param name="errorCode">OSStatus error code</param>
		///         <summary>Initializes a new instance of the IOErrorEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
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

	/// <summary>Provides data for the <see cref="CoreMidi.MidiPort.MessageReceived" /> and <see cref="CoreMidi.MidiEndpoint.MessageReceived" /> events.</summary>
	///     <remarks>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/CoreMidiSample/">CoreMidiSample</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
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

		/// <summary>Releases the resources used by the MidiPacketsEventArgs object.</summary>
		///         <remarks>
		///           <para>The Dispose method releases the resources used by the MidiPacketsEventArgs class.</para>
		///           <para>Calling the Dispose method when the application is finished using the MidiPacketsEventArgs ensures that all external resources used by this managed object are released as soon as possible.  Once developers have invoked the Dispose method, the object is no longer useful and developers should no longer make any calls to it.  For more information on releasing resources see ``Cleaning up Unmananaged Resources'' at https://msdn.microsoft.com/en-us/library/498928w2.aspx</para>
		///         </remarks>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <include file="../../docs/api/CoreMidi/MidiPacketsEventArgs.xml" path="/Documentation/Docs[@DocId='M:CoreMidi.MidiPacketsEventArgs.Dispose(System.Boolean)']/*" />
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
#endif // TVOS
}
