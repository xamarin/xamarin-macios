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
			set
			{
				var val = value.GetConstant ();
				if (val is not null)
					_Priority = val;
			}
		}
	}
}

#endif
