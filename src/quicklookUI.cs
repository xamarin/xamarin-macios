using ObjCRuntime;
using Foundation;
using CoreGraphics;
using AppKit;

using System;
using System.ComponentModel;
using UniformTypeIdentifiers;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace QuickLookUI {

	[Native]
	enum QLPreviewViewStyle : ulong {
		Normal = 0,
		Compact = 1
	}

	interface IQLPreviewPanelDataSource { }

	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface QLPreviewPanelDataSource {
		[Export ("numberOfPreviewItemsInPreviewPanel:")]
		[Abstract]
		nint NumberOfPreviewItemsInPreviewPanel (QLPreviewPanel panel);

		[Export ("previewPanel:previewItemAtIndex:")]
		[Abstract]
		IQLPreviewItem PreviewItemAtIndex (QLPreviewPanel panel, nint index);
	}

	interface IQLPreviewPanelDelegate { }

	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface QLPreviewPanelDelegate : NSWindowDelegate {
		[Export ("previewPanel:handleEvent:")]
		bool HandleEvent (QLPreviewPanel panel, NSEvent theEvent);

		[Export ("previewPanel:sourceFrameOnScreenForPreviewItem:")]
		CGRect SourceFrameOnScreenForPreviewItem (QLPreviewPanel panel, IQLPreviewItem item);

		[Export ("previewPanel:transitionImageForPreviewItem:contentRect:")]
		NSObject TransitionImageForPreviewItem (QLPreviewPanel panel, IQLPreviewItem item, CGRect contentRect);
	}

	interface IQLPreviewItem { }

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface QLPreviewItem {
#if NET
		[Abstract]
#endif
		[Export ("previewItemURL")]
#if NET
		NSUrl PreviewItemUrl { get; }
#else
		NSUrl PreviewItemURL { get; }
#endif

		[Export ("previewItemTitle")]
		string PreviewItemTitle { get; }

		[Export ("previewItemDisplayState")]
		NSObject PreviewItemDisplayState { get; }
	}

	[Category]
	[BaseType (typeof (NSObject))]
	interface QLPreviewPanelController {

		[Export ("acceptsPreviewPanelControl:")]
		bool AcceptsPreviewPanelControl (QLPreviewPanel panel);

		[Export ("beginPreviewPanelControl:")]
		void BeginPreviewPanelControl (QLPreviewPanel panel);

		[Export ("endPreviewPanelControl:")]
		void EndPreviewPanelControl (QLPreviewPanel panel);
	}

	[BaseType (typeof (NSPanel))]
	interface QLPreviewPanel {
		[Export ("currentController")]
		NSObject CurrentController { get; }

		[Export ("dataSource", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[NullAllowed]
		IQLPreviewPanelDataSource DataSource { get; set; }

		[Export ("currentPreviewItemIndex")]
		nint CurrentPreviewItemIndex { get; set; }

		[Export ("currentPreviewItem")]
		IQLPreviewItem CurrentPreviewItem { get; }

		[Export ("displayState", ArgumentSemantic.Retain)]
		NSObject DisplayState { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IQLPreviewPanelDelegate Delegate { get; set; }

		[Export ("inFullScreenMode")]
		bool InFullScreenMode { [Bind ("isInFullScreenMode")] get; }

		[Static, Export ("sharedPreviewPanel")]
		QLPreviewPanel SharedPreviewPanel ();

		[Static, Export ("sharedPreviewPanelExists")]
		bool SharedPreviewPanelExists ();

		[Export ("updateController")]
		void UpdateController ();

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("refreshCurrentPreviewItem")]
		void RefreshCurrentPreviewItem ();

		// @required - (BOOL)enterFullScreenMode:(NSScreen *)screen withOptions:(NSDictionary *)options;
		[Export ("enterFullScreenMode:withOptions:")]
		bool EnterFullScreenMode ([NullAllowed] NSScreen screen, [NullAllowed] NSDictionary options);

		// @required - (void)exitFullScreenModeWithOptions:(NSDictionary *)options;
		[Export ("exitFullScreenModeWithOptions:")]
		void ExitFullScreenModeWithOptions ([NullAllowed] NSDictionary options);
	}

	[BaseType (typeof (NSView))] // Mac 10.6
	interface QLPreviewView {

		[Export ("initWithFrame:style:")]
		NativeHandle Constructor (CGRect frame, QLPreviewViewStyle style);

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("previewItem", ArgumentSemantic.Retain)]
		IQLPreviewItem PreviewItem { get; set; }

		[Export ("refreshPreviewItem")]
		void RefreshPreviewItem ();

		[Export ("displayState", ArgumentSemantic.Retain)]
		NSObject DisplayState { get; set; }

		[Export ("close")]
		void Close ();

		[Export ("shouldCloseWithWindow")]
		bool ShouldCloseWithWindow { get; set; }

		[Export ("autostarts")]
		bool Autostarts { get; set; }
	}

	[Protocol]
	interface QLPreviewingController {
#if !NET
		[Abstract]
#endif
		[Export ("preparePreviewOfSearchableItemWithIdentifier:queryString:completionHandler:")]
		void PreparePreviewOfSearchableItem (string identifier, string queryString, Action<NSError> ItemLoadingHandler);

		[Export ("preparePreviewOfFileAtURL:completionHandler:")]
		void PreparePreviewOfFile (NSUrl url, Action<NSError> completionHandler);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("providePreviewForFileRequest:completionHandler:")]
		void ProvidePreview (QLFilePreviewRequest request, Action<QLPreviewReply, NSError> handler);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface QLFilePreviewRequest {
		[Export ("fileURL")]
		NSUrl FileUrl { get; }
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface QLPreviewProvider : NSExtensionRequestHandling {
	}

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

	delegate bool QLPreviewReplyDrawingHandler (CGContext context, QLPreviewReply reply, out NSError error);
	delegate NSData QLPreviewReplyDataCreationHandler (QLPreviewReply reply, out NSError error);
	delegate CGPDFDocument QLPreviewReplyUIDocumentCreationHandler (QLPreviewReply reply, out NSError error);

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
}
