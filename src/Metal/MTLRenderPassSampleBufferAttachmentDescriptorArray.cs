using System;

namespace Metal {

	public partial class MTLRenderPassSampleBufferAttachmentDescriptorArray {

		public MTLRenderPassSampleBufferAttachmentDescriptor this[nuint i] {
 			get => GetObject (i);
			set => SetObject (value, i);
		}
	}

}
