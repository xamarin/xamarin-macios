//
// Obsoletes.cs: Contains methods declarations for consumption
// compatibility, that have been flagged with Obsoletes
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc
//
// MIT X11
//
using System;

using Foundation;

#if !XAMCORE_2_0

namespace AppKit {

	public partial class NSBrowser {
		[Obsolete ("Use 'GetItem' instead.")]
		public NSObject ItemAtRowinColumn (int row, int column)
		{
			return GetItem (row, column);
		}
	}

	public partial class NSMatrix {
		[Obsolete ("Use 'MakeCell' instead.")]
		public NSCell MakeCellAtRowcolumn (int row, int col)
		{
			return MakeCell (row, col);
		}

		[Obsolete ("Use 'SelectCell' instead.")]
		public void SelectCellAtRowcolumn(int row, int column)
		{
			SelectCell (row, column);
		}

		[Obsolete ("Use 'GetRowColumnForPoint' instead.")]
		public bool GetRowCoumnForPoint (out int row, out int column, CGPoint aPoint)
		{
			return GetRowColumnForPoint (out row, out column, aPoint);
		}

		[Obsolete ("Use 'PutCell' instead.")]
		public void PutCellatRowColumn (NSCell newCell, int row, int column)
		{
			PutCell (newCell, row, column);
		}

		[Obsolete ("Use 'InsertColumn' instead.")]
		public void InsertColumnwithCells (int column, NSCell [] newCells)
		{
			InsertColumn (column, newCells);
		}

		[Obsolete ("Use 'HighlightCell' instead.")]
		public void HighlightCellatRowColumn(bool highlight, int row, int column)
		{
			HighlightCell (highlight, row, column);
		}

		[Obsolete ("Use 'ScrollCellToVisible' instead.")]
		public void ScrollCellToVisibleAtRowcolumn (int row, int column)
		{
			ScrollCellToVisible (row, column);
		}
		
		[Obsolete ("Use 'SetToolTipForCell' instead.")]
		public void SetToolTipforCell (string toolTipString, NSCell cell)
		{
			SetToolTipForCell (toolTipString, cell);
		}
	}

	public partial class NSStatusItem {
		[Obsolete ("Use 'DrawStatusBarBackground' instead.")]
		public void DrawStatusBarBackgroundInRectwithHighlight (CGRect rect, bool highlight)
		{
			DrawStatusBarBackground (rect, highlight);
		}
	}

	public partial class NSBitmapImageRep {
		[Obsolete ("Use 'GetCompressionFactor' instead.")]
		public void GetCompressionfactor(out NSTiffCompression compression, out float factor)
		{
			GetCompressionFactor (out compression, out factor);
		}
		
		[Obsolete ("Use 'SetCompressionFactor' instead.")]
		public void SetCompressionfactor (NSTiffCompression compression, float factor)
		{
			SetCompressionFactor (compression, factor);
		}
		
	}

	public partial class NSEvent {
		[Obsolete ("Use 'TouchesMatchingPhase' instead.")]
		public NSSet TouchesMatchingPhaseinView (NSTouchPhase phase, NSView view)
		{
			return TouchesMatchingPhase (phase, view);
		}
		
	}

	public partial class NSMenu {
		[Obsolete ("Use 'InsertItem' instead.")]
		public void InsertItematIndex (NSMenuItem newItem, int index)
		{
			InsertItem (newItem, index);
		}
		
	}

	public partial class NSWindow {
		[Obsolete ("Use 'SetFrameFrom' instead.")]
		public void SetFrameFroom (string str)
		{
			SetFrameFrom (str);
		}
		
	}

	public partial class NSBrowserDelegate {
		[Obsolete ("Use 'SelectRowInColumn' instead.")]
		public bool SelectRowinColumn (NSBrowser sender, int row, int column)
		{
			return SelectRowInColumn (sender, row, column);
		}
		
		[Obsolete ("Use 'DidChangeLastColumn' instead.")]
		public void DidChangeLastColumntoColumn (NSBrowser browser, int oldLastColumn, int toColumn)
		{
			DidChangeLastColumn (browser, oldLastColumn, toColumn);
		}
		
	}

	public partial class NSDocumentController {
		[Obsolete ("Use 'RunModalOpenPanelforTypes' instead.")]
		public int RunModalOpenPanelforTypes (NSOpenPanel openPanel,string[] types)
		{
			return RunModalOpenPanel (openPanel, types);
		}
		
	}
	
	public partial class NSScroller {
		[Obsolete ("Use 'PreferredStyleChangedNotification'.")]
		public static NSString NSPreferredScrollerStyleDidChangeNotification {
			get { return PreferredStyleChangedNotification; }
		}

