//
// Copyright 2015 Xamarin Inc
//

using System;
using XamCore.Foundation;

namespace XamCore.AVFoundation {
#if MONOMAC && !XAMCORE_3_0
	public partial class AVFragmentedAssetTrack {
		[Obsolete ("Default constructor should not be used")]
		public AVFragmentedAssetTrack () : base (NSObjectFlag.Empty)
		{
		}
	}
#endif
}
