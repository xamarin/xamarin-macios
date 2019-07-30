#if !XAMCORE_4_0

using System;

namespace MetalPerformanceShaders {

	public partial class MPSCnnConvolutionTransposeNode {

		[Obsolete ("Always return null (not a public API).")]
		static public MPSCnnConvolutionTransposeNode Create (MPSNNImageNode sourceNode,  MPSCnnConvolutionStateNode convolutionState, IMPSCnnConvolutionDataSource weights)
		{
			return null;
		}

		[Obsolete ("Always throw a 'NotSupportedException' (not a public API).")]
		public MPSCnnConvolutionTransposeNode (MPSNNImageNode sourceNode, MPSCnnConvolutionStateNode convolutionState, IMPSCnnConvolutionDataSource weights) : base (IntPtr.Zero)
		{
			throw new NotSupportedException ();
		}
	}
}

#endif
