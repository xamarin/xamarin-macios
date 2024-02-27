//
// Copyright 2015 Xamarin Inc (http://www.xamarin.com)
//
// This file contains a generic version of NSSet.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[Register ("NSSet", SkipRegistration = true)]
	public sealed class NSSet<TKey> : NSSet, IEnumerable<TKey>
		where TKey : class, INativeObject {
		public NSSet ()
		{
		}

		public NSSet (NSCoder coder)
			: base (coder)
		{
		}

		internal NSSet (NativeHandle handle)
			: base (handle)
		{
		}

		public NSSet (params TKey [] objs)
			: base (objs)
		{
		}

		public NSSet (NSSet<TKey> other)
			: base (other)
		{
		}

		public NSSet (NSMutableSet<TKey> other)
			: base (other)
		{
		}

		// Strongly typed versions of API from NSSet

		public TKey LookupMember (TKey probe)
		{
			if (probe is null)
				throw new ArgumentNullException (nameof (probe));

			return Runtime.GetINativeObject<TKey> (_LookupMember (probe.Handle), false);
		}

		public TKey AnyObject {
			get {
				return Runtime.GetINativeObject<TKey> (_AnyObject, false);
			}
		}

		public bool Contains (TKey obj)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			return _Contains (obj.Handle);
		}

		public TKey [] ToArray ()
		{
			return base.ToArray<TKey> ();
		}

		public static NSSet<TKey> operator + (NSSet<TKey> first, NSSet<TKey> second)
		{
			if (first is null || first.Count == 0)
				return new NSSet<TKey> (second);
			if (second is null || second.Count == 0)
				return new NSSet<TKey> (first);
			return new NSSet<TKey> (first._SetByAddingObjectsFromSet (second.Handle));
		}

		public static NSSet<TKey> operator - (NSSet<TKey> first, NSSet<TKey> second)
		{
			if (first is null || first.Count == 0)
				return null;
			if (second is null || second.Count == 0)
				return new NSSet<TKey> (first);
			var copy = new NSMutableSet<TKey> (first);
			copy.MinusSet (second);
			return new NSSet<TKey> (copy);
		}

		#region IEnumerable<TKey>
		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator ()
		{
			return new NSFastEnumerator<TKey> (this);
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new NSFastEnumerator<TKey> (this);
		}
		#endregion
	}
}
