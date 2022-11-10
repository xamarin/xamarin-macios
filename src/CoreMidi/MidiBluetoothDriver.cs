//
// MidiBluetoothDriver.cs
//
// Authors: TJ Lambert (TJ.Lambert@microsoft.com)
//
// Copyright 2022 Microsoft Corp.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreMidi {

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
#else
	[iOS (16, 0), Mac (13, 0), Watch (9, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif // NET
	public partial class MidiBluetoothDriver {
		[DllImport (Constants.CoreMidiLibrary)]
		static extern int MIDIBluetoothDriverActivateAllConnections ();

		public static int ActivateAllConnections () => MIDIBluetoothDriverActivateAllConnections ();

		[DllImport (Constants.CoreMidiLibrary)]
		static extern unsafe int MIDIBluetoothDriverDisconnect (/* CFStringRef* */ NativeHandle uuid);

		public static int Disconnect (NSString uuid) => MIDIBluetoothDriverDisconnect (uuid.GetHandle ());
	}
}
