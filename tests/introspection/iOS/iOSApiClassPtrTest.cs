//
// Test fixture for class_ptr introspection tests
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc.
//
using System;
using System.Reflection;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace Introspection {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class iOSApiClassPtrTest : ApiClassPtrTest {

		protected override bool Skip (Type type)
		{
			switch (type.Namespace) {
			case "Phase": // missing in the sim
			case "ShazamKit": // missing in the sim
			case "ThreadNetwork": // missing in the sim
			case "PushToTalk": // missing in the sim
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
#if __WATCHOS__
			case "GameKit":
				if (IntPtr.Size == 4)
					return true;
				break;
#endif
			}

			// While the following types are categories and contains a class_ptr
			// they are not used at all as extensions since they are just used to expose 
			// static properties.
			switch (type.Name) {
			case "NSUrlUtilities_NSCharacterSet":
			case "AVAssetTrackTrackAssociation":
				return true;
			}
			return base.Skip (type);
		}
	}
}
