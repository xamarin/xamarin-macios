#if !TVOS
// 
// MidiThruConnection.cs
//
// Authors: Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2016 Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

using MidiThruConnectionRef = System.UInt32;
using System.Runtime.Versioning;

namespace CoreMidi {
#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiThruConnection : IDisposable {
		MidiThruConnectionRef handle;
		const MidiThruConnectionRef InvalidRef = 0;

		protected internal MidiThruConnection (MidiThruConnectionRef handle)
		{
			this.handle = handle;
		}

		/// <summary>Handle (pointer) to the unmanaged object representation.</summary>
		///         <value>A pointer</value>
		///         <remarks>This IntPtr is a handle to the underlying unmanaged representation for this object.</remarks>
		public MidiThruConnectionRef Handle {
			get { return handle; }
		}

		~MidiThruConnection ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		static extern /* OSStatus */ MidiError MIDIThruConnectionDispose (
			/* MIDIThruConnectionRef* */ MidiThruConnectionRef connection);

		protected virtual void Dispose (bool disposing)
		{
			if (handle != InvalidRef) {
				MIDIThruConnectionDispose (handle);
				handle = InvalidRef;
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe static extern /* OSStatus */ MidiError MIDIThruConnectionCreate (
			/* CFStringRef */ IntPtr inPersistentOwnerID, /* can be null */
			/* CFDataRef */ IntPtr inConnectionParams,
			/* MIDIThruConnectionRef* */ MidiThruConnectionRef* outConnection);

		public static MidiThruConnection? Create (string persistentOwnerID, MidiThruConnectionParams connectionParams, out MidiError error)
		{
			MidiThruConnectionRef ret;

			using (var data = connectionParams.WriteStruct ()) {
				using var retStr = new TransientCFString (persistentOwnerID);
				unsafe {
					error = MIDIThruConnectionCreate (retStr, data.Handle, &ret);
				}
			}

			if (error != MidiError.Ok)
				return null;

			return new MidiThruConnection (ret);
		}

		public static MidiThruConnection? Create (string persistentOwnerID, MidiThruConnectionParams connectionParams)
		{
			MidiError error;
			return Create (persistentOwnerID, connectionParams, out error);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe static extern /* OSStatus */ MidiError MIDIThruConnectionGetParams (
			/* MIDIThruConnectionRef* */ MidiThruConnectionRef connection,
			/* CFDataRef */ IntPtr* outConnectionParams);

		public MidiThruConnectionParams? GetParams (out MidiError error)
		{
			if (Handle == InvalidRef)
				throw new ObjectDisposedException ("MidiThruConnection");

			IntPtr ret;
			unsafe {
				error = MIDIThruConnectionGetParams (Handle, &ret);
			}
			if (error != MidiError.Ok || ret == IntPtr.Zero)
				return null;
			using (var data = Runtime.GetNSObject<NSData> (ret, true)) {
				if (data is null)
					return null;
				var cnnParams = new MidiThruConnectionParams ();
				cnnParams.ReadStruct (data);
				return cnnParams;
			}
		}

		public MidiThruConnectionParams? GetParams ()
		{
			MidiError error;
			return GetParams (out error);
		}

		[DllImport (Constants.CoreMidiLibrary)]
		static extern /* OSStatus */ MidiError MIDIThruConnectionSetParams (
			/* MIDIThruConnectionRef* */ MidiThruConnectionRef connection,
			/* CFDataRef */ IntPtr inConnectionParams);

		public MidiError SetParams (MidiThruConnectionParams connectionParams)
		{
			if (Handle == InvalidRef)
				throw new ObjectDisposedException ("MidiThruConnection");
			if (connectionParams is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (connectionParams));

			using (var data = connectionParams.WriteStruct ()) {
				var error = MIDIThruConnectionSetParams (Handle, data.Handle);
				return error;
			}
		}

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe static extern /* OSStatus */ MidiError MIDIThruConnectionFind (
			/* CFStringRef* */ IntPtr inPersistentOwnerID,
			/* CFDataRef */ IntPtr* outConnectionList);

		public static MidiThruConnection []? Find (string persistentOwnerID, out MidiError error)
		{
			if (persistentOwnerID is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (persistentOwnerID));

			IntPtr ret;
			using var persistentOwnerIDHandle = new TransientCFString (persistentOwnerID);
			unsafe {
				error = MIDIThruConnectionFind (persistentOwnerIDHandle, &ret);
			}
			using (var data = Runtime.GetNSObject<NSData> (ret)) {
				if (data is null)
					return null;
				var typeSize = Marshal.SizeOf<MidiThruConnectionRef> ();
				var totalObjs = (int) data.Length / typeSize;
				if (totalObjs == 0)
					return null;

				var connections = new MidiThruConnection [totalObjs];
				unsafe {
					uint* handles = (uint*) (IntPtr) data.Bytes;
					for (int i = 0; i < totalObjs; i++) {
						connections [i] = new MidiThruConnection (handles [i]);
					}
				}

				return connections;
			}
		}

		public static MidiThruConnection []? Find (string persistentOwnerID)
		{
			MidiError error;
			return Find (persistentOwnerID, out error);
		}
	}
#endif // !COREBUILD
}
#endif
