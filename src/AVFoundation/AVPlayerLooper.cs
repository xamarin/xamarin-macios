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

#if !WATCH

using Foundation;
using CoreFoundation;
using AudioToolbox;
using ObjCRuntime;
using System;

#nullable enable

namespace AVFoundation {

	public partial class AVPlayerLooper {

#if !NET // This API got introduced in Xcode 8.0 binding but is not currently present nor in Xcode 8.3 or Xcode 9.0 needs research

		bool loopingEnabled = true;

		[Obsolete ("This selector does not exist in the header and was wrongly added.")]
		public virtual bool LoopingEnabled {
			get {
				return loopingEnabled;
			}
		}
#endif
	}
}
#endif // !WATCH
