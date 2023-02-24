using System;

#nullable enable

namespace Metal {

	public partial class MTLComputePassSampleBufferAttachmentDescriptorArray {

		public MTLComputePassSampleBufferAttachmentDescriptor this [nuint i] {
			get => GetObject (i);
			set => SetObject (value, i);
		}
	}

}
