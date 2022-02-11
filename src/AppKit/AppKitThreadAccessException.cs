using System;
using System.Runtime.Versioning;

namespace AppKit
{
#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class AppKitThreadAccessException : Exception
	{
		public AppKitThreadAccessException() : base("AppKit Consistency error: you are calling a method that can only be invoked from the UI thread.")
		{
		}
	}
}
