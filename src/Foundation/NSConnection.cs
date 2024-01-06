//
// NSConnection.cs
//
// Author:
//   Alex Corrado <corrado@xamarin.com>
//
// Copyright 2013 Xamarin Inc. (http://xamarin.com)
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
using System.Diagnostics.CodeAnalysis;

using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation
{
	public partial class NSConnection
	{

		public TProxy GetRootProxy<TProxy> () where TProxy : NSObject
		{
			return GetRootProxy<TProxy> (_GetRootProxy ());
		}

		public static TProxy GetRootProxy<TProxy> (string name, string hostName) where TProxy : NSObject
		{
			return GetRootProxy<TProxy> (_GetRootProxy (name, hostName));
		}

		public static TProxy GetRootProxy<TProxy> (string name, string hostName, NSPortNameServer server) where TProxy : NSObject
		{
			return GetRootProxy<TProxy> (_GetRootProxy (name, hostName, server));
		}

#if NET
		static TProxy GetRootProxy<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TProxy> (IntPtr handle) where TProxy : NSObject
#else
		static TProxy GetRootProxy<TProxy> (IntPtr handle) where TProxy : NSObject
#endif
		{
			var result = Runtime.TryGetNSObject (handle) as TProxy;

			if (result is null)
				result = (TProxy)Activator.CreateInstance (typeof (TProxy), new object[] { handle });

			return result;
		}
	}
}

#endif // MONOMAC
