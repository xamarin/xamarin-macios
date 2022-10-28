//
// UIListSeparatorConfiguration.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#if IOS
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace UIKit {
	public partial class UIListSeparatorConfiguration {

		static NSDirectionalEdgeInsets? automaticInsets = null;
		public static NSDirectionalEdgeInsets AutomaticInsets {
			get {
				if (automaticInsets == null) {
					var lib = Libraries.UIKit.Handle;
					var ret = Dlfcn.dlsym (lib, "UIListSeparatorAutomaticInsets");
					automaticInsets = (NSDirectionalEdgeInsets) Marshal.PtrToStructure (ret, typeof (NSDirectionalEdgeInsets));
				}

				return automaticInsets.Value;
			}
		}
	}
}
#endif // IOS
