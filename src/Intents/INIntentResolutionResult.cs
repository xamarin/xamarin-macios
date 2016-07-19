//
// INIntentResolutionResult Generic variant
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Intents {
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 10, 12, PlatformArchitecture.Arch64)]
	[Register ("INIntentResolutionResult", SkipRegistration = true)]
	public sealed partial class INIntentResolutionResult<ObjectType> : INIntentResolutionResult
		where ObjectType : class, INativeObject 
	{
		internal INIntentResolutionResult (IntPtr handle) : base (handle)
		{
		}
	}
}
#endif // XAMCORE_2_0
