using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {
	/// <summary>A structure that is used to add additional data (tags) to a resource.</summary>
	[SupportedOSPlatform ("ios17.0")]
	[SupportedOSPlatform ("maccatalyst17.0")]
	[SupportedOSPlatform ("macos14.0")]
	[SupportedOSPlatform ("tvos17.0")]
	public struct CMTag {
#if COREBUILD
#pragma warning disable CS0169 //  The field 'CMTag.*' is never used
		uint /* CMTagCategory */ category;
		uint /* CMTagDataType */ dataType;
		ulong /* CMTagValue */ value;
#pragma warning restore CS0169
#else
		CMTagCategory /* uint */ category;
		CMTagDataType /* uint */ dataType;
		ulong /* CMTagValue */ value;
#endif

#if !COREBUILD
		/// <summary>The category for this tag.</summary>
		/// <returns>The category for this tag.</returns>
		public CMTagCategory Category {
			get => (CMTagCategory) category;
		}

		/// <summary>The data type for this tag.</summary>
		/// <returns>The data type for this tag.</returns>
		public CMTagDataType DataType {
			get => CMTagGetValueDataType (this);
		}

		/// <summary>The raw 64-bit value of the data for this tag.</summary>
		/// <returns>The raw 64-bit value of the data for this tag.</returns>
		public ulong Value {
			get => this.value;
		}

		/// <summary>Checks if the tag is valid.</summary>
		/// <returns>True if the tag is valid, false otherwise.</returns>
		/// <remarks>A tag is valid as long as DataType != CMTagDataType.Invalid.</remarks>
		public bool IsValid {
			get => dataType != CMTagDataType.Invalid;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTagDataType CMTagGetValueDataType (CMTag tag);

		/// <inheritdoc cref="CMTagConstants.Invalid" />
		public static CMTag Invalid { get => CMTagConstants.Invalid; }

		/// <inheritdoc cref="CMTagConstants.MediaTypeVideo" />
		public static CMTag MediaTypeVideo { get => CMTagConstants.MediaTypeVideo; }

		/// <inheritdoc cref="CMTagConstants.MediaSubTypeMebx" />
		public static CMTag MediaSubTypeMebx { get => CMTagConstants.MediaSubTypeMebx; }

		/// <inheritdoc cref="CMTagConstants.MediaTypeAudio" />
		public static CMTag MediaTypeAudio { get => CMTagConstants.MediaTypeAudio; }

		/// <inheritdoc cref="CMTagConstants.MediaTypeMetadata" />
		public static CMTag MediaTypeMetadata { get => CMTagConstants.MediaTypeMetadata; }

		/// <inheritdoc cref="CMTagConstants.StereoLeftEye" />
		public static CMTag StereoLeftEye { get => CMTagConstants.StereoLeftEye; }

		/// <inheritdoc cref="CMTagConstants.StereoRightEye" />
		public static CMTag StereoRightEye { get => CMTagConstants.StereoRightEye; }

		/// <inheritdoc cref="CMTagConstants.StereoLeftAndRightEye" />
		public static CMTag StereoLeftAndRightEye { get => CMTagConstants.StereoLeftAndRightEye; }

		/// <inheritdoc cref="CMTagConstants.StereoNone" />
		public static CMTag StereoNone { get => CMTagConstants.StereoNone; }

		/// <inheritdoc cref="CMTagConstants.StereoInterpretationOrderReversed" />
		public static CMTag StereoInterpretationOrderReversed { get => CMTagConstants.StereoInterpretationOrderReversed; }

		/// <inheritdoc cref="CMTagConstants.ProjectionTypeRectangular" />
		public static CMTag ProjectionTypeRectangular { get => CMTagConstants.ProjectionTypeRectangular; }

		/// <inheritdoc cref="CMTagConstants.ProjectionTypeEquirectangular" />
		public static CMTag ProjectionTypeEquirectangular { get => CMTagConstants.ProjectionTypeEquirectangular; }

		/// <inheritdoc cref="CMTagConstants.ProjectionTypeHalfEquirectangular" />
		public static CMTag ProjectionTypeHalfEquirectangular { get => CMTagConstants.ProjectionTypeHalfEquirectangular; }

		/// <inheritdoc cref="CMTagConstants.ProjectionTypeFisheye" />
		public static CMTag ProjectionTypeFisheye { get => CMTagConstants.ProjectionTypeFisheye; }

		/// <inheritdoc cref="CMTagConstants.PackingTypeNone" />
		public static CMTag PackingTypeNone { get => CMTagConstants.PackingTypeNone; }

		/// <inheritdoc cref="CMTagConstants.PackingTypeSideBySide" />
		public static CMTag PackingTypeSideBySide { get => CMTagConstants.PackingTypeSideBySide; }

		/// <inheritdoc cref="CMTagConstants.PackingTypeOverUnder" />
		public static CMTag PackingTypeOverUnder { get => CMTagConstants.PackingTypeOverUnder; }

		[DllImport (Constants.CoreMediaLibrary)]
		static extern byte CMTagHasSInt64Value (CMTag tag);

		/// <summary>Checks whether the tag contains a signed 64-bit value.</summary>
		/// <returns>True if the tag has a signed 64-bit value, false otherwise.</returns>
		public bool HasInt64Value {
			get => CMTagHasSInt64Value (this) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern long CMTagGetSInt64Value (CMTag tag);

		/// <summary>Gets the signed 64-bit value for this tag.</summary>
		/// <returns>The signed 64-bit value for this tag.</returns>
		/// <remarks>The return value is undefined if the tag's data type isn't <see cref="CMTagDataType.SInt64" />.</remarks>
		public long Int64Value {
			get => CMTagGetSInt64Value (this);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern byte CMTagHasFloat64Value (CMTag tag);

		/// <summary>Checks whether the tag contains a 64-bit floating point value.</summary>
		/// <returns>True if the tag has a 64-bit floating point value, false otherwise.</returns>
		public bool HasFloat64Value {
			get => CMTagHasFloat64Value (this) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern double CMTagGetFloat64Value (CMTag tag);

		/// <summary>Gets the 64-bit floating point value for this tag.</summary>
		/// <returns>The 64-bit floating point value for this tag.</returns>
		/// <remarks>The return value is undefined if the tag's data type isn't <see cref="CMTagDataType.Float64" />.</remarks>
		public double Float64Value {
			get => CMTagGetFloat64Value (this);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern byte CMTagHasOSTypeValue (CMTag tag);

		/// <summary>Checks whether the tag contains a 64-bit floating point value.</summary>
		/// <returns>True if the tag has a 64-bit floating point value, false otherwise.</returns>
		public bool HasOSTypeValue {
			get => CMTagHasOSTypeValue (this) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern uint CMTagGetOSTypeValue (CMTag tag);

		/// <summary>Gets the OSType value for this tag.</summary>
		/// <returns>The OSType value for this tag.</returns>
		/// <remarks>The return value is undefined if the tag's data type isn't <see cref="CMTagDataType.OSType" />.</remarks>
		public uint OSTypeValue {
			get => CMTagGetOSTypeValue (this);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern byte CMTagHasFlagsValue (CMTag tag);

		/// <summary>Checks whether the tag contains a 64-bit floating point value.</summary>
		/// <returns>True if the tag has a 64-bit floating point value, false otherwise.</returns>
		public bool HasFlagsValue {
			get => CMTagHasFlagsValue (this) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern ulong CMTagGetFlagsValue (CMTag tag);

		/// <summary>Gets the flags value for this tag.</summary>
		/// <returns>The flags value for this tag.</returns>
		/// <remarks>The return value is undefined if the tag's data type isn't <see cref="CMTagDataType.Flags" />.</remarks>
		public ulong FlagsValue {
			get => CMTagGetFlagsValue (this);
		}


		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTag CMTagMakeWithSInt64Value (CMTagCategory category, long value);

		/// <summary>Create a new tag with the specified signed 64-bit value.</summary>
		/// <param name="category">The category for the new tag.</param>
		/// <param name="value">The signed 64-bit value for the new tag.</param>
		/// <returns>A new tag with the specified tag and signed 64-bit value.</returns>
		public static CMTag CreateWithSInt64Value (CMTagCategory category, long value)
		{
			return CMTagMakeWithSInt64Value (category, value);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTag CMTagMakeWithFloat64Value (CMTagCategory category, double value);

		/// <summary>Create a new tag with the specified 64-bit floating point value.</summary>
		/// <param name="category">The category for the new tag.</param>
		/// <param name="value">The 64-bit floating point value for the new tag.</param>
		/// <returns>A new tag with the specified tag and 64-bit floating point value.</returns>
		public static CMTag CreateWithFloat64Value (CMTagCategory category, double value)
		{
			return CMTagMakeWithFloat64Value (category, value);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTag CMTagMakeWithOSTypeValue (CMTagCategory category, uint value);

		/// <summary>Create a new tag with the specified OSType value.</summary>
		/// <param name="category">The category for the new tag.</param>
		/// <param name="osTypeValue">The OSType value for the new tag.</param>
		/// <returns>A new tag with the specified tag and OSType value.</returns>
		public static CMTag CreateWithOSTypeValue (CMTagCategory category, uint osTypeValue)
		{
			return CMTagMakeWithOSTypeValue (category, osTypeValue);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTag CMTagMakeWithFlagsValue (CMTagCategory category, ulong flagsForTag);

		/// <summary>Create a new tag with the specified flags.</summary>
		/// <param name="category">The category for the new tag.</param>
		/// <param name="flags">The flags for the new tag.</param>
		/// <returns>A new tag with the specified tag and flags.</returns>
		public static CMTag CreateWithFlagsValue (CMTagCategory category, ulong flags)
		{
			return CMTagMakeWithFlagsValue (category, flags);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern byte CMTagEqualToTag (CMTag tag1, CMTag tag2);

		/// <summary>Checks if two tags are equal.</summary>
		/// <param name="tag1">The first tag to compare for equality.</param>
		/// <param name="tag2">The second tag to compare for equality.</param>
		/// <returns>True if both tags are equal, false otherwise.</returns>
		public static bool Equals (CMTag tag1, CMTag tag2)
		{
			return CMTagEqualToTag (tag1, tag2) != 0;
		}

		/// <inheritdoc />
		public override bool Equals (object? obj)
		{
			if (obj is CMTag tag)
				return Equals (this, tag);
			return false;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern nint CMTagCompare (CMTag tag1, CMTag tag2);

		/// <summary>Compares two tags and returns a result suitable for ordering.</summary>
		/// <param name="tag1">The first tag to compare.</param>
		/// <param name="tag2">The second tag to compare.</param>
		/// <returns>A <see cref="CFComparisonResult" /> value for the result of the comparison.</returns>
		public static CFComparisonResult Compare (CMTag tag1, CMTag tag2)
		{
			return (CFComparisonResult) (long) CMTagCompare (tag1, tag2);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern nuint CMTagHash (CMTag tag);

		/// <summary>Gets a hash code for this tag.</summary>
		/// <returns>A hash code for this tag.</returns>
		public override int GetHashCode ()
		{
			unchecked {
				return (int) CMTagHash (this);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr /* CM_RETURNS_RETAINED CFStringRef */ CMTagCopyDescription (
				IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
				CMTag /* CMTag */ tag);

		/// <summary>Returns a description of the tag.</summary>
		/// <returns>A description of the tag.</returns>
		public override string? ToString ()
		{
			var handle = CMTagCopyDescription (IntPtr.Zero, this);
			return CFString.FromHandle (handle, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static IntPtr /* CFDictionaryRef CM_NULLABLE */ CMTagCopyAsDictionary (
			CMTag /* CMTag */ tag,
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator);

		/// <summary>Serialize the tag to an <see cref="NSDictionary" /> instance.</summary>
		/// <returns>An <see cref="NSDictionary" /> instance with the serialized tag if successful, or null in case of failure.</returns>
		/// <remarks>Deserialize the <see cref="NSDictionary" /> instance using <see cref="Create(NSDictionary)" />.</remarks>
		public NSDictionary? ToDictionary ()
		{
			var rv = CMTagCopyAsDictionary (this, IntPtr.Zero);
			return Runtime.GetNSObject<NSDictionary> (rv, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static CMTag CMTagMakeFromDictionary (
			IntPtr /* CFDictionaryRef CM_NONNULL */ dict);

		/// <summary>Deserialize a tag collection from the specified <see cref="NSDictionary" /> instance.</summary>
		/// <param name="dictionary">The <see cref="NSDictionary" /> instance of the data to use.</param>
		/// <returns>The deserialized tag, or <see cref="Invalid" /> in case of failure.</returns>
		/// <remarks>Create the <see cref="NSDictionary" /> instance using <see cref="ToDictionary" />.</remarks>
		public static CMTag? Create (NSDictionary dictionary)
		{
			return CMTagMakeFromDictionary (dictionary.GetNonNullHandle (nameof (dictionary)));
		}
#endif // COREBUILD
	}
}
