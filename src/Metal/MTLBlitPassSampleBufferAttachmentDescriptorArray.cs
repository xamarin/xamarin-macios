using System;

namespace Metal {

	public partial class MTLBlitPassSampleBufferAttachmentDescriptorArray {

		public MTLBlitPassSampleBufferAttachmentDescriptor this[nuint i] {
 			get => GetObject (i);
			set => SetObject (value, i);
		}
	}

}
