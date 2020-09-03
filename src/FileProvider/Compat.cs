#if !XAMCORE_4_0

using System;
using ObjCRuntime;

namespace FileProvider {

	[iOS (13,0)]
	[Obsoleted (PlatformName.iOS, 14,0)]
	public interface INSFileProviderItemDecorating : INSFileProviderItem {
	}

	[iOS (13,0)][Obsoleted (PlatformName.iOS, 14,0)]
	[Mac (10,15)][Obsoleted (PlatformName.MacOSX, 11,0)]
	public interface INSFileProviderItemFlags : INativeObject, IDisposable {
		bool Hidden { get; }
		bool PathExtensionHidden { get; }
		bool UserExecutable { get; }
		bool UserReadable { get; }
		bool UserWritable { get; }
	}
}

#endif
