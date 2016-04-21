// Copyright 2015 Xamarin Inc.

#if XAMCORE_2_0 || !MONOMAC

using XamCore.Foundation;
using XamCore.ModelIO;

namespace XamCore.GLKit {

	public partial class GLKMesh {
		public static GLKMesh [] FromAsset (MDLAsset asset, out MDLMesh [] sourceMeshes, out NSError error)
		{
			NSArray aret;

			var ret = FromAsset (asset, out aret, out error);
			sourceMeshes = NSArray.FromArray<MDLMesh> (aret);
			return ret;
		}
	}
}

#endif
