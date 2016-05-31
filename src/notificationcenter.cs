using System;
using System.ComponentModel;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

#if !MONOMAC
using XamCore.UIKit;
#else
using XamCore.AppKit;
#endif

namespace XamCore.NotificationCenter {
#if XAMCORE_2_0 || !MONOMAC
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // not meant to be user created
	interface NCWidgetController {

		[Static]
		[Export ("widgetController")]
		NCWidgetController GetWidgetController();

		[Export ("setHasContent:forWidgetWithBundleIdentifier:")]
		void SetHasContent (bool flag, string bundleID);
	}

	[iOS (8,0)][Mac (10,10)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NCWidgetProviding {

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler);

		[Export ("widgetMarginInsetsForProposedMarginInsets:"), DelegateName ("NCWidgetProvidingMarginInsets"), DefaultValueFromArgument ("defaultMarginInsets")]
#if !MONOMAC
		UIEdgeInsets GetWidgetMarginInsets (UIEdgeInsets defaultMarginInsets);
#else
		NSEdgeInsets GetWidgetMarginInsets (NSEdgeInsets defaultMarginInsets);
#endif

#if MONOMAC
		[Export ("widgetAllowsEditing")]
		bool WidgetAllowsEditing { get; set; }

		[Export ("widgetDidBeginEditing")]
		void WidgetDidBeginEditing ();

		[Export ("widgetDidEndEditing")]
		void WidgetDidEndEditing ();
#endif
	}

#if !MONOMAC
	[iOS (8,0)]
	[BaseType (typeof (UIVibrancyEffect))]
	[Category]
	interface UIVibrancyEffect_NotificationCenter {
#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		[Static, Export ("notificationCenterVibrancyEffect")]
		UIVibrancyEffect NotificationCenterVibrancyEffect ();
	}
#endif

#if MONOMAC
	[Mac (10,10)]
	[BaseType (typeof(NSViewController), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NCWidgetListViewDelegate)})]
	interface NCWidgetListViewController
	{
		[Wrap ("WeakDelegate")]
		NCWidgetListViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

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

	public interface INCWidgetListViewDelegate {}

	[Mac (10, 10)]
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

	[Mac (10,10)]
	[BaseType (typeof(NSViewController), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NCWidgetSearchViewDelegate)})]
	interface NCWidgetSearchViewController
	{
		[Wrap ("WeakDelegate")]
		NCWidgetSearchViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("searchResults", ArgumentSemantic.Copy)]
		NSObject[] SearchResults { get; set; }
	
		[Export ("searchDescription")]
		string SearchDescription { get; set; }

		[Export ("searchResultsPlaceholderString")]
		string SearchResultsPlaceholderString { get; set; }

		[Export ("searchResultKeyPath")]
		string SearchResultKeyPath { get; set; }
	}

	public interface INCWidgetSearchViewDelegate {}

	[Mac (10,10)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NCWidgetSearchViewDelegate
	{
		[Abstract]
		[Export ("widgetSearch:searchForTerm:maxResults:"), EventArgs ("NSWidgetSearchForTerm"), DefaultValue (false)]
		void SearchForTearm (NCWidgetSearchViewController controller, string searchTerm, nuint max);

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
