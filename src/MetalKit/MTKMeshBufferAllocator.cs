//
// MTKMeshBufferAllocator.cs: just so we can implement IMDLMeshBufferAllocator
//
#if XAMCORE_2_0 || !MONOMAC
using XamCore.ModelIO;
namespace XamCore.MetalKit {

	public partial class MTKMeshBufferAllocator : IMDLMeshBufferAllocator {
	}
}
#endif