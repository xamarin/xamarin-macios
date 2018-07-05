//
// Mac specific fields validator
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacApiFieldTest : ApiFieldTest {

		public MacApiFieldTest ()
		{
			ContinueOnFailure = true;
		}

		static bool IsUnified {
			get {
				return AppDomain.CurrentDomain.GetAssemblies ().Any (x => x.FullName.Contains ("Xamarin.Mac"));
			}
		}

		protected override bool Skip (Type type)
		{
			switch (type.FullName) {
			// OS X 10.8 +
			case "MonoMac.AppKit.NSSharingService":
			case "AppKit.NSSharingService":
			case "MonoMac.Foundation.NSUserNotification":
			case "Foundation.NSUserNotification":
				return !Mac.CheckSystemVersion (10, 8);
			}

			switch (type.Namespace) {
			// OSX 10.8+
			case "MonoMac.Accounts":
			case "Accounts":
			case "MonoMac.GameKit":
			case "GameKit":
			case "MonoMac.Social":
			case "Social":
				return !Mac.CheckSystemVersion (10, 8);
			case "SceneKit":
			case "MonoMac.SceneKit":
				return !Mac.CheckSystemVersion (10, 8) || IntPtr.Size != 8;
			case "MediaPlayer":
			case "MonoMac.MediaPlayer":
				if (!Mac.CheckSystemVersion (10, 12) || IntPtr.Size != 8)
					return true;
				break;
			}
			return false;
		}

		protected override bool Skip (PropertyInfo p)
		{
			switch (p.DeclaringType.Name) {
			case "NSUrlSessionDownloadDelegate":
				switch (p.Name) {
				case "TaskResumeDataKey":
					// This field was introduced as 64-bit only in Mavericks, and 32+64bits in Yosemite. We can't
					// express that with our AvailabilityAttribute, we set it as available (for all architectures, since
					// we can't distinguish them) starting with Mavericks.
					if (Mac.Is32BitMavericks)
						return true;
					break;
				}
				break;
			// Xcode 10
			case "QCComposition":
				switch (p.Name) {
				case "InputRSSArticleDurationKey":
				case "InputRSSFeedURLKey":
				case "ProtocolRSSVisualizer":
					if (Mac.CheckSystemVersion (10,14)); // radar 41125938
						return true;
					break;
				}
				break;
			}

			switch (p.Name) {
			case "CharacteristicValidRangeString":
				return Mac.CheckSystemVersion (10, 13); // radar 32858911 
			// NSTableView
			case "RowViewKey":
				return true;
			// NSBitmapImageRep
			case "CompressionMethod":
			case "CompressionFactor":
			case "DitherTransparency":
				return true;
			// CIImage 10.8+
			case "AutoAdjustFeaturesKey":	// kCIImageAutoAdjustFeatures - documented in 10.8
			case "AutoAdjustRedEyeKey":
			case "AutoAdjustEnhanceKey":
			case "CIImagePropertiesKey":
				return true;
			// CIImage
			case "CIImageColorSpaceKey":
				return true; // returns null but documented prior to 10.8
			// CLLocation 10.8+
			case "ErrorUserInfoAlternateRegionKey":		// kCLErrorUserInfoAlternateRegionKey also returns null on iOS
				return true;
			// NSUrl 10.8+
			case "PathKey":
				return !Mac.CheckSystemVersion (10, 8);
			// AVMediaCharacteristic 10.8+
			case "IsMainProgramContent":
			case "IsAuxiliaryContent":
			case "ContainsOnlyForcedSubtitles":
			case "TranscribesSpokenDialogForAccessibility":
			case "DescribesMusicAndSoundForAccessibility":
			case "DescribesVideoForAccessibility":
				return !Mac.CheckSystemVersion (10, 8);
			// AVVideo 10.8+
			case "ProfileLevelH264Main32":
				return !Mac.CheckSystemVersion (10, 8);
			// AVMetadata 10.8+
			case "QuickTimeUserDataKeyTaggedCharacteristic":
				return !Mac.CheckSystemVersion (10, 8);
			// CIDetector
			case "TypeFace":	// CIDetectorTypeFace - documented as available in 10.7
			case "Accuracy":	// CIDetectorAccuracy 10.7
			case "AccuracyLow":	// CIDetectorAccuracyLow 10.7
			case "AccuracyHigh":	// CIDetectorAccuracyHigh 10.7
				return true; 
			// CIDetector
			case "ImageOrientation":		// documented in 10.8
			case "Tracking":
			case "MinFeatureSize":
				return true; 
			// CGImageProperties - all new (10.7) values are missing (from pre-6 iOS too iirc)
			case "ExifCameraOwnerName":	// kCGImagePropertyExifCameraOwnerName
			case "ExifBodySerialNumber":	// kCGImagePropertyExifBodySerialNumber
			case "ExifLensSpecification":	// kCGImagePropertyExifLensSpecification
			case "ExifLensMake":		// kCGImagePropertyExifLensMake
			case "ExifLensModel":		// kCGImagePropertyExifLensModel
			case "ExifLensSerialNumber":	// kCGImagePropertyExifLensSerialNumber
				return true;
			// ACErrorDomain is _listed_ in the 10.8 Accounts Changes but like iOS it's not documented anywhere else (and does not seems to exists)
			case "ErrorDomain":
			// ACAccountStoreDidChangeNotification - documented in 10.8
			case "ChangeNotification":
			// ACAccountType - documented in 10.8
			case "Twitter":				// ACAccountTypeIdentifierTwitter
			case "SinaWeibo":
			case "Facebook":
			case "AppId":				// ACFacebookAppIdKey
			case "Permissions":			// ACFacebookPermissionsKey
			case "Audience":			// *** NOT documented in 10.8 - but part of our API enhancement
			case "Everyone":			// ^ MonoMac.Accounts.ACFacebookAudienceValue.Everyone
			case "Friends":				// ^ MonoMac.Accounts.ACFacebookAudienceValue.Friends
			case "OnlyMe":				// ^ MonoMac.Accounts.ACFacebookAudienceValue.OnlyMe
				return true;
			// MonoMac.CoreServices.CFHTTPMessage - document in 10.9 but returns null
			case "_AuthenticationSchemeOAuth1":
				return true;
			case "CBUUIDValidRangeString":
				if (Mac.CheckSystemVersion (10, 13)); // radar 32858911
					return true;
				break;
			default:
				return base.Skip (p);
			}
		}

		protected override bool Skip (string constantName, string libraryName)
		{
			switch (constantName) {
			case "CBUUIDValidRangeString":
				if (Mac.CheckSystemVersion (10, 13)); // radar 32858911
					return true;
				break;
			// Only there for API compat
			case "kSecUseNoAuthenticationUI":
			case "kSecUseOperationPrompt":
				return !IsUnified;
			// MonoMac.CoreServices.CFHTTPMessage - document in 10.9 but returns null
			case "kCFHTTPAuthenticationSchemeOAuth1":
				return true;
			// kCLErrorUserInfoAlternateRegionKey also returns null on iOS
			case "kCLErrorUserInfoAlternateRegionKey":
				return true;
			case "NSURLSessionTransferSizeUnknown":
			case "NSURLSessionDownloadTaskResumeData":
				if (Mac.Is32BitMavericks)
					return true;
				goto default;
			// Xcode 10
			case "QCCompositionInputRSSFeedURLKey":
			case "QCCompositionInputRSSArticleDurationKey":
			case "QCCompositionProtocolRSSVisualizer":
				if (Mac.CheckSystemVersion (10,14)); // radar 41125938
					return true;
				break;
			default:
				return base.Skip (constantName, libraryName);
			}
		}

		protected override bool SkipNotification (Type declaredType, string notificationName)
		{
			switch (declaredType.Name){
#if !XAMCORE_4_0
			case "NSWorkspaceAccessibilityNotifications":
			case "NSAccessibilityNotifications":
				return true;
			case "AVFragmentedMovieTrack":
				switch (notificationName) {
				case "AVFragmentedMovieTrackTotalSampleDataLengthDidChangeNotification":
				case "AVFragmentedMovieTrackTimeRangeDidChangeNotification":
					return true;
				}
				break;
			case "NSTask":
				return notificationName == "NSTaskDidTerminateNotification";
#endif
			}
			return base.SkipNotification (declaredType, notificationName);
		}

		protected override string FindLibrary (string libraryName, bool requiresFullPath = false)
		{
			switch (libraryName) {
			case "CFNetwork":
				if (Mac.IsAtLeast (10,8))
					break;
				return "/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/CFNetwork.framework/CFNetwork";
			case "CoreText":
			case "CoreGraphics":
				if (Mac.IsAtLeast (10,8))
					break;
				return string.Format ("/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/{0}.framework/{0}", libraryName);
			case "ImageIO":
				if (Mac.IsAtLeast (10,9))
					break;
				return "/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/ImageIO.framework/ImageIO";
			case "CoreImage":
				// generated code uses QuartzCore correctly - even if the [Field] property is wrong
				libraryName = "QuartzCore";
				break;
			case "QuartzComposer":
			case "PdfKit":
			case "ImageKit":
				// generated code uses Quartz correctly - even if the [Field] property is wrong
				libraryName = "Quartz";
				break;
			case "CoreBluetooth":
				if (Mac.IsYosemiteOrHigher) {
					// CoreBluetooth is in /System/Library/Frameworks/CoreBluetooth.framework starting with Yosemite.
					break;
				}

				// CoreBluetooth is inside IOBluetooth.framework in earlier OSXs
				return "/System/Library/Frameworks/IOBluetooth.framework/Versions/A/Frameworks/CoreBluetooth.framework/CoreBluetooth";
			case "SearchKit":
				return "/System/Library/Frameworks/CoreServices.framework/Frameworks/SearchKit.framework/SearchKit";
			}

			return base.FindLibrary (libraryName, requiresFullPath);
		}
	}
}
