// 
// CALayer.cs: support for CALayer
//
// Authors:
//   Geoff Norton.
//     
// Copyright 2009-2010 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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

using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#endif
using CoreGraphics;

#nullable enable

namespace CoreAnimation {

	public partial class CALayer {
		const string selInitWithLayer = "initWithLayer:";

		[Export ("initWithLayer:")]
		public CALayer (CALayer other)
		{
			if (other is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (other));

			if (this.GetType () == typeof (CALayer)) {
				Messaging.IntPtr_objc_msgSend_IntPtr (Handle, Selector.GetHandle (selInitWithLayer), other.Handle);
			} else {
				Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, Selector.GetHandle (selInitWithLayer), other.Handle);
				Clone (other);
			}
			MarkDirtyIfDerived ();
		}

		void MarkDirtyIfDerived ()
		{
			if (GetType () == typeof (CALayer))
				return;
			MarkDirty (true);
		}

		public virtual void Clone (CALayer other)
		{
			// Subclasses must copy any instance values that they care from other
		}

		//    Work around race condition during CALayer[Delegate] destruction.  
		//
		//    The race condition goes as follows:
		//    
		//    * UIView.Layer.Delegate is set to a CALayerDelegate.
		//    * The GC schedules UIVIew and the CALayerDelegate for collection.
		//    * The GC collects, in this order:
		//      1. CALayerDelegate
		//      2. UIView
		//    * UIView's native dealloc method tries to use the delegate property.
		//    * The app crashes because the UILayerDelegate has been freed.
		//    
		//    The workaround will ensure that UIView.Layer.Delegate is nulled out when
		//    the CALayerDelegate is disposed, which will prevent the UIView from
		//    having a pointer to a freed object.
		WeakReference? calayerdelegate;
		void SetCALayerDelegate (CALayerDelegate? value)
		{
			// Remove ourselves from any existing CALayerDelegate.
			if (calayerdelegate is not null) {
				var del = (CALayerDelegate?) calayerdelegate.Target;
				if (del == value)
					return;
				del?.SetCALayer (null);
			}
			// Tell the new CALayerDelegate about ourselves
			if (value is null) {
				calayerdelegate = null;
			} else {
				calayerdelegate = new WeakReference (value);
				value.SetCALayer (this);
			}
		}

		// Note: preserving this member allows us to re-enable the `Optimizable` binding flag
		[Preserve (Conditional = true)]
		void OnDispose ()
		{
			if (calayerdelegate is not null) {
				var del = (CALayerDelegate?) calayerdelegate.Target;
				if (del is not null)
					WeakDelegate = null;
			}
		}

		public T? GetContentsAs<T> () where T : NSObject
		{
			return Runtime.GetNSObject<T> (_Contents);
		}

		public void SetContents (NSObject value)
		{
			_Contents = value.GetHandle ();
		}

#if MONOMAC && !NET
		[Obsolete ("Use 'AutoresizingMask' instead.")]
		public virtual CAAutoresizingMask AutoresizinMask { 
			get {
				return AutoresizingMask;
			}
			set {
				AutoresizingMask = value;
			}
		}
#endif
#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public CAContentsFormat ContentsFormat {
			get { return CAContentsFormatExtensions.GetValue (_ContentsFormat); }
			set { _ContentsFormat = value.GetConstant ()!; }
		}
	}

#if !MONOMAC
	public partial class CADisplayLink {
		NSActionDispatcher? dispatcher;

		public static CADisplayLink Create (Action action)
		{
			var dispatcher = new NSActionDispatcher (action);
			var rv = Create (dispatcher, NSActionDispatcher.Selector);
			rv.dispatcher = dispatcher;
			return rv;
		}
	}
#endif
}
