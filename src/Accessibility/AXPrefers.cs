#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

namespace Accessibility {

	// accessibility.cs already provide the following attributes on the type
	// [Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public static partial class AXPrefers {

		[DllImport (Constants.AccessibilityLibrary)]
		static extern byte AXPrefersHorizontalTextLayout ();

		public static bool HorizontalTextEnabled ()
		{
			return AXPrefersHorizontalTextLayout () != 0;
		}

	}
}
