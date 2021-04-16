using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace MLCompute {

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCActivationTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCActivationTypeDebugDescription (MLCActivationType activationType);

		public static string GetDebugDescription (this MLCActivationType self)
		{
			return CFString.FetchString (MLCActivationTypeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCArithmeticOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCArithmeticOperationDebugDescription (MLCArithmeticOperation operation);

		public static string GetDebugDescription (this MLCArithmeticOperation self)
		{
			return CFString.FetchString (MLCArithmeticOperationDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCPaddingPolicyExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPaddingPolicyDebugDescription (MLCPaddingPolicy paddingPolicy);

		public static string GetDebugDescription (this MLCPaddingPolicy self)
		{
			return CFString.FetchString (MLCPaddingPolicyDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCLossTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCLossTypeDebugDescription (MLCLossType lossType);

		public static string GetDebugDescription (this MLCLossType self)
		{
			return CFString.FetchString (MLCLossTypeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCReductionTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCReductionTypeDebugDescription (MLCReductionType reductionType);

		public static string GetDebugDescription (this MLCReductionType self)
		{
			return CFString.FetchString (MLCReductionTypeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCPaddingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPaddingTypeDebugDescription (MLCPaddingType paddingType);

		public static string GetDebugDescription (this MLCPaddingType self)
		{
			return CFString.FetchString (MLCPaddingTypeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCConvolutionTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCConvolutionTypeDebugDescription (MLCConvolutionType convolutionType);

		public static string GetDebugDescription (this MLCConvolutionType self)
		{
			return CFString.FetchString (MLCConvolutionTypeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCPoolingTypeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCPoolingTypeDebugDescription (MLCPoolingType poolingType);

		public static string GetDebugDescription (this MLCPoolingType self)
		{
			return CFString.FetchString (MLCPoolingTypeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCSoftmaxOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCSoftmaxOperationDebugDescription (MLCSoftmaxOperation operation);

		public static string GetDebugDescription (this MLCSoftmaxOperation self)
		{
			return CFString.FetchString (MLCSoftmaxOperationDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCSampleModeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCSampleModeDebugDescription (MLCSampleMode mode);

		public static string GetDebugDescription (this MLCSampleMode self)
		{
			return CFString.FetchString (MLCSampleModeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCLstmResultModeExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCLSTMResultModeDebugDescription (MLCLstmResultMode mode);

		public static string GetDebugDescription (this MLCLstmResultMode self)
		{
			return CFString.FetchString (MLCLSTMResultModeDebugDescription (self));
		}
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (14,0)][TV (14,0)][Mac (11,0)]
	[NoWatch]
#endif
	public static class MLCComparisonOperationExtensions {

		[DllImport (Constants.MLComputeLibrary)]
		static extern /* NSString */ IntPtr MLCComparisonOperationDebugDescription (MLCComparisonOperation operation);

		public static string GetDebugDescription (this MLCComparisonOperation self)
		{
			return CFString.FetchString (MLCComparisonOperationDebugDescription (self));
		}
	}
}
