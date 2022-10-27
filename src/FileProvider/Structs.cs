#if MONOMAC

#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace FileProvider {

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("maccatalyst")]
#else
	[NoiOS]
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
