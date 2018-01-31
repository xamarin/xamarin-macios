//
// Unit tests for the linker's OptimizeGeneratedCodeSubStep.
//
// This class is included in both Xamarin.Mac and Xamarin.iOS's link all tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace Linker.Shared
{
	[Preserve (AllMembers = true)]
	public class BaseOptimizeGeneratedCodeTest
	{
	}
}
