using System;

#nullable enable

namespace Metal {

#if !TVOS

	public partial class MTLResourceStatePassSampleBufferAttachmentDescriptorArray {

		public MTLResourceStatePassSampleBufferAttachmentDescriptor this [nuint i] {
			get => GetObject (i);
			set => SetObject (value, i);
		}
	}

#endif
}
