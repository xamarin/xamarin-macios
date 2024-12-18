using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using CoreVideo;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {
	/// <summary>This class contains a list of pixel buffers or sample buffers, where each buffer is associated with a <see cref="CMTagCollection" />.</summary>
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("tvos17.0")]
	public class CMTaggedBufferGroup : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CMTaggedBufferGroup (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CFTypeID */ nint CMTaggedBufferGroupGetTypeID ();

		/// <summary>Get this type's CFTypeID.</summary>
		public static nint GetTypeId ()
		{
			return CMTaggedBufferGroupGetTypeID ();
		}

		internal static CMTaggedBufferGroup? Create (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				return null;
			return new CMTaggedBufferGroup (handle, owns);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTaggedBufferGroupError /* OSStatus */ CMTaggedBufferGroupCreate (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			IntPtr /* CFArrayRef CM_NONNULL */ tagCollections,
			IntPtr /* CFArrayRef CM_NONNULL */ buffers,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTaggedBufferGroupRef CM_NULLABLE * CM_NONNULL */ group
		);

		/// <summary>Create a new <see cref="CMTaggedBufferGroup" /> instance.</summary>
		/// <param name="tagCollections">An array of <see cref="CMTagCollection" /> for this buffer group.</param>
		/// <param name="buffers">An array of pixel buffers.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A newly created <see cref="CMTaggedBufferGroup" /> instance if successful, otherwise null (and <paramref name="status" /> will contain an error code).</returns>
		public static CMTaggedBufferGroup? Create (CMTagCollection[] tagCollections, CVPixelBuffer[] buffers, out CMTaggedBufferGroupError status)
		{
			return Create (tagCollections, (NativeObject[]) buffers, out status);
		}

		/// <summary>Create a new <see cref="CMTaggedBufferGroup" /> instance.</summary>
		/// <param name="tagCollections">An array of <see cref="CMTagCollection" /> for this buffer group.</param>
		/// <param name="buffers">An array of sample buffers.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A newly created <see cref="CMTaggedBufferGroup" /> instance if successful, otherwise null (and <paramref name="status" /> will contain an error code).</returns>
		public static CMTaggedBufferGroup? Create (CMTagCollection[] tagCollections, CMSampleBuffer[] buffers, out CMTaggedBufferGroupError status)
		{
			return Create (tagCollections, (NativeObject[]) buffers, out status);
		}

		/// <summary>Create a new <see cref="CMTaggedBufferGroup" /> instance.</summary>
		/// <param name="tagCollections">An array of <see cref="CMTagCollection" /> for this buffer group.</param>
		/// <param name="buffers">An array of buffers, either <see cref="CMSampleBuffer" /> or <see cref="CVPixelBuffer" />.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A newly created <see cref="CMTaggedBufferGroup" /> instance if successful, otherwise null (and <paramref name="status" /> will contain an error code).</returns>
		public static CMTaggedBufferGroup? Create (CMTagCollection[] tagCollections, NativeObject[] buffers, out CMTaggedBufferGroupError status)
		{
			IntPtr handle;

			if (tagCollections is null)
				ThrowHelper.ThrowArgumentNullException (nameof (tagCollections));

			if (buffers is null)
				ThrowHelper.ThrowArgumentNullException (nameof (buffers));

			if (tagCollections.Length != buffers.Length)
				throw new ArgumentException ($"The '{nameof (tagCollections)}' and '{nameof (buffers)}' arrays must both have the same number of elements.");

			for (var i = 0; i < buffers.Length; i++) {
				if (buffers [i] is CMSampleBuffer || buffers [i] is CVPixelBuffer)
					continue;
				throw new ArgumentException (string.Format ($"The '{nameof (buffers)}' array must be an array of CMSampleBuffer or CVPixelBuffer. The object at index {{0}} is of type '{{1}}'", i, buffers [i]?.GetType ()), nameof (buffers));
			}

			using var tagCollectionsArray = CFArray.FromNullableNativeObjects (tagCollections);
			using var buffersArray = CFArray.FromNullableNativeObjects (buffers);
			unsafe {
				status = CMTaggedBufferGroupCreate (IntPtr.Zero, tagCollectionsArray.GetHandle (), buffersArray.GetHandle (), &handle);
			}
			return Create (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTaggedBufferGroupError /* OSStatus */ CMTaggedBufferGroupCreateCombined (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			IntPtr /* CFArrayRef CM_NONNULL */ taggedBufferGroups,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTaggedBufferGroupRef CM_NULLABLE * CM_NONNULL */ groupOut);

		/// <summary>Create a new <see cref="CMTaggedBufferGroup" /> by combining other tagged buffer groups.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <param name="groups">The group of <see cref="CMTaggedBufferGroup" /> instances to combine.</param>
		/// <returns>A newly created <see cref="CMTaggedBufferGroup" /> instance if successful, otherwise null (and <paramref name="status" /> will contain an error code).</returns>
		public static CMTaggedBufferGroup? Combine (out CMTaggedBufferGroupError status, params CMTaggedBufferGroup[] groups)
		{
			IntPtr handle;

			using var groupsArray = CFArray.FromNativeObjects (groups);
			unsafe {
				status = CMTaggedBufferGroupCreateCombined (IntPtr.Zero, groupsArray.GetHandle (), &handle);
			}
			return Create (handle, true);
		}

		/// <summary>Create a new <see cref="CMTaggedBufferGroup" /> by combining other tagged buffer groups.</summary>
		/// <param name="groups">The group of <see cref="CMTaggedBufferGroup" /> instances to combine.</param>
		/// <returns>A newly created <see cref="CMTaggedBufferGroup" /> instance if successful, otherwise null.</returns>
		public static CMTaggedBufferGroup? Combine (params CMTaggedBufferGroup[] groups)
		{
			return Combine (out var _, groups);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static nint /* CMItemCount */ CMTaggedBufferGroupGetCount (
			IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group);

		/// <summary>Get the number of buffers in this tagged buffer group.</summary>
		/// <returns>The number of buffers in this tagged buffer group.</returns>
		public nint Count {
			get {
				return CMTaggedBufferGroupGetCount (GetCheckedHandle ());
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr /* CMTagCollectionRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetTagCollectionAtIndex (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				nint /* CFIndex */ index);

		/// <summary>Get the tag collection at the specified index.</summary>
		/// <param name="index">The 0-based index of the tag collection to get.</param>
		/// <returns>The tag collection at the specified index, or null in case of failure.</returns>
		public CMTagCollection? GetTagCollection (nint index)
		{
			if (index < 0)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (index), "Must be 0 or higher");
			if (index >= Count)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (index), "Must be less than Count");
			var rv = CMTaggedBufferGroupGetTagCollectionAtIndex (GetCheckedHandle (), index);
			return CMTagCollection.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr /* CVPixelBufferRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetCVPixelBufferAtIndex (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				nint /* CFIndex */ index);

		/// <summary>Get the pixel buffer at the specified index.</summary>
		/// <param name="index">The 0-based index of the pixel buffer to get.</param>
		/// <returns>The pixel buffer at the specified index, or null in case of failure (including if the buffer at the specified index is not a <see cref="CVPixelBuffer" />).</returns>
		public CVPixelBuffer? GetPixelBuffer (nint index)
		{
			if (index < 0)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (index), "Must be 0 or higher");
			if (index >= Count)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (index), "Must be less than Count");
			var rv = CMTaggedBufferGroupGetCVPixelBufferAtIndex (GetCheckedHandle (), index);
			return CVPixelBuffer.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CVPixelBufferRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetCVPixelBufferForTag (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				CMTag tag,
				nint* /* CFIndex* */ index);

		/// <summary>Get the pixel buffer matching the specified tag.</summary>
		/// <param name="tag">The tag to search for. The search will fail if the tag is found in more than one buffer's tag collection.</param>
		/// <param name="index">The index of the matching pixel buffer.</param>
		/// <returns>The matching pixel buffer, or null in case of failure (including if the buffer at the specified index is not a <see cref="CVPixelBuffer" />).</returns>
		public unsafe CVPixelBuffer? GetPixelBuffer (CMTag tag, out nint index)
		{
			index = 0;
			var rv = CMTaggedBufferGroupGetCVPixelBufferForTag (GetCheckedHandle (), tag, (nint *) Unsafe.AsPointer<nint> (ref index));
			return CVPixelBuffer.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CVPixelBufferRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetCVPixelBufferForTagCollection (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
				IntPtr* /* CFIndex* */ index);

		/// <summary>Get the pixel buffer matching the specified tag collection.</summary>
		/// <param name="tagCollection">The tag collection to match. The search will fail if the tag collection is found in more than one pixel buffer.</param>
		/// <param name="index">The index of the matching pixel buffer.</param>
		/// <returns>The matching pixel buffer, or null in case of failure (including if the buffer at the specified index is not a <see cref="CVPixelBuffer" />).</returns>
		public unsafe CVPixelBuffer? GetPixelBuffer (CMTagCollection tagCollection, out nint index)
		{
			index = 0;
			var rv = CMTaggedBufferGroupGetCVPixelBufferForTagCollection (GetCheckedHandle (), tagCollection.GetCheckedHandle (), (nint *) Unsafe.AsPointer<nint> (ref index));
			return CVPixelBuffer.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr /* CMSampleBufferRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetCMSampleBufferAtIndex (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				nint /* CFIndex */ index);

		/// <summary>Get the sample buffer at the specified index.</summary>
		/// <param name="index">The 0-based index of the sample buffer to get.</param>
		/// <returns>The sample buffer at the specified index, or null in case of failure (including if the buffer at the specified index is not a <see cref="CMSampleBuffer" />).</returns>
		public CMSampleBuffer? GetSampleBuffer (nint index)
		{
			if (index < 0)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (index), "Must be 0 or higher");
			if (index >= Count)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (index), "Must be less than Count");
			var rv = CMTaggedBufferGroupGetCMSampleBufferAtIndex (GetCheckedHandle (), index);
			return CMSampleBuffer.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CMSampleBufferRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetCMSampleBufferForTag (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				CMTag tag,
				nint* /* CFIndex* */ index);

		/// <summary>Get the sample buffer at the specified index.</summary>
		/// <param name="tag">The tag to search for. The search will fail if the tag is found in more than one buffer's tag collection.</param>
		/// <param name="index">The index of the matching sample buffer.</param>
		/// <returns>The matching sample buffer, or null in case of failure (including if the buffer at the specified index is not a <see cref="CMSampleBuffer" />).</returns>
		public unsafe CMSampleBuffer? GetSampleBuffer (CMTag tag, out nint index)
		{
			index = 0;
			var rv = CMTaggedBufferGroupGetCMSampleBufferForTag (GetCheckedHandle (), tag, (nint *) Unsafe.AsPointer<nint> (ref index));
			return CMSampleBuffer.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CMSampleBufferRef CF_RETURNS_NOT_RETAINED CM_NULLABLE */ CMTaggedBufferGroupGetCMSampleBufferForTagCollection (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
				IntPtr* /* CFIndex* */ index);

		/// <summary>Get the sample buffer matching the specified tag collection.</summary>
		/// <param name="tagCollection">The tag collection to match. The search will fail if the tag collection is found in more than one sample buffer.</param>
		/// <param name="index">The index of the matching sample buffer.</param>
		/// <returns>The matching sample buffer, or null in case of failure (including if the buffer at the specified index is not a <see cref="CMSampleBuffer" />).</returns>
		public unsafe CMSampleBuffer? GetSampleBuffer (CMTagCollection tagCollection, out nint index)
		{
			index = 0;
			var rv = CMTaggedBufferGroupGetCMSampleBufferForTagCollection (GetCheckedHandle (), tagCollection.GetCheckedHandle (), (nint *) Unsafe.AsPointer<nint> (ref index));
			return CMSampleBuffer.Create (rv, false);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static nint /* CMItemCount */ CMTaggedBufferGroupGetNumberOfMatchesForTagCollection (
				IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ group,
				IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection);

		/// <summary>Calculate the number of times a tag collection matches in this tagged buffer group.</summary>
		/// <param name="tagCollection">The tag collection to use for the calculation.</param>
		/// <returns>The number of times a tag collection matches in this tagged buffer group.</returns>
		/// <remarks>Buffer lookups using a tag collection will fail unless there's exactly one match for the tag collection.</remarks>
		public nint GetNumberOfMatches (CMTagCollection tagCollection)
		{
			return CMTaggedBufferGroupGetNumberOfMatchesForTagCollection (GetCheckedHandle (), tagCollection.GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTaggedBufferGroupError /* OSStatus */ CMTaggedBufferGroupFormatDescriptionCreateForTaggedBufferGroup (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ taggedBufferGroup,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTaggedBufferGroupFormatDescriptionRef CM_NULLABLE * CM_NONNULL */ formatDescription);

		/// <summary>Craete a <see cref="CMFormatDescription" /> for this tagged buffer group.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A <see cref="CMFormatDescription" /> for this tagged buffer group, or null in case of failure.</returns>
		public CMFormatDescription? CreateFormatDescription (out CMTaggedBufferGroupError status)
		{
			IntPtr formatDescription;
			unsafe {
				status = CMTaggedBufferGroupFormatDescriptionCreateForTaggedBufferGroup (IntPtr.Zero, GetCheckedHandle (), &formatDescription);
			}
			return CMFormatDescription.Create (formatDescription, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte CMTaggedBufferGroupFormatDescriptionMatchesTaggedBufferGroup (
			IntPtr /* CMTaggedBufferGroupFormatDescriptionRef CM_NONNULL */ desc,
			IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ taggedBufferGroup);

		/// <summary>Checks if the specified format description matches this tagged buffer group.</summary>
		/// <param name="formatDescription">The format description to match.</param>
		/// <returns>True if the format description matches, false otherwise.</returns>
		public bool Matches (CMFormatDescription formatDescription)
		{
			return CMTaggedBufferGroupFormatDescriptionMatchesTaggedBufferGroup (formatDescription.GetCheckedHandle (), GetCheckedHandle ()) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTaggedBufferGroupError /* OSStatus */ CMSampleBufferCreateForTaggedBufferGroup (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			IntPtr /* CMTaggedBufferGroupRef CM_NONNULL */ taggedBufferGroup,
			CMTime sbufPTS,
			CMTime sbufDuration,
			IntPtr /* CMTaggedBufferGroupFormatDescriptionRef CM_NONNULL */ formatDescription,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMSampleBufferRef CM_NULLABLE * CM_NONNULL */ sBufOut);

		/// <summary>Create a <see cref="CMSampleBuffer" /> with this tagged buffer group.</summary>
		/// <param name="sampleBufferPts">The media time PTS of the sample buffer.</param>
		/// <param name="sampleBufferDuration">The media time duration of the sample buffer.</param>
		/// <param name="formatDescription">The format description describing this tagged buffer group. This format description may be created by calling <see cref="CreateFormatDescription" />.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A new sample buffer for this tagged buffer group, or null in case of failure.</returns>
		public CMSampleBuffer? CreateSampleBuffer (CMTime sampleBufferPts, CMTime sampleBufferDuration, CMFormatDescription formatDescription, out CMTaggedBufferGroupError status)
		{
			IntPtr handle;
			unsafe {
				status = CMSampleBufferCreateForTaggedBufferGroup (IntPtr.Zero, GetCheckedHandle (), sampleBufferPts, sampleBufferDuration, formatDescription.GetCheckedHandle (), &handle);
			}
			return CMSampleBuffer.Create (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr /* CMTaggedBufferGroupRef CM_NULLABLE */ CMSampleBufferGetTaggedBufferGroup (
			IntPtr /* CMSampleBufferRef CM_NONNULL */ sbuf);

		/// <summary>Get a sample buffer's tagged buffer group.</summary>
		/// <param name="sampleBuffer">The sample buffer whose tagged buffer group to get.</param>
		/// <returns>The tagged buffer group for the specified sample buffer, or null in case of failure or if the specified sample buffer doesn't contain a tagged buffer group.</returns>
		public static CMTaggedBufferGroup? GetTaggedBufferGroup (CMSampleBuffer sampleBuffer)
		{
			var handle = CMSampleBufferGetTaggedBufferGroup (sampleBuffer.GetNonNullHandle (nameof (sampleBuffer)));
			return Create (handle, false);
		}
#endif // COREBUILD
	}
}
