//
// MTKMesh.cs: just so we can implement IMDLMeshBufferAllocator
//
#if XAMCORE_2_0 || !MONOMAC
using XamCore.ModelIO;
using XamCore.Metal;
using XamCore.Foundation;
namespace XamCore.MetalKit {

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
#endif
