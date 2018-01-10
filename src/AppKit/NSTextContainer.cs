using System;
using CoreGraphics;
using ObjCRuntime;

namespace AppKit
{
	public partial class NSTextContainer
	{
		[Obsoleted (PlatformName.MacOSX, 10, 11, message : "Use NSTextContainer.FromSize instead.")]
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

		[Mac (10,11)]
		public static NSTextContainer FromSize (CGSize size)
		{
			return new NSTextContainer (size, false);
		}

		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use NSTextContainer.FromSize instead.")]
		public static NSTextContainer FromContainerSize (CGSize containerSize)
		{
			return new NSTextContainer (containerSize, true);
		}
	}
}

