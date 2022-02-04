#if !NET

using System;
using Metal;
using System.Runtime.Versioning;

using ObjCRuntime;

using NativeHandle = System.IntPtr;

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

		[Obsolete ("Empty stub (not a public API).")]
		public virtual MPSCnnConvolutionStateNode ConvolutionState { get; }
	}

	public partial class MPSCnnConvolutionTranspose {
		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public virtual MPSImage EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSCnnConvolutionState convolutionState) 
			=> throw new NotSupportedException ();
	}

	public partial class MPSCnnConvolution {
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
#else
		[TV (11, 0)]
		[iOS (11, 0)]
#endif
		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public virtual void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSImage destinationImage, out MPSCnnConvolutionState state)
			=> throw new NotSupportedException ();
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11,0)]
	[Mac (10, 13)]
	[iOS (11,0)]
#endif
	[Obsolete ("Empty stub (not a public API).")]
	public partial class MPSCnnConvolutionState : MPSState, IMPSImageSizeEncodingState {

		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		protected MPSCnnConvolutionState (Foundation.NSObjectFlag t) : base (t)
			=> throw new NotSupportedException ();

		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		protected MPSCnnConvolutionState (NativeHandle handle) : base (handle)
			=> throw new NotSupportedException ();

		[Obsolete ("Empty stub (not a public API).")]
		public virtual nuint KernelWidth { get; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual nuint KernelHeight { get; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual MPSOffset SourceOffset { get; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual nuint SourceWidth { get; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual nuint SourceHeight { get; }

#pragma warning disable CS0809
		[Obsolete ("Empty stub (not a public API).")]
		public override NativeHandle ClassHandle { get; }
#pragma warning restore CS0809
	}
}

#endif // !NET
