#nullable enable

#if !NET
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

	public partial class NSLayoutManager {
		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public virtual void ReplaceTextStorage (NSTextStorage newTextStorage)
			=> throw new NotSupportedException ();

#if !MONOMAC
		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public virtual void SetTemporaryAttributes (Foundation.NSDictionary<Foundation.NSString, Foundation.NSObject> attributes, Foundation.NSRange characterReange)
			=> throw new NotSupportedException ();
#endif

	}
#endif // COREBUILD

#endif // WATCH

}
#endif // NET
