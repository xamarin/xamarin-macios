//
// AdoptsAttribute.cs: Attribute applied to classes to specify that the
// class implements a given protocol
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011 Xamarin Inc.
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
using System.Runtime.InteropServices;

namespace ObjCRuntime {

	[AttributeUsage (AttributeTargets.Class, AllowMultiple = true)]
	public sealed class AdoptsAttribute : Attribute {
#if !COREBUILD
		IntPtr handle;

		public AdoptsAttribute (string protocolType)
		{
			ProtocolType = protocolType;
		}

		/// <summary>The name of the protocol type adopted.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public string ProtocolType {
			get; private set;
		}

		/// <summary>Returns the underlying handle to the Protocol.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public IntPtr ProtocolHandle {
			get {
				if (handle == IntPtr.Zero && ProtocolType is not null)
					handle = Runtime.GetProtocol (ProtocolType);

				return handle;
			}
		}
#endif
	}
}
