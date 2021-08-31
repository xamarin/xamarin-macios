//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc
//

#if !MONOMAC

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

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
		public CGImageMetadata CopyMetadata (nint index, NSDictionary options)
		{
			IntPtr o = options == null ? IntPtr.Zero : options.Handle;
			IntPtr result = CGImageSourceCopyMetadataAtIndex (Handle, index, o);
			return (result == IntPtr.Zero) ? null : new CGImageMetadata (result);
		}

#if !NET
		[iOS (7,0)]
#endif
		public CGImageMetadata CopyMetadata (nint index, CGImageOptions options)
		{
			NSDictionary o = null;
			if (options != null)
				o = options.ToDictionary ();
			var result = CopyMetadata (index, o);
			if (options != null)
				o.Dispose ();
			return result;
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
