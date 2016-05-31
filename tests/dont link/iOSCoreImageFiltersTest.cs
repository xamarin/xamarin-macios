//
// Test the generated API for all iOS CoreImage filters
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//	Alex Soto <alex.soto@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Reflection;
#if XAMCORE_2_0
using CoreImage;
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.CoreImage;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;
using TouchUnit.Bindings;

namespace MonoTouchFixtures {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSCoreImageFiltersTest : ApiCoreImageFiltersTest  {


	}
}
#endif // !__WATCHOS__
