using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace MLCompute {

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCActivationTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCActivationTypeDebugDescription (MLCActivationType activationType);

		public static string GetDebugDescription (this MLCActivationType self)
		{
			return CFString.FromHandle (MLCActivationTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCArithmeticOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCArithmeticOperationDebugDescription (MLCArithmeticOperation operation);

		public static string GetDebugDescription (this MLCArithmeticOperation self)
		{
			return CFString.FromHandle (MLCArithmeticOperationDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCPaddingPolicyExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPaddingPolicyDebugDescription (MLCPaddingPolicy paddingPolicy);

		public static string GetDebugDescription (this MLCPaddingPolicy self)
		{
			return CFString.FromHandle (MLCPaddingPolicyDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCLossTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCLossTypeDebugDescription (MLCLossType lossType);

		public static string GetDebugDescription (this MLCLossType self)
		{
			return CFString.FromHandle (MLCLossTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCReductionTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCReductionTypeDebugDescription (MLCReductionType reductionType);

		public static string GetDebugDescription (this MLCReductionType self)
		{
			return CFString.FromHandle (MLCReductionTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCPaddingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPaddingTypeDebugDescription (MLCPaddingType paddingType);

		public static string GetDebugDescription (this MLCPaddingType self)
		{
			return CFString.FromHandle (MLCPaddingTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCConvolutionTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCConvolutionTypeDebugDescription (MLCConvolutionType convolutionType);

		public static string GetDebugDescription (this MLCConvolutionType self)
		{
			return CFString.FromHandle (MLCConvolutionTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCPoolingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPoolingTypeDebugDescription (MLCPoolingType poolingType);

		public static string GetDebugDescription (this MLCPoolingType self)
		{
			return CFString.FromHandle (MLCPoolingTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCSoftmaxOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCSoftmaxOperationDebugDescription (MLCSoftmaxOperation operation);

		public static string GetDebugDescription (this MLCSoftmaxOperation self)
		{
			return CFString.FromHandle (MLCSoftmaxOperationDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCSampleModeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCSampleModeDebugDescription (MLCSampleMode mode);

		public static string GetDebugDescription (this MLCSampleMode self)
		{
			return CFString.FromHandle (MLCSampleModeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
	public static class MLCLstmResultModeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCLSTMResultModeDebugDescription (MLCLstmResultMode mode);

		public static string GetDebugDescription (this MLCLstmResultMode self)
		{
			return CFString.FromHandle (MLCLSTMResultModeDebugDescription (self));
		}
	}

	[TV (14,5)][Mac (11,3)][iOS (14,5)]
	[NoWatch]
	public static class MLCComparisonOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCComparisonOperationDebugDescription (MLCComparisonOperation operation);

		public static string GetDebugDescription (this MLCComparisonOperation self)
		{
			return CFString.FromHandle (MLCComparisonOperationDebugDescription (self));
		}
	}

	[NoWatch]
	[TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	public static class MLCGradientClippingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCGradientClippingTypeDebugDescription (MLCGradientClippingType gradientClippingType);

		public static string GetDebugDescription (this MLCGradientClippingType self)
		{
			return CFString.FromHandle (MLCGradientClippingTypeDebugDescription (self));
		}
	}
}
