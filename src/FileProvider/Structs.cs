#if MONOMAC
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ObjCRuntime;

namespace FileProvider {

#if !NET
	[NoiOS, NoMacCatalyst, Mac (12,0)]
#else
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos12.0")]
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
