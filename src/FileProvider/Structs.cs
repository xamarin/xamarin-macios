#if MONOMAC || IOS

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace FileProvider {

#if !(XAMCORE_5_0 && __MACCATALYST__)

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios16.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
#if __MACCATALYST__
	[EditorBrowsable (EditorBrowsableState.Never)]
#endif
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

#endif // !(XAMCORE_5_0 && __MACCATALYST__)

}
#endif
