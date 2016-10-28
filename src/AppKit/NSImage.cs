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
		MobileMe,
		ShareTemplate,
		TouchBarAddDetailTemplate,
		TouchBarAddTemplate,
		TouchBarAlarmTemplate,
		TouchBarAudioInputMuteTemplate,
		TouchBarAudioInputTemplate,
		TouchBarAudioOutputMuteTemplate,
		TouchBarAudioOutputVolumeHighTemplate,
		TouchBarAudioOutputVolumeLowTemplate,
		TouchBarAudioOutputVolumeMediumTemplate,
		TouchBarAudioOutputVolumeOffTemplate,
		TouchBarBookmarksTemplate,
		TouchBarColorPickerFill,
		TouchBarColorPickerFont,
		TouchBarColorPickerStroke,
		TouchBarCommunicationAudioTemplate,
		TouchBarCommunicationVideoTemplate,
		TouchBarComposeTemplate,
		TouchBarDeleteTemplate,
		TouchBarDownloadTemplate,
		TouchBarEnterFullScreenTemplate,
		TouchBarExitFullScreenTemplate,
		TouchBarFastForwardTemplate,
		TouchBarFolderCopyToTemplate,
		TouchBarFolderMoveToTemplate,
		TouchBarFolderTemplate,
		TouchBarGetInfoTemplate,
		TouchBarGoBackTemplate,
		TouchBarGoDownTemplate,
		TouchBarGoForwardTemplate,
		TouchBarGoUpTemplate,
		TouchBarHistoryTemplate,
		TouchBarIconViewTemplate,
		TouchBarListViewTemplate,
		TouchBarMailTemplate,
		TouchBarNewFolderTemplate,
		TouchBarNewMessageTemplate,
		TouchBarOpenInBrowserTemplate,
		TouchBarPauseTemplate,
		TouchBarPlayheadTemplate,
		TouchBarPlayPauseTemplate,
		TouchBarPlayTemplate,
		TouchBarQuickLookTemplate,
		TouchBarRecordStartTemplate,
		TouchBarRecordStopTemplate,
		TouchBarRefreshTemplate,
		TouchBarRewindTemplate,
		TouchBarRotateLeftTemplate,
		TouchBarRotateRightTemplate,
		TouchBarSearchTemplate,
		TouchBarShareTemplate,
		TouchBarSidebarTemplate,
		TouchBarSkipAhead15SecondsTemplate,
		TouchBarSkipAhead30SecondsTemplate,
		TouchBarSkipAheadTemplate,
		TouchBarSkipBack15SecondsTemplate,
		TouchBarSkipBack30SecondsTemplate,
		TouchBarSkipBackTemplate,
		TouchBarSkipToEndTemplate,
		TouchBarSkipToStartTemplate,
		TouchBarSlideshowTemplate,
		TouchBarTagIconTemplate,
		TouchBarTextBoldTemplate,
		TouchBarTextBoxTemplate,
		TouchBarTextCenterAlignTemplate,
		TouchBarTextItalicTemplate,
		TouchBarTextJustifiedAlignTemplate,
		TouchBarTextLeftAlignTemplate,
		TouchBarTextListTemplate,
		TouchBarTextRightAlignTemplate,
		TouchBarTextStrikethroughTemplate,
		TouchBarTextUnderlineTemplate,
		TouchBarUserAddTemplate,
		TouchBarUserGroupTemplate,
		TouchBarUserTemplate,
		TouchBarVolumeDownTemplate,
		TouchBarVolumeUpTemplate
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
				Handle = InitWithContentsOfFile (fileName);
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
			case NSImageName.ShareTemplate:
				return ImageNamed (NSImageNameShareTemplate);
			case NSImageName.TouchBarAddDetailTemplate:
				return ImageNamed (NSImageNameTouchBarAddDetailTemplate);
			case NSImageName.TouchBarAddTemplate:
				return ImageNamed (NSImageNameTouchBarAddTemplate);
			case NSImageName.TouchBarAlarmTemplate:
				return ImageNamed (NSImageNameTouchBarAlarmTemplate);
			case NSImageName.TouchBarAudioInputMuteTemplate:
				return ImageNamed (NSImageNameTouchBarAudioInputMuteTemplate);
			case NSImageName.TouchBarAudioInputTemplate:
				return ImageNamed (NSImageNameTouchBarAudioInputTemplate);
			case NSImageName.TouchBarAudioOutputMuteTemplate:
				return ImageNamed (NSImageNameTouchBarAudioOutputMuteTemplate);
			case NSImageName.TouchBarAudioOutputVolumeHighTemplate:
				return ImageNamed (NSImageNameTouchBarAudioOutputVolumeHighTemplate);
			case NSImageName.TouchBarAudioOutputVolumeLowTemplate:
				return ImageNamed (NSImageNameTouchBarAudioOutputVolumeLowTemplate);
			case NSImageName.TouchBarAudioOutputVolumeMediumTemplate:
				return ImageNamed (NSImageNameTouchBarAudioOutputVolumeMediumTemplate);
			case NSImageName.TouchBarAudioOutputVolumeOffTemplate:
				return ImageNamed (NSImageNameTouchBarAudioOutputVolumeOffTemplate);
			case NSImageName.TouchBarBookmarksTemplate:
				return ImageNamed (NSImageNameTouchBarBookmarksTemplate);
			case NSImageName.TouchBarColorPickerFill:
				return ImageNamed (NSImageNameTouchBarColorPickerFill);
			case NSImageName.TouchBarColorPickerFont:
				return ImageNamed (NSImageNameTouchBarColorPickerFont);
			case NSImageName.TouchBarColorPickerStroke:
				return ImageNamed (NSImageNameTouchBarColorPickerStroke);
			case NSImageName.TouchBarCommunicationAudioTemplate:
				return ImageNamed (NSImageNameTouchBarCommunicationAudioTemplate);
			case NSImageName.TouchBarCommunicationVideoTemplate:
				return ImageNamed (NSImageNameTouchBarCommunicationVideoTemplate);
			case NSImageName.TouchBarComposeTemplate:
				return ImageNamed (NSImageNameTouchBarComposeTemplate);
			case NSImageName.TouchBarDeleteTemplate:
				return ImageNamed (NSImageNameTouchBarDeleteTemplate);
			case NSImageName.TouchBarDownloadTemplate:
				return ImageNamed (NSImageNameTouchBarDownloadTemplate);
			case NSImageName.TouchBarEnterFullScreenTemplate:
				return ImageNamed (NSImageNameTouchBarEnterFullScreenTemplate);
			case NSImageName.TouchBarExitFullScreenTemplate:
				return ImageNamed (NSImageNameTouchBarExitFullScreenTemplate);
			case NSImageName.TouchBarFastForwardTemplate:
				return ImageNamed (NSImageNameTouchBarFastForwardTemplate);
			case NSImageName.TouchBarFolderCopyToTemplate:
				return ImageNamed (NSImageNameTouchBarFolderCopyToTemplate);
			case NSImageName.TouchBarFolderMoveToTemplate:
				return ImageNamed (NSImageNameTouchBarFolderMoveToTemplate);
			case NSImageName.TouchBarFolderTemplate:
				return ImageNamed (NSImageNameTouchBarFolderTemplate);
			case NSImageName.TouchBarGetInfoTemplate:
				return ImageNamed (NSImageNameTouchBarGetInfoTemplate);
			case NSImageName.TouchBarGoBackTemplate:
				return ImageNamed (NSImageNameTouchBarGoBackTemplate);
			case NSImageName.TouchBarGoDownTemplate:
				return ImageNamed (NSImageNameTouchBarGoDownTemplate);
			case NSImageName.TouchBarGoForwardTemplate:
				return ImageNamed (NSImageNameTouchBarGoForwardTemplate);
			case NSImageName.TouchBarGoUpTemplate:
				return ImageNamed (NSImageNameTouchBarGoUpTemplate);
			case NSImageName.TouchBarHistoryTemplate:
				return ImageNamed (NSImageNameTouchBarHistoryTemplate);
			case NSImageName.TouchBarIconViewTemplate:
				return ImageNamed (NSImageNameTouchBarIconViewTemplate);
			case NSImageName.TouchBarListViewTemplate:
				return ImageNamed (NSImageNameTouchBarListViewTemplate);
			case NSImageName.TouchBarMailTemplate:
				return ImageNamed (NSImageNameTouchBarMailTemplate);
			case NSImageName.TouchBarNewFolderTemplate:
				return ImageNamed (NSImageNameTouchBarNewFolderTemplate);
			case NSImageName.TouchBarNewMessageTemplate:
				return ImageNamed (NSImageNameTouchBarNewMessageTemplate);
			case NSImageName.TouchBarOpenInBrowserTemplate:
				return ImageNamed (NSImageNameTouchBarOpenInBrowserTemplate);
			case NSImageName.TouchBarPauseTemplate:
				return ImageNamed (NSImageNameTouchBarPauseTemplate);
			case NSImageName.TouchBarPlayheadTemplate:
				return ImageNamed (NSImageNameTouchBarPlayheadTemplate);
			case NSImageName.TouchBarPlayPauseTemplate:
				return ImageNamed (NSImageNameTouchBarPlayPauseTemplate);
			case NSImageName.TouchBarPlayTemplate:
				return ImageNamed (NSImageNameTouchBarPlayTemplate);
			case NSImageName.TouchBarQuickLookTemplate:
				return ImageNamed (NSImageNameTouchBarQuickLookTemplate);
			case NSImageName.TouchBarRecordStartTemplate:
				return ImageNamed (NSImageNameTouchBarRecordStartTemplate);
			case NSImageName.TouchBarRecordStopTemplate:
				return ImageNamed (NSImageNameTouchBarRecordStopTemplate);
			case NSImageName.TouchBarRefreshTemplate:
				return ImageNamed (NSImageNameTouchBarRefreshTemplate);
			case NSImageName.TouchBarRewindTemplate:
				return ImageNamed (NSImageNameTouchBarRewindTemplate);
			case NSImageName.TouchBarRotateLeftTemplate:
				return ImageNamed (NSImageNameTouchBarRotateLeftTemplate);
			case NSImageName.TouchBarRotateRightTemplate:
				return ImageNamed (NSImageNameTouchBarRotateRightTemplate);
			case NSImageName.TouchBarSearchTemplate:
				return ImageNamed (NSImageNameTouchBarSearchTemplate);
			case NSImageName.TouchBarShareTemplate:
				return ImageNamed (NSImageNameTouchBarShareTemplate);
			case NSImageName.TouchBarSidebarTemplate:
				return ImageNamed (NSImageNameTouchBarSidebarTemplate);
			case NSImageName.TouchBarSkipAhead15SecondsTemplate:
				return ImageNamed (NSImageNameTouchBarSkipAhead15SecondsTemplate);
			case NSImageName.TouchBarSkipAhead30SecondsTemplate:
				return ImageNamed (NSImageNameTouchBarSkipAhead30SecondsTemplate);
			case NSImageName.TouchBarSkipAheadTemplate:
				return ImageNamed (NSImageNameTouchBarSkipAheadTemplate);
			case NSImageName.TouchBarSkipBack15SecondsTemplate:
				return ImageNamed (NSImageNameTouchBarSkipBack15SecondsTemplate);
			case NSImageName.TouchBarSkipBack30SecondsTemplate:
				return ImageNamed (NSImageNameTouchBarSkipBack30SecondsTemplate);
			case NSImageName.TouchBarSkipBackTemplate:
				return ImageNamed (NSImageNameTouchBarSkipBackTemplate);
			case NSImageName.TouchBarSkipToEndTemplate:
				return ImageNamed (NSImageNameTouchBarSkipToEndTemplate);
			case NSImageName.TouchBarSkipToStartTemplate:
				return ImageNamed (NSImageNameTouchBarSkipToStartTemplate);
			case NSImageName.TouchBarSlideshowTemplate:
				return ImageNamed (NSImageNameTouchBarSlideshowTemplate);
			case NSImageName.TouchBarTagIconTemplate:
				return ImageNamed (NSImageNameTouchBarTagIconTemplate);
			case NSImageName.TouchBarTextBoldTemplate:
				return ImageNamed (NSImageNameTouchBarTextBoldTemplate);
			case NSImageName.TouchBarTextBoxTemplate:
				return ImageNamed (NSImageNameTouchBarTextBoxTemplate);
			case NSImageName.TouchBarTextCenterAlignTemplate:
				return ImageNamed (NSImageNameTouchBarTextCenterAlignTemplate);
			case NSImageName.TouchBarTextItalicTemplate:
				return ImageNamed (NSImageNameTouchBarTextItalicTemplate);
			case NSImageName.TouchBarTextJustifiedAlignTemplate:
				return ImageNamed (NSImageNameTouchBarTextJustifiedAlignTemplate);
			case NSImageName.TouchBarTextLeftAlignTemplate:
				return ImageNamed (NSImageNameTouchBarTextLeftAlignTemplate);
			case NSImageName.TouchBarTextListTemplate:
				return ImageNamed (NSImageNameTouchBarTextListTemplate);
			case NSImageName.TouchBarTextRightAlignTemplate:
				return ImageNamed (NSImageNameTouchBarTextRightAlignTemplate);
			case NSImageName.TouchBarTextStrikethroughTemplate:
				return ImageNamed (NSImageNameTouchBarTextStrikethroughTemplate);
			case NSImageName.TouchBarTextUnderlineTemplate:
				return ImageNamed (NSImageNameTouchBarTextUnderlineTemplate);
			case NSImageName.TouchBarUserAddTemplate:
				return ImageNamed (NSImageNameTouchBarUserAddTemplate);
			case NSImageName.TouchBarUserGroupTemplate:
				return ImageNamed (NSImageNameTouchBarUserGroupTemplate);
			case NSImageName.TouchBarUserTemplate:
				return ImageNamed (NSImageNameTouchBarUserTemplate);
			case NSImageName.TouchBarVolumeDownTemplate:
				return ImageNamed (NSImageNameTouchBarVolumeDownTemplate);
			case NSImageName.TouchBarVolumeUpTemplate:
				return ImageNamed (NSImageNameTouchBarVolumeUpTemplate);
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
