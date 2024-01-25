#if !COREBUILD
using System;

using Foundation;

using ObjCRuntime;

namespace Foundation {
	public partial class NSProgress {
		//Manual bindings until BindAs support is merged
		public nint? EstimatedTimeRemaining {
			get { return _EstimatedTimeRemaining?.NIntValue; }
			set { _EstimatedTimeRemaining = value is not null ? new NSNumber (value.Value) : null; }
		}

		public nint? Throughput {
			get { return _Throughput?.NIntValue; }
			set { _Throughput = value is not null ? new NSNumber (value.Value) : null; }
		}

		public nint? FileTotalCount {
			get { return _FileTotalCount?.NIntValue; }
			set { _FileTotalCount = value is not null ? new NSNumber (value.Value) : null; }
		}

		public nint? FileCompletedCount {
			get { return _FileCompletedCount?.NIntValue; }
			set { _FileCompletedCount = value is not null ? new NSNumber (value.Value) : null; }
		}
	}
}
#endif
