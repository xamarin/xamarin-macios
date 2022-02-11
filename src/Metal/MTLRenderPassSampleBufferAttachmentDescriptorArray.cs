using System;
using System.Runtime.Versioning;

#nullable enable

namespace Metal {

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#endif
	public partial class MTLRenderPassSampleBufferAttachmentDescriptorArray {

		public MTLRenderPassSampleBufferAttachmentDescriptor this[nuint i] {
 			get => GetObject (i);
			set => SetObject (value, i);
		}
	}

}