		[Obsolete ("Use 'DrawArrow' instead.")]
		public void DrawArrowhighlight (NSScrollerArrow whichArrow, bool highlight)
		{
			DrawArrow (whichArrow, highlight);
		}
		
	}

	public partial class NSSplitView {
		[Obsolete ("Use 'SetPositionOfDivider' instead.")]
		public void SetPositionofDivider (float position, int dividerIndex)
		{
			SetPositionOfDivider (position, dividerIndex);
		}
		
	}

	public partial class NSView {
		[Obsolete ("Use 'FrameChangedNotification' instead.")]
		public static NSString NSViewFrameDidChangeNotification {
			get { return FrameChangedNotification; }
		}

		[Obsolete ("Use 'FocusChangedNotification' instead.")]
		public static NSString NSViewFocusDidChangeNotification {
			get { return FocusChangedNotification; }
		}

		[Obsolete ("Use 'BoundsChangedNotification' instead.")]
		public static NSString NSViewBoundsDidChangeNotification {
			get { return BoundsChangedNotification; }
		}

		[Obsolete ("Use 'GlobalFrameChangedNotification' instead.")]
		public static NSString NSViewGlobalFrameDidChangeNotification {
			get { return GlobalFrameChangedNotification; }
		}

		[Obsolete ("Use 'UpdatedTrackingAreasNotification' instead.")]
		public static NSString NSViewDidUpdateTrackingAreasNotification {
			get { return UpdatedTrackingAreasNotification; }
		}

		[Obsolete ("Use 'DisplayRectIgnoringOpacity' instead.")]
		public void DisplayRectIgnoringOpacityinContext (CGRect aRect, NSGraphicsContext context)
		{
			DisplayRectIgnoringOpacity (aRect, context);
		}
		
		[Obsolete ("Use 'CacheDisplay' instead.")]
		public void CacheDisplayInRecttoBitmapImageRep (CGRect rect, NSBitmapImageRep bitmapImageRep)
		{
			CacheDisplay (rect, bitmapImageRep);
		}
		
		[Obsolete ("Use 'ScrollRect' instead.")]
		public void ScrollRectby (CGRect aRect, CGSize delta)
		{
			ScrollRect (aRect, delta);
		}
		
		[Obsolete ("Use 'TranslateRectsNeedingDisplay' instead.")]
		public void TranslateRectsNeedingDisplayInRectby (CGRect clipRect, CGSize delta)
		{
			TranslateRectsNeedingDisplay (clipRect, delta);
		}
		
		[Obsolete ("Use 'IsMouseInRect' instead.")]
		public bool MouseinRect (CGPoint aPoint, CGRect aRect)
		{
			return IsMouseInRect (aPoint, aRect);
		}
		
		[Obsolete ("Use 'SetContentHuggingPriorityForOrientation' instead.")]
		public void SetContentHuggingPriorityforOrientation (float priority, NSLayoutConstraintOrientation orientation)
		{
			SetContentHuggingPriorityForOrientation (priority, orientation);
		}
		
	}

	public partial class NSTableRowView {
		// void NSTableRowView::DrawBackgrounn(CGRect)
		[Obsolete ("Use 'DrawBackground' instead.")]
		public void DrawBackgrounn(CGRect dirtyRect)
		{
			DrawBackground (dirtyRect);
		}
		
	}

	public partial class NSTextAttachmentCell {
		// bool NSTextAttachmentCell::TrackMouseinRectofViewuntilMouseUp(NSEvent,CGRect,NSView,bool)
		[Obsolete ("Use 'TrackMouse' instead.")]
		public 	bool TrackMouseinRectofViewuntilMouseUp (NSEvent theEvent, CGRect cellFrame, NSView controlView, bool untilMouseUp)
		{
			return TrackMouse (theEvent, cellFrame, controlView, untilMouseUp);
		}
	}

	public partial class NSTextBlock {
		[Obsolete ("Use 'WidthValueTypeForLayer' instead.")]
		public NSTextBlockValueType WidthValueTypeForLayeredge (NSTextBlockLayer layer, NSRectEdge edge)
		{
			return WidthValueTypeForLayer (layer, edge);
		}
		
	}

	public partial class NSTextView {
		[Obsolete ("Use 'SetAlignmentRange' instead.")]
		public void SetAlignmentrange (NSTextAlignment alignment, NSRange range)
		{
			SetAlignmentRange (alignment, range);
		}
		
		[Obsolete ("Use 'SetBaseWritingDirection' instead.")]
		public void SetBaseWritingDirectionrange (NSWritingDirection writingDirection, NSRange range)
		{
			SetBaseWritingDirection (writingDirection, range);
		}
		
		[Obsolete ("Use 'RulerViewWillMoveMarker' instead.")]
		public float RulerViewWillMoveMarkertoLocation (NSRulerView ruler, NSRulerMarker marker, float location)
		{
			return RulerViewWillMoveMarker (ruler, marker, location);
		}
		
