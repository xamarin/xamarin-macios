using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace MLCompute {

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCActivationTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCActivationTypeDebugDescription (MLCActivationType activationType);

		public static string GetDebugDescription (this MLCActivationType self)
		{
			return CFString.FetchString (MLCActivationTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCArithmeticOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCArithmeticOperationDebugDescription (MLCArithmeticOperation operation);

		public static string GetDebugDescription (this MLCArithmeticOperation self)
		{
			return CFString.FetchString (MLCArithmeticOperationDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCPaddingPolicyExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPaddingPolicyDebugDescription (MLCPaddingPolicy paddingPolicy);

		public static string GetDebugDescription (this MLCPaddingPolicy self)
		{
			return CFString.FetchString (MLCPaddingPolicyDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCLossTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCLossTypeDebugDescription (MLCLossType lossType);

		public static string DebugDescription (this MLCLossType self)
		{
			return CFString.FetchString (MLCLossTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCReductionTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCReductionTypeDebugDescription (MLCReductionType reductionType);

		public static string DebugDescription (this MLCReductionType self)
		{
			return CFString.FetchString (MLCReductionTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCPaddingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPaddingTypeDebugDescription (MLCPaddingType paddingType);

		public static string DebugDescription (this MLCPaddingType self)
		{
			return CFString.FetchString (MLCPaddingTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCConvolutionTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCConvolutionTypeDebugDescription (MLCConvolutionType convolutionType);

		public static string DebugDescription (this MLCConvolutionType self)
		{
			return CFString.FetchString (MLCConvolutionTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCPoolingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPoolingTypeDebugDescription (MLCPoolingType poolingType);

		public static string DebugDescription (this MLCPoolingType self)
		{
			return CFString.FetchString (MLCPoolingTypeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCSoftmaxOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCSoftmaxOperationDebugDescription (MLCSoftmaxOperation operation);

		public static string DebugDescription (this MLCSoftmaxOperation self)
		{
			return CFString.FetchString (MLCSoftmaxOperationDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCSampleModeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCSampleModeDebugDescription (MLCSampleMode mode);

		public static string DebugDescription (this MLCSampleMode self)
		{
			return CFString.FetchString (MLCSampleModeDebugDescription (self));
		}
	}

	[iOS (14,0)][TV (14,0)][Mac (10,16)]
	[NoWatch]
	public static class MLCLstmResultModeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCLSTMResultModeDebugDescription (MLCLstmResultMode mode);

		public static string DebugDescription (this MLCLstmResultMode self)
		{
			return CFString.FetchString (MLCLSTMResultModeDebugDescription (self));
		}
	}
}