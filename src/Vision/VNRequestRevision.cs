//
// VNRequestRevision.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

#nullable enable

using System;

using Foundation;

using ObjCRuntime;

namespace Vision {
	public partial class VNRequest {

		internal static T []? GetSupportedVersions<T> (NSIndexSet indexSet) where T : Enum
		{
			if (indexSet is null)
				return null;

			var count = indexSet.Count;
			var supportedRevisions = new T [indexSet.Count];

			if (count == 0)
				return supportedRevisions;

			int j = 0;
			for (var i = indexSet.FirstIndex; i <= indexSet.LastIndex;) {
				supportedRevisions [j++] = (T) Enum.Parse (typeof (T), i.ToString (), true);
				i = indexSet.IndexGreaterThan (i);
			}

			return supportedRevisions;
		}
	}
}
