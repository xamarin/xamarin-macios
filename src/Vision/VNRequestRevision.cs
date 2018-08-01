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

		internal static T [] GetSupportedVersions<T> (NSIndexSet indexSet) where T : struct, IConvertible // Enum is sadly a C# 7.3 feature
		{
			if (indexSet == null)
				return null;

			if (!typeof (T).IsEnum)
				throw new ArgumentException ("T must be an enum.");

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
#endif
