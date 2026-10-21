#if !TVOS
//
// MidiBluetoothDriver.cs
//
// Authors: TJ Lambert (TJ.Lambert@microsoft.com)
//
// Copyright 2022 Microsoft Corp.
//

#nullable enable

using CoreFoundation;

namespace CoreMidi {
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	public partial class MidiBluetoothDriver {
		[DllImport (Constants.CoreMidiLibrary)]
		static extern int MIDIBluetoothDriverActivateAllConnections ();

		public static int ActivateAllConnections () => MIDIBluetoothDriverActivateAllConnections ();

		[DllImport (Constants.CoreMidiLibrary)]
		static extern unsafe int MIDIBluetoothDriverDisconnect (/* CFStringRef* */ NativeHandle uuid);

		public static int Disconnect (NSString uuid)
		{
			int result = MIDIBluetoothDriverDisconnect (uuid.GetHandle ());
			GC.KeepAlive (uuid);
			return result;
		}
	}
}
#endif
