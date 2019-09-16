//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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

using System;
using System.Reflection;
using ObjCRuntime;

namespace Foundation {
	public partial class NSXpcInterface : NSObject {
		public static NSXpcInterface CreateForType (Type interfaceType)
		{
			if (!interfaceType.IsInterface)
			{
				throw new InvalidOperationException ($"Type {interfaceType.FullName} must be an interface type");
			}

			ProtocolAttribute attribute = interfaceType.GetCustomAttribute<ProtocolAttribute> ();
			if (attribute == null)
			{
				throw new InvaidOperationException ($"Type {interfaceType.FullName} is not annotated with ProtocolAttribute");
			}

			if (string.IsNullOrEmpty (attribute.Name))
			{
				throw new InvalidOperationException ($"Type {interfaceType.FullName} does not have an explicit protocol name");
			}

			return CreateForProtocol (new Protocol (interfaceType));
		}
	}
}
