using System;

namespace AVFoundation {

	public partial class AVTextStyleRule {
#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public AVTextStyleRule ()
		{
		}
#endif
	}
}