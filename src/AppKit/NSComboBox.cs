using System;
using XamCore.Foundation;
using XamCore.CoreGraphics;

namespace XamCore.AppKit {

	public partial class NSComboBox {
		[Obsolete ("Use GetItemObject instead")]
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
