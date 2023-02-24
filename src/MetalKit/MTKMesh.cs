//
// MTKMesh.cs: just so we can implement IMDLMeshBufferAllocator
//

#nullable enable

using ModelIO;
using Metal;
using Foundation;
namespace MetalKit {

	public partial class MTKMesh {
		public static MTKMesh []? FromAsset (MDLAsset asset, IMTLDevice device, out MDLMesh [] sourceMeshes, out NSError error)
		{
			NSArray aret;

			var ret = FromAsset (asset, device, out aret, out error);
			sourceMeshes = NSArray.FromArray<MDLMesh> (aret);
			return ret;
		}
	}
}
