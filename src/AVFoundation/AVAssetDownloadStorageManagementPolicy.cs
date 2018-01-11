#if IOS

using System;

using Foundation;
using ObjCRuntime;

namespace AVFoundation {
	public partial class AVAssetDownloadStorageManagementPolicy {

		[iOS (11,0)]
		[NoTV][NoMac][NoWatch]
		public virtual AVAssetDownloadedAssetEvictionPriority Priority {
			get { return AVAssetDownloadedAssetEvictionPriorityExtensions.GetValue (_Priority); }
			set { throw new NotImplementedException (); }
		}
	}

	public partial class AVMutableAssetDownloadStorageManagementPolicy {
		
		[iOS (11,0)]
		[NoTV][NoMac][NoWatch]
		public override AVAssetDownloadedAssetEvictionPriority Priority {
			get { return AVAssetDownloadedAssetEvictionPriorityExtensions.GetValue (_Priority); }
			set { _Priority = value.GetConstant (); }
		}
	}
}

#endif
