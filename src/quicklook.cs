//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2012 Xamarin Inc
// Copyright 2019 Microsoft Corporation
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
using UIWindowSceneActivationConfiguration=Foundation.NSObject;
#else
using UIKit;
#endif
using System;
using System.ComponentModel;
using UniformTypeIdentifiers;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace QuickLook {
#if !MONOMAC
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (QLPreviewControllerDelegate) })]
	interface QLPreviewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("dataSource", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		QLPreviewControllerDataSource DataSource { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		QLPreviewControllerDelegate Delegate { get; set; }

		[Export ("currentPreviewItemIndex")]
		nint CurrentPreviewItemIndex { get; set; }

		[Export ("currentPreviewItem")]
		[Protocolize]
		[NullAllowed]
		QLPreviewItem CurrentPreviewItem { get; }

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
	[NoMac]
	[MacCatalyst (13, 1)]
	interface QLPreviewControllerDataSource {
		[Abstract]
		[Export ("numberOfPreviewItemsInPreviewController:")]
		nint PreviewItemCount (QLPreviewController controller);

		[Abstract]
		[Export ("previewController:previewItemAtIndex:")]
		[return: Protocolize]
		QLPreviewItem GetPreviewItem (QLPreviewController controller, nint index);
	}

	[NoMac]
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum QLPreviewItemEditingMode : long {
		Disabled = 0,
		UpdateContents,
		CreateCopy,
	}

	[NoMac]
	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Export ("previewController:transitionViewForPreviewItem:"), DelegateName ("QLTransitionView"), DefaultValue (null)]
		[return: NullAllowed]
		UIView TransitionViewForPreviewItem (QLPreviewController controller, IQLPreviewItem item);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("previewController:editingModeForPreviewItem:"), DelegateName ("QLEditingMode"), DefaultValue ("QLPreviewItemEditingMode.Disabled")]
		QLPreviewItemEditingMode GetEditingMode (QLPreviewController controller, IQLPreviewItem previewItem);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("previewController:didUpdateContentsOfPreviewItem:"), EventArgs ("QLPreviewControllerDelegateDidUpdate")]
		void DidUpdateContents (QLPreviewController controller, IQLPreviewItem previewItem);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("previewController:didSaveEditedCopyOfPreviewItem:atURL:"), EventArgs ("QLPreviewControllerDelegateDidSave")]
		void DidSaveEditedCopy (QLPreviewController controller, IQLPreviewItem previewItem, NSUrl modifiedContentsUrl);

#endif
	}

	interface IQLPreviewItem { }

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface QLPreviewItem {
		[Abstract]
		[NullAllowed]
		[Export ("previewItemURL")]
#if NET
		NSUrl PreviewItemUrl { get; }
#else
		NSUrl ItemUrl { get; }
#endif

		[Export ("previewItemTitle")]
		[NullAllowed]
#if !NET
		[Abstract]
		string ItemTitle { get; }
#else
		string PreviewItemTitle { get; }
#endif
	}

	delegate bool QLPreviewReplyDrawingHandler (CGContext context, QLPreviewReply reply, out NSError error);
	delegate NSData QLPreviewReplyDataCreationHandler (QLPreviewReply reply, out NSError error);
	delegate CGPDFDocument QLPreviewReplyUIDocumentCreationHandler (QLPreviewReply reply, out NSError error);

	[NoMac]
	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface QLPreviewReply {
		[Export ("stringEncoding")]
		NSStringEncoding StringEncoding { get; set; }

		[Export ("attachments", ArgumentSemantic.Copy)]
		NSDictionary<NSString, QLPreviewReplyAttachment> Attachments { get; set; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("initWithContextSize:isBitmap:drawingBlock:")]
		NativeHandle Constructor (CGSize contextSize, bool isBitmap, QLPreviewReplyDrawingHandler drawingHandler);

		[Export ("initWithFileURL:")]
		NativeHandle Constructor (NSUrl fileUrl);

		[Export ("initWithDataOfContentType:contentSize:dataCreationBlock:")]
		NativeHandle Constructor (UTType contentType, CGSize contentSize, QLPreviewReplyDataCreationHandler dataCreationHandler);

		// QLPreviewReply_UI
		[Export ("initForPDFWithPageSize:documentCreationBlock:")]
		NativeHandle Constructor (CGSize defaultPageSize, QLPreviewReplyUIDocumentCreationHandler documentCreationHandler);
	}

	[NoMac]
	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface QLPreviewReplyAttachment {
		[Export ("data")]
		NSData Data { get; }

		[Export ("contentType")]
		UTType ContentType { get; }

		[Export ("initWithData:contentType:")]
		NativeHandle Constructor (NSData data, UTType contentType);
	}

	[NoMac]
	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface QLFilePreviewRequest {
		[Export ("fileURL")]
		NSUrl FileUrl { get; }
	}

	[NoMac]
	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface QLPreviewProvider : NSExtensionRequestHandling {
	}

	[NoWatch]
	[NoTV]
	[NoMac] // availability not mentioned in the header files
	[iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface QLPreviewSceneOptions {
		[Export ("initialPreviewIndex")]
		nint InitialPreviewIndex { get; set; }
	}

	[NoMac]
	[iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UIWindowSceneActivationConfiguration))]
	interface QLPreviewSceneActivationConfiguration {
		[Export ("initWithItemsAtURLs:options:")]

		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl [] urls, [NullAllowed] QLPreviewSceneOptions options);

		[Export ("initWithUserActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUserActivity userActivity);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface QLPreviewingController {
		[Export ("preparePreviewOfSearchableItemWithIdentifier:queryString:completionHandler:")]
		void PreparePreviewOfSearchableItem (string identifier, [NullAllowed] string queryString, Action<NSError> handler);

		[Export ("preparePreviewOfFileAtURL:completionHandler:")]
		void PreparePreviewOfFile (NSUrl url, Action<NSError> handler);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("providePreviewForFileRequest:completionHandler:")]
		void ProvidePreview (QLFilePreviewRequest request, Action<QLPreviewReply, NSError> handler);
	}
#else
	[Static]
	[NoiOS][NoMacCatalyst][NoWatch][NoTV]
	interface QLThumbnailImage {
		[Internal, Field ("kQLThumbnailOptionScaleFactorKey")]
		NSString OptionScaleFactorKey { get; }

		[Internal, Field ("kQLThumbnailOptionIconModeKey")]
		NSString OptionIconModeKey { get; }
	}
#endif

}
