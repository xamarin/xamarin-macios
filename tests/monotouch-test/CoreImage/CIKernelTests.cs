#if !__WATCHOS__

using System;
using System.Threading;

using Foundation;
using CoreImage;
using CoreGraphics;
using ObjCRuntime;
#if MONOMAC
using AppKit;
using PlatformImage = AppKit.NSImage;
#else
using UIKit;
using PlatformImage = UIKit.UIImage;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreImage {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CIKernelTests {
		// macOS: the __sample type is available starting in 10.11
		const string NoOpColorKernel = @"kernel vec4 doNothing ( __sample s) { return s.rgba; }";
		const string NoOpWithParamColorKernel = @"kernel vec4 doNothingWithParam ( __sample s, float d) { return s.rgba; }";
		const string PositionColorKernel = @"kernel vec4 vignette ( __sample s, vec2 centerOffset, float radius )
{
 vec2 vecFromCenter = destCoord () - centerOffset;
 float distance = length ( vecFromCenter );
 float darken = 1.0 - (distance / radius);
 return vec4 (s.rgb * darken, s.a);
}";
		const string NoOpWarpKernel = @"kernel vec2 doNothingWarp () { return destCoord (); }";

		enum CustomerFilterType { NoOpColor, NoOpColorWithParam, ColorPositionKernel, NoOpWarpKernel };

		class MyCustomFilter : CIFilter {
			public MyCustomFilter (CustomerFilterType type)
			{
				Type = type;
				kernel = CIKernel.FromProgramSingle (GetKernelString ());
				Assert.IsNotNull (kernel, $"Kernel: {Type}");
			}

			public bool IsColorKernel {
				get {
					switch (Type) {
					case CustomerFilterType.NoOpColor:
					case CustomerFilterType.NoOpColorWithParam:
					case CustomerFilterType.ColorPositionKernel:
						return true;
					case CustomerFilterType.NoOpWarpKernel:
						return false;
					}
					throw new InvalidOperationException ();
				}
			}

			string GetKernelString ()
			{
				switch (Type) {
				case CustomerFilterType.NoOpColor:
					return NoOpColorKernel;
				case CustomerFilterType.NoOpColorWithParam:
					return NoOpWithParamColorKernel;
				case CustomerFilterType.ColorPositionKernel:
					return PositionColorKernel;
				case CustomerFilterType.NoOpWarpKernel:
					return NoOpWarpKernel;
				}
				throw new InvalidOperationException ();
			}

			NSObject [] GetParms ()
			{
				switch (Type) {
				case CustomerFilterType.NoOpColor:
					return new NSObject [] { MyImage };
				case CustomerFilterType.NoOpColorWithParam:
					return new NSObject [] { MyImage, new NSNumber (5) };
				case CustomerFilterType.ColorPositionKernel:
					var dod = MyImage.Extent;
					double radius = 0.5 * Math.Sqrt (Math.Pow (dod.Width, 2) + Math.Pow (dod.Height, 2));
					CIVector centerOffset = new CIVector ((float) (dod.Size.Width * .5), (float) (dod.Size.Height * .5));
					return new NSObject [] { MyImage, centerOffset, new NSNumber (radius) };
				case CustomerFilterType.NoOpWarpKernel:
					return new NSObject [] { };
				}
				throw new InvalidOperationException ();
			}

			CustomerFilterType Type { get; set; }
			public CIImage MyImage { get; set; }
			public bool CallbackHit { get; private set; }

			public CIKernel kernel;

			public override CIImage OutputImage {
				get {
					if (IsColorKernel) {
						return kernel.ApplyWithExtent (MyImage.Extent, (index, rect) => {
							CallbackHit = true;
							return rect;
						}, GetParms ());
					} else {
						CIWarpKernel warp = (CIWarpKernel) kernel;
						return warp.ApplyWithExtent (MyImage.Extent, (index, rect) => {
							CallbackHit = true;
							return rect;
						}, MyImage, GetParms ());
					}
				}
			}
		}

		[Test]
		public void CIKernel_BasicTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			Exception ex = null;
			var t = new Thread (() => {
				// This code will crash if an MKMapView has been created previously on 
				// the same thread, so just run it on a different thread (MKMapViews can
				// only be created on the main thread). This is obviously an Apple bug,
				// and a radar has been filed: 19249153. ObjC test case: https://github.com/rolfbjarne/CIKernelMKMapViewCrash
				try {
					PlatformImage uiImg = new PlatformImage (NSBundle.MainBundle.PathForResource ("Xam", "png", "CoreImage"));
#if MONOMAC
					CIImage ciImg = new CIImage (uiImg.CGImage);
					CIContext context = new CIContext (null);
#else
					CIImage ciImg = new CIImage (uiImg);
					CIContext context = CIContext.FromOptions (null);
#endif


#if NET
					foreach (var type in Enum.GetValues<CustomerFilterType> ()) {
#else
					foreach (CustomerFilterType type in Enum.GetValues (typeof (CustomerFilterType))) {
#endif
						MyCustomFilter filter = new MyCustomFilter (type);
						filter.MyImage = ciImg;

						CIImage outputImage = filter.OutputImage;

						CGImage cgImage = context.CreateCGImage (outputImage, outputImage.Extent);
#if MONOMAC
						NSImage finalImg = new NSImage (cgImage, new CGSize ());
#else
						UIImage finalImg = new UIImage (cgImage);
#endif
						Assert.IsNotNull (finalImg, "CIKernel_BasicTest should not be null");
						Assert.IsTrue (filter.CallbackHit, "CIKernel_BasicTest callback must be hit");
						if (filter.IsColorKernel)
							Assert.IsTrue (filter.kernel is CIColorKernel, "CIKernel_BasicTest we disagree that it is a color kernel");
						else
							Assert.IsTrue (filter.kernel is CIWarpKernel, "CIKernel_BasicTest we disagree that it is a warp kernel");
					}
				} catch (Exception ex2) {
					ex = ex2;
				}
			});
			t.Start ();
			t.Join ();
			if (ex is not null)
				throw ex;
		}

		[Test]
		public void CIKernel_TestFromPrograms ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 11, throwIfOtherPlatform: false);

			CIKernel [] kernels =
				CIKernel.FromProgramMultiple (
					NoOpColorKernel + "\n" + NoOpWithParamColorKernel + "\n" + PositionColorKernel + "\n" + NoOpWarpKernel);
			Assert.AreEqual (4, kernels.Length, "CIKernel_TestFromPrograms did not get back the right number of programs");
			foreach (CIKernel kernel in kernels) {
				Assert.IsTrue (kernel is CIColorKernel || kernel is CIWarpKernel, "CIKernel_TestFromPrograms is neither type of kernel?");
			}
		}
	}
}

#endif // !__WATCHOS__
