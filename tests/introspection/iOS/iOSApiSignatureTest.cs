//
// iOS tests for the generated API selectors against typos or non-existing cases
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;

using NUnit.Framework;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiSignatureTest : ApiSignatureTest {

		public iOSApiSignatureTest ()
		{
			ContinueOnFailure = true;
			//LogProgress = true;
		}

		protected override int Size (Type t, bool simd = false)
		{
			switch (t.Name) {
			// rdar 21375616 - Breaking change with EventKit[UI] enum base type
			// EventKit.EK* enums are anonymous enums in 10.10 and iOS 8, but an NSInteger in 10.11 and iOS 9.
			case "EKCalendarChooserSelectionStyle":
			case "EKCalendarChooserDisplayStyle":
				if (!TestRuntime.CheckXcodeVersion (7, 0))
					return 4;
				break;
			}
			return base.Size (t, simd);
		}

		protected override bool Skip (Type type, MethodBase method, string selector)
		{
			switch (type.Name) {
			case "UIAlertView":
				// variable length parameters ...
				if (selector == "initWithTitle:message:delegate:cancelButtonTitle:otherButtonTitles:")
					return true;
				break;
			case "UIViewController":
				switch (selector) {
				// offically added in 6.0 - but retroactively supported in iOS5 (including documentation)
				// the code works (see monotouch-test.app) but Apple changed their return value in 6.0
				// (they used to return `bool`)
				case "beginAppearanceTransition:animated:":
				case "endAppearanceTransition":
					if (!TestRuntime.CheckXcodeVersion (4, 5))
						return true;
					break;
				}
				break;
			case "NKIssue":
				// Apple "promoted" this to `NSInteger` in iOS8 but this already existed (as a 32bits value) in iOS 7.x
				// sadly, and even with a bug report with a few exchanges, this was not fixed before iOS8 GM :-(
				// 64bits application for iOS 7.x will be uncommon so we prefer to be forward compatible
				if (selector == "status")
					return !TestRuntime.CheckXcodeVersion (6, 0);
				break;
			case "CMMotionManager":
				// iOS 8.3 changed CMMotionManager from 4 to 8 bytes on 64bits CPU and we have to follow that breaking
				// change unless Apple revert that before final. [radar 20295259]
				if ((IntPtr.Size == 4) || TestRuntime.CheckXcodeVersion (6, 3))
					return false;
				// which means iOS 8.2 (and earlier can't match)
				switch (selector) {
				case "startDeviceMotionUpdatesUsingReferenceFrame:":
				case "startDeviceMotionUpdatesUsingReferenceFrame:toQueue:withHandler:":
				case "attitudeReferenceFrame":
					return true;
				default:
					return false;
				}
			}

			return base.Skip (type, method, selector);
		}

		protected override bool IsValidStruct (Type type, string structName)
		{
			switch (structName) {
			// CIImage 'static MonoTouch.CoreImage.CIImage FromImageBuffer(MonoTouch.CoreVideo.CVPixelBuffer)' selector: imageWithCVPixelBuffer: == @12@0:4^{__CVBuffer=}
			case "__CVBuffer":
				return type.Name == "CVPixelBuffer" || type.Name == "CVImageBuffer";
			}
			return base.IsValidStruct (type, structName);
		}

		// only handle exception here (to return true) otherwise call base to deal with it
		// `caller` is provided to make it easier to detect "special" cases
		protected override bool Check (char encodedType, Type type)
		{
			// return an error if null (instead of throwing) so we can continue execution
			if (type is null)
				return false;

			switch (encodedType) {
			case 'c': // char, used for C# bool
				break;
			// float (32 bits)
			case 'f':
				switch (type.FullName) {
				// documented (web and header file) as NSInteger
				// UIImageView 'instance Void set_AnimationRepeatCount(Int32)' selector: setAnimationRepeatCount: == v12@0:4f8
				case "System.Int32":
					return CurrentType.FullName == "MonoTouch.UIKit.UIImageView";
				}
				break;
			case 'i':
				switch (type.FullName) {
				case "MonoTouch.EventKitUI.EKCalendarChooserSelectionStyle":
				case "MonoTouch.EventKitUI.EKCalendarChooserDisplayStyle":
				case "EventKitUI.EKCalendarChooserSelectionStyle":
				case "EventKitUI.EKCalendarChooserDisplayStyle":
					return (IntPtr.Size == 4) || !TestRuntime.CheckXcodeVersion (7, 0);
				case "System.UInt32":
					// numberOfTouchesRequired was signed before iOS6, unsigned since then
					return true;
				}
				break;
			// unsigned 32 bits
			case 'I':
				switch (type.FullName) {
				case "System.Int32":
					// sign-ness mis-binding, several of them (not critical)
					// CBATTRequest 'instance Int32 get_Offset()' selector: offset == I8@0:4
					return true;
				}
				break;
			// unsigned 32 bits
			case 'L':
				switch (type.FullName) {
				// sign-ness mis-binding (not critical) e.g.
				// CAMediaTimingFunction 'instance Void GetControlPointAtIndex(Int32, IntPtr)' selector: getControlPointAtIndex:values: == v16@0:4L8[2f]12
				case "System.Int32":
					return true;
				}
				break;
			// unsigned 64 bits
			case 'Q':
				switch (type.FullName) {
				// sign-ness mis-binding (not critical) e.g.
				// NSIncrementalStoreNode 'instance Int64 get_Version()' selector: version == Q8@0:4
				case "System.Int64":
					return true;
				}
				break;
			}
			return base.Check (encodedType, type);
		}

		protected override void CheckManagedMemberSignatures (MethodBase m, Type t, ref int n)
		{
#if !XAMCORE_5_0 // let's review the tests exceptions if we break things
			switch (m.Name) {
			case "get_Source":
			case "set_Source":
				// UITableViewSource is our own creation and we did not make an interface out of it
				if (t.Name == "UITableView")
					return;
				// UICollectionViewSource is our own creation and we did not make an interface out of it
				if (t.Name == "UICollectionView")
					return;
				break;
			case "get_SearchResultsSource":
			case "set_SearchResultsSource":
				// UITableViewSource is our own creation and we did not make an interface out of it
				if (t.Name == "UISearchDisplayController")
					return;
				break;
#endif // !XAMCORE_5_0
#if !NET
			case "get_ImagePickerControllerDelegate":
			case "set_ImagePickerControllerDelegate":
			case "get_NavigationControllerDelegate":
			case "set_NavigationControllerDelegate":
				// fixed in NET - alternative are the Weak* delegates
				if (t.Name == "UIImagePickerController")
					return;
				break;
#endif
#if !XAMCORE_5_0 // let's review the tests exceptions if we break things
			case "get_Model":
			case "set_Model":
				// UIPickerViewModel is our own creation and we did not make an interface out of it
				if (t.Name == "UIPickerView")
					return;
				break;
			}
#endif
			base.CheckManagedMemberSignatures (m, t, ref n);
		}

		protected override bool IgnoreAsync (MethodInfo m)
		{
			switch (m.Name) {
			// Called by the OS, i.e. meant to be overridden (not called) by user code.
			case "DidReceiveNotification":
				return m.DeclaringType.Name == "WKUserNotificationInterfaceController";
			case "AddCompletion":
				return true;
			// comes from NSFilePresenter protocol, where we cannot put [Async] today
			// proposal: https://trello.com/c/dSOh6PXE/680-rfc-async-on-protocol-proposal
			case "AccommodatePresentedItemDeletion":
			case "AccommodatePresentedSubitemDeletion":
			case "SavePresentedItemChanges":
				return m.DeclaringType.Name == "UIDocument";
			case "PostNotification": // completion handler is not always called
				return m.DeclaringType.Name == "ICNotificationManager";
			}
			return base.IgnoreAsync (m);
		}
	}
}
