// Copyright 2015 Xamarin Inc.

using Foundation;
using ModelIO;

#nullable enable

namespace GLKit {

	public partial class GLKMesh {
		public static GLKMesh []? FromAsset (MDLAsset asset, out MDLMesh [] sourceMeshes, out NSError error)
		{
			NSArray aret;

			var ret = FromAsset (asset, out aret, out error);
			sourceMeshes = NSArray.FromArray<MDLMesh> (aret);
			return ret;
		}
	}
}
