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
using ObjCRuntime;

namespace Vision {
	public partial class VNRequest {

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
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

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		public const VNRequestRevision Revision2 = VNRequestRevision.Two;
	}

	public partial class VNDetectFaceRectanglesRequest {

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		public const VNRequestRevision Revision2 = VNRequestRevision.Two;
	}
}
#endif
