#if !__MACCATALYST__
using System;
using Foundation;
using System.Runtime.Versioning;

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos10.10")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSPathControlItem 
#if !XAMCORE_3_0
	: INSCoding
#endif 
	{
#if !XAMCORE_3_0
		public NSPathControlItem (NSCoder coder) : this ()
		{
		}

		public virtual void EncodeTo (NSCoder coder)
		{
		}
#endif
	}
}
#endif // !__MACCATALYST__
