// 
// RPBroadcastConfiguration.cs:
//
// Authors: Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2016 Xamarin Inc.
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
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Foundation;
using AVFoundation;

namespace ReplayKit {
	public partial class RPBroadcastConfiguration {
		public AVVideoCodecSettings VideoCompressionProperties {
			get {
				var weak = WeakVideoCompressionProperties; 
				return weak == null ? null : new AVVideoCodecSettings (new NSMutableDictionary (weak));
			}
			set {
#if XAMCORE_2_0
				WeakVideoCompressionProperties = value == null ? null : new NSDictionary<NSString, INSSecureCoding> (value.Dictionary.Handle);
#else
				WeakVideoCompressionProperties = value == null ? null : new NSDictionary (value.Dictionary.Handle);
#endif
			}
		}		
	}
}
