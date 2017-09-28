#if !WATCH

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.UIKit {
	static public partial class UIContentSizeCategoryExtensions {

		[DllImport (Constants.UIKitLibrary)]
		static extern nint /* NSComparisonResult */ UIContentSizeCategoryCompareToCategory (IntPtr /* NSString */ lhs, IntPtr /* NSString */ rhs);

		[iOS (11, 0), TV (11, 0)]
		public static NSComparisonResult Compare (UIContentSizeCategory category1, UIContentSizeCategory category2)
		{
			if (!UIContentSizeCategory.IsDefined (typeof (UIContentSizeCategory), category1))
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value", nameof (category1));

			if (!UIContentSizeCategory.IsDefined (typeof (UIContentSizeCategory), category2))
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value", nameof (category2));

			return (NSComparisonResult)(long)UIContentSizeCategoryCompareToCategory (category1.GetConstant ().Handle, category2.GetConstant ().Handle);
		}

		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIContentSizeCategoryIsAccessibilityCategory (IntPtr /* NSString */ category);

		[iOS (11, 0), TV (11, 0)]
		static public bool IsAccessibilityCategory (this UIContentSizeCategory self)
		{
			return UIContentSizeCategoryIsAccessibilityCategory (self.GetConstant ().Handle);
		}
	}
}

#endif // !WATCH
