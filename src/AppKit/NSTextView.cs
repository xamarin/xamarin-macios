using System;

using Foundation;

#nullable enable

#if !__MACCATALYST__
namespace AppKit {
#if NET
	public partial class NSTextView {
		// this avoids:
		//  error MM4119: Could not register the selector 'changeColor:' of the member 'AppKit.NSTextView.ChangeColor' because the selector is already registered on the member 'AppKit.INSColorChanging.ChangeColor'.
		// this Export shouldn't ever matter, because this method is private, and it will only be really exported if someone overrides it, which can't happen.
		[Export ("xamarinDoNotExport:")]
		void INSColorChanging.ChangeColor (NSColorPanel? sender)
		{
			ChangeColor (sender);
		}
	}
#endif // NET
}
#endif // !__MACCATALYST__
