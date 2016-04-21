using System;
using XamCore.CoreGraphics;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit
{
	public partial class NSTextContainer
	{
		[Availability (Obsoleted = Platform.Mac_10_11, Message = "Use NSTextContainer.FromSize instead")]
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

		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use NSTextContainer.FromSize instead")]
		public static NSTextContainer FromContainerSize (CGSize containerSize)
		{
			return new NSTextContainer (containerSize, true);
		}
	}
}