		// System.Single NSTextView::RulerViewWillAddMarkeratLocation(NSRulerView,NSRulerMarker,System.Single)
		[Obsolete ("Use 'RulerViewWillAddMarker' instead.")]
		public float RulerViewWillAddMarkeratLocation (NSRulerView ruler, NSRulerMarker marker, float location)
		{
			return RulerViewWillAddMarker (ruler, marker, location);
		}
		
		[Obsolete ("Use 'DrawInsertionPoint' instead.")]
		public void DrawInsertionPointInRectcolorturnedOn (CGRect rect, NSColor color, bool turnedOn)
		{
			DrawInsertionPoint (rect, color, turnedOn);
		}
		
		[Obsolete ("Use 'InsertCompletion' instead.")]
		public void InsertCompletionforPartialWord (string word, NSRange charRange, int movement, bool isFinal)
		{
			InsertCompletion (word, charRange, movement, isFinal);
		}
		
		[Obsolete ("Use 'WriteSelectionToPasteboard' instead.")]
		public bool WriteSelectionToPasteboardtype (NSPasteboard pboard, string type)
		{
			return WriteSelectionToPasteboard (pboard, type);
		}
		
		[Obsolete ("Use '__WriteSelectionToPasteboard' instead.")]
		public bool WriteSelectionToPasteboardtypes(NSPasteboard pboard, string [] types)
		{
			return WriteSelectionToPasteboardtypes (pboard, types);
		}
		
		[Obsolete ("Use 'GetPreferredPasteboardType' instead.")]
		public string PreferredPasteboardTypeFromArrayrestrictedToTypesFromArray (string [] availableTypes, string [] allowedTypes)
		{
			return GetPreferredPasteboardType (availableTypes, allowedTypes);
		}
		
		[Obsolete ("Use 'ReadSelectionFromPasteboard' instead.")]
		public bool ReadSelectionFromPasteboardtype (NSPasteboard pboard, string type)
		{
			return ReadSelectionFromPasteboard (pboard, type);
		}
		
		[Obsolete ("Use 'ValidRequestorForSendType' instead.")]
		public NSObject ValidRequestorForSendTypereturnType (string sendType, string returnType)
		{
			return ValidRequestorForSendType (sendType, returnType);
		}
		
		[Obsolete ("Use 'DragOperationForDraggingInfo' instead.")]
		public NSDragOperation DragOperationForDraggingInfotype (NSDraggingInfo dragInfo, string type)
		{
			return DragOperationForDraggingInfo (dragInfo, type);
		}
		
		[Obsolete ("Use 'SetSelectedRanges' instead.")]
		public void SetSelectedRangesaffinitystillSelecting (NSArray ranges, NSSelectionAffinity affinity, bool stillSelectingFlag)
		{
			SetSelectedRanges (ranges, affinity, stillSelectingFlag);
		}
		
		[Obsolete ("Use 'SetSelectedRange' instead.")]
		public void SetSelectedRangeaffinitystillSelecting  (NSRange charRange, NSSelectionAffinity affinity, bool stillSelectingFlag)
		{
			SetSelectedRange (charRange, affinity, stillSelectingFlag);
		}
		
		[Obsolete ("Use 'SetSpellingState' instead.")]
		public void SetSpellingStaterange (int value, NSRange charRange)
		{
			SetSpellingState (value, charRange);
		}
		
		// bool MonoMac.AppKit.NSTextView::ShouldChangeTextInRangesreplacementStrings(MonoMac.Foundation.NSArray,string[])
		[Obsolete ("Use 'ShouldChangeText' instead.")]
		public bool ShouldChangeTextInRangesreplacementStrings (NSArray /* NSRange [] */ affectedRanges, string [] replacementStrings)
		{
			return ShouldChangeText (affectedRanges, replacementStrings);
		}
		
		// bool MonoMac.AppKit.NSTextView::ShouldChangeTextInRangereplacementString(MonoMac.Foundation.NSRange,string)
		[Obsolete ("Use 'ShouldChangeTextInRangereplacementString' instead.")]
		public bool ShouldChangeTextInRangereplacementString (NSRange affectedCharRange, string replacementString)
		{
			return ShouldChangeText (affectedCharRange, replacementString);
		}
		
	}

	public partial class NSFontCollection {
		[Obsolete ("Use 'ChangedNotification' instead.")]
		public static NSString DidChangeNotification {
			get { return ChangedNotification; }
		}
	}
}

namespace SceneKit {
	public partial class SCNNode {
		[Obsolete ("Use 'SCNNode.AddChildNode' instead.")]
		public void AddChildNodechild (SCNNode child)
		{
			AddChildNode (child);
		}
	}
}

#endif
