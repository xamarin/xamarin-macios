using System;
using System.Runtime.Versioning;
#if NET
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

#nullable enable

namespace ModelIO {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class MDLTransform {
#if !NET
		// Inlined from the MDLTransformComponent protocol.
		public static MatrixFloat4x4 CreateGlobalTransform4x4 (MDLObject obj, double atTime)
		{
			return MatrixFloat4x4.Transpose ((MatrixFloat4x4) CreateGlobalTransform (obj, atTime));
		}
#endif
	}
}
