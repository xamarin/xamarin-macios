#if !__MACCATALYST__ // there's a version in UIKit, use that one instead
using System;
using CoreGraphics;
using ObjCRuntime;

#nullable enable

namespace AppKit {
	public partial class NSTextContainer {
#if !NET
		[Obsoleted (PlatformName.MacOSX, 10, 11, message: "Use NSTextContainer.FromSize instead.")]
		public NSTextContainer (CGSize size)
		{
			Handle = InitWithContainerSize (size);
		}
#endif // !NET

		internal NSTextContainer (CGSize size, bool isContainer)
		{
			if (isContainer)
				Handle = InitWithContainerSize (size);
			else
				Handle = InitWithSize (size);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static NSTextContainer FromSize (CGSize size)
		{
			return new NSTextContainer (size, false);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Use NSTextContainer.FromSize instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use NSTextContainer.FromSize instead.")]
#endif
		public static NSTextContainer FromContainerSize (CGSize containerSize)
		{
			return new NSTextContainer (containerSize, true);
		}
	}
}
#endif // !__MACCATALYST__
