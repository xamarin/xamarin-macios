using System;
using Foundation;
using CoreGraphics;

namespace AppKit {

	public partial class NSComboBox {
		[Obsolete ("Use GetItemObject instead.")]
		public virtual NSComboBox GetItem (nint index)
		{
			return (NSComboBox) GetItemObject (index);
		}

		public NSObject this [nint index] { 
			get {
				return GetItemObject (index);
			}
		}
	}
}
