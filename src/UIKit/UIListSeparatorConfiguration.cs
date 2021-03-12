//
// UIListSeparatorConfiguration.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#if IOS || WATCH
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	public partial class UIListSeparatorConfiguration {

		public static NSDirectionalEdgeInsets AutomaticInsets { get; private set; }

		static UIListSeparatorConfiguration ()
		{
				var lib = Libraries.UIKit.Handle;
				var ret = Dlfcn.dlsym (lib, "UIListSeparatorAutomaticInsets");
				AutomaticInsets = (NSDirectionalEdgeInsets) Marshal.PtrToStructure (ret, typeof (NSDirectionalEdgeInsets));
		}
	}
}
#endif // IOS || WATCH
