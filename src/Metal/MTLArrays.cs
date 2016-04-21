//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014, Xamarin Inc.
//
#if XAMCORE_2_0 || !MONOMAC

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Metal {

	public partial class MTLRenderPipelineColorAttachmentDescriptorArray {
		public MTLRenderPipelineColorAttachmentDescriptor this [nint idx] {
			get {
				return ObjectAtIndexedSubscript ((nuint) idx);
			}
			set {
				SetObject (value, (nuint) idx);
			}
		}
	}

	public partial class MTLRenderPassColorAttachmentDescriptorArray {
		public MTLRenderPassColorAttachmentDescriptor this [nint idx] {
			get {
				return ObjectAtIndexedSubscript ((nuint) idx);
			}
			set {
				SetObject (value, (nuint) idx);
			}
		}
	}

	public partial class MTLVertexAttributeDescriptorArray {
		public MTLVertexAttributeDescriptor this [nint idx] {
			get {
				return ObjectAtIndexedSubscript ((nuint) idx);
			}
			set {
				SetObject (value, (nuint) idx);
			}
		}
	}
	public partial class MTLVertexBufferLayoutDescriptorArray {
		public MTLVertexBufferLayoutDescriptor this [nint idx] {
			get {
				return ObjectAtIndexedSubscript ((nuint) idx);
			}
			set {
				SetObject (value, (nuint) idx);
			}
		}
	}
	
}
#endif