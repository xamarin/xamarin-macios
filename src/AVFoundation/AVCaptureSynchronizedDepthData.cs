//
// Copyright 2019 Microsoft Corp.
//

using System;
using Foundation;

namespace AVFoundation {
#if !XAMCORE_4_0 && __IOS__
	public partial class AVCaptureSynchronizedDepthData {
		[Obsolete ("Default constructor should not be used")]
		public AVCaptureSynchronizedDepthData () : base (NSObjectFlag.Empty)
		{
		}
	}
#endif
}
