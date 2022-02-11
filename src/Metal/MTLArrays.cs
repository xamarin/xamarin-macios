//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014, Xamarin Inc.
//

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

#nullable enable

namespace Metal {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MTLBufferLayoutDescriptorArray {
		public MTLBufferLayoutDescriptor this [nuint idx] {
			get {
				return ObjectAtIndexedSubscript (idx);
			}
			set {
				SetObject (value, idx);
			}
		}
	}
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MTLAttributeDescriptorArray {
		public MTLAttributeDescriptor this [nuint idx] {
			get {
				return ObjectAtIndexedSubscript (idx);
			}
			set {
				SetObject (value, idx);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MTLPipelineBufferDescriptorArray {
		public MTLPipelineBufferDescriptor this [nuint index]
		{
			get {
				return GetObject (index);
			}
			set {
				SetObject (value, index);
			}
		}
	}

#if IOS
	public partial class MTLTileRenderPipelineColorAttachmentDescriptorArray {
		public MTLTileRenderPipelineColorAttachmentDescriptor this [nuint index]
		{
			get {
				return GetObject (index);
			}
			set {
				SetObject (value, index);
			}
		}
	}
#endif
}
