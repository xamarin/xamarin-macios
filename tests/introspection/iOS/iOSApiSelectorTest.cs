//
// Test the generated API selectors against typos or non-existing cases
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using Foundation;
using ObjCRuntime;
using UIKit;
#if HAS_WATCHCONNECTIVITY
using WatchConnectivity;
#endif
using NUnit.Framework;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiSelectorTest : CoreSelectorTest {

		public iOSApiSelectorTest ()
		{
			ContinueOnFailure = true;
			//LogProgress = true;
		}

		protected override bool Skip (Type type)
		{
			switch (type.Namespace) {
#if __WATCHOS__
			case "GameKit":
				if (IntPtr.Size == 4)
					return true;
				break;
#endif
			// they don't answer on the simulator (Apple implementation does not work) but fine on devices
			case "GameController":
			case "MonoTouch.GameController":
			case "MLCompute": // xcode 12 beta 3
				return TestRuntime.IsSimulatorOrDesktop;

			case "CoreAudioKit":
			case "MonoTouch.CoreAudioKit":
			case "Metal":
			case "MonoTouch.Metal":
				// they works with iOS9 beta 4 (but won't work on older simulators)
				if (TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (7, 0))
					return true;
				break;
			case "Chip":
			case "MetalFX":
			case "MetalKit":
			case "MonoTouch.MetalKit":
			case "MetalPerformanceShaders":
			case "MonoTouch.MetalPerformanceShaders":
			case "Phase":
			case "ThreadNetwork":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
#if __TVOS__
			case "MetalPerformanceShadersGraph":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
#endif // __TVOS__
			// Xcode 9
			case "CoreNFC": // Only available on devices that support NFC, so check if NFCNDEFReaderSession is present.
				if (Class.GetHandle ("NFCNDEFReaderSession") == IntPtr.Zero)
					return true;
				break;
			case "DeviceCheck":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;

			// Apple does not ship a PushKit for every arch on some devices :(
			//			case "PushKit":
			//			case "MonoTouch.PushKit":
			//				if (Runtime.Arch == Arch.DEVICE)
			//					return true;
			//				break;
#if HAS_WATCHCONNECTIVITY
			case "WatchConnectivity":
			case "MonoTouch.WatchConnectivity":
				if (!WCSession.IsSupported)
					return true;
				break;
#endif // HAS_WATCHCONNECTIVITY
			case "Cinematic":
			case "PushToTalk":
			case "ShazamKit":
				// ShazamKit is not fully supported in the simulator
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
			}

			switch (type.Name) {
			// abstract superclass
			case "UIBarItem":
				return true;

			// does not answer to anything ?
			case "UILocalNotification":
				return true;

			// Metal is not available on the simulator
			case "CAMetalLayer":
				return TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (11, 0);
			case "SKRenderer":
				return TestRuntime.IsSimulatorOrDesktop;

			// iOS 10 - this type can only be instantiated on devices, but the selectors are forwarded
			//  to a MTLHeapDescriptorInternal and don't respond - so we'll add unit tests for them
			case "MTLHeapDescriptor":
			case "MTLLinkedFunctions":
			case "MTLRenderPipelineDescriptor":
			case "MTLTileRenderPipelineDescriptor":
			case "MTLRasterizationRateLayerDescriptor":
			case "MTLRasterizationRateMapDescriptor":
				return TestRuntime.IsSimulatorOrDesktop;
#if __WATCHOS__
				// The following watchOS 3.2 Beta 2 types Fails, but they can be created we verified using an ObjC app, we will revisit before stable
			case "INPersonResolutionResult":
			case "INPlacemarkResolutionResult":
			case "INPreferences":
			case "INRadioTypeResolutionResult":
			case "INRelativeReferenceResolutionResult":
			case "INRelativeSettingResolutionResult":
			case "INRideCompletionStatus":
			case "INSpeakableStringResolutionResult":
			case "INStringResolutionResult":
			case "INTemperatureResolutionResult":
			case "INWorkoutGoalUnitTypeResolutionResult":
			case "INWorkoutLocationTypeResolutionResult":
			case "INBillPayeeResolutionResult":
			case "INBillTypeResolutionResult":
			case "INBooleanResolutionResult":
			case "INCallRecordTypeResolutionResult":
			case "INCarAirCirculationModeResolutionResult":
			case "INCarAudioSourceResolutionResult":
			case "INCarDefrosterResolutionResult":
			case "INCarSeatResolutionResult":
			case "INCarSignalOptionsResolutionResult":
			case "INCurrencyAmountResolutionResult":
			case "INDateComponentsRangeResolutionResult":
			case "INDateComponentsResolutionResult":
			case "INDoubleResolutionResult":
			case "INImage":
			case "INIntegerResolutionResult":
			case "INInteraction":
			case "INMessageAttributeOptionsResolutionResult":
			case "INPaymentAmountResolutionResult":
			case "INMessageAttributeResolutionResult":
			case "INPaymentMethod":
			case "INPaymentStatusResolutionResult":
			case "INPaymentAccountResolutionResult":
				return true;
			case "CMMovementDisorderManager":
				// From Xcode 10 beta 2:
				// This requires a special entitlement:
				//     Usage of CMMovementDisorderManager requires a special entitlement.  Please see for more information https://developer.apple.com/documentation/coremotion/cmmovementdisordermanager
				// but that web page doesn't explain anything (it's mostly empty, so this is probably just lagging documentation)
				// I also tried enabling every entitlement in Xcode, but it still didn't work.
				return true;
#endif

			default:
				return base.Skip (type);
			}
		}

		protected override bool CheckResponse (bool value, Type actualType, MethodBase method, ref string name)
		{
			if (value)
				return true;

			var declaredType = method.DeclaringType;

			switch (declaredType.Name) {
			case "AVUrlAsset":
				switch (name) {
				// fails because it is in-lined via protocol AVContentKeyRecipient
				case "contentKeySession:didProvideContentKey:":
					return true;
				}
				break;
			case "NSNull":
				switch (name) {
				// conformance to CAAction started with iOS8
				case "runActionForKey:object:arguments:":
					if (!TestRuntime.CheckXcodeVersion (8, 0))
						return true;
					break;
				}
				break;
			case "NSUrlSession":
				switch (name) {
				case "delegateQueue":
				case "sessionDescription":
				case "setSessionDescription:":
				case "delegate":
					// does not respond anymore but the properties works (see monotouch-test)
					if (TestRuntime.CheckXcodeVersion (7, 0))
						return true;
					break;
				}
				break;
			case "NSUrlSessionTask":
				switch (name) {
				case "countOfBytesExpectedToReceive":
				case "countOfBytesExpectedToSend":
				case "countOfBytesReceived":
				case "countOfBytesSent":
				case "currentRequest":
				case "error":
				case "originalRequest":
				case "response":
				case "state":
				case "taskDescription":
				case "setTaskDescription:":
				case "taskIdentifier":
					// does not respond anymore but the properties works (see monotouch-test)
					if (TestRuntime.CheckXcodeVersion (7, 0))
						return true;
					break;
				case "earliestBeginDate":
				case "setEarliestBeginDate:":
				case "countOfBytesClientExpectsToSend":
				case "setCountOfBytesClientExpectsToSend:":
				case "countOfBytesClientExpectsToReceive":
				case "setCountOfBytesClientExpectsToReceive:":
				case "progress":
					if (TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				case "priority":
				case "setPriority:":
					if (TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				}
				break;
			case "NSUrlSessionConfiguration":
			case "NSUrlSessionStreamTask":
				// does not respond anymore but the properties works (see monotouch-test for a partial list)
				if (TestRuntime.CheckXcodeVersion (7, 0))
					return true;
				break;

			case "AVAssetDownloadTask":
				switch (name) {
				case "currentRequest":
				case "originalRequest":
				case "response":
					if (TestRuntime.CheckXcodeVersion (7, 0))
						return true;
					break;
				case "loadedTimeRanges":
				case "options":
				case "URLAsset":
					if (TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				}
				break;
			case "CMSensorRecorder":
				switch (name) {
				// breaking change from Apple in iOS 9.3 betas
				// https://trello.com/c/kqlEkPbG/30-24508290-cmsensorrecorder-breaking-change-re-opening-24231250
				// https://trello.com/c/pKLOLjVJ/29-24231250-coremotion-api-removal-without-deprecation
				case "accelerometerDataFromDate:toDate:":
				case "recordAccelerometerForDuration:":
					if (!TestRuntime.CheckXcodeVersion (7, 3))
						return true;
					break;
				}
				break;
			case "SKNode":  // iOS 10+
			case "SCNNode": // iOS 11+
				switch (name) {
				// UIFocus protocol conformance
				case "didUpdateFocusInContext:withAnimationCoordinator:":
				case "setNeedsFocusUpdate":
				case "shouldUpdateFocusInContext:":
				case "updateFocusIfNeeded":
				case "canBecomeFocused":
#if !__TVOS__ && !__MACCATALYST__
				case "preferredFocusedView":
#endif
					int major = declaredType.Name == "SKNode" ? 8 : 9;
					if (!TestRuntime.CheckXcodeVersion (major, 0))
						return true;
					break;
#if __TVOS__ || __MACCATALYST__
				case "preferredFocusedView":
					return true;
#endif
				}
				break;
			case "CIContext":
				switch (name) {
				case "render:toIOSurface:bounds:colorSpace:":
					if (TestRuntime.IsSimulatorOrDesktop)
						return !TestRuntime.CheckXcodeVersion (11, 0);
					if (!TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				}
				break;
			case "CIImage":
				switch (name) {
				case "initWithIOSurface:":
				case "initWithIOSurface:options:":
					// works on both sim/device with Xcode 11 (continue main logic)
					if (TestRuntime.CheckXcodeVersion (11, 0))
						break;
					// did not work on simulator before iOS 13 (shortcut logic)
					if (TestRuntime.IsSimulatorOrDesktop)
						return true;
					// was a private framework (on iOS) before Xcode 9.0 (shortcut logic)
					if (!TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				}
				break;
			case "CIRenderDestination":
				switch (name) {
				case "initWithIOSurface:":
					if (TestRuntime.IsSimulatorOrDesktop)
						return !TestRuntime.CheckXcodeVersion (11, 0);
					if (!TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				}
				break;
			case "EAGLContext":
			case "SCNGeometry":
				switch (name) {
				// symbol only exists on devices (not in simulator libraries)
				case "texImageIOSurface:target:internalFormat:width:height:format:type:plane:":
				case "setTessellator:":
				case "tessellator":
					if (TestRuntime.IsSimulatorOrDesktop)
						return !TestRuntime.CheckXcodeVersion (11, 0);
					if (!TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				}
				break;
			case "PKSuicaPassProperties":
				switch (name) {
				// Selectors do not respond anymore in Xcode 9.3. Radar https://trello.com/c/B31EMqSg.
				case "isGreenCarTicketUsed":
				case "isInShinkansenStation":
					if (TestRuntime.CheckXcodeVersion (9, 3))
						return true;
					break;
				}
				break;
			case "UIPreviewInteraction":
				switch (name) {
				// Selectors do not respond anymore in Xcode 10.2 beta 1.
				case "cancelInteraction":
				case "locationInCoordinateSpace:":
				case "delegate":
				case "setDelegate:":
				case "view":
					if (TestRuntime.CheckXcodeVersion (10, 2))
						return true;
					break;
				}
				break;
#if __TVOS__ || __MACCATALYST__
			// broken with Xcode 12 beta 1
			case "CKDiscoveredUserInfo":
				switch (name) {
				case "copyWithZone:":
				case "encodeWithCoder:":
					if (TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			case "CKSubscription":
				switch (name) {
				case "setZoneID:":
					if (TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
#endif
#if __IOS__
			// broken with Xcode 12 beta 1
			case "ARBodyTrackingConfiguration":
			case "ARImageTrackingConfiguration":
			case "ARObjectScanningConfiguration":
			case "ARWorldTrackingConfiguration":
				switch (name) {
				case "isAutoFocusEnabled":
				case "setAutoFocusEnabled:":
					if (TestRuntime.IsSimulatorOrDesktop && TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			case "ARReferenceImage":
				switch (name) {
				case "copyWithZone:":
					if (TestRuntime.IsSimulatorOrDesktop && TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			// ARImageAnchor was added in iOS 11.3 but the conformance to ARTrackable, where `isTracked` comes from, started with iOS 12.0
			case "ARImageAnchor":
				switch (name) {
				case "isTracked":
					if (!TestRuntime.CheckXcodeVersion (10, 0))
						return true;
					break;
				}
				break;
			case "UIHoverGestureRecognizer":
				switch (name) {
				case "azimuthAngleInView:": // Only works on iPad according to docs.
				case "azimuthUnitVectorInView:":
					if (TestRuntime.CheckXcodeVersion (14, 3) && UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
						return true;
					break;
				}
				break;
			case "HKHealthStore":
				switch (name) {
				case "workoutSessionMirroringStartHandler":
					if (TestRuntime.IsSimulatorOrDesktop)
						return true;
					break;
				}
				break;
#endif
#if __WATCHOS__
			case "INUserContext":
				switch (name) {
				case "encodeWithCoder:":
					if (!TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			case "HKHealthStore":
				switch (name) {
				case "workoutSessionMirroringStartHandler":
					if (TestRuntime.IsSimulatorOrDesktop)
						return true;
					break;
				}
				break;
#endif
				break;
			}

			switch (name) {
			// UIResponderStandardEditActions - stuffed inside UIResponder
			case "cut:":
			case "copy:":
			case "paste:":
			case "delete:":
			case "select:":
			case "selectAll:":
			case "pasteAndGo:":
			case "pasteAndMatchStyle:":
			case "pasteAndSearch:":
			case "print:":
			// A subclass of UIResponder typically implements this method...
			case "toggleBoldface:":
			case "toggleItalics:":
			case "toggleUnderline:":
				if (declaredType.Name == "UIResponder")
					return true;
				break;
			case "makeTextWritingDirectionLeftToRight:":
			case "makeTextWritingDirectionRightToLeft:":
				// MonoTouch.AddressBookUI.ABNewPersonViewController
				// MonoTouch.AddressBookUI.ABPeoplePickerNavigationController
				// MonoTouch.AddressBookUI.ABPersonViewController
				if (declaredType.Name == "UIResponder")
					return true;
				break;
			case "autocapitalizationType":
			case "setAutocapitalizationType:":
			case "autocorrectionType":
			case "setAutocorrectionType:":
			case "keyboardType":
			case "setKeyboardType:":
			case "spellCheckingType":
			case "setSpellCheckingType:":
				// UITextInputTraits and UITextInputProtocol
				if (declaredType.Name == "UITextField" || declaredType.Name == "UITextView")
					return true;
				if (TestRuntime.CheckXcodeVersion (5, 1) && declaredType.Name == "UISearchBar")
					return true;
				break;
			case "keyboardAppearance":
			case "setKeyboardAppearance:":
			case "returnKeyType":
			case "setReturnKeyType:":
			case "enablesReturnKeyAutomatically":
			case "setEnablesReturnKeyAutomatically:":
			case "isSecureTextEntry":
			case "setSecureTextEntry:":
				// UITextInputTraits and UITextInput Protocol
				switch (declaredType.Name) {
				case "UITextField":
				case "UITextView":
				case "UISearchBar":
					return true;
				}
				break;
			case "textStylingAtPosition:inDirection:":
			case "positionWithinRange:atCharacterOffset:":
			case "characterOffsetOfPosition:withinRange:":
			case "shouldChangeTextInRange:replacementText:":
				// UITextInputTraits and UITextInputProtocol
				if (declaredType.Name == "UITextField" || declaredType.Name == "UITextView")
					return true;
				// ignore UISearchBar before iOS8 - it did not really implement UITextInput
				if (declaredType.Name == "UISearchBar" && !TestRuntime.CheckXcodeVersion (6, 0))
					return true;
				break;
			case "dictationRecognitionFailed":
			case "dictationRecordingDidEnd":
			case "insertDictationResult:":
				// iOS 5.1 and not every device (or simulator)
				if (declaredType.Name == "UITextField" || declaredType.Name == "UITextView")
					return true;
				break;
			// special case: see http://developer.apple.com/library/ios/#documentation/GLkit/Reference/GLKViewController_ClassRef/Reference/Reference.html
			case "update":
				if (declaredType.Name == "GLKViewController")
					return true;
				break;
			case "thumbnailImageAtTime:timeOption:":
			case "requestThumbnailImagesAtTimes:timeOption:":
			case "cancelAllThumbnailImageRequests":
			case "accessLog":
			case "errorLog":
			case "timedMetadata":
				if (declaredType.Name == "MPMoviePlayerController")
					return true;
				break;
			// deprecated (removed in iOS 3.2)
			case "backgroundColor":
			case "setBackgroundColor:":
			case "movieControlMode":
			case "setMovieControlMode:":
				if (declaredType.Name == "MPMoviePlayerController")
					return true;
				break;
			case "skipToNextItem":
			case "skipToBeginning":
			case "skipToPreviousItem":
			case "setNowPlayingItem:":
				if (actualType.Name == "MPMusicPlayerController")
					return true;
				break;
			// deprecated (according to docs) but actually removed (test) in iOS 6
			case "useApplicationAudioSession":
			case "setUseApplicationAudioSession:":
				if (declaredType.Name == "MPMoviePlayerController")
					return TestRuntime.CheckXcodeVersion (4, 5);
				break;

			// iOS6 - headers says readwrite but they do not respond
			case "setUUID:":
			case "setIsPrimary:":
				if (declaredType.Name == "CBMutableService")
					return TestRuntime.CheckXcodeVersion (4, 5);
				if (declaredType.Name == "CBMutableCharacteristic")
					return TestRuntime.CheckXcodeVersion (7, 0);
				break;

			// documented since 4.0 - but does not answer on an iPad1 with 5.1.1
			case "isAdjustingFocus":
				if (declaredType.Name == "AVCaptureDevice")
					return true;
				break;

			// GameKit: documented since 4.1 - but does not answer
			case "alias":
				if (declaredType.Name == "GKPlayer")
					return true;
				break;
			case "playerID":
				switch (declaredType.Name) {
				case "GKPlayer":
				case "GKScore":
				case "GKTurnBasedParticipant": // iOS 5
					return true;
				}
				break;
			case "category":
			case "setCategory:":
			case "date":
			case "formattedValue":
			case "rank":
			case "value":
			case "setValue:":
			case "context": // iOS5
			case "setContext:": // iOS5
				if (declaredType.Name == "GKScore")
					return true;
				break;
			case "isUnderage":
				if (declaredType.Name == "GKLocalPlayer")
					return true;
				break;
			case "identifier":
				if (declaredType.Name == "GKAchievement" || declaredType.Name == "GKAchievementDescription")
					return true;
				break;
			case "setIdentifier:":
			case "percentComplete":
			case "setPercentComplete:":
			case "lastReportedDate":
			case "setLastReportedDate:":
				if (declaredType.Name == "GKAchievement")
					return true;
				break;
			case "achievedDescription":
			case "isHidden":
			case "maximumPoints":
			case "title":
			case "unachievedDescription":
				if (declaredType.Name == "GKAchievementDescription")
					return true;
				break;
			// 5.0
			case "lastTurnDate":
			case "matchOutcome":
			case "setMatchOutcome:":
				if (declaredType.Name == "GKTurnBasedParticipant")
					return true;
				break;
			case "creationDate":
			case "matchData":
			case "message":
			case "setMessage:":
				if (declaredType.Name == "GKTurnBasedMatch")
					return true;
				break;

			// iOS6 - protocols for UICollectionView
			case "numberOfSectionsInCollectionView:":
			case "collectionView:viewForSupplementaryElementOfKind:atIndexPath:":
			case "collectionView:shouldHighlightItemAtIndexPath:":
			case "collectionView:didHighlightItemAtIndexPath:":
			case "collectionView:didUnhighlightItemAtIndexPath:":
			case "collectionView:shouldSelectItemAtIndexPath:":
			case "collectionView:shouldDeselectItemAtIndexPath:":
			case "collectionView:didSelectItemAtIndexPath:":
			case "collectionView:didDeselectItemAtIndexPath:":
			case "collectionView:didEndDisplayingCell:forItemAtIndexPath:":
			case "collectionView:didEndDisplayingSupplementaryView:forElementOfKind:atIndexPath:":
			case "collectionView:shouldShowMenuForItemAtIndexPath:":
			case "collectionView:canPerformAction:forItemAtIndexPath:withSender:":
			case "collectionView:performAction:forItemAtIndexPath:withSender:":
			// which also inherits from UIScrollViewDelegate
			case "scrollViewDidScroll:":
			case "scrollViewWillBeginDragging:":
			case "scrollViewDidEndDragging:willDecelerate:":
			case "scrollViewWillBeginDecelerating:":
			case "scrollViewDidEndDecelerating:":
			case "scrollViewDidEndScrollingAnimation:":
			case "viewForZoomingInScrollView:":
			case "scrollViewShouldScrollToTop:":
			case "scrollViewDidScrollToTop:":
			case "scrollViewDidEndZooming:withView:atScale:":
			case "scrollViewDidZoom:":
			case "scrollViewWillBeginZooming:withView:":
			case "scrollViewWillEndDragging:withVelocity:targetContentOffset:":
				if (declaredType.Name == "UICollectionViewController")
					return TestRuntime.CheckXcodeVersion (4, 5);
				break;

			// failing (check why)
			case "initialLayoutAttributesForInsertedItemAtIndexPath:":
			case "initialLayoutAttributesForInsertedSupplementaryElementOfKind:atIndexPath:":
			case "finalLayoutAttributesForDeletedItemAtIndexPath:":
			case "finalLayoutAttributesForDeletedSupplementaryElementOfKind:atIndexPath:":
				if (declaredType.Name == "UICollectionViewLayout")
					return TestRuntime.CheckXcodeVersion (4, 5);
				break;

			// This is implemented by internal concrete classes of NSFileHandle
			case "readInBackgroundAndNotify":
			case "readInBackgroundAndNotifyForModes:":
			case "readToEndOfFileInBackgroundAndNotifyForModes:":
			case "readToEndOfFileInBackgroundAndNotify":
			case "acceptConnectionInBackgroundAndNotifyForModes:":
			case "acceptConnectionInBackgroundAndNotify":
			case "waitForDataInBackgroundAndNotifyForModes:":
			case "waitForDataInBackgroundAndNotify":
				if (declaredType.Name == "NSFileHandle")
					return true;
				break;

			// UITableViewController conforms to both UITableViewDelegate and UITableViewDataSource
			case "tableView:canEditRowAtIndexPath:":
			case "tableView:canMoveRowAtIndexPath:":
			case "sectionIndexTitlesForTableView:":
			case "tableView:sectionForSectionIndexTitle:atIndex:":
			case "tableView:commitEditingStyle:forRowAtIndexPath:":
			case "tableView:moveRowAtIndexPath:toIndexPath:":
			case "tableView:willDisplayCell:forRowAtIndexPath:":
			case "tableView:accessoryTypeForRowWithIndexPath:":
			case "tableView:accessoryButtonTappedForRowWithIndexPath:":
			case "tableView:willSelectRowAtIndexPath:":
			case "tableView:willDeselectRowAtIndexPath:":
			case "tableView:didSelectRowAtIndexPath:":
			case "tableView:didDeselectRowAtIndexPath:":
			case "tableView:editingStyleForRowAtIndexPath:":
			case "tableView:titleForDeleteConfirmationButtonForRowAtIndexPath:":
			case "tableView:shouldIndentWhileEditingRowAtIndexPath:":
			case "tableView:targetIndexPathForMoveFromRowAtIndexPath:toProposedIndexPath:":
			case "tableView:shouldShowMenuForRowAtIndexPath:":
			case "tableView:canPerformAction:forRowAtIndexPath:withSender:":
			case "tableView:performAction:forRowAtIndexPath:withSender:":
			case "tableView:willDisplayHeaderView:forSection:":
			case "tableView:willDisplayFooterView:forSection:":
			case "tableView:didEndDisplayingCell:forRowAtIndexPath:":
			case "tableView:didEndDisplayingHeaderView:forSection:":
			case "tableView:didEndDisplayingFooterView:forSection:":
			case "tableView:shouldHighlightRowAtIndexPath:":
			case "tableView:didHighlightRowAtIndexPath:":
			case "tableView:didUnhighlightRowAtIndexPath:":
			// iOS7
			case "tableView:estimatedHeightForRowAtIndexPath:":
			case "tableView:estimatedHeightForHeaderInSection:":
			case "tableView:estimatedHeightForFooterInSection:":
			// iOS 8
			case "tableView:editActionsForRowAtIndexPath:":
				if (declaredType.Name == "UITableViewController")
					return true;
				break;

			// iOS7 beta issue ? remains in beta 5 / sim
			// MCSession documents the cancelConnectPeer: selector but it does not answer
			case "cancelConnectPeer:":
			// CBCharacteristic documents the selector but does not respond
			case "subscribedCentrals":
			// UIPrintFormatter header attributedText says the API is not approved yet - and it does not respond
			case "attributedText":
			case "setAttributedText:":
			// UISplitViewController
			case "splitViewControllerSupportedInterfaceOrientations:":
			case "splitViewControllerPreferredInterfaceOrientationForPresentation:":
				return true;

			case "color":
			case "setColor:":
			case "font":
			case "setFont:":
			case "text":
			case "setText:":
			case "textAlignment":
			case "setTextAlignment:":
				// iOS7 GM a "no text" instance does not answer to the selector (but you can call them)
				if (declaredType.Name == "UISimpleTextPrintFormatter")
					return true;
				break;

			case "copyWithZone:":
				switch (declaredType.Name) {
				// not conforming to NSCopying in 5.1 SDK
				case "UIFont":
					return !TestRuntime.CheckXcodeVersion (4, 5);
				// not conforming to NSCopying before 7.0 SDK
				case "CBPeripheral":
					return !TestRuntime.CheckXcodeVersion (5, 0);
				// not conforming to NSCopying before 8.0 SDK
				case "AVMetadataFaceObject":
					return !TestRuntime.CheckXcodeVersion (6, 0);
				// not conforming to NSCopying before 8.2 SDK
				case "HKUnit":
					return !TestRuntime.CheckXcodeVersion (6, 2);
				case "HKBiologicalSexObject":
				case "HKBloodTypeObject":
					return !TestRuntime.CheckXcodeVersion (7, 0);
				case "HKWorkoutEvent":
					return !TestRuntime.CheckXcodeVersion (8, 0);
				case "HMLocationEvent":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				case "MPSGraphExecutableExecutionDescriptor":
					return TestRuntime.CheckXcodeVersion (14, 0);
				case "HKElectrocardiogramVoltageMeasurement":
					// NSCopying conformance added in Xcode 14
					return !TestRuntime.CheckXcodeVersion (14, 0);
#if __WATCHOS__
				case "INParameter":
					// NSCopying conformance added in Xcode 10
					return !TestRuntime.CheckXcodeVersion (10, 0);
#endif
				}
				break;

			// on iOS8.0 this does not work on the simulator (but works on devices)
			case "language":
				if (declaredType.Name == "AVSpeechSynthesisVoice" && TestRuntime.CheckXcodeVersion (6, 0) && TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;

			// new, optional members of UIDynamicItem protocol in iOS9
			case "collisionBoundingPath":
			case "collisionBoundsType":
				switch (declaredType.Name) {
				case "UICollectionViewLayoutAttributes":
				case "UIView":
				case "UIDynamicItemGroup":
					return true;
				}
				break;

			// SceneKit integration with Metal is not working on simulators (at least for iOS9 beta 5)
			case "currentRenderCommandEncoder":
			case "colorPixelFormat":
			case "commandQueue":
			case "depthPixelFormat":
			case "device":
			case "stencilPixelFormat":
				switch (declaredType.Name) {
				case "SCNRenderer":
				case "SCNView":
					return TestRuntime.IsSimulatorOrDesktop;
				}
				break;

			case "preferredFocusedView":
				switch (declaredType.Name) {
				// UIFocusGuide (added in iOS 9.0 and deprecated in iOS 10)
				case "UIView":
				case "UIViewController":
					return !TestRuntime.CheckXcodeVersion (7, 0);
				}
				break;

			// some types adopted NS[Secure]Coding after the type was added
			// and for unified that's something we generate automatically (so we can't put [iOS] on them)
			case "encodeWithCoder:":
				switch (declaredType.Name) {
				// UITextInputMode was added in 4.2 but conformed to NSSecureCoding only from 7.0+ 
				case "UITextInputMode":
					return !TestRuntime.CheckXcodeVersion (5, 0);
				// iOS9
				case "HKBiologicalSexObject":
				case "HKBloodTypeObject":
					return !TestRuntime.CheckXcodeVersion (7, 0);
				case "MPSKernel":
				case "MPSCnnConvolutionDescriptor":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				// Protocol conformance removed in Xcode 9.3
				case "HKSeriesBuilder":
					if (TestRuntime.CheckXcodeVersion (9, 3))
						return true;
					break;
#if __TVOS__
				case "SKAttribute":
				case "SKAttributeValue":
					return !TestRuntime.CheckXcodeVersion (7, 2);
#endif
				}
				break;
			case "mutableCopyWithZone:":
				switch (declaredType.Name) {
				case "HMLocationEvent":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;
			case "addTracksForCinematicAssetInfo:preferredStartingTrackID:": // cinematic method only supported on devices
				switch (declaredType.Name) {
				case "AVMutableComposition":
					return TestRuntime.IsSimulatorOrDesktop;
				}
				break;
			}

			return base.CheckResponse (value, actualType, method, ref name);
		}

		protected override bool CheckStaticResponse (bool value, Type actualType, Type declaredType, ref string name)
		{
			switch (name) {
			// new API in iOS9 beta 5 but is does not respond when queried - https://bugzilla.xamarin.com/show_bug.cgi?id=33431
			case "geometrySourceWithBuffer:vertexFormat:semantic:vertexCount:dataOffset:dataStride:":
				switch (declaredType.Name) {
				case "SCNGeometrySource":
					return true;
				}
				break;
			case "imageWithIOSurface:":
			case "imageWithIOSurface:options:":
				switch (declaredType.Name) {
				case "CIImage":
					// works on both sim/device with Xcode 11 (continue main logic)
					if (TestRuntime.CheckXcodeVersion (11, 0))
						break;
					// did not work on simulator before iOS 13 (shortcut logic)
					if (TestRuntime.IsSimulatorOrDesktop)
						return true;
					// was a private framework (on iOS) before Xcode 9.0 (shortcut logic)
					if (!TestRuntime.CheckXcodeVersion (9, 0))
						return true;
					break;
				}
				break;
			case "objectWithItemProviderData:typeIdentifier:error:":
			case "readableTypeIdentifiersForItemProvider":
				switch (declaredType.Name) {
				case "PHLivePhoto":
					// not yet conforming to NSItemProviderReading
					if (!TestRuntime.CheckXcodeVersion (10, 0))
						return true;
					break;
				}
				break;
#if __WATCHOS__
			case "fetchAllRecordZonesOperation":
			case "fetchCurrentUserRecordOperation":
			case "notificationFromRemoteNotificationDictionary:":
			case "containerWithIdentifier:":
			case "defaultContainer":
			case "defaultRecordZone":
				// needs investigation, seems all class selectors from CloudKit don't answer
				if (declaredType.Namespace == "CloudKit")
					return true;
				break;
#endif
			}
			return base.CheckStaticResponse (value, actualType, declaredType, ref name);
		}

		static List<NSObject> do_not_dispose = new List<NSObject> ();

		protected override void Dispose (NSObject obj, Type type)
		{
			switch (type.Name) {
			// this crash the application after test completed their execution so we keep them alive
			case "GKFriendRequestComposeViewController":
			case "PKAddPassesViewController":
			case "SKView":
				do_not_dispose.Add (obj);
				break;
			default:
				base.Dispose (obj, type);
				break;
			}
		}
	}
}
