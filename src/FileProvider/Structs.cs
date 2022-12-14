#if MONOMAC || IOS

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace FileProvider {

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios16.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[iOS (16,0)]
	[NoMacCatalyst]
	[Mac (12,0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct NSFileProviderTypeAndCreator
	{
		public uint Type;
		public uint Creator;

#if !COREBUILD
		public string GetTypeAsFourCC ()
			=> Runtime.ToFourCCString (Type);

		public string GetCreatorAsFourCC()
			=> Runtime.ToFourCCString (Creator);
#endif
	}

}
#endif
