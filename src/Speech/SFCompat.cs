#nullable enable

using System;
using Foundation;
using ObjCRuntime;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Speech {

	[Obsolete (Constants.ApiRemovedGeneral)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public unsafe partial class SFAnalysisContextTag {

		[Obsolete (Constants.ApiRemovedGeneral)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static NSString LeftContext => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);

		[Obsolete (Constants.ApiRemovedGeneral)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static NSString RightContext => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);

		[Obsolete (Constants.ApiRemovedGeneral)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static NSString SelectedText => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);
	}
}
