using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;


namespace CoreMidi {

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0), Mac (11,0), Watch (8,0), TV (14,0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiEventList
	{
#if !COREBUILD
		public MidiProtocolId protocol;
#endif
		public uint NumPackets;
		public MidiEventPacket[] packet;
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0), Mac (11,0), Watch (8,0), TV (14,0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiEventPacket
	{
		public ulong TimeStamp;
		public uint WordCount;
		public uint[] Words;
	}

}
