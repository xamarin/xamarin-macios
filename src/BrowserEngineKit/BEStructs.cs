#if NET
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace BrowserEngineKit {
#if IOS || MACCATALYST || TVOS
	[SupportedOSPlatform ("ios17.4")]
	[SupportedOSPlatform ("maccatalyst17.4")]
	[SupportedOSPlatform ("tvos17.4")]
	[UnsupportedOSPlatform ("macos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct BEDirectionalTextRange {
		public nint Offset;
		public nint Length;
	}
#endif // __IOS__ || __MACCATALYST__
}
#endif // NET
