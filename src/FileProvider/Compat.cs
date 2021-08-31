#if !XAMCORE_4_0 && !__MACCATALYST__

using System;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace FileProvider {

#if MONOMAC
	public static partial class NSFileProviderItem_Extensions {

#if !NET
		[Obsolete ("This API was removed from macOS in Xcode 12.2 beta 2.")]
#else
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[Obsolete ("This API was removed from macOS in Xcode 12.2 beta 2.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static Foundation.NSNumber GetFavoriteRank (this INSFileProviderItem This)
		{
			throw new NotSupportedException ();
		}
	}
#else
#if !NET
	[iOS (13,0)]
	[Obsoleted (PlatformName.iOS, 14,0)]
#else
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("ios14.0")]
#endif
	public interface INSFileProviderItemDecorating : INSFileProviderItem {
	}
#endif

#if !NET
	[iOS (13,0)][Obsoleted (PlatformName.iOS, 14,0)]
	[Mac (10,15)][Obsoleted (PlatformName.MacOSX, 11,0)]
#else
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[UnsupportedOSPlatform ("ios14.0")]
	[UnsupportedOSPlatform ("macos11.0")]
#endif
	public interface INSFileProviderItemFlags : INativeObject, IDisposable {
		bool Hidden { get; }
		bool PathExtensionHidden { get; }
		bool UserExecutable { get; }
		bool UserReadable { get; }
		bool UserWritable { get; }
	}
}

#endif
