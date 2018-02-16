//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012, Xamarin Inc.
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

using ObjCRuntime;

namespace Foundation {

	public partial class NSMutableSet : IEnumerable<NSObject> {
		public NSMutableSet (params NSObject [] objs)
			: this (NSArray.FromNSObjects (objs))
		{
		}

		public NSMutableSet (params string [] strings)
			: this (NSArray.FromStrings (strings))
		{
		}

		internal NSMutableSet (params INativeObject [] objs)
			: this (NSArray.FromNSObjects (objs))
		{
		}

		public static NSMutableSet operator + (NSMutableSet first, NSMutableSet second)
		{
			if (first == null || first.Count == 0)
				return new NSMutableSet (second);
			if (second == null || second.Count == 0)
				return new NSMutableSet (first);

			var copy = new NSMutableSet (first);
			copy.UnionSet (second);
			return copy;
		}

		public static NSMutableSet operator - (NSMutableSet first, NSMutableSet second)
		{
			if (first == null || first.Count == 0)
				return null;
			if (second == null || second.Count == 0)
				return new NSMutableSet (first);

			var copy = new NSMutableSet (first);
			copy.MinusSet (second);
			return copy;
		}
	}
}
