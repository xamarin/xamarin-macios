//
// MTKMeshBufferAllocator.cs: just so we can implement IMDLMeshBufferAllocator
//

using ModelIO;
using System.Runtime.Versioning;
namespace MetalKit {

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class MTKMeshBufferAllocator : IMDLMeshBufferAllocator {
	}
}
