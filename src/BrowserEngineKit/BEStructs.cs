using System.Diagnostics.CodeAnalysis;

namespace BrowserEngineKit {
#if IOS || MACCATALYST || TVOS
	[SupportedOSPlatform ("ios17.4")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos17.4")]
	[UnsupportedOSPlatform ("macos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct BEDirectionalTextRange {
		public nint Offset;
		public nint Length;
	}
#endif // __IOS__ || __MACCATALYST__
}
