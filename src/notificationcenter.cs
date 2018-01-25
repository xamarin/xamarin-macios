using System;
using System.ComponentModel;
using CoreGraphics;
using ObjCRuntime;
using Foundation;

#if !MONOMAC
using UIKit;
#else
using AppKit;
#endif

namespace NotificationCenter {
#if XAMCORE_2_0 || !MONOMAC
	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // not meant to be user created
	interface NCWidgetController {

		[Static]
		[Export ("widgetController")]
		NCWidgetController GetWidgetController();

		[Export ("setHasContent:forWidgetWithBundleIdentifier:")]
		void SetHasContent (bool flag, string bundleID);
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NCWidgetProviding {

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler);

		[Export ("widgetMarginInsetsForProposedMarginInsets:"), DelegateName ("NCWidgetProvidingMarginInsets"), DefaultValueFromArgument ("defaultMarginInsets")]
#if !MONOMAC
		[Deprecated (PlatformName.iOS, 10,0)]
		UIEdgeInsets GetWidgetMarginInsets (UIEdgeInsets defaultMarginInsets);
#else
		NSEdgeInsets GetWidgetMarginInsets (NSEdgeInsets defaultMarginInsets);
#endif

#if MONOMAC
		[Export ("widgetAllowsEditing")]
		bool WidgetAllowsEditing {
			get;
#if !XAMCORE_4_0
			[NotImplemented]
			set;
#endif
		}

		[Export ("widgetDidBeginEditing")]
		void WidgetDidBeginEditing ();

		[Export ("widgetDidEndEditing")]
		void WidgetDidEndEditing ();
#else
		[iOS (10,0)]
		[Export ("widgetActiveDisplayModeDidChange:withMaximumSize:")]
		void WidgetActiveDisplayModeDidChange (NCWidgetDisplayMode activeDisplayMode, CGSize maxSize);
#endif
	}

#if !MONOMAC
	[iOS (8,0)]
	[BaseType (typeof (UIVibrancyEffect))]
	[Category (allowStaticMembers: true)] // Classic isn't internal so we need this
	interface UIVibrancyEffect_NotificationCenter {
#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		[Deprecated (PlatformName.iOS, 10,0)]
		[Static, Export ("notificationCenterVibrancyEffect")]
		UIVibrancyEffect NotificationCenterVibrancyEffect ();
	}

	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_NCWidgetAdditions {
		[iOS (10,0)]
		[Export ("widgetLargestAvailableDisplayMode")]
		NCWidgetDisplayMode GetWidgetLargestAvailableDisplayMode ();

		[iOS (10,0)]
		[Export ("setWidgetLargestAvailableDisplayMode:")]
		void SetWidgetLargestAvailableDisplayMode (NCWidgetDisplayMode mode);

		[iOS (10,0)]
		[Export ("widgetActiveDisplayMode")]
		NCWidgetDisplayMode GetWidgetActiveDisplayMode ();

		[iOS (10,0)]
		[Export ("widgetMaximumSizeForDisplayMode:")]
		CGSize GetWidgetMaximumSize (NCWidgetDisplayMode displayMode);
	}

	[Category]
	[Internal] // only static methods, which are not _nice_ to use as extension methods
	[BaseType (typeof (UIVibrancyEffect))]
	interface UIVibrancyEffect_NCWidgetAdditions {
		[iOS (10,0)]
		[Static]
		[Export ("widgetPrimaryVibrancyEffect")]
		UIVibrancyEffect GetWidgetPrimaryVibrancyEffect ();

