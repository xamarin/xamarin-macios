#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Accessibility {

	public static partial class AXAnimatedImagesUtilities {

		[DllImport (Constants.AccessibilityLibrary)]
		extern static byte AXAnimatedImagesEnabled ();

		public static bool Enabled => AXAnimatedImagesEnabled () != 0;
	}
}
