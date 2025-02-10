#if NET
#if MONOMAC
using System;

using AVFoundation;
using Foundation;
using ObjCRuntime;

namespace MediaExtension {
	public partial interface IMESampleCursor {
		/// <summary>Compute an exact sample location based on data returned from <see cref="GetEstimatedSampleLocation" />.</summary>
		/// <param name="estimatedSampleLocation">The estimated sample location returned from <see cref="GetEstimatedSampleLocation" />.</param>
		/// <param name="refinementData">The refinement data returned from <see cref="GetEstimatedSampleLocation" />.</param>
		/// <param name="refinedLocation">Upon return, the exact location of the sample.</param>
		/// <param name="error">Upon return, null if successful, otherwise an <see cref="NSError" /> instance for the error.</param>
		/// <returns>True if successful, false otherwise.</returns>
		public bool RefineSampleLocation (AVSampleCursorStorageRange estimatedSampleLocation, byte	[] refinementData, out AVSampleCursorStorageRange refinedLocation, out NSError? error)
		{
			unsafe {
				fixed (byte* refinementDataPtr = refinementData) {
					return RefineSampleLocation (estimatedSampleLocation, refinementDataPtr, (nuint) refinementData.Length, out refinedLocation, out error);
				}
			}
		}
	}
}
#endif // MONOMAC
#endif // NET
