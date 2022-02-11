using System;

using CoreFoundation;
using Foundation;
using System.Runtime.Versioning;

namespace NaturalLanguage {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NLEmbedding {

		unsafe public bool TryGetVector (string @string, out float[] vector)
		{
			var result = false;
			vector = new float [Dimension];
			fixed (float *ptr = vector) {
				result = GetVector ((IntPtr) ptr, @string);
			}
			if (!result)
				vector = null; // to be consistent with GetVector API
			return result;
		}
	}
}
