//
// Copyright 2011, Novell, Inc.
// Copyright 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using XamCore.Foundation;
using XamCore.CoreGraphics;

namespace XamCore.AppKit {

	public enum NSImageName {
		QuickLookTemplate,
		BluetoothTemplate,
		IChatTheaterTemplate,
		SlideshowTemplate,
		ActionTemplate,
		SmartBadgeTemplate,
		PathTemplate,
		InvalidDataFreestandingTemplate,
		LockLockedTemplate,
		LockUnlockedTemplate,
		GoRightTemplate,
		GoLeftTemplate,
		RightFacingTriangleTemplate,
		LeftFacingTriangleTemplate,
		AddTemplate,
		RemoveTemplate,
		RevealFreestandingTemplate,
		FollowLinkFreestandingTemplate,
		EnterFullScreenTemplate,
		ExitFullScreenTemplate,
		StopProgressTemplate,
		StopProgressFreestandingTemplate,
		RefreshTemplate,
		RefreshFreestandingTemplate,
		Folder,
		TrashEmpty,
		TrashFull,
		HomeTemplate,
		BookmarksTemplate,
		Caution,
		StatusAvailable,
		StatusPartiallyAvailable,
		StatusUnavailable,
		StatusNone,
		ApplicationIcon,
		MenuOnStateTemplate,
		MenuMixedStateTemplate,
		UserGuest,
		MobileMe
	}

	public partial class NSImage {
		object __mt_reps_var;
		
		public CGImage CGImage {
			get {
				var rect = CGRect.Empty;
				return AsCGImage (ref rect, null, null);
			}
		}

		public static NSImage FromStream (System.IO.Stream stream)
		{
			using (NSData data = NSData.FromStream (stream)) {
				return new NSImage (data);
			}
		}

		public NSImage (string fileName, bool lazy)
		{
			if (lazy)
				Handle = InitByReferencingFile (fileName);
			else
				Handle = new NSImage (fileName).Handle;
		}

		public NSImage (NSData data, bool ignoresOrientation)
		{
			if (ignoresOrientation) {
				Handle = InitWithDataIgnoringOrientation (data);
			} else {
				Handle = InitWithData (data);
			}
		}

		// note: if needed override the protected Get|Set methods
		public string Name { 
			get { return GetName (); }
			// ignore return value (bool)
			set { SetName (value); }
		}

		public static NSImage ImageNamed (NSImageName name)
		{
			switch (name) {
			case NSImageName.QuickLookTemplate:
				return ImageNamed (NSImageNameQuickLookTemplate);
			case NSImageName.BluetoothTemplate:
				return ImageNamed (NSImageNameBluetoothTemplate);
			case NSImageName.IChatTheaterTemplate:
				return ImageNamed (NSImageNameIChatTheaterTemplate);
			case NSImageName.SlideshowTemplate:
				return ImageNamed (NSImageNameSlideshowTemplate);
			case NSImageName.ActionTemplate:
				return ImageNamed (NSImageNameActionTemplate);
			case NSImageName.SmartBadgeTemplate:
				return ImageNamed (NSImageNameSmartBadgeTemplate);
			case NSImageName.PathTemplate:
				return ImageNamed (NSImageNamePathTemplate);
			case NSImageName.InvalidDataFreestandingTemplate:
				return ImageNamed (NSImageNameInvalidDataFreestandingTemplate);
			case NSImageName.LockLockedTemplate:
				return ImageNamed (NSImageNameLockLockedTemplate);
			case NSImageName.LockUnlockedTemplate:
				return ImageNamed (NSImageNameLockUnlockedTemplate);
			case NSImageName.GoRightTemplate:
				return ImageNamed (NSImageNameGoRightTemplate);
			case NSImageName.GoLeftTemplate:
				return ImageNamed (NSImageNameGoLeftTemplate);
			case NSImageName.RightFacingTriangleTemplate:
				return ImageNamed (NSImageNameRightFacingTriangleTemplate);
			case NSImageName.LeftFacingTriangleTemplate:
				return ImageNamed (NSImageNameLeftFacingTriangleTemplate);
			case NSImageName.AddTemplate:
				return ImageNamed (NSImageNameAddTemplate);
			case NSImageName.RemoveTemplate:
				return ImageNamed (NSImageNameRemoveTemplate);
			case NSImageName.RevealFreestandingTemplate:
				return ImageNamed (NSImageNameRevealFreestandingTemplate);
			case NSImageName.FollowLinkFreestandingTemplate:
				return ImageNamed (NSImageNameFollowLinkFreestandingTemplate);
			case NSImageName.EnterFullScreenTemplate:
				return ImageNamed (NSImageNameEnterFullScreenTemplate);
			case NSImageName.ExitFullScreenTemplate:
				return ImageNamed (NSImageNameExitFullScreenTemplate);
			case NSImageName.StopProgressTemplate:
				return ImageNamed (NSImageNameStopProgressTemplate);
			case NSImageName.StopProgressFreestandingTemplate:
				return ImageNamed (NSImageNameStopProgressFreestandingTemplate);
			case NSImageName.RefreshTemplate:
				return ImageNamed (NSImageNameRefreshTemplate);
			case NSImageName.RefreshFreestandingTemplate:
				return ImageNamed (NSImageNameRefreshFreestandingTemplate);
			case NSImageName.Folder:
				return ImageNamed (NSImageNameFolder);
			case NSImageName.TrashEmpty:
				return ImageNamed (NSImageNameTrashEmpty);
			case NSImageName.TrashFull:
				return ImageNamed (NSImageNameTrashFull);
			case NSImageName.HomeTemplate:
				return ImageNamed (NSImageNameHomeTemplate);
			case NSImageName.BookmarksTemplate:
				return ImageNamed (NSImageNameBookmarksTemplate);
			case NSImageName.Caution:
				return ImageNamed (NSImageNameCaution);
			case NSImageName.StatusAvailable:
				return ImageNamed (NSImageNameStatusAvailable);
			case NSImageName.StatusPartiallyAvailable:
				return ImageNamed (NSImageNameStatusPartiallyAvailable);
			case NSImageName.StatusUnavailable:
				return ImageNamed (NSImageNameStatusUnavailable);
			case NSImageName.StatusNone:
				return ImageNamed (NSImageNameStatusNone);
			case NSImageName.ApplicationIcon:
				return ImageNamed (NSImageNameApplicationIcon);
			case NSImageName.MenuOnStateTemplate:
				return ImageNamed (NSImageNameMenuOnStateTemplate);
			case NSImageName.MenuMixedStateTemplate:
				return ImageNamed (NSImageNameMenuMixedStateTemplate);
			case NSImageName.UserGuest:
				return ImageNamed (NSImageNameUserGuest);
			case NSImageName.MobileMe:
				return ImageNamed (NSImageNameMobileMe);
			}

			throw new ArgumentException ("Invalid enum value", "name");
		}
	}

	public partial class NSImageRep {

		public CGImage CGImage {
			get {
				var rect = CGRect.Empty;
				return AsCGImage (ref rect, null, null);
			}
		}
	}
}