		[iOS (10,0)]
		[Static]
		[Export ("widgetSecondaryVibrancyEffect")]
		UIVibrancyEffect GetWidgetSecondaryVibrancyEffect ();
	}
#endif

#if MONOMAC
	[Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof(NSViewController), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NCWidgetListViewDelegate)})]
	interface NCWidgetListViewController
	{
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INCWidgetListViewDelegate Delegate { get; set; }

		[Export ("contents", ArgumentSemantic.Copy)]
		NSViewController[] Contents { get; set; }

		[Export ("minimumVisibleRowCount", ArgumentSemantic.Assign)]
		nuint MinimumVisibleRowCount { get; set; }

		[Export ("hasDividerLines")]
		bool HasDividerLines { get; set; }

		[Export ("editing")]
		bool Editing { get; set; }

		[Export ("showsAddButtonWhenEditing")]
		bool ShowsAddButtonWhenEditing { get; set; }

		[Export ("viewControllerAtRow:makeIfNecessary:")]
		NSViewController GetViewController (nuint row, bool makeIfNecesary);

		[Export ("rowForViewController:")]
		nuint GetRow (NSViewController viewController);
	}

	interface INCWidgetListViewDelegate {}

	[Mac (10, 10, onlyOn64 : true)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NCWidgetListViewDelegate
	{
		[Abstract]
		[Export ("widgetList:viewControllerForRow:"), DelegateName ("NCWidgetListViewGetController"), DefaultValue (null)]
		NSViewController GetViewControllerForRow (NCWidgetListViewController list, nuint row);

		[Export ("widgetListPerformAddAction:"), DelegateName ("NCWidgetListViewController")]
		void PerformAddAction (NCWidgetListViewController list);

		[Export ("widgetList:shouldReorderRow:"), DelegateName ("NCWidgetListViewControllerShouldReorderRow"), DefaultValue (false)]
		bool ShouldReorderRow (NCWidgetListViewController list, nuint row);

		[Export ("widgetList:didReorderRow:toRow:"), EventArgs ("NCWidgetListViewControllerDidReorder"), DefaultValue (false)]
		void DidReorderRow (NCWidgetListViewController list, nuint row, nuint newIndex);

		[Export ("widgetList:shouldRemoveRow:"), DelegateName ("NCWidgetListViewControllerShouldRemoveRow"), DefaultValue (false)]
		bool ShouldRemoveRow (NCWidgetListViewController list, nuint row);

		[Export ("widgetList:didRemoveRow:"), EventArgs ("NCWidgetListViewControllerDidRemoveRow"), DefaultValue (false)]
		void DidRemoveRow (NCWidgetListViewController list, nuint row);
	}

	[Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof(NSViewController), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NCWidgetSearchViewDelegate)})]
	interface NCWidgetSearchViewController
	{
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INCWidgetSearchViewDelegate Delegate { get; set; }

		[Export ("searchResults", ArgumentSemantic.Copy)]
		NSObject[] SearchResults { get; set; }
	
		[Export ("searchDescription")]
		string SearchDescription { get; set; }

		[Export ("searchResultsPlaceholderString")]
		string SearchResultsPlaceholderString { get; set; }

		[Export ("searchResultKeyPath")]
		string SearchResultKeyPath { get; set; }
	}

	interface INCWidgetSearchViewDelegate {}

	[Mac (10,10, onlyOn64 : true)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NCWidgetSearchViewDelegate
	{
#if !XAMCORE_4_0
		[Abstract]
		[Export ("widgetSearch:searchForTerm:maxResults:"), EventArgs ("NSWidgetSearchForTerm"), DefaultValue (false)]
		void SearchForTearm (NCWidgetSearchViewController controller, string searchTerm, nuint max);
#else
		[Abstract]
		[Export ("widgetSearch:searchForTerm:maxResults:"), EventArgs ("NSWidgetSearchForTerm"), DefaultValue (false)]
		void SearchForTerm (NCWidgetSearchViewController controller, string searchTerm, nuint max);
#endif

		[Abstract]
		[Export ("widgetSearchTermCleared:"), EventArgs ("NSWidgetSearchViewController"), DefaultValue (false)]
		void TermCleared (NCWidgetSearchViewController controller);

		[Abstract]
		[Export ("widgetSearch:resultSelected:"), EventArgs ("NSWidgetSearchResultSelected"), DefaultValue (false)]
		void ResultSelected (NCWidgetSearchViewController controller, NSObject obj);
	}
#endif
#endif
}
