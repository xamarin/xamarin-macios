#if !__MACCATALYST__
using System;
using Foundation;
using CoreGraphics;

#nullable enable

namespace AppKit {

	public partial class NSComboBox {
#if !NET
		[Obsolete ("Use GetItemObject instead.")]
		public virtual NSComboBox GetItem (nint index)
		{
			return (NSComboBox) GetItemObject (index);
		}
#endif

		public NSObject this [nint index] {
			get {
				return GetItemObject (index);
			}
		}
	}
}
#endif // !__MACCATALYST__
