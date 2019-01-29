//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2012 Xamarin Inc
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
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using System;
using System.ComponentModel;

namespace QuickLook {
#if !MONOMAC
	[BaseType (typeof (UIViewController), Delegates = new string [] { "WeakDelegate" }, Events=new Type [] { typeof (QLPreviewControllerDelegate)})]
	interface QLPreviewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("dataSource", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDataSource { get; set; }
		
		[Wrap ("WeakDataSource")]
		[Protocolize]
		QLPreviewControllerDataSource DataSource { get; set;  }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		QLPreviewControllerDelegate Delegate { get; set; }
		
		[Export ("currentPreviewItemIndex")]
		nint CurrentPreviewItemIndex  { get; set;  }

		[Export ("currentPreviewItem")]
		[Protocolize]
		QLPreviewItem CurrentPreviewItem { get;  }

		[Static]
		[Export ("canPreviewItem:")]
		bool CanPreviewItem ([Protocolize] QLPreviewItem item);

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("refreshCurrentPreviewItem")]
		void RefreshCurrentPreviewItem ();
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface QLPreviewControllerDataSource {
		[Abstract]
		[Export ("numberOfPreviewItemsInPreviewController:")]
		nint PreviewItemCount (QLPreviewController controller);

		[Abstract]
		[Export ("previewController:previewItemAtIndex:")]
		[return:Protocolize]
		QLPreviewItem GetPreviewItem (QLPreviewController controller, nint index);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface QLPreviewControllerDelegate {
		[Export ("previewControllerWillDismiss:")]
		void WillDismiss (QLPreviewController controller);

		[Export ("previewControllerDidDismiss:")]
		void DidDismiss (QLPreviewController controller);

		[Export ("previewController:shouldOpenURL:forPreviewItem:"), DelegateName ("QLOpenUrl"), DefaultValue (false)]
		bool ShouldOpenUrl (QLPreviewController controller, NSUrl url, [Protocolize] QLPreviewItem item);

#if !MONOMAC
		// UIView and UIImage do not exists in MonoMac
		
		[Export ("previewController:frameForPreviewItem:inSourceView:"), DelegateName ("QLFrame"), DefaultValue (typeof (CGRect))]
		CGRect FrameForPreviewItem (QLPreviewController controller, [Protocolize] QLPreviewItem item, ref UIView view);
		
		[Export ("previewController:transitionImageForPreviewItem:contentRect:"), DelegateName ("QLTransition"), DefaultValue (null)]
		[return: NullAllowed]
		UIImage TransitionImageForPreviewItem (QLPreviewController controller, [Protocolize] QLPreviewItem item, CGRect contentRect);

		[iOS (10,0)]
		[Export ("previewController:transitionViewForPreviewItem:"), DelegateName ("QLTransitionView"), DefaultValue (null)]
		[return: NullAllowed]
		UIView TransitionViewForPreviewItem (QLPreviewController controller, IQLPreviewItem item);
#endif
	}

	interface IQLPreviewItem {}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface QLPreviewItem {
		[Abstract]
		[Export ("previewItemURL")]
#if XAMCORE_4_0
		NSUrl PreviewItemUrl { get; }
#else
		NSUrl ItemUrl { get; }
#endif

		[Export ("previewItemTitle")]
#if !XAMCORE_4_0
		[Abstract]
		string ItemTitle { get; }
#else
		string PreviewItemTitle { get; }
#endif
	}

	[iOS (11,0)]
	[Protocol]
	interface QLPreviewingController {
		[Export ("preparePreviewOfSearchableItemWithIdentifier:queryString:completionHandler:")]
		void PreparePreviewOfSearchableItem (string identifier, [NullAllowed] string queryString, Action<NSError> handler);

		[Export ("preparePreviewOfFileAtURL:completionHandler:")]
		void PreparePreviewOfFile (NSUrl url, Action<NSError> handler);
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface QLThumbnailProvider {
		[Export ("provideThumbnailForFileRequest:completionHandler:")]
		void ProvideThumbnail (QLFileThumbnailRequest request, Action<QLThumbnailReply, NSError> handler);
	}

	[ThreadSafe] // Members get called inside 'QLThumbnailProvider.ProvideThumbnail' which runs on a background thread.
	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface QLThumbnailReply {
		[Static]
		[Export ("replyWithContextSize:drawingBlock:")]
		QLThumbnailReply CreateReply (CGSize contextSize, Func<CGContext, bool> drawingBlock);

		[Static]
		[Export ("replyWithContextSize:currentContextDrawingBlock:")]
		QLThumbnailReply CreateReply (CGSize contextSize, Func<bool> drawingBlock);

		[Static]
		[Export ("replyWithImageFileURL:")]
		QLThumbnailReply CreateReply (NSUrl fileUrl);
	}

	[ThreadSafe]
	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface QLFileThumbnailRequest {
		[Export ("maximumSize")]
		CGSize MaximumSize { get; }

		[Export ("minimumSize")]
		CGSize MinimumSize { get; }

		[Export ("scale")]
		nfloat Scale { get; }

		[Export ("fileURL", ArgumentSemantic.Copy)]
		NSUrl FileUrl { get; }
	}
#else
	[Static]
	interface QLThumbnailImage {
		[Internal, Field ("kQLThumbnailOptionScaleFactorKey")]
		NSString OptionScaleFactorKey { get; }

		[Internal, Field ("kQLThumbnailOptionIconModeKey")]
		NSString OptionIconModeKey { get; }
	}
#endif
}
