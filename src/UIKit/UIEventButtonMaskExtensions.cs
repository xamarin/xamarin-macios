//
// UIEventButtonMaskExtensions.cs
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
#if NET
	[SupportedOSPlatform ("ios13.4")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[Introduced (PlatformName.iOS, 13,4, PlatformArchitecture.All)]
#endif
	[BindingImpl (BindingImplOptions.Optimizable)]
	public static partial class UIEventButtonMaskExtensions {

		[DllImport (Constants.UIKitLibrary)]
		static extern nuint UIEventButtonMaskForButtonNumber (nint buttonNumber);

		public static UIEventButtonMask Convert (nint buttonNumber)
		{
			return (UIEventButtonMask) (ulong) UIEventButtonMaskForButtonNumber (buttonNumber);
		}
	}
}
#endif // IOS
