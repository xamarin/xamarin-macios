#if !WATCH

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace UIKit {
	static public partial class UIContentSizeCategoryExtensions {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		static extern nint /* NSComparisonResult */ UIContentSizeCategoryCompareToCategory (IntPtr /* NSString */ lhs, IntPtr /* NSString */ rhs);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static NSComparisonResult Compare (UIContentSizeCategory category1, UIContentSizeCategory category2)
		{
			var c1 = category1.GetConstant ();
			if (c1 is null)
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value", nameof (category1));

			var c2 = category2.GetConstant ();
			if (c2 is null)
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value", nameof (category2));

			return (NSComparisonResult) (long) UIContentSizeCategoryCompareToCategory (c1.Handle, c2.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool UIContentSizeCategoryIsAccessibilityCategory (IntPtr /* NSString */ category);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		static public bool IsAccessibilityCategory (this UIContentSizeCategory self)
		{
			var c1 = self.GetConstant ();
			if (c1 is null)
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value");

			return UIContentSizeCategoryIsAccessibilityCategory (c1.Handle);
		}
	}
}

#endif // !WATCH
