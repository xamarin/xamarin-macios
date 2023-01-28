//
// Copyright 2010, Novell, Inc.
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

#if __MACOS__

#nullable enable

using System;
using System.Runtime.Versioning;

namespace WebKit {

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class DomEventArgs : EventArgs {
		public DomEventArgs (DomEvent evt)
		{
			Event = evt;
		}
		public DomEvent Event { get; set; }
	}

	public delegate void DomEventListenerHandler (object sender, DomEventArgs args);

	public partial class DomNode {

		internal class DomNodeEventProxy : DomEventListener {
			DomEventListenerHandler handler;
			DomNode root;

			public DomNodeEventProxy (DomNode root, DomEventListenerHandler handler)
			{
				this.root = root;
				this.handler = handler;
			}

			public override void HandleEvent (DomEvent evt)
			{
				handler (root, new DomEventArgs (evt));
			}
		}

		internal class DomNodeEventProxy2 : DomEventListener {
			Action<DomEvent> callback;

			public DomNodeEventProxy2 (Action<DomEvent> callback)
			{
				this.callback = callback;
			}

			public override void HandleEvent (DomEvent evt)
			{
				callback (evt);
			}
		}

#if NET
		public IDomEventListener AddEventListener (string type, DomEventListenerHandler handler, bool useCapture)
#else
		public DomEventListener AddEventListener (string type, DomEventListenerHandler handler, bool useCapture)
#endif
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));
			var obj = new DomNodeEventProxy (this, handler);
			AddEventListener (type, obj, useCapture);
			return obj;
		}

#if NET
		public IDomEventListener AddEventListener (string type, Action<DomEvent> callback, bool useCapture)
#else
		public DomEventListener AddEventListener (string type, Action<DomEvent> callback, bool useCapture)
#endif
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));
			var obj = new DomNodeEventProxy2 (callback);
			AddEventListener (type, obj, useCapture);
			return obj;
		}
	}
}

#endif // __MACOS__
