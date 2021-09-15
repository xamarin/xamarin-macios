#if !__MACCATALYST__ // there's a version in UIKit, use that one instead
using System;
using CoreGraphics;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace AppKit
{
	public partial class NSTextContainer
	{
#if !NET
		[Obsoleted (PlatformName.MacOSX, 10, 11, message : "Use NSTextContainer.FromSize instead.")]
#else
#if MONOMAC
		[Obsolete ("Starting with macos10.11 use NSTextContainer.FromSize instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public NSTextContainer (CGSize size)
		{
			Handle = InitWithContainerSize (size);
		}

		internal NSTextContainer (CGSize size, bool isContainer)
		{
			if (isContainer)
				Handle = InitWithContainerSize (size);
			else
				Handle = InitWithSize (size);
		}

#if !NET
		[Mac (10,11)]
#endif
		public static NSTextContainer FromSize (CGSize size)
		{
			return new NSTextContainer (size, false);
		}

#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use NSTextContainer.FromSize instead.")]
#else
		[UnsupportedOSPlatform ("macos10.11")]
#if MONOMAC
		[Obsolete ("Starting with macos10.11 use NSTextContainer.FromSize instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public static NSTextContainer FromContainerSize (CGSize containerSize)
		{
			return new NSTextContainer (containerSize, true);
		}
	}
}
#endif // !__MACCATALYST__
