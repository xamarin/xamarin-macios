#if IOS

using System;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {
	public partial class AVAssetDownloadStorageManagementPolicy {

		public virtual AVAssetDownloadedAssetEvictionPriority Priority {
			get { return AVAssetDownloadedAssetEvictionPriorityExtensions.GetValue (_Priority); }
			set { throw new NotImplementedException (); }
		}
	}

	public partial class AVMutableAssetDownloadStorageManagementPolicy {
		
		public override AVAssetDownloadedAssetEvictionPriority Priority {
			get { return AVAssetDownloadedAssetEvictionPriorityExtensions.GetValue (_Priority); }
			set { _Priority = value.GetConstant () ?? throw new ArgumentOutOfRangeException (nameof (Priority)); }
		}
	}
}

#endif
