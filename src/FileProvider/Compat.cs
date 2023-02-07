#if !NET && !__MACCATALYST__

using System;
using ObjCRuntime;

#nullable enable

namespace FileProvider {

#if MONOMAC
	public static partial class NSFileProviderItem_Extensions {

		[Obsolete ("This API was removed from macOS in Xcode 12.2 beta 2.")]
		public static Foundation.NSNumber GetFavoriteRank (this INSFileProviderItem This)
		{
			throw new NotSupportedException ();
		}
	}
#else
	[iOS (13, 0)]
	[Obsoleted (PlatformName.iOS, 14, 0)]
	public interface INSFileProviderItemDecorating : INSFileProviderItem {
	}
#endif

	[iOS (13, 0)]
	[Obsoleted (PlatformName.iOS, 14, 0)]
	[Obsoleted (PlatformName.MacOSX, 11, 0)]
	public interface INSFileProviderItemFlags : INativeObject, IDisposable {
		bool Hidden { get; }
		bool PathExtensionHidden { get; }
		bool UserExecutable { get; }
		bool UserReadable { get; }
		bool UserWritable { get; }
	}
}

#endif
