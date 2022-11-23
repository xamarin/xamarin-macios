#nullable enable

using System;

namespace Photos {

	public partial class PHChangeRequest {
#if !XAMCORE_3_0
		// This constructor is required for the default constructor in PHAssetChangeRequest to compile.
		internal PHChangeRequest ()
		{
		}
#endif
	}
}
