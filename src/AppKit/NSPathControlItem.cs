using System;
using Foundation;

namespace AppKit {
	public partial class NSPathControlItem 
#if !XAMCORE_3_0
	: INSCoding
#endif 
	{
#if XAMCORE_2_0 && !XAMCORE_3_0
		public NSPathControlItem (NSCoder coder) : this ()
		{
		}

		public virtual void EncodeTo (NSCoder coder)
		{
		}
#endif
	}
}

