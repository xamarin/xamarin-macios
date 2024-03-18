using System;
using System.Runtime.Versioning;

#nullable enable

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public class AppKitThreadAccessException : Exception {
		public AppKitThreadAccessException () : base ("AppKit Consistency error: you are calling a method that can only be invoked from the UI thread.")
		{
		}
	}
}
