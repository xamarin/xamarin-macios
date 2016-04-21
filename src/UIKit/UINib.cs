#if !XAMCORE_2_0

using System;
using XamCore.Foundation;

namespace XamCore.UIKit {
	
	public partial class UINib {

		[Obsolete ("Use Instantiate method")]
		public NSObject[] InstantiateWithOwneroptions (NSObject ownerOrNil, NSDictionary optionsOrNil)
		{
			return Instantiate (ownerOrNil, optionsOrNil);
		}
	}
}

#endif
