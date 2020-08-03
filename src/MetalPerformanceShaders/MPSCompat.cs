#if !XAMCORE_4_0

using System;
using Metal;

using ObjCRuntime;

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

	public partial class MPSCnnConvolutionNode {

		[Obsolete ("Empty stub. (not a public API).")]
		public virtual MPSCnnConvolutionStateNode ConvolutionState { get; }
	}

	public partial class MPSCnnConvolutionTranspose {
		[Obsolete ("Always throws NotSupportedException (). (not a public API).")]
		public virtual MPSImage EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSCnnConvolutionState convolutionState) 
			=> throw new NotSupportedException ();
	}

	public partial class MPSCnnConvolution {
		[TV (11, 0), iOS (11, 0)]
		[Obsolete ("Always throws NotSupportedException (). (not a public API).")]
		public virtual void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSImage destinationImage, out MPSCnnConvolutionState state)
			=> throw new NotSupportedException ();
	}

	public partial class MPSCnnConvolutionState {

		[Obsolete ("Empty stub. (not a public API).")]
		public virtual MPSOffset SourceOffset { get; }
	}
}

#endif
