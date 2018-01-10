#if XAMCORE_2_0 && !XAMCORE_4_0
using OpenTK;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;

namespace ModelIO {
	public partial class MDLTransformComponent_Extensions {
		public static MatrixFloat4x4 GetMatrix4x4 (this IMDLTransformComponent self)
		{
			return MatrixFloat4x4.Transpose ((MatrixFloat4x4) self.Matrix);
		}

		public static void SetMatrix4x4 (this IMDLTransformComponent self, MatrixFloat4x4 value)
		{
			self.Matrix = (Matrix4) MatrixFloat4x4.Transpose (value);
		}

		public static MatrixFloat4x4 GetLocalTransform4x4(this IMDLTransformComponent This, double time)
		{
			return MatrixFloat4x4.Transpose ((MatrixFloat4x4) GetLocalTransform (This, time));
		}

		public static void SetLocalTransform4x4 (this IMDLTransformComponent This, MatrixFloat4x4 transform, double time)
		{
			SetLocalTransform (This, (Matrix4) MatrixFloat4x4.Transpose (transform), time);
		}

		public static void SetLocalTransform4x4 (this IMDLTransformComponent This, MatrixFloat4x4 transform)
		{
			SetLocalTransform (This, (Matrix4) MatrixFloat4x4.Transpose (transform));
		}
	}
}
#endif