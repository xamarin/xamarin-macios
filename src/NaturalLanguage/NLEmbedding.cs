#nullable enable

using System;

using CoreFoundation;

using Foundation;

namespace NaturalLanguage {

	public partial class NLEmbedding {

		unsafe public bool TryGetVector (string @string, out float []? vector)
		{
			var result = false;
			vector = new float [Dimension];
			fixed (float* ptr = vector) {
				result = GetVector ((IntPtr) ptr, @string);
			}
			if (!result)
				vector = null; // to be consistent with GetVector API
			return result;
		}
	}
}
