// Copyright 2014 Xamarin Inc. All rights reserved.

using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using UIKit;

using Foundation;
using CoreMotion;
using ObjCRuntime;
using AVFoundation;
using FileProvider;
using CoreFoundation;


#if !MONOMAC && !WATCH

using System;

namespace Foundation {

	// NSIndexPath UIKit Additions Reference
	// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/NSIndexPath_UIKitAdditions/Reference/Reference.html
	public partial class NSIndexPath {

		// to avoid a lot of casting inside user source code we decided to expose `int` returning properties
		// https://trello.com/c/5SoMWz30/336-nsindexpath-expose-longrow-longsection-longitem-instead-of-changing-the-int-nature-of-them
		// their usage makes it very unlikely to ever exceed 2^31

		public int Row { 
			get { return (int) LongRow; }
		}

		public int Section { 
			get { return (int) LongSection; }
		}
	}
}

#endif // !MONOMAC && !WATCH

namespace ObjCRuntime {

	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	static partial class TrampolinesTest   {

		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		[UserDelegateType (typeof (Func<NSOrderedCollectionChange<INativeObject>, NSOrderedCollectionChange<INativeObject>>))]
		internal delegate IntPtr DFuncArity2V0Test<ObjectType>  (IntPtr block, IntPtr arg) where ObjectType : INativeObject;

		//
		// This class bridges native block invocations that call into C#
		//
		static internal class SDFuncArity2V0Test<ObjectType> where ObjectType : INativeObject {
			static internal readonly DFuncArity2V0Test<ObjectType> Handler = Invoke;

			[MonoPInvokeCallback (typeof (DFuncArity2V0Test<INativeObject>))]
			static unsafe IntPtr Invoke (IntPtr block, IntPtr arg) {
				var descriptor = (BlockLiteral *) block;
				var del = (Func<NSOrderedCollectionChange<ObjectType>, NSOrderedCollectionChange<ObjectType>>) (descriptor->Target);
				global::Foundation.NSOrderedCollectionChange<ObjectType> retval = del ( Runtime.GetNSObject<global::Foundation.NSOrderedCollectionChange<ObjectType>> (arg));
				return retval != null ? retval.Handle : IntPtr.Zero;
			}
		} /* class SDFuncArity2V0 */
	}
}
