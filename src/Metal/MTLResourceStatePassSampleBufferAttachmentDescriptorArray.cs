using System;
using System.Runtime.Versioning;

#nullable enable

namespace Metal {

#if !TVOS
#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("tvos")]
#endif
	public partial class MTLResourceStatePassSampleBufferAttachmentDescriptorArray {

		public MTLResourceStatePassSampleBufferAttachmentDescriptor this[nuint i] {
 			get => GetObject (i);
			set => SetObject (value, i);
		}
	}

#endif
}
