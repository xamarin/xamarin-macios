#if !XAMCORE_5_0 && __MACOS__

using System;
using System.ComponentModel;

using Foundation;
using AudioToolbox;

#nullable enable

namespace AVFoundation {
	public partial class AVSampleCursor {
		[Obsolete ("Use 'CurrentSampleDependencyInfo2' instead. The property type of this property is wrong.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public virtual AVSampleCursorSyncInfo CurrentSampleDependencyInfo {
			get => throw new NotSupportedException ("Use 'CurrentSampleDependencyInfo2' instead. The property type of this property is wrong.");
		}
	}
}

#endif // !XAMCORE_5_0 && __MACOS__
