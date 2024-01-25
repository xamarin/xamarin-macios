//
// Copyright 2019 Microsoft Corp.
//

using System;

using Foundation;

#nullable enable

namespace AVFoundation {
#if !NET && __IOS__
	public partial class AVCaptureSynchronizedDepthData {
		[Obsolete ("Default constructor should not be used")]
		public AVCaptureSynchronizedDepthData () : base (NSObjectFlag.Empty)
		{
		}
	}
#endif
}
