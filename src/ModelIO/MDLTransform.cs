#if XAMCORE_2_0 || !MONOMAC
using System;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;

namespace ModelIO {
	public partial class MDLTransform {
#if !XAMCORE_4_0
		// Inlined from the MDLTransformComponent protocol.
		public static MatrixFloat4x4 CreateGlobalTransform4x4 (MDLObject obj, double atTime)
		{
			return MatrixFloat4x4.Transpose ((MatrixFloat4x4) CreateGlobalTransform (obj, atTime));
		}
#endif
	}
}
#endif