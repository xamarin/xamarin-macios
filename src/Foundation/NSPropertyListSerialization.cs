//
// NSPropertyListSerialization.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc
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

#if MONOMAC

using System;

namespace Foundation
{
	public partial class NSPropertyListSerialization
	{
		public static NSData DataWithPropertyList (NSObject plist, NSPropertyListFormat format, out NSError error)
		{
			return DataWithPropertyList (plist, format, NSPropertyListWriteOptions.Immutable, out error);
		}

		public static nint WritePropertyList (NSObject plist, NSOutputStream stream, NSPropertyListFormat format, out NSError error)
		{
			return WritePropertyList (plist, stream, format, NSPropertyListWriteOptions.Immutable, out error);
		}

		public static NSObject PropertyListWithData (NSData data, ref NSPropertyListFormat format, out NSError error)
		{
			return PropertyListWithData (data, NSPropertyListReadOptions.Immutable, ref format, out error);
		}

		public static NSObject PropertyListWithStream (NSInputStream stream, ref NSPropertyListFormat format, out NSError error)
		{
			return PropertyListWithStream (stream, NSPropertyListReadOptions.Immutable, ref format, out error);
		}
	}
}

#endif
