//
// CLLocationDistance.cs: Type wrapper for CLLocationDistance
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2013, Xamarin, Inc
//
// The class can be either constructed from a string (from user code)
// or from a handle (from iphone-sharp.dll internal calls).  This
// delays the creation of the actual managed string until actually
// required
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

// moved to corelocation.cs
#if false

using System;
using ObjCRuntime;

namespace CoreLocation {

	public static class CLLocationDistance
	{
		static double? max_distance;
		static double? filter_none;
		
		[iOS (6, 0)]
		public static double MaxDistance {
			get {
				if (max_distance == null)
					max_distance = Dlfcn.GetDouble (Libraries.CoreLocation.Handle, "CLLocationDistanceMax");
				return max_distance.Value; 
			}
		}

		public static double FilterNone {
			get {
				if (filter_none == null)
					filter_none = Dlfcn.GetDouble (Libraries.CoreLocation.Handle, "kCLDistanceFilterNone");
				return filter_none.Value;
			}
		}
	}
}

#endif