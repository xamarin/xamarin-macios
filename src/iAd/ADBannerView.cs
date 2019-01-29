using System.Runtime.InteropServices;
using CoreGraphics;
using ObjCRuntime;

namespace iAd {
	public partial class ADBannerView {

			[DllImport (Constants.iAdLibrary)]
			static extern CGSize ADClampedBannerSize (CGSize size);

			public static CGSize GetClampedBannerSize (CGSize size) {
				return ADClampedBannerSize (size);
			}
	}
}