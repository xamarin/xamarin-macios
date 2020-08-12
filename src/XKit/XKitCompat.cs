#if !XAMCORE_4_0
using System;
using Foundation;
using ObjCRuntime;

#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

#if MONOMAC
namespace AppKit {
#else // macOS
namespace UIKit {
#endif // iOS, tvOS, WatchOS

#if !WATCH

#if !COREBUILD

	[iOS (7,0)]
	public partial class NSLayoutManager {
		[Obsolete ("Always throws NotSupportedException (). (not a public API).")]
		public virtual void ReplaceTextStorage (NSTextStorage newTextStorage)
			=> throw new NotSupportedException ();


		[Obsolete ("Always throws NotSupportedException (). (not a public API).")]
		public virtual void SetTemporaryAttributes (NSDictionary attrs, NSRange charRange)
			=> throw new NotSupportedException ();

	}
#endif // COREBUILD

#endif // WATCH

}
#endif
