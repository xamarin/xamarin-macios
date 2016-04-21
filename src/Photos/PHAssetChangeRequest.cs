using System;

namespace XamCore.Photos {

	public partial class PHAssetChangeRequest {

#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public PHAssetChangeRequest ()
		{
		}
#endif
	}
}
