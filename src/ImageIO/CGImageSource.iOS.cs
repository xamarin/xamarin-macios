//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc
//

#nullable enable

#if !MONOMAC

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

namespace ImageIO {
	
	public partial class CGImageSource {

		// CGImageSource.h
		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageMetadataRef __nullable */ IntPtr CGImageSourceCopyMetadataAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* size_t */ nint idx,
			/* CFDictionaryRef __nullable */ IntPtr options);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
#if !NET
		[iOS (7,0)]
#endif
		public CGImageMetadata? CopyMetadata (nint index, NSDictionary? options)
		{
			var result = CGImageSourceCopyMetadataAtIndex (Handle, index, options.GetHandle ());
			return (result == IntPtr.Zero) ? null : new CGImageMetadata (result, true);
		}

#if !NET
		[iOS (7,0)]
#endif
		public CGImageMetadata? CopyMetadata (nint index, CGImageOptions? options)
		{
			using var o = options?.ToDictionary ();
			return CopyMetadata (index, o);
		}

		// CGImageSource.h
#if !NET
		[iOS (7,0)]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageSourceRemoveCacheAtIndex (/* CGImageSourceRef __nonnull */ IntPtr isrc,
			/* size_t */ nint index);

#if !NET
		[iOS (7,0)]
#endif
		public void RemoveCache (nint index)
		{
			CGImageSourceRemoveCacheAtIndex (Handle, index);
		}
	}
}

#endif // !MONOMAC
