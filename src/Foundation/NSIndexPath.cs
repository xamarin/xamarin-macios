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
#if !XAMCORE_2_0
		[Advice ("Use 'NSIndexPath.Create (int[])' instead.")]
		public unsafe NSIndexPath FromIndexes (uint [] indexes)
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			fixed (uint* ptr = indexes)
				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}
#endif

		public unsafe static NSIndexPath Create (params nint [] indexes)
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			fixed (nint* ptr = indexes)
				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}

		public unsafe static NSIndexPath Create (params nuint [] indexes)
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			fixed (nuint* ptr = indexes)
				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}
		
#if XAMCORE_2_0
		public unsafe static NSIndexPath Create (params int [] indexes)
		{
			if (indexes == null)
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
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

#if ARCH_32
			fixed (uint* ptr = indexes)
#else
			fixed (nuint* ptr = Array.ConvertAll<uint, nuint> (indexes, (v) => v))
#endif

				return _FromIndex ((IntPtr) ptr, indexes.Length);
		}
#endif

		public unsafe nuint [] GetIndexes ()
		{
			var ret = new nuint [Length];
			fixed (nuint *ptr = ret)
				_GetIndexes ((IntPtr) ptr);
			return ret;
		}

		[iOS (9,0), Mac(10,11)]
		public unsafe nuint [] GetIndexes (NSRange range)
		{
			var ret = new nuint [range.Length];
			fixed (nuint *ptr = ret)
				_GetIndexes ((IntPtr) ptr, range);
			return ret;
		}

#if !XAMCORE_2_0
		// in unified NSObject has the correct logic to handle Equals and GetHashCode correctly
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;

			NSIndexPath other = obj as NSIndexPath;
			if (other == null)
				return false;

			if ((object) other == (object) this)
				return true;

			if (other.Handle == Handle)
				return true;

			return Compare (other) == 0;
		}

		public override int GetHashCode ()
		{
			return Length.GetHashCode ();
		}
#endif
	}
}
