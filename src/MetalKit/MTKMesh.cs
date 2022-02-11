//
// MTKMesh.cs: just so we can implement IMDLMeshBufferAllocator
//

using ModelIO;
using Metal;
using Foundation;
using System.Runtime.Versioning;
namespace MetalKit {

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class MTKMesh {
		public static MTKMesh [] FromAsset (MDLAsset asset, IMTLDevice device, out MDLMesh [] sourceMeshes, out NSError error)
		{
			NSArray aret;
			
			var ret = FromAsset (asset, device, out aret, out error);
			sourceMeshes = NSArray.FromArray<MDLMesh> (aret);
			return ret;
		}
	}
}
