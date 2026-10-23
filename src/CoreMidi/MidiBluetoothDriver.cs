#if !TVOS
// MidiBluetoothDriver.cs
//
// Authors: TJ Lambert (TJ.Lambert@microsoft.com)
//
// Copyright 2022 Microsoft Corp.
//

#nullable enable

using CoreFoundation;

namespace CoreMidi {
	/// <summary>Provides access to the MIDI Bluetooth driver for managing Bluetooth MIDI connections.</summary>
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	public partial class MidiBluetoothDriver {
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreMidiLibrary)]
		static extern int MIDIBluetoothDriverActivateAllConnections ();

		/// <summary>Activates all Bluetooth MIDI connections.</summary>
		/// <returns>A status code indicating the result of the operation (0 for success).</returns>
		public static int ActivateAllConnections () => MIDIBluetoothDriverActivateAllConnections ();

		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreMidiLibrary)]
		static extern unsafe int MIDIBluetoothDriverDisconnect (/* CFStringRef */ NativeHandle uuid);

		/// <summary>Disconnects a Bluetooth MIDI device identified by its UUID.</summary>
		/// <param name="uuid">The UUID of the Bluetooth MIDI device to disconnect.</param>
		/// <returns>A status code indicating the result of the operation (0 for success).</returns>
		public static int Disconnect (NSString uuid)
		{
			int result = MIDIBluetoothDriverDisconnect (uuid.GetHandle ());
			GC.KeepAlive (uuid);
			return result;
		}
	}
}
#endif
