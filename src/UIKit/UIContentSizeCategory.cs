#if !WATCH

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;

namespace XamCore.UIKit {
	static public partial class UIContentSizeCategoryExtensions {

		[iOS (11, 0), TV (11, 0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern nint /* NSComparisonResult */ UIContentSizeCategoryCompareToCategory (IntPtr /* NSString */ lhs, IntPtr /* NSString */ rhs);

		[iOS (11, 0), TV (11, 0)]
		public static NSComparisonResult Compare (UIContentSizeCategory category1, UIContentSizeCategory category2)
		{
			var c1 = category1.GetConstant ();
			if (c1 == null)
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value", nameof (category1));

			var c2 = category2.GetConstant ();
			if (c2 == null)
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value", nameof (category2));

			return (NSComparisonResult)(long)UIContentSizeCategoryCompareToCategory (c1.Handle, c2.Handle);
		}

		[iOS (11, 0), TV (11, 0)]
		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIContentSizeCategoryIsAccessibilityCategory (IntPtr /* NSString */ category);

		[iOS (11, 0), TV (11, 0)]
		static public bool IsAccessibilityCategory (this UIContentSizeCategory self)
		{
			var c1 = self.GetConstant ();
			if (c1 == null)
				throw new ArgumentException ($"Unknown 'UIContentSizeCategory' value");

			return UIContentSizeCategoryIsAccessibilityCategory (c1.Handle);
		}
	}
}

#endif // !WATCH
