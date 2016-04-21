using System;

using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;

namespace XamCore.CoreAnimation {
	public partial class CAKeyFrameAnimation {
		public T[] GetValuesAs <T> () where T : class, INativeObject
		{
			return NSArray.FromArrayNative<T> (_Values);
		}

		public void SetValues (INativeObject [] value)
		{
			_Values = NSArray.FromNSObjects (value);
		}

		// The underlying objective-c API appears to be doing pointer comparisions
		// or some other trickery. Our NSString -> C# string -> NSString conversions
		// were breaking this on the Mac. We look up the equivilant NSString and pass that
		// along to "fix" this
		public virtual string CalculationMode {
			get {
				return _CalculationMode;
			}
			set {
				NSString result;
				if (value == CAAnimation.AnimationLinear)
					result = CAAnimation.AnimationLinear;
				else if (value == CAAnimation.AnimationDescrete)
					result = CAAnimation.AnimationDescrete;
				else if (value == CAAnimation.AnimationPaced)
					result = CAAnimation.AnimationPaced;
				else if (value == CAAnimation.AnimationCubic)
					result = CAAnimation.AnimationCubic;
				else if (value == CAAnimation.AnimationCubicPaced)
					result = CAAnimation.AnimationCubicPaced;
				else
					result = (NSString)value;

				_CalculationMode = result;
			}
		}
	}
}


