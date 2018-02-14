//
// Copyright 2011, Novell, Inc.
// Copyright 2011, Regan Sarwas
//
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

//
// imagekit.cs: Bindings for the Image Kit API
//
using System;
using AppKit;
using Foundation;
using ObjCRuntime;
using CoreImage;
//using ImageCaptureCore;
using CoreGraphics;
using CoreAnimation;

namespace ImageKit {

	[BaseType (typeof (NSView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (IKCameraDeviceViewDelegate)})]
	interface IKCameraDeviceView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		IKCameraDeviceViewDelegate Delegate { get; set; }
		
//		FIXME need ImageCaptureCore;
//		[Export ("cameraDevice", ArgumentSemantic.Assign)]
//		ICCameraDevice CameraDevice { get; set;  }

		[Export ("hasDisplayModeTable")]
		bool HasDisplayModeTable { get; set;  }

		[Export ("hasDisplayModeIcon")]
		bool HasDisplayModeIcon { get; set;  }

		[Export ("downloadAllControlLabel", ArgumentSemantic.Copy)]
		string DownloadAllControlLabel { get; set;  }

		[Export ("downloadSelectedControlLabel", ArgumentSemantic.Copy)]
		string DownloadSelectedControlLabel { get; set;  }

		[Export ("iconSize")]
		nint IconSize { get; set;  }

		[Export ("transferMode")]
		IKCameraDeviceViewTransferMode TransferMode { get; set;  }

		[Export ("displaysDownloadsDirectoryControl")]
		bool DisplaysDownloadsDirectoryControl { get; set;  }

		[Export ("downloadsDirectory", ArgumentSemantic.Retain)]
		NSUrl DownloadsDirectory { get; set;  }

		[Export ("displaysPostProcessApplicationControl")]
		bool DisplaysPostProcessApplicationControl { get; set;  }

		[Export ("postProcessApplication", ArgumentSemantic.Retain)]
		NSUrl PostProcessApplication { get; set;  }

		[Export ("canRotateSelectedItemsLeft")]
		bool CanRotateSelectedItemsLeft { get;  }

		[Export ("canRotateSelectedItemsRight")]
		bool CanRotateSelectedItemsRight { get;  }

		[Export ("canDeleteSelectedItems")]
		bool CanDeleteSelectedItems { get;  }

		[Export ("canDownloadSelectedItems")]
		bool CanDownloadSelectedItems { get;  }

		[Export ("selectedIndexes")]
		NSIndexSet SelectedIndexes { get;  }

		[Export ("selectIndexes:byExtendingSelection:")]
		void SelectItemsAt (NSIndexSet indexes, bool extendSelection);

		[Export ("rotateLeft:")]
		void RotateLeft (NSObject sender);

		[Export ("rotateRight:")]
		void RotateRight (NSObject sender);

		[Export ("deleteSelectedItems:")]
		void DeleteSelectedItems (NSObject sender);

		[Export ("downloadSelectedItems:")]
		void DownloadSelectedItems (NSObject sender);

		[Export ("downloadAllItems:")]
		void DownloadAllItems (NSObject sender);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface IKCameraDeviceViewDelegate {
		[Export ("cameraDeviceViewSelectionDidChange:"), EventArgs ("IKCameraDeviceView")]
		void SelectionDidChange (IKCameraDeviceView cameraDeviceView);

//		FIXME need ImageCaptureCore;
//		[Export ("cameraDeviceView:didDownloadFile:location:fileData:error:"), EventArgs ("IKCameraDeviceViewICCameraFileNSUrlNSDataNSError")]
//		void DidDownloadFile (IKCameraDeviceView cameraDeviceView, ICCameraFile file, NSUrl url, NSData data, NSError error);

		[Export ("cameraDeviceView:didEncounterError:"), EventArgs ("IKCameraDeviceViewNSError")]
		void DidEncounterError (IKCameraDeviceView cameraDeviceView, NSError error);
	}

	[BaseType (typeof (NSView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (IKDeviceBrowserViewDelegate)})]
	interface IKDeviceBrowserView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		IKDeviceBrowserViewDelegate Delegate { get; set; }
		
		[Export ("displaysLocalCameras")]
		bool DisplaysLocalCameras { get; set;  }

		[Export ("displaysNetworkCameras")]
		bool DisplaysNetworkCameras { get; set;  }

		[Export ("displaysLocalScanners")]
		bool DisplaysLocalScanners { get; set;  }

		[Export ("displaysNetworkScanners")]
		bool DisplaysNetworkScanners { get; set;  }

