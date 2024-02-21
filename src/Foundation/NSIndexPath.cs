//
// NSIndexPath.cs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014, Xamarin Inc.
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {

	public partial class NSIndexPath {
		public unsafe static NSIndexPath Create (params nint [] indexes)
		{
			if (indexes is null)
				throw new ArgumentNullException ("indexes");

			fixed (nint* ptr = indexes)
				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}

		public unsafe static NSIndexPath Create (params nuint [] indexes)
		{
			if (indexes is null)
				throw new ArgumentNullException ("indexes");

			fixed (nuint* ptr = indexes)
				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}

		public unsafe static NSIndexPath Create (params int [] indexes)
		{
			if (indexes is null)
				throw new ArgumentNullException ("indexes");

#if ARCH_32
			fixed (int* ptr = indexes)
#else
			fixed (nint* ptr = Array.ConvertAll<int, nint> (indexes, (v) => v))
#endif

				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}

		public unsafe static NSIndexPath Create (params uint [] indexes)
		{
			if (indexes is null)
				throw new ArgumentNullException ("indexes");

#if ARCH_32
			fixed (uint* ptr = indexes)
#else
			fixed (nuint* ptr = Array.ConvertAll<uint, nuint> (indexes, (v) => v))
#endif

				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}

		public unsafe nuint [] GetIndexes ()
		{
			var ret = new nuint [Length];
			fixed (nuint* ptr = ret)
				_GetIndexes ((IntPtr) ptr);
			return ret;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public unsafe nuint [] GetIndexes (NSRange range)
		{
			var ret = new nuint [range.Length];
			fixed (nuint* ptr = ret)
				_GetIndexes ((IntPtr) ptr, range);
			return ret;
		}
	}
}
