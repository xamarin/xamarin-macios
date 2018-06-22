//
// VNRequestRevision.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#if XAMCORE_2_0

using System;
using Foundation;

namespace Vision {
	public partial class VNRequest {

		public const VNRequestRevision Revision1 = VNRequestRevision.One;

		internal static VNRequestRevision [] GetSupportedVersions (NSIndexSet indexSet)
		{
			if (indexSet == null)
				return null;

			var count = indexSet.Count;
			var supportedRevisions = new VNRequestRevision [indexSet.Count];

			if (count == 0)
				return supportedRevisions;

			int j = 0;
			for (var i = indexSet.FirstIndex; i <= indexSet.LastIndex;) {
				supportedRevisions [j++] = (VNRequestRevision) (uint) i;
				i = indexSet.IndexGreaterThan (i);
			}

			return supportedRevisions;
		}
	}

	public partial class VNDetectFaceLandmarksRequest {
		public const VNRequestRevision Revision2 = VNRequestRevision.Two;
	}

	public partial class VNDetectFaceRectanglesRequest {
		public const VNRequestRevision Revision2 = VNRequestRevision.Two;
	}
}
#endif