		[Export ("mode")]
		IKDeviceBrowserViewDisplayMode Mode { get; set;  }

//		FIXME need ImageCaptureCore;
//		[Export ("selectedDevice")]
//		ICDevice SelectedDevice { get;  }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface IKDeviceBrowserViewDelegate {
//		FIXME need ImageCaptureCore;
//		[Abstract]
//		[Export ("deviceBrowserView:selectionDidChange:"), EventArgs ("IKDeviceBrowserViewICDevice")]
//		void SelectionDidChange (IKDeviceBrowserView deviceBrowserView, ICDevice device);

		[Export ("deviceBrowserView:didEncounterError:"), EventArgs ("IKDeviceBrowserViewNSError")]
		void DidEncounterError (IKDeviceBrowserView deviceBrowserView, NSError error);
	}

	[BaseType (typeof (NSPanel))]
	interface IKFilterBrowserPanel {
		[Static]
		[Export ("filterBrowserPanelWithStyleMask:")]
		IKFilterBrowserPanel Create (IKFilterBrowserPanelStyleMask styleMask);

		[Export ("filterName")]
		string FilterName { get; }
		
		//FIXME - can we do this in a more C#ish way.
		[Export ("beginWithOptions:modelessDelegate:didEndSelector:contextInfo:")]
		void Begin (NSDictionary options, NSObject modelessDelegate, Selector didEndSelector, IntPtr contextInfo);

		[Export ("beginSheetWithOptions:modalForWindow:modalDelegate:didEndSelector:contextInfo:")]
		void BeginSheet (NSDictionary options, NSWindow docWindow, NSObject modalDelegate, Selector didEndSelector, IntPtr contextInfo);

		[Export ("runModalWithOptions:")]
		int RunModal (NSDictionary options); /* int, not NSInteger */

		[Export ("filterBrowserViewWithOptions:")]
		IKFilterBrowserView FilterBrowserView (NSDictionary options);

		[Export ("finish:")]
		void Finish (NSObject sender);

		//Check - Do we need Notifications strings?
		[Notification]
		[Field ("IKFilterBrowserFilterSelectedNotification")]
		NSString FilterSelectedNotification { get; }

		[Notification]
		[Field ("IKFilterBrowserFilterDoubleClickNotification")]
		NSString FilterDoubleClickNotification { get; }

		[Notification]
		[Field ("IKFilterBrowserWillPreviewFilterNotification")]
		NSString WillPreviewFilterNotification { get; }

		//Dictionary Keys
		[Field ("IKFilterBrowserShowCategories")]
		NSString ShowCategories { get; }

		[Field ("IKFilterBrowserShowPreview")]
		NSString ShowPreview { get; }

		[Field ("IKFilterBrowserExcludeCategories")]
		NSString ExcludeCategories { get; }

		[Field ("IKFilterBrowserExcludeFilters")]
		NSString ExcludeFilters { get; }

		[Field ("IKFilterBrowserDefaultInputImage")]
		NSString DefaultInputImage { get; }
	}

	[BaseType (typeof (NSView))]
	interface IKFilterBrowserView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("setPreviewState:")]
		void SetPreviewState (bool showPreview);

		[Export ("filterName")]
		string FilterName { get; }
	}

	//This protocol is an addition to CIFilter.  It is implemented by any filter that provides its own user interface.
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface IKFilterCustomUIProvider {
		// The Apple documentation says the native implementation of CIFilter.GetFilterUIView will call
		// this method (if it exists). This means that This method should not be called GetFilterUIView
		// (because it seems like you shouldn't override CIFilter.GetFilterUIView, and implementing
		// IIKFilterCustomUIProvider.GetFilterUIView in a CIFilter subclass without overriding CIFilter.GetFilterUIView
		// just turns ugly). So rename this for new-style assemblies to ProvideFilterUIView.
		[Abstract]
		[Export ("provideViewForUIConfiguration:excludedKeys:")]
#if XAMCORE_2_0
		IKFilterUIView ProvideFilterUIView (NSDictionary configurationOptions, [NullAllowed] NSArray excludedKeys);
#else
		IKFilterUIView GetFilterUIView (NSDictionary configurationOptions, [NullAllowed] NSArray excludedKeys);
#endif

		//UIConfiguration keys for NSDictionary
		[Field ("IKUISizeFlavor")]
		NSString SizeFlavor { get; }
		
		[Field ("IKUISizeMini")]
		NSString SizeMini { get; }

		[Field ("IKUISizeSmall")]
		NSString SizeSmall { get; }

		[Field ("IKUISizeRegular")]
		NSString SizeRegular { get; }

		[Field ("IKUImaxSize")]
		NSString MaxSize { get; }

		[Field ("IKUIFlavorAllowFallback")]
		NSString FlavorAllowFallback { get; }
	}

	[BaseType (typeof (NSView))]
	interface IKFilterUIView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("initWithFrame:filter:")]
		IntPtr Constructor (CGRect frame, CIFilter filter);

		[Export ("filter")]
		CIFilter Filter { get; }

		[Export ("objectController")]
		NSObjectController ObjectController { get; }
	}

	[BaseType (typeof (NSObject))]
	interface IKImageBrowserCell {
		[Export ("imageBrowserView")]
		IKImageBrowserView ImageBrowserView  { get; }

		[Export ("representedItem")]
		NSObject RepresentedItem  { get; }

		[Export ("indexOfRepresentedItem")]
		nint IndexOfRepresentedItem  { get; }

		[Export ("frame")]
		CGRect Frame  { get; }

		[Export ("imageContainerFrame")]
		CGRect ImageContainerFrame  { get; }

		[Export ("imageFrame")]
		CGRect ImageFrame  { get; }

		[Export ("selectionFrame")]
		CGRect SelectionFrame  { get; }

		[Export ("titleFrame")]
		CGRect TitleFrame  { get; }

		[Export ("subtitleFrame")]
		CGRect SubtitleFrame  { get; }

		[Export ("imageAlignment")]
		NSImageAlignment ImageAlignment  { get; }

		[Export ("isSelected")]
		bool IsSelected  { get; }

		[Export ("cellState")]
		IKImageBrowserCellState CellState  { get; }

		[Export ("opacity")]
		nfloat Opacity  { get; }

		[Export ("layerForType:")]
		CALayer Layer (string layerType);

		// layerType is one of the following
		[Field ("IKImageBrowserCellBackgroundLayer")]
		NSString BackgroundLayer { get; }

		[Field ("IKImageBrowserCellForegroundLayer")]
		NSString ForegroundLayer { get; }

		[Field ("IKImageBrowserCellSelectionLayer")]
		NSString SelectionLayer { get; }

		[Field ("IKImageBrowserCellPlaceHolderLayer")]
		NSString PlaceHolderLayer { get; }
	}

	[BaseType (typeof (NSView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (IKImageBrowserDelegate)})]
	interface IKImageBrowserView {
		//@category IKImageBrowserView (IKMainMethods)
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		//Having a weak and strong datasource seems to work.
		[Export ("dataSource", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		IKImageBrowserDataSource DataSource { get; set; }

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		IKImageBrowserDelegate Delegate { get; set; }

		//@category IKImageBrowserView (IKAppearance)
		[Export ("cellsStyleMask")]
		IKCellsStyle CellsStyleMask { get; set; }

		[Export ("constrainsToOriginalSize")]
		bool ConstrainsToOriginalSize { get; set; }

		[Export ("backgroundLayer")]
		CALayer BackgroundLayer { get; set; }

		[Export ("foregroundLayer")]
		CALayer ForegroundLayer { get; set; }

		[Export ("newCellForRepresentedItem:")]
		IKImageBrowserCell NewCell ([Protocolize] IKImageBrowserItem representedItem);

		[Export ("cellForItemAtIndex:")]
		IKImageBrowserCell GetCellAt (nint itemIndex);

		//@category IKImageBrowserView (IKBrowsing)
		[Export ("zoomValue")]
		float ZoomValue { get; set; } /* float, not CGFloat */

		[Export ("contentResizingMask")]
		NSViewResizingMask ContentResizingMask  { get; set; }

		[Export ("scrollIndexToVisible:")]
		void ScrollIndexToVisible (nint index);

		[Export ("cellSize")]
		CGSize CellSize  { get; set; }

		[Export ("intercellSpacing")]
		CGSize IntercellSpacing { get; set; }

		[Export ("indexOfItemAtPoint:")]
		nint GetIndexOfItem (CGPoint point);

		[Export ("itemFrameAtIndex:")]
		CGRect GetItemFrame (nint index);

		[Export ("visibleItemIndexes")]
		NSIndexSet GetVisibleItemIndexes ();

		[Export ("rowIndexesInRect:")]
		NSIndexSet GetRowIndexes (CGRect rect);

		[Export ("columnIndexesInRect:")]
		NSIndexSet GetColumnIndexes (CGRect rect);

		[Export ("rectOfColumn:")]
		CGRect GetRectOfColumn (nint columnIndex);

		[Export ("rectOfRow:")]
		CGRect GetRectOfRow (nint rowIndex);

		[Export ("numberOfRows")]
		nint RowCount { get; }

		[Export ("numberOfColumns")]
		nint ColumnCount { get; }

		[Export ("canControlQuickLookPanel")]
		bool CanControlQuickLookPanel { get; set; }

		//@category IKImageBrowserView (IKSelectionReorderingAndGrouping)
		[Export ("selectionIndexes")]
		NSIndexSet SelectionIndexes  { get; }

		[Export ("setSelectionIndexes:byExtendingSelection:")]
		void SelectItemsAt (NSIndexSet indexes, bool extendSelection);

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection  { get; set; }

		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection  { get; set; }

		[Export ("allowsReordering")]
		bool AllowsReordering  { get; set; }

		[Export ("animates")]
		bool Animates  { get; set; }

		[Export ("expandGroupAtIndex:")]
		void ExpandGroup (nint index);

		[Export ("collapseGroupAtIndex:")]
		void CollapseGroup (nint index);

		[Export ("isGroupExpandedAtIndex:")]
		bool IsGroupExpanded (nint index);

		//@category IKImageBrowserView (IKDragNDrop)
		[Export ("draggingDestinationDelegate", ArgumentSemantic.Weak)]
		[Protocolize]
		NSDraggingDestination DraggingDestinationDelegate  { get; set; }

		[Export ("indexAtLocationOfDroppedItem")]
		nint GetIndexAtLocationOfDroppedItem ();

		[Export ("dropOperation")]
		IKImageBrowserDropOperation DropOperation ();

		[Export ("allowsDroppingOnItems")]
		bool AllowsDroppingOnItems  { get; set; }

		[Export ("setDropIndex:dropOperation:")]
		void SetDropIndex (nint index, IKImageBrowserDropOperation operation);

		// Keys for the view options, set with base.setValue
		[Field ("IKImageBrowserBackgroundColorKey")]
		NSString BackgroundColorKey { get; }
		
		[Field ("IKImageBrowserSelectionColorKey")]
		NSString SelectionColorKey { get; }
		
		[Field ("IKImageBrowserCellsOutlineColorKey")]
		NSString CellsOutlineColorKey { get; }
		
		[Field ("IKImageBrowserCellsTitleAttributesKey")]
		NSString CellsTitleAttributesKey { get; }
		
		[Field ("IKImageBrowserCellsHighlightedTitleAttributesKey")]
		NSString CellsHighlightedTitleAttributesKey { get; }
		
		[Field ("IKImageBrowserCellsSubtitleAttributesKey")]
		NSString CellsSubtitleAttributesKey { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (IsInformal = true)]
	interface IKImageBrowserDataSource {
		[Abstract]
		[Export ("numberOfItemsInImageBrowser:")]
		nint ItemCount (IKImageBrowserView aBrowser);

		[Abstract]
		[Export ("imageBrowser:itemAtIndex:")]
#if XAMCORE_2_0
		IIKImageBrowserItem GetItem (IKImageBrowserView aBrowser, nint index);
#else
		IKImageBrowserItem GetItem (IKImageBrowserView aBrowser, nint index);
#endif

		[Export ("imageBrowser:removeItemsAtIndexes:")]
		void RemoveItems (IKImageBrowserView aBrowser, NSIndexSet indexes);

		[Export ("imageBrowser:moveItemsAtIndexes:toIndex:")]
		bool MoveItems (IKImageBrowserView aBrowser, NSIndexSet indexes, nint destinationIndex);

		[Export ("imageBrowser:writeItemsAtIndexes:toPasteboard:")]
		nint WriteItemsToPasteboard (IKImageBrowserView aBrowser, NSIndexSet itemIndexes, NSPasteboard pasteboard);

		[Export ("numberOfGroupsInImageBrowser:")]
		nint GroupCount (IKImageBrowserView aBrowser);

		[Export ("imageBrowser:groupAtIndex:")]
		NSDictionary GetGroup (IKImageBrowserView aBrowser, nint index);

		// Keys for Dictionary returned by GetGroup
		[Field ("IKImageBrowserGroupRangeKey")]
		NSString GroupRangeKey { get; }

		[Field ("IKImageBrowserGroupBackgroundColorKey")]
		NSString GroupBackgroundColorKey { get; }

		[Field ("IKImageBrowserGroupTitleKey")]
		NSString GroupTitleKey { get; }

		[Field ("IKImageBrowserGroupStyleKey")]
		NSString GroupStyleKey { get; }

		[Field ("IKImageBrowserGroupHeaderLayer")]
		NSString GroupHeaderLayer { get; }

		[Field ("IKImageBrowserGroupFooterLayer")]
		NSString GroupFooterLayer { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (IsInformal = true)]
	interface IKImageBrowserItem {
		[Abstract]
		[Export ("imageUID")]
		string ImageUID { get; }

		[Abstract]
		[Export ("imageRepresentationType")]
		NSString ImageRepresentationType { get; }

		//possible strings returned by ImageRepresentationType
		[Field ("IKImageBrowserPathRepresentationType")]
		NSString PathRepresentationType { get; }

		[Field ("IKImageBrowserNSURLRepresentationType")]
		NSString NSURLRepresentationType { get; }

		[Field ("IKImageBrowserNSImageRepresentationType")]
		NSString NSImageRepresentationType { get; }

		[Field ("IKImageBrowserCGImageRepresentationType")]
		NSString CGImageRepresentationType { get; }

		[Field ("IKImageBrowserCGImageSourceRepresentationType")]
		NSString CGImageSourceRepresentationType { get; }

		[Field ("IKImageBrowserNSDataRepresentationType")]
		NSString NSDataRepresentationType { get; }

		[Field ("IKImageBrowserNSBitmapImageRepresentationType")]
		NSString NSBitmapImageRepresentationType { get; }

		[Field ("IKImageBrowserQTMovieRepresentationType")]
		NSString QTMovieRepresentationType { get; }

		[Field ("IKImageBrowserQTMoviePathRepresentationType")]
		NSString QTMoviePathRepresentationType { get; }

		[Field ("IKImageBrowserQCCompositionRepresentationType")]
		NSString QCCompositionRepresentationType { get; }

		[Field ("IKImageBrowserQCCompositionPathRepresentationType")]
		NSString QCCompositionPathRepresentationType { get; }

		[Field ("IKImageBrowserQuickLookPathRepresentationType")]
		NSString QuickLookPathRepresentationType { get; }

		[Field ("IKImageBrowserIconRefPathRepresentationType")]
		NSString IconRefPathRepresentationType { get; }

		[Field ("IKImageBrowserIconRefRepresentationType")]
		NSString IconRefRepresentationType { get; }

		[Field ("IKImageBrowserPDFPageRepresentationType")]
		NSString PDFPageRepresentationType { get; }

		[Abstract]
		[Export ("imageRepresentation")]
		NSObject ImageRepresentation { get; }

		[Export ("imageVersion")]
		nint ImageVersion { get; }

		[Export ("imageTitle")]
		string ImageTitle { get; }

		[Export ("imageSubtitle")]
		string ImageSubtitle { get; }

		[Export ("isSelectable")]
		bool IsSelectable { get; }
	}

	interface IIKImageBrowserItem {}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (IsInformal = true)]
	interface IKImageBrowserDelegate {
		[Export ("imageBrowserSelectionDidChange:"), EventArgs ("IKImageBrowserView")]
		void SelectionDidChange (IKImageBrowserView browser);

		[Export ("imageBrowser:cellWasDoubleClickedAtIndex:"), EventArgs ("IKImageBrowserViewIndex")]
		void CellWasDoubleClicked (IKImageBrowserView browser, nint index);

		[Export ("imageBrowser:cellWasRightClickedAtIndex:withEvent:"), EventArgs ("IKImageBrowserViewIndexEvent")]
		void CellWasRightClicked (IKImageBrowserView browser, nint index, NSEvent nsevent);

		[Export ("imageBrowser:backgroundWasRightClickedWithEvent:"), EventArgs ("IKImageBrowserViewEvent")]
		void BackgroundWasRightClicked (IKImageBrowserView browser, NSEvent nsevent);
	}

	[BaseType (typeof (NSPanel))]
	[DisableDefaultCtor] // crash when disposed, sharedImageEditPanel must be used
	interface IKImageEditPanel {
		[Static]
		[Export ("sharedImageEditPanel")]
		IKImageEditPanel SharedPanel { get; }

		[Export ("dataSource", ArgumentSemantic.Assign), NullAllowed]
		[Protocolize]
		IKImageEditPanelDataSource DataSource { get; set; }

		[Export ("filterArray")]
		NSArray filterArray { get;  }

		[Export ("reloadData")]
		void ReloadData ();
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface IKImageEditPanelDataSource {
		[Abstract]
		[Export ("image")]
		CGImage Image { get; }

		[Abstract]
		[Export ("setImage:imageProperties:")]
		void SetImageAndProperties (CGImage image, NSDictionary metaData);

		[Export ("thumbnailWithMaximumSize:")]
		CGImage GetThumbnail (CGSize maximumSize);

		[Export ("imageProperties")]
		NSDictionary ImageProperties { get; }

		[Export ("hasAdjustMode")]
		bool HasAdjustMode { get; }

		[Export ("hasEffectsMode")]
		bool HasEffectsMode { get; }

		[Export ("hasDetailsMode")]
		bool HasDetailsMode { get; }
	}

	[BaseType (typeof (NSView))]
	interface IKImageView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		//There is no protocol for this delegate.  used to respond to messages in the responder chain
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject Delegate { get; set; }

		[Export ("zoomFactor")]
		nfloat ZoomFactor { get; set; }

		[Export ("rotationAngle")]
		nfloat RotationAngle { get; set; }

		[Export ("currentToolMode")]
		string CurrentToolMode { get; set; }

		[Export ("autoresizes")]
		bool Autoresizes { get; set; }

		[Export ("hasHorizontalScroller")]
		bool HasHorizontalScroller { get; set; }

		[Export ("hasVerticalScroller")]
		bool HasVerticalScroller { get; set; }

		[Export ("autohidesScrollers")]
		bool AutohidesScrollers { get; set; }

		[Export ("supportsDragAndDrop")]
		bool SupportsDragAndDrop { get; set; }

		[Export ("editable")]
		bool Editable { get; set; }

		[Export ("doubleClickOpensImageEditPanel")]
		bool DoubleClickOpensImageEditPanel { get; set; }

		[Export ("imageCorrection", ArgumentSemantic.Assign)]
		CIFilter ImageCorrection { get; set;  }

		[Export ("backgroundColor", ArgumentSemantic.Assign)]
		NSColor BackgroundColor { get; set;  }

#if !XAMCORE_4_0
		[Export ("setImage:imageProperties:")]
		void SetImageimageProperties (CGImage image, NSDictionary metaData);
#else
		[Export ("setImage:imageProperties:")]
		void SetImage (CGImage image, NSDictionary metaData);
#endif

		[Export ("setImageWithURL:")]
		void SetImageWithURL (NSUrl url);

		[Export ("image")]
		CGImage Image { get; }

		[Export ("imageSize")]
		CGSize ImageSize { get; }

		[Export ("imageProperties")]
		NSDictionary ImageProperties { get; }

		[Export ("setRotationAngle:centerPoint:")]
		void SetRotation (nfloat rotationAngle, CGPoint centerPoint);

		[Export ("rotateImageLeft:")]
		void RotateImageLeft (NSObject sender);

		[Export ("rotateImageRight:")]
		void RotateImageRight (NSObject sender);

		[Export ("setImageZoomFactor:centerPoint:")]
		void SetImageZoomFactor (nfloat zoomFactor, CGPoint centerPoint);

		[Export ("zoomImageToRect:")]
		void ZoomImageToRect (CGRect rect);

		[Export ("zoomImageToFit:")]
		void ZoomImageToFit (NSObject sender);

		[Export ("zoomImageToActualSize:")]
		void ZoomImageToActualSize (NSObject sender);

		[Export ("zoomIn:")]
		void ZoomIn (NSObject sender);

		[Export ("zoomOut:")]
		void ZoomOut (NSObject sender);

		[Export ("flipImageHorizontal:")]
		void FlipImageHorizontal (NSObject sender);

		[Export ("flipImageVertical:")]
		void FlipImageVertical (NSObject sender);

		[Export ("crop:")]
		void Crop (NSObject sender);

		[Export ("setOverlay:forType:")]
		void SetOverlay (CALayer layer, string layerType);

		[Export ("overlayForType:")]
		CALayer GetOverlay (string layerType);

		[Export ("scrollToPoint:")]
		void ScrollTo (CGPoint point);

		[Export ("scrollToRect:")]
		void ScrollTo (CGRect rect);

		[Export ("convertViewPointToImagePoint:")]
		CGPoint ConvertViewPointToImagePoint (CGPoint viewPoint);

		[Export ("convertViewRectToImageRect:")]
		CGRect ConvertViewRectToImageRect (CGRect viewRect);

		[Export ("convertImagePointToViewPoint:")]
		CGPoint ConvertImagePointToViewPoint (CGPoint imagePoint);

		[Export ("convertImageRectToViewRect:")]
		CGRect ConvertImageRectToViewRect (CGRect imageRect);
	}

	[BaseType (typeof (NSPanel))]
	interface IKPictureTaker {
		[Static]
		[Export ("pictureTaker")]
		IKPictureTaker SharedPictureTaker { get; }

		[Export ("runModal")]
		nint RunModal ();

		//FIXME - Yuck.  What can I do to fix these three methods?
		[Export ("beginPictureTakerWithDelegate:didEndSelector:contextInfo:")]
		void BeginPictureTaker (NSObject aDelegate, Selector didEndSelector, IntPtr contextInfo);

		[Export ("beginPictureTakerSheetForWindow:withDelegate:didEndSelector:contextInfo:")]
		void BeginPictureTakerSheet (NSWindow aWindow, NSObject aDelegate, Selector didEndSelector, IntPtr contextInfo);

		[Export ("popUpRecentsMenuForView:withDelegate:didEndSelector:contextInfo:")]
		void PopUpRecentsMenu (NSView aView, NSObject aDelegate, Selector didEndSelector, IntPtr contextInfo);

		[Export ("inputImage")]
		NSImage InputImage { get; set; }

		[Export ("outputImage")]
		NSImage GetOutputImage ();

		[Export ("mirroring")]
		bool Mirroring { get; set; }

		//Use with NSKeyValueCoding to customize the pictureTaker panel
		[Field ("IKPictureTakerAllowsVideoCaptureKey")]
		NSString AllowsVideoCaptureKey { get; }
		
		[Field ("IKPictureTakerAllowsFileChoosingKey")]
		NSString AllowsFileChoosingKey { get; }
		
		[Field ("IKPictureTakerShowRecentPictureKey")]
		NSString ShowRecentPictureKey { get; }
		
		[Field ("IKPictureTakerUpdateRecentPictureKey")]
		NSString UpdateRecentPictureKey { get; }

		[Field ("IKPictureTakerAllowsEditingKey")]
		NSString AllowsEditingKey { get; }

		[Field ("IKPictureTakerShowEffectsKey")]
		NSString ShowEffectsKey { get; }

		[Field ("IKPictureTakerInformationalTextKey")]
		NSString InformationalTextKey { get; }

		[Field ("IKPictureTakerImageTransformsKey")]
		NSString ImageTransformsKey { get; }

		[Field ("IKPictureTakerOutputImageMaxSizeKey")]
		NSString OutputImageMaxSizeKey { get; }

		[Field ("IKPictureTakerCropAreaSizeKey")]
		NSString CropAreaSizeKey { get; }

		[Field ("IKPictureTakerShowAddressBookPictureKey")]
		NSString ShowAddressBookPictureKey { get; }

		[Field ("IKPictureTakerShowEmptyPictureKey")]
		NSString ShowEmptyPictureKey { get; }

		[Field ("IKPictureTakerRemainOpenAfterValidateKey")]
		NSString RemainOpenAfterValidateKey { get; }
	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (IKSaveOptionsDelegate)})]
	interface IKSaveOptions {
		[Export ("imageProperties")]
		NSDictionary ImageProperties { get;  }

		[Export ("imageUTType")]
		string ImageUTType { get;  }

		[Export ("userSelection")]
		NSDictionary UserSelection { get;  }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		IKSaveOptionsDelegate Delegate { get; set; }

		[Export ("initWithImageProperties:imageUTType:")]
		IntPtr Constructor (NSDictionary imageProperties, string imageUTType);

		[Export ("addSaveOptionsAccessoryViewToSavePanel:")]
		void AddSaveOptionsToPanel (NSSavePanel savePanel);

		[Export ("addSaveOptionsToView:")]
		void AddSaveOptionsToView (NSView view);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (IsInformal = true)]
	interface IKSaveOptionsDelegate {
		[Export ("saveOptions:shouldShowUTType:"), DelegateName ("SaveOptionsShouldShowUTType"), DefaultValue (false)]
		bool ShouldShowType (IKSaveOptions saveOptions, string imageUTType);
	}

	[BaseType (typeof (NSView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (IKScannerDeviceViewDelegate)})]
	interface IKScannerDeviceView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		IKScannerDeviceViewDelegate Delegate { get; set; }

//		FIXME need ImageCaptureCore;
//		[Export ("scannerDevice", ArgumentSemantic.Assign)]
//		ICScannerDevice ScannerDevice { get; set; }

		[Export ("mode")]
		IKScannerDeviceViewDisplayMode DisplayMode { get; set; }

		[Export ("hasDisplayModeSimple")]
		bool HasDisplayModeSimple { get; set; }

		[Export ("hasDisplayModeAdvanced")]
		bool HasDisplayModeAdvanced { get; set; }

		[Export ("transferMode")]
		IKScannerDeviceViewTransferMode TransferMode { get; set; }

		[Export ("scanControlLabel", ArgumentSemantic.Copy)]
		string ScanControlLabel { get; set; }

		[Export ("overviewControlLabel", ArgumentSemantic.Copy)]
		string OverviewControlLabel { get; set; }

		[Export ("displaysDownloadsDirectoryControl")]
		bool DisplaysDownloadsDirectoryControl { get; set; }

		[Export ("downloadsDirectory", ArgumentSemantic.Retain)]
		NSUrl DownloadsDirectory { get; set; }

		[Export ("documentName", ArgumentSemantic.Copy)]
		string DocumentName { get; set; }

		[Export ("displaysPostProcessApplicationControl")]
		bool DisplaysPostProcessApplicationControl { get; set; }

		[Export ("postProcessApplication", ArgumentSemantic.Retain)]
		NSUrl PostProcessApplication { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface IKScannerDeviceViewDelegate {
		[Export ("scannerDeviceView:didScanToURL:fileData:error:"), EventArgs ("IKScannerDeviceViewScan")]
		void DidScan (IKScannerDeviceView scannerDeviceView, NSUrl url, NSData data, NSError error);

		[Export ("scannerDeviceView:didEncounterError:"), EventArgs ("IKScannerDeviceViewError")]
		void DidEncounterError (IKScannerDeviceView scannerDeviceView, NSError error);
	}

	[BaseType (typeof (NSObject))]
	interface IKSlideshow {
		[Static]
		[Export ("sharedSlideshow")]
		IKSlideshow SharedSlideshow { get; }

		[Export ("autoPlayDelay")]
		double autoPlayDelay { get; set; }

		[Export ("runSlideshowWithDataSource:inMode:options:")]
		void RunSlideshow ([Protocolize] IKSlideshowDataSource dataSource, string slideshowMode, NSDictionary slideshowOptions);

		[Export ("stopSlideshow:")]
		void StopSlideshow (NSObject sender);

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("reloadSlideshowItemAtIndex:")]
		void ReloadSlideshowItem (nint index);

		[Export ("indexOfCurrentSlideshowItem")]
		nint IndexOfCurrentSlideshowItem { get; }

		[Static]
		[Export ("canExportToApplication:")]
		bool CanExportToApplication (string applicationBundleIdentifier);

		[Static]
		[Export ("exportSlideshowItem:toApplication:")]
		void ExportSlideshowItemtoApplication (NSObject item, string applicationBundleIdentifier);

		[Field ("IKSlideshowModeImages")]
		NSString ModeImages { get; }

		[Field ("IKSlideshowModePDF")]
		NSString ModePDF { get; }

		[Field ("IKSlideshowModeOther")]
		NSString ModeOther { get; }
		
		[Field ("IKSlideshowWrapAround")]
		NSString WrapAround { get; }

		[Field ("IKSlideshowStartPaused")]
		NSString StartPaused { get; }

		[Field ("IKSlideshowStartIndex")]
		NSString StartIndex { get; }

		[Field ("IKSlideshowScreen")]
		NSString Screen { get; }

		[Field ("IKSlideshowAudioFile")]
		NSString AudioFile { get; }

		[Field ("IKSlideshowPDFDisplayBox")]
		NSString PDFDisplayBox { get; }

		[Field ("IKSlideshowPDFDisplayMode")]
		NSString PDFDisplayMode { get; }

		[Field ("IKSlideshowPDFDisplaysAsBook")]
		NSString PDFDisplaysAsBook { get; }

		[Field ("IK_iPhotoBundleIdentifier")]
		NSString IPhotoBundleIdentifier { get; }

		[Field ("IK_ApertureBundleIdentifier")]
		NSString ApertureBundleIdentifier { get; }

		[Field ("IK_MailBundleIdentifier")]
		NSString MailBundleIdentifier { get; }

		[Mac (10,10,3)]
		[Field ("IK_PhotosBundleIdentifier")]
		NSString PhotosBundleIdentifier { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface IKSlideshowDataSource {
		[Abstract]
		[Export ("numberOfSlideshowItems")]
		nint ItemCount { get; }

		[Abstract]
		[Export ("slideshowItemAtIndex:")]
		NSObject GetItemAt (nint index);

		[Export ("nameOfSlideshowItemAtIndex:")]
		string GetNameOfItemAt (nint index);

		[Export ("canExportSlideshowItemAtIndex:toApplication:")]
		bool CanExportItemToApplication (nint index, string applicationBundleIdentifier);

		[Export ("slideshowWillStart")]
		void WillStart ();

		[Export ("slideshowDidStop")]
		void DidStop ();

		[Export ("slideshowDidChangeCurrentIndex:")]
		void DidChange (nint newIndex);
	}
}
