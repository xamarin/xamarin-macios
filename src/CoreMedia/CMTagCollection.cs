using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {
	/// <summary>A delegate that is used to iterate over a <see cref="CMTagCollection" />.</summary>
	/// <param name="tag">The tag to evaluate.</param>
	public delegate void CMTagCollectionApplyFunction (CMTag tag);

	/// <summary>A delegate that is used to filter when iterating over a <see cref="CMTagCollection" />.</summary>
	/// <param name="tag">The tag to evaluate.</param>
	/// <returns>True if the filter matches, false otherwise.</returns>
	public delegate bool CMTagCollectionTagFilterFunction (CMTag tag);

	/// <summary>An unordered collection of zero or more <see cref="CMTag" /> values.</summary>
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("tvos17.0")]
	public partial class CMTagCollection : NativeObject	{
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CMTagCollection (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CFTypeID */ nint CMTagCollectionGetTypeID ();

		/// <summary>Get this type's CFTypeID.</summary>
		public static nint GetTypeId ()
		{
			return CMTagCollectionGetTypeID ();
		}

		internal static CMTagCollection? Create (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				return null;
			return new CMTagCollection (handle, owns);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreate (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			CMTag* /* const CMTag * CM_NULLABLE */ tags,
			nint /* CMItemCount */ tagCount,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ newCollectionOut);

		/// <summary>Create a new tag collection for the specified tags.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <param name="tags">The tags in the new tag collection.</param>
		/// <returns>A new tag collection with the specified tags, or null in case of failure.</returns>
		public static CMTagCollection? Create (out CMTagCollectionError status, params CMTag[] tags)
		{
			IntPtr handle;
			unsafe {
				fixed (CMTag* tagPointer = tags) {
					status = CMTagCollectionCreate (IntPtr.Zero, tagPointer, tags?.Length ?? 0, &handle);
				}
			}
			return Create (handle, true);
		}

		/// <summary>Create a new tag collection for the specified tags.</summary>
		/// <param name="tags">The tags in the new tag collection.</param>
		/// <returns>A new tag collection with the specified tags, or null in case of failure.</returns>
		public static CMTagCollection? Create (params CMTag[] tags)
		{
			return Create (out var _, tags);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateMutable (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			nint /* CFIndex */ capacity,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMMutableTagCollectionRef CM_NULLABLE * CM_NONNULL */ newMutableCollectionOut);

		/// <summary>Create a mutable tag collection with the specified maximum capacity.</summary>
		/// <param name="capacity">The maximum capacity for the new collection, or 0 to have an unlimited capacity.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A mutable tag collection with the specified maximum capacity.</returns>
		public static CMTagCollection? CreateMutable (nint capacity, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateMutable (IntPtr.Zero, capacity, &handle);
			}
			return Create (handle, true);
		}

		/// <summary>Create a mutable tag collection with an unlimited capacity.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A mutable tag collection with an unlimited capacity.</returns>
		public static CMTagCollection? CreateMutable (out CMTagCollectionError status)
		{
			return CreateMutable (0, out status);
		}

		/// <summary>Create a mutable tag collection with an unlimited capacity.</summary>
		/// <returns>A mutable tag collection with an unlimited capacity.</returns>
		public static CMTagCollection? CreateMutable ()
		{
			return CreateMutable (0, out var _);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateCopy (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
				IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ newCollectionCopyOut);

		/// <summary>Create a copy of this tag collection.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A copy of this tag collection.</returns>
		public CMTagCollection? Copy (out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateCopy (GetCheckedHandle (), IntPtr.Zero, &handle);
			}
			return Create (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateMutableCopy (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
				IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ newCollectionCopyOut);

		/// <summary>Create a copy of this tag collection.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A copy of this tag collection.</returns>
		public CMTagCollection? CreateMutableCopy (out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateMutableCopy (GetCheckedHandle (), IntPtr.Zero, &handle);
			}
			return Create (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr /* CM_RETURNS_RETAINED CFStringRef */ CMTagCollectionCopyDescription (
				IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
				IntPtr /* CMTagCollectionRef */ tagCollection);

		/// <summary>Returns a description of the tag collection.</summary>
		/// <returns>A description of the tag collection.</returns>
		public override string? ToString ()
		{
			if (Handle == IntPtr.Zero)
				return null;
			IntPtr handle = CMTagCollectionCopyDescription (IntPtr.Zero, this.GetHandle ());
			return CFString.FromHandle (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static nint /* CMItemCount */ CMTagCollectionGetCount (
				IntPtr /* CMTagCollectionRef */ tagCollection);

		/// <summary>Get the number of tags in this tag collection.</summary>
		/// <returns>The number of tags in this tag collection.</returns>
		public nint Count {
			get => CMTagCollectionGetCount (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte /* Boolean */ CMTagCollectionContainsTag (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				CMTag tag);

		/// <summary>Checks if this tag collection contains the specified tag.</summary>
		/// <param name="tag">The tag to check for.</param>
		/// <returns>True if the tag collection contains the specified tag, false otherwise.</returns>
		public bool ContainsTag (CMTag tag)
		{
			return CMTagCollectionContainsTag (GetCheckedHandle (), tag) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte /* Boolean */ CMTagCollectionContainsTagsOfCollection (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				IntPtr /* CMTagCollectionRef CM_NONNULL */ containedTagCollection);

		/// <summary>Checks if all the tags in another tag collection is contained in this tag collection.</summary>
		/// <param name="tagCollection">The other tag collection whose tags should be checked for.</param>
		/// <returns>True if this tag collection contains all the tags in <paramref name="tagCollection" />, false otherwise.</returns>
		public bool ContainsTagCollection (CMTagCollection tagCollection)
		{
			return CMTagCollectionContainsTagsOfCollection (GetCheckedHandle (), tagCollection.GetNonNullHandle (nameof (tagCollection))) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static byte /* Boolean */ CMTagCollectionContainsSpecifiedTags (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				CMTag* /* const CMTag * CM_NONNULL */ containedTags,
				nint /* CMItemCount */ containedTagCount );

		/// <summary>Checks if this tag collection contains all the specified tags.</summary>
		/// <param name="tags">The tags to check for.</param>
		/// <returns>True if the tag collection contains all the specified tags, false otherwise.</returns>
		public bool ContainsTags (params CMTag[] tags)
		{
			if (tags is null)
				ThrowHelper.ThrowArgumentNullException (nameof (tags));

			unsafe {
				fixed (CMTag* tagPtr = tags)
					return CMTagCollectionContainsSpecifiedTags (GetCheckedHandle (), tagPtr, tags.Length) != 0;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static byte /* Boolean */ CMTagCollectionContainsCategory (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				CMTagCategory category);

		/// <summary>Checks if this tag collection contains any tags with the specified tag category.</summary>
		/// <param name="category">The tag category to check for.</param>
		/// <returns>True if the tag collection contains any tags with the specified tag category, false otherwise.</returns>
		public bool ContainsCategory (CMTagCategory category)
		{
			return CMTagCollectionContainsCategory (GetCheckedHandle (), category) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static nint /* CMItemCount */ CMTagCollectionGetCountOfCategory (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				CMTagCategory category);

		/// <summary>Count the number of tags that contain the specified tag category.</summary>
		/// <param name="category">The tag category to check for.</param>
		/// <returns>The number of tags that contain the specified tag category.</returns>
		public nint GetCount (CMTagCategory category)
		{
			return CMTagCollectionGetCountOfCategory (GetCheckedHandle (), category);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionGetTags (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				CMTag* /* CMTag * CM_NONNULL */ tagBuffer,
				nint /* CMItemCount */ tagBufferCount,
				nint* /* CMItemCount * CM_NULLABLE */ numberOfTagsCopied);

		/// <summary>Get all the tags in the current tag collection.</summary>
		/// <returns>All the tags in the current tag collection, or null in case of failure.</returns>
		public CMTag[]? Tags {
			get => GetTags (out var _);
		}

		/// <summary>Get all the tags in the current tag collection.</summary>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>All the tags in the current tag collection, or null case of failure (and <paramref name="status" /> will contain an error code).</returns>
		public CMTag[]? GetTags (out CMTagCollectionError status)
		{
			var array = new CMTag [Count];
			status = GetTags (array, array.Length, out var _);
			if (status != 0)
				return null;
			return array;
		}

		/// <summary>Get all the tags in the current tag collection.</summary>
		/// <param name="tags">The array where the tags will be copied to.</param>
		/// <param name="tagCount">The number of tags to copy.</param>
		/// <param name="tagsCopied">The number of tags copied.</param>
		/// <returns>An error code in case of failure, 0 in case of success. <see cref="CMTagCollectionError.ExhaustedBufferSize" /> is returned if the <paramref name="tags" /> array isn't big enough for all the tags (but as many tags as the array could hold were copied to the array).</returns>
		public CMTagCollectionError GetTags (CMTag[] tags, nint tagCount, out nint tagsCopied)
		{
			if (tags is null)
				ThrowHelper.ThrowArgumentNullException (nameof (tags));

			if (tagCount > tags.Length || tagCount < 0)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (tagCount), "Must not be higher than the length of the 'tags' array.");

			tagsCopied = 0;

			unsafe {
				fixed (CMTag* tagPtr = tags)
					return CMTagCollectionGetTags (GetCheckedHandle (), tagPtr, tagCount, (nint *) Unsafe.AsPointer<nint> (ref tagsCopied));
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionGetTagsWithCategory (
				IntPtr /* CMTagCollectionRef */ tagCollection,
				CMTagCategory category,
				CMTag* /* CMTag * CM_NONNULL */ tagBuffer,
				nint /* CMItemCount */ tagBufferCount,
				nint* /* CMItemCount * CM_NULLABLE */ numberOfTagsCopied);

		/// <summary>Get all the tags in the current tag collection with the specified category.</summary>
		/// <param name="category">The category of the tags to find.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>All the tags in the current tag collection with the specified category, or null case of failure (and <paramref name="status" /> will contain an error code).</returns>
		public CMTag[]? GetTags (CMTagCategory category, out CMTagCollectionError status)
		{
			var array = new CMTag [GetCount (category)];
			status = GetTags (category, array, array.Length, out var _);
			if (status != 0)
				return null;
			return array;
		}

		/// <summary>Get all the tags in the current tag collection with the specified category.</summary>
		/// <param name="category">The category of the tags to find.</param>
		/// <param name="tags">The array where the tags will be copied to.</param>
		/// <param name="tagCount">The number of tags to copy.</param>
		/// <param name="tagsCopied">The number of tags copied.</param>
		/// <returns>An error code in case of failure, 0 in case of success. <see cref="CMTagCollectionError.ExhaustedBufferSize" /> is returned if the <paramref name="tags" /> array isn't big enough for all the tags (but as many tags as the array could hold were copied to the array).</returns>
		public CMTagCollectionError GetTags (CMTagCategory category, CMTag[] tags, nint tagCount, out nint tagsCopied)
		{
			if (tags is null)
				ThrowHelper.ThrowArgumentNullException (nameof (tags));

			if (tagCount > tags.Length || tagCount < 0)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (tagCount), "Must not be higher than the length of the 'tags' array.");

			tagsCopied = 0;

			unsafe {
				fixed (CMTag* tagPtr = tags)
					return CMTagCollectionGetTagsWithCategory (GetCheckedHandle (), category, tagPtr, tagCount, (nint *) Unsafe.AsPointer<nint> (ref tagsCopied));
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static nint /* CMItemCount */ CMTagCollectionCountTagsWithFilterFunction (
				IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
				delegate* unmanaged<CMTag, IntPtr, byte> filterApplier,
				IntPtr /* void * CM_NULLABLE */ context);

		[UnmanagedCallersOnly]
		static byte CMTagCollectionTagFilterFunction_Callback (CMTag tag, IntPtr context)
		{
			var callback = (CMTagCollectionTagFilterFunction) GCHandle.FromIntPtr (context).Target!;
			var rv = callback (tag);
			return rv.AsByte ();
		}

		/// <summary>Count the number of tags that matches the specified filter.</summary>
		/// <param name="filter">The callback to call for each tag.</param>
		/// <returns>The number of tags that matches the specified filter.</returns>
		public nint GetCount (CMTagCollectionTagFilterFunction filter)
		{
			var gchandle = GCHandle.Alloc (filter);
			nint rv;
			unsafe {
				rv = CMTagCollectionCountTagsWithFilterFunction (GetCheckedHandle (), &CMTagCollectionTagFilterFunction_Callback, GCHandle.ToIntPtr (gchandle));
			}
			gchandle.Free ();
			return rv;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionGetTagsWithFilterFunction (
				IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
				CMTag* /* CMTag * CM_NONNULL */ tagBuffer,
				nint /* CMItemCount */ tagBufferCount,
				nint* /* CMItemCount * CM_NULLABLE */ numberOfTagsCopied,
				delegate* unmanaged<CMTag, IntPtr, byte> filter,
				IntPtr /* void * CM_NULLABLE */ context);


		/// <summary>Count the number of tags that matches the specified filter.</summary>
		/// <param name="filter">The callback to call for each tag.</param>
		/// <returns>The number of tags that matches the specified filter.</returns>
		/// <remarks>This will call the <paramref name="filter" /> function twice for each tag, once to count them, once again to return them.</remarks>
		public CMTag[]? GetTags (CMTagCollectionTagFilterFunction filter)
		{
			var array = new CMTag [GetCount (filter)];
			var status = GetTags (filter, array, array.Length, out var _);
			if (status != 0)
				return null;
			return array;
		}

		/// <summary>Get all the tags in the current tag collection that matches the specified filter.</summary>
		/// <param name="filter">The filter to call for each tag.</param>
		/// <param name="tags">The array where the tags will be copied to.</param>
		/// <param name="tagCount">The number of tags to copy.</param>
		/// <param name="tagsCopied">The number of tags copied.</param>
		/// <returns>An error code in case of failure, 0 in case of success. <see cref="CMTagCollectionError.ExhaustedBufferSize" /> is returned if the <paramref name="tags" /> array isn't big enough for all the tags (but as many tags as the array could hold were copied to the array).</returns>
		public CMTagCollectionError GetTags (CMTagCollectionTagFilterFunction filter, CMTag[] tags, nint tagCount, out nint tagsCopied)
		{
			if (filter is null)
				ThrowHelper.ThrowArgumentNullException (nameof (filter));

			if (tags is null)
				ThrowHelper.ThrowArgumentNullException (nameof (tags));

			if (tagCount > tags.Length || tagCount < 0)
				ThrowHelper.ThrowArgumentOutOfRangeException (nameof (tagCount), "Must not be higher than the length of the 'tags' array.");

			tagsCopied = 0;

			var gchandle = GCHandle.Alloc (filter);
			CMTagCollectionError rv;
			unsafe {
				fixed (CMTag* tagPtr = tags) {
					rv = CMTagCollectionGetTagsWithFilterFunction (
						GetCheckedHandle (),
						tagPtr,
						tagCount,
						(nint *) Unsafe.AsPointer<nint> (ref tagsCopied),
						&CMTagCollectionTagFilterFunction_Callback,
						GCHandle.ToIntPtr (gchandle));
				}
			}
			gchandle.Free ();
			return rv;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCopyTagsOfCategories (
				IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
				IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
				CMTagCategory* /* CMTagCategory * CM_NONNULL */ categories,
				nint /* CMItemCount */ categoriesCount,
				IntPtr* /* M_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ collectionWithTagsOfCategories);

		/// <summary>Create a copy of this tag collection, copying all tags that match the specified categories.</summary>
		/// <param name="categories">The categories to match.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A new tag collection, with all the tags matching the specified categories from this tag collection, or null in case of failure.</returns>
		public CMTagCollection? CreateWithCopyOfTags (out CMTagCollectionError status, params CMTagCategory[] categories)
		{
			if (categories is null)
				ThrowHelper.ThrowArgumentNullException (nameof (categories));

			IntPtr handle;
			unsafe {
				fixed (CMTagCategory* categoriesPtr = categories) {
					status = CMTagCollectionCopyTagsOfCategories (IntPtr.Zero, GetCheckedHandle (), categoriesPtr, categories.Length, &handle);
				}
			}
			return Create (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static void CMTagCollectionApply (
			IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
			delegate* unmanaged<CMTag, IntPtr, void> /* CMTagCollectionApplyFunction */ applier,
			IntPtr /* void * CM_NULLABLE */ context);

		[UnmanagedCallersOnly]
		static void CMTagCollectionApplyFunction_Callback (CMTag tag, IntPtr context)
		{
			var callback = (CMTagCollectionApplyFunction) GCHandle.FromIntPtr (context).Target!;
			callback (tag);
		}

		/// <summary>Iterate over all the tags in this tag collection, calling the provided callback function.</summary>
		/// <param name="callback">The callback function to call for each tag in this tag collection.</param>
		public void Apply (CMTagCollectionApplyFunction callback)
		{
			var gchandle = GCHandle.Alloc (callback);
			unsafe {
				CMTagCollectionApply (GetCheckedHandle (), &CMTagCollectionApplyFunction_Callback, GCHandle.ToIntPtr (gchandle));
			}
			gchandle.Free ();
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTag /* CMTag */ CMTagCollectionApplyUntil (
			IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection,
			delegate* unmanaged<CMTag, IntPtr, byte> /* CMTagCollectionTagFilterFunction */ applier,
			IntPtr /* void * CM_NULLABLE */ context);

		/// <summary>Iterate over all the tags in this tag collection, calling the provided callback function until the callback returns true.</summary>
		/// <param name="callback">The callback function to call for each tag in this tag collection (until the function returns true).</param>
		/// <returns>The tag that made the callback return true, or <see cref="CMTag.Invalid" /> if the callback function never returned true.</returns>
		public CMTag ApplyUntil (CMTagCollectionTagFilterFunction callback)
		{
			var gchandle = GCHandle.Alloc (callback);
			CMTag rv;
			unsafe {
				rv = CMTagCollectionApplyUntil (GetCheckedHandle (), &CMTagCollectionTagFilterFunction_Callback, GCHandle.ToIntPtr (gchandle));
			}
			gchandle.Free ();
			return rv;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static byte /* Boolean */ CMTagCollectionIsEmpty (
			IntPtr /* CMTagCollectionRef CM_NONNULL */ tagCollection);

		/// <summary>Returns true if the tag collection is empty.</summary>
		/// <returns>True if the tag collection is empty.</returns>
		/// <remarks>This is equivalent to checking if Count is 0.</remarks>
		public bool IsEmpty {
			get => CMTagCollectionIsEmpty (GetCheckedHandle ()) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateIntersection (
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection1,
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection2,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ tagCollection);

		/// <summary>Create a new tag collection with the intersection of the tags from two other tag collections.</summary>
		/// <param name="collection1">The first tag collection to get tags from.</param>
		/// <param name="collection2">The second tag collection to get tags from.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>The intersection of all the tags from the first and the second tag collections.</returns>
		public static CMTagCollection? Intersect (CMTagCollection? collection1, CMTagCollection? collection2, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateIntersection (collection1.GetHandle (), collection2.GetHandle (), &handle);
			}
			return Create (handle, true);
		}

		/// <summary>Create a new tag collection with the intersection of the tags from this tag collection and another tag collection.</summary>
		/// <param name="collection">The other tag collection to get tags from.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>The intersection of all the tags from this tag collection and the other tag collection.</returns>
		public CMTagCollection? Intersect (CMTagCollection? collection, out CMTagCollectionError status)
		{
			return Intersect (this, collection, out status);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateUnion (
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection1,
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection2,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ tagCollection);

		/// <summary>Create a new tag collection with the union of the tags from two other tag collections.</summary>
		/// <param name="collection1">The first tag collection to get tags from.</param>
		/// <param name="collection2">The second tag collection to get tags from.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>The union of all the tags from the first and the second tag collections.</returns>
		public static CMTagCollection? Union (CMTagCollection? collection1, CMTagCollection? collection2, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateUnion (collection1.GetHandle (), collection2.GetHandle (), &handle);
			}
			return Create (handle, true);
		}

		/// <summary>Create a new tag collection with the union of the tags from this tag collection and another tag collection.</summary>
		/// <param name="collection">The other tag collection to get tags from.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>The union of all the tags from this tag collection and the other tag collection.</returns>
		public CMTagCollection? Union (CMTagCollection? collection, out CMTagCollectionError status)
		{
			return Union (this, collection, out status);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateDifference (
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection1,
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection2,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ tagCollection);

		/// <summary>Create a new tag collection with the all the tags from the first tag collection that are not in the second tag collections.</summary>
		/// <param name="collection1">The tag collection to get tags from.</param>
		/// <param name="collection2">The tag collection whose tags not to include.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>All the tags from the first tag collection, except any tags in the second tag collection.</returns>
		public static CMTagCollection? Subtract (CMTagCollection? collection1, CMTagCollection? collection2, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateDifference (collection1.GetHandle (), collection2.GetHandle (), &handle);
			}
			return Create (handle, true);
		}

		/// <summary>Create a new tag collection with all the tags from the current tag collection that are not in the specified tag collection.</summary>
		/// <param name="collection">The tags not to include.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>All the tags from the current tag collection, except any tags in the specified tag collection.</returns>
		public CMTagCollection? Subtract (CMTagCollection? collection, out CMTagCollectionError status)
		{
			return Subtract (this, collection, out status);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateExclusiveOr (
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection1,
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection2,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ tagCollection);

		/// <summary>Create a new tag collection with the all the tags from the specified tag collections, except those tags in both collections.</summary>
		/// <param name="collection1">The first tag collection to get tags from.</param>
		/// <param name="collection2">The second tag collection to get tags from.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>All the tags from both tag collection, except those tags in both collections.</returns>
		public static CMTagCollection? ExclusiveOr (CMTagCollection? collection1, CMTagCollection? collection2, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateExclusiveOr (collection1.GetHandle (), collection2.GetHandle (), &handle);
			}
			return Create (handle, true);
		}

		/// <summary>Create a new tag collection with all the tags from the current tag collection and the tags from the specified tag collection, except those tags in both tag collections.</summary>
		/// <param name="collection">The other tag collection to get tags from.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>All the tags from both tag collection, except those tags in both collections.</returns>
		public CMTagCollection? ExclusiveOr (CMTagCollection? collection, out CMTagCollectionError status)
		{
			return ExclusiveOr (this, collection, out status);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionAddTag (
			IntPtr /* CMMutableTagCollectionRef CM_NULLABLE */ tagCollection,
			CMTag /* CMTag */ tagToAdd);

		/// <summary>Add a tag to the current tag collection.</summary>
		/// <param name="tag">The tag to add.</param>
		/// <returns>An error code in case of failure, 0 in case of success.</returns>
		/// <remarks>The tag is not added if the tag collection already contains it.</remarks>
		public CMTagCollectionError Add (CMTag tag)
		{
			return CMTagCollectionAddTag (GetCheckedHandle (), tag);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionRemoveTag (
			IntPtr /* CMMutableTagCollectionRef CM_NULLABLE */ tagCollection,
			CMTag /* CMTag */ tagToRemove);

		/// <summary>Remove a tag from the current tag collection.</summary>
		/// <param name="tag">The tag to remove.</param>
		/// <returns>An error code in case of failure, 0 in case of success.</returns>
		/// <remarks>If the tag doesn't exist, the returned value will be <see cref="CMTagCollectionError.TagNotFound" />.</remarks>
		public CMTagCollectionError Remove (CMTag tag)
		{
			return CMTagCollectionRemoveTag (GetCheckedHandle (), tag);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionRemoveAllTags (
			IntPtr /* CMMutableTagCollectionRef CM_NULLABLE */ tagCollection);

		/// <summary>Remove all the tags from the current tag collection.</summary>
		/// <returns>An error code in case of failure, 0 in case of success.</returns>
		public CMTagCollectionError RemoveAllTags ()
		{
			return CMTagCollectionRemoveAllTags (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionRemoveAllTagsOfCategory (
			IntPtr /* CMMutableTagCollectionRef CM_NULLABLE */ tagCollection,
			CMTagCategory /* CMTagCategory */ category);

		/// <summary>Remove all the tags from the current tag collection with the specified category.</summary>
		/// <param name="category">The category of tags to remove.</param>
		/// <returns>An error code in case of failure, 0 in case of success.</returns>
		public CMTagCollectionError RemoveAllTags (CMTagCategory category)
		{
			return CMTagCollectionRemoveAllTagsOfCategory (GetCheckedHandle (), category);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionAddTagsFromCollection (
			IntPtr /* CMMutableTagCollectionRef CM_NULLABLE */ tagCollection,
			IntPtr /* CMTagCollectionRef CM_NONNULL */ collectionWithTagsToAdd);

		/// <summary>Add all the tags from the specified tag collection to this tag collection.</summary>
		/// <param name="collection">The tag collection whose tags tags to add.</param>
		/// <returns>An error code in case of failure, 0 in case of success.</returns>
		public CMTagCollectionError Add (CMTagCollection collection)
		{
			return CMTagCollectionAddTagsFromCollection (GetCheckedHandle (), collection.GetNonNullHandle (nameof (collection)));
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionAddTagsFromArray (
			IntPtr /* CMMutableTagCollectionRef CM_NULLABLE */ tagCollection,
			CMTag* /* CMTag * CM_NONNULL */ tags,
			nint /* CMItemCount */ tagCount);

		/// <summary>Add all the specified tags this tag collection.</summary>
		/// <param name="tags">The tags to add.</param>
		/// <returns>An error code in case of failure, 0 in case of success.</returns>
		public CMTagCollectionError Add (params CMTag[] tags)
		{
			if (tags is null)
				ThrowHelper.ThrowArgumentNullException (nameof (tags));

			unsafe {
				fixed (CMTag* tagPtr = tags)
					return CMTagCollectionAddTagsFromArray (GetCheckedHandle (), tagPtr, tags.Length);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CM_RETURNS_RETAINED CFDictionaryRef CM_NULLABLE */ CMTagCollectionCopyAsDictionary (
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection,
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator);

		/// <summary>Serialize the tag collection to an <see cref="NSDictionary" /> instance.</summary>
		/// <returns>An <see cref="NSDictionary" /> instance with the serialized tag collection if successful, or null in case of failure.</returns>
		/// <remarks>Deserialize the <see cref="NSDictionary" /> instance using <see cref="Create(NSDictionary,out CMTagCollectionError)" />.</remarks>
		public NSDictionary? ToDictionary ()
		{
			var rv = CMTagCollectionCopyAsDictionary (GetCheckedHandle (), IntPtr.Zero);
			return Runtime.GetNSObject<NSDictionary> (rv, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateFromDictionary (
			IntPtr /* CFDictionaryRef CM_NONNULL */ dict,
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ newCollection);

		/// <summary>Deserialize a tag collection from the specified <see cref="NSDictionary" /> instance.</summary>
		/// <param name="dictionary">The <see cref="NSDictionary" /> instance of the data to use.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A new tag collection if successful, null in case of failure.</returns>
		/// <remarks>Create the <see cref="NSDictionary" /> instance using <see cref="ToDictionary" />.</remarks>
		public static CMTagCollection? Create (NSDictionary dictionary, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateFromDictionary (dictionary.GetNonNullHandle (nameof (dictionary)), IntPtr.Zero, &handle);
			}
			return Create (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CM_RETURNS_RETAINED CFDataRef CM_NULLABLE */ CMTagCollectionCopyAsData (
			IntPtr /* CMTagCollectionRef CM_NULLABLE */ tagCollection,
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator);

		/// <summary>Serialize the tag collection to an <see cref="NSData" /> instance.</summary>
		/// <returns>An <see cref="NSData" /> instance with the serialized tag collection if successful, or null in case of failure.</returns>
		/// <remarks>Deserialize the <see cref="NSData" /> instance using <see cref="Create(NSData,out CMTagCollectionError)" />.</remarks>
		public NSData? ToData ()
		{
			var rv = CMTagCollectionCopyAsData (GetCheckedHandle (), IntPtr.Zero);
			return Runtime.GetNSObject<NSData> (rv, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTagCollectionError /* OSStatus */ CMTagCollectionCreateFromData (
			IntPtr /* CFDataRef CM_NONNULL */ dict,
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ newCollection);

		/// <summary>Deserialize a tag collection from the specified <see cref="NSData" /> instance.</summary>
		/// <param name="data">The <see cref="NSData" /> instance of the data to use.</param>
		/// <param name="status">An error code in case of failure, 0 in case of success.</param>
		/// <returns>A new tag collection if successful, null in case of failure.</returns>
		/// <remarks>Create the <see cref="NSData" /> instance using <see cref="ToData" />.</remarks>
		public static CMTagCollection? Create (NSData data, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateFromData (data.GetNonNullHandle (nameof (data)), IntPtr.Zero, &handle);
			}
			return Create (handle, true);
		}
#endif // COREBUILD
	}
}
