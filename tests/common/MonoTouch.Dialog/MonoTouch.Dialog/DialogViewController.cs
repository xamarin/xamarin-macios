//
// DialogViewController.cs: drives MonoTouch.Dialog
//
// Author:
//   Miguel de Icaza
//
//
using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreGraphics;
using ObjCRuntime;
using NSAction = global::System.Action;

namespace MonoTouch.Dialog
{
	/// <summary>
	///   The DialogViewController is the main entry point to use MonoTouch.Dialog,
	///   it provides a simplified API to the UITableViewController.
	/// </summary>
	public class DialogViewController : UITableViewController
	{
		public UITableViewStyle Style = UITableViewStyle.Grouped;
		public event Action<NSIndexPath> OnSelection;
#if !__TVOS__
		UISearchBar searchBar;
#endif
		UITableView tableView;
		RootElement root;
		bool pushing;
		bool dirty;
		bool reloading;

		/// <summary>
		/// The root element displayed by the DialogViewController, the value can be changed during runtime to update the contents.
		/// </summary>
		public RootElement Root {
			get {
				return root;
			}
			set {
				if (root == value)
					return;
				if (root != null)
					root.Dispose ();
				
				root = value;
				root.TableView = tableView;		
				ReloadData ();
			}
		} 

		EventHandler refreshRequested;
		/// <summary>
		/// If you assign a handler to this event before the view is shown, the
		/// DialogViewController will have support for pull-to-refresh UI.
		/// </summary>
		public event EventHandler RefreshRequested {
			add {
				if (tableView != null)
					throw new ArgumentException ("You should set the handler before the controller is shown");
				refreshRequested += value; 
			}
			remove {
				refreshRequested -= value;
			}
		}
		
		// If the value is true, we are enabled, used in the source for quick computation
		bool enableSearch;
		public bool EnableSearch {
			get {
			   return enableSearch;
			}
			set {
				if (enableSearch == value)
					return;
				
				// After MonoTouch 3.0, we can allow for the search to be enabled/disable
				if (tableView != null)
					throw new ArgumentException ("You should set EnableSearch before the controller is shown");
				enableSearch = value;
			}
		}
		
		// If set, we automatically scroll the content to avoid showing the search bar until 
		// the user manually pulls it down.
		public bool AutoHideSearch { get; set; }
		
		public string SearchPlaceholder { get; set; }
			
		/// <summary>
		/// Invoke this method to trigger a data refresh.   
		/// </summary>
		/// <remarks>
		/// This will invoke the RerfeshRequested event handler, the code attached to it
		/// should start the background operation to fetch the data and when it completes
		/// it should call ReloadComplete to restore the control state.
		/// </remarks>
		public void TriggerRefresh ()
		{
			TriggerRefresh (false);
		}
		
		void TriggerRefresh (bool showStatus)
		{
			if (refreshRequested == null)
				return;

			if (reloading)
				return;
			
			reloading = true;
			refreshRequested (this, EventArgs.Empty);

		}
		
		/// <summary>
		/// Invoke this method to signal that a reload has completed, this will update the UI accordingly.
		/// </summary>
		public void ReloadComplete ()
		{
			if (!reloading)
				return;

			reloading = false;

#if !__TVOS__
			RefreshControl.EndRefreshing ();
#endif
		}
		
		/// <summary>
		/// Controls whether the DialogViewController should auto rotate
		/// </summary>
		public bool Autorotate { get; set; }

#if !__TVOS__
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return Autorotate || toInterfaceOrientation == UIInterfaceOrientation.Portrait;
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			
			//Fixes the RefreshView's size if it is shown during rotation
			ReloadData ();
		}
#endif
		
		Section [] originalSections;
		Element [][] originalElements;
		
		/// <summary>
		/// Allows caller to programatically activate the search bar and start the search process
		/// </summary>
		public void StartSearch ()
		{
			if (originalSections != null)
				return;
			
#if !__TVOS__
			searchBar.BecomeFirstResponder ();
#endif
			originalSections = Root.Sections.ToArray ();
			originalElements = new Element [originalSections.Length][];
			for (int i = 0; i < originalSections.Length; i++)
				originalElements [i] = originalSections [i].Elements.ToArray ();
		}
		
		/// <summary>
		/// Allows the caller to programatically stop searching.
		/// </summary>
		public virtual void FinishSearch ()
		{
			if (originalSections == null)
				return;
			
			Root.Sections = new List<Section> (originalSections);
			originalSections = null;
			originalElements = null;
#if !__TVOS__
			searchBar.ResignFirstResponder ();
#endif
			ReloadData ();
		}
		
		public delegate void SearchTextEventHandler (object sender, SearchChangedEventArgs args);
		public event SearchTextEventHandler SearchTextChanged;
		
		public virtual void OnSearchTextChanged (string text)
		{
			if (SearchTextChanged != null)
				SearchTextChanged (this, new SearchChangedEventArgs (text));
		}
		                                     
		public void PerformFilter (string text)
		{
			if (originalSections == null)
				return;
			
			OnSearchTextChanged (text);
			
			var newSections = new List<Section> ();
			
			for (int sidx = 0; sidx < originalSections.Length; sidx++){
				Section newSection = null;
				var section = originalSections [sidx];
				Element [] elements = originalElements [sidx];
				
				for (int eidx = 0; eidx < elements.Length; eidx++){
					if (elements [eidx].Matches (text)){
						if (newSection == null){
							newSection = new Section (section.Header, section.Footer){
								FooterView = section.FooterView,
								HeaderView = section.HeaderView
							};
							newSections.Add (newSection);
						}
						newSection.Add (elements [eidx]);
					}
				}
			}
			
			Root.Sections = newSections;
			
			ReloadData ();
		}
		
		public virtual void SearchButtonClicked (string text)
		{
		}
			
		class SearchDelegate : UISearchBarDelegate {
			DialogViewController container;
			
			public SearchDelegate (DialogViewController container)
			{
				this.container = container;
			}
			
			public override void OnEditingStarted (UISearchBar searchBar)
			{
#if !__TVOS__
				searchBar.ShowsCancelButton = true;
#endif
				container.StartSearch ();
			}
			
			public override void OnEditingStopped (UISearchBar searchBar)
			{
#if !__TVOS__
				searchBar.ShowsCancelButton = false;
#endif
				container.FinishSearch ();
			}
			
			public override void TextChanged (UISearchBar searchBar, string searchText)
			{
				container.PerformFilter (searchText ?? "");
			}
			
#if !__TVOS__
			public override void CancelButtonClicked (UISearchBar searchBar)
			{
				searchBar.ShowsCancelButton = false;
				container.searchBar.Text = "";
				container.FinishSearch ();
				searchBar.ResignFirstResponder ();
			}
#endif
			
			public override void SearchButtonClicked (UISearchBar searchBar)
			{
				container.SearchButtonClicked (searchBar.Text);
			}
		}
		
		public class Source : UITableViewSource {
			const float yboundary = 65;
			WeakReference<DialogViewController> container;
			protected DialogViewController Container => container.TryGetTarget (out var result) ? result : null;
			protected RootElement Root;
			
			public Source (DialogViewController container)
			{
				this.container = new WeakReference<DialogViewController> (container);
				Root = container.root;
			}
			
			public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
			{
				var section = Root.Sections [(int) indexPath.Section];
				var element = (section.Elements [(int) indexPath.Row] as StyledStringElement);
				if (element != null)
					element.AccessoryTap ();
			}
			
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				var s = Root.Sections [(int) section];
				var count = s.Elements.Count;
				
				return count;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return Root.Sections.Count;
			}

			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return Root.Sections [(int) section].Caption;
			}

			public override string TitleForFooter (UITableView tableView, nint section)
			{
				return Root.Sections [(int) section].Footer;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var section = Root.Sections [(int) indexPath.Section];
				var element = section.Elements [(int) indexPath.Row];
				
				return element.GetCell (tableView);
			}
			
			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				if (Root.NeedColorUpdate){
					var section = Root.Sections [(int) indexPath.Section];
					var element = section.Elements [(int) indexPath.Row];
					var colorized = element as IColorizeBackground;
					if (colorized != null)
						colorized.WillDisplay (tableView, cell, indexPath);
				}
			}
			
			public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
			{
				Container.Deselected (indexPath);
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				var onSelection = Container.OnSelection;
				if (onSelection != null)
					onSelection (indexPath);
				Container.Selected (indexPath);
			}			
			
			public override UIView GetViewForHeader (UITableView tableView, nint sectionIdx)
			{
				var section = Root.Sections [(int) sectionIdx];
				return section.HeaderView;
			}

			public override nfloat GetHeightForHeader (UITableView tableView, nint sectionIdx)
			{
				var section = Root.Sections [(int) sectionIdx];
				if (section.HeaderView == null)
					return -1;
				return section.HeaderView.Frame.Height;
			}

			public override UIView GetViewForFooter (UITableView tableView, nint sectionIdx)
			{
				var section = Root.Sections [(int) sectionIdx];
				return section.FooterView;
			}
			
			public override nfloat GetHeightForFooter (UITableView tableView, nint sectionIdx)
			{
				var section = Root.Sections [(int) sectionIdx];
				if (section.FooterView == null)
					return -1;
				return section.FooterView.Frame.Height;
			}			

			public override void Scrolled (UIScrollView scrollView) 
			{
				
			}

			public override void DraggingStarted (UIScrollView scrollView)
			{
				
			}

			public override void DraggingEnded (UIScrollView scrollView, bool willDecelerate)
			{
			}
		}

		//
		// Performance trick, if we expose GetHeightForRow, the UITableView will
		// probe *every* row for its size;   Avoid this by creating a separate
		// model that is used only when we have items that require resizing
		//
		public class SizingSource : Source {
			public SizingSource (DialogViewController controller) : base (controller) {}
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				var section = Root.Sections [(int) indexPath.Section];
				var element = section.Elements [(int) indexPath.Row];
				
				var sizable = element as IElementSizing;
				if (sizable == null)
					return (nfloat)tableView.RowHeight;
				return sizable.GetHeight (tableView, indexPath);
			}
		}
			
		/// <summary>
		/// Activates a nested view controller from the DialogViewController.   
		/// If the view controller is hosted in a UINavigationController it
		/// will push the result.   Otherwise it will show it as a modal
		/// dialog
		/// </summary>
		public void ActivateController (UIViewController controller)
		{
			dirty = true;
			
			var parent = ParentViewController;
			var nav = parent as UINavigationController;
			
			// We can not push a nav controller into a nav controller
			if (nav != null && !(controller is UINavigationController))
				nav.PushViewController (controller, true);
			else {
#if __TVOS__
				PresentViewController (controller, true, null);
#else
				PresentModalViewController (controller, true);
#endif
			}
		}

		/// <summary>
		/// Dismisses the view controller.   It either pops or dismisses
		/// based on the kind of container we are hosted in.
		/// </summary>
		public void DeactivateController (bool animated)
		{
			var parent = ParentViewController;
			var nav = parent as UINavigationController;
			if (nav != null)
				nav.PopViewController (animated);
			else {
#if __TVOS__
				DismissViewController (animated, null);
#else
				DismissModalViewController (animated);
#endif
			}
		}

		void SetupSearch ()
		{
#if __TVOS__
			// Can't create a UISearchBar in tvOS, you can only use one from a UISearchController,
			// which require bigger changes, so just skip this for now.
#else
			if (enableSearch){
				searchBar = new UISearchBar (new CGRect (0, 0, tableView.Bounds.Width, 44)) {
					Delegate = new SearchDelegate (this)
				};
				if (SearchPlaceholder != null)
					searchBar.Placeholder = this.SearchPlaceholder;
				tableView.TableHeaderView = searchBar;					
			} else {
				// Does not work with current Monotouch, will work with 3.0
				// tableView.TableHeaderView = null;
			}
#endif
		}
		
		public virtual void Deselected (NSIndexPath indexPath)
		{
			var section = root.Sections [(int) indexPath.Section];
			var element = section.Elements [(int) indexPath.Row];
			
			element.Deselected (this, tableView, indexPath);
		}
		
		public virtual void Selected (NSIndexPath indexPath)
		{
			var section = root.Sections [(int) indexPath.Section];
			var element = section.Elements [(int) indexPath.Row];

			element.Selected (this, tableView, indexPath);
		}
		
		public virtual UITableView MakeTableView (CGRect bounds, UITableViewStyle style)
		{
			return new UITableView (bounds, style);
		}
		
		public override void LoadView ()
		{
			tableView = MakeTableView (UIScreen.MainScreen.Bounds, Style);
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			tableView.AutosizesSubviews = true;
			
			if (root != null)
				root.Prepare ();
			
			UpdateSource ();
			View = tableView;
			SetupSearch ();
			ConfigureTableView ();
			
			if (root == null)
				return;
			root.TableView = tableView;
		}

		void ConfigureTableView ()
		{
#if !__TVOS__
			if (refreshRequested != null) {
				RefreshControl = new UIRefreshControl ();
				RefreshControl.AddTarget ((sender,args)=> TriggerRefresh (), UIControlEvent.ValueChanged);
				return;
			}
#endif
		}
		
		public event EventHandler ViewAppearing;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (AutoHideSearch){
				if (enableSearch){
					if (TableView.ContentOffset.Y < 44)
						TableView.ContentOffset = new CGPoint (0, 44);
				}
			}
			if (root == null)
				return;
			
			root.Prepare ();

#if !__TVOS__
			NavigationItem.HidesBackButton = !pushing;
#endif
			if (root.Caption != null)
				NavigationItem.Title = root.Caption;
			if (dirty){
				tableView.ReloadData ();
				dirty = false;
			}

			if (ViewAppearing != null)
				ViewAppearing (this, EventArgs.Empty);
		}

		public bool Pushing {
			get {
				return pushing;
			}
			set {
				pushing = value;
#if !__TVOS__
				if (NavigationItem != null)
					NavigationItem.HidesBackButton = !pushing;
#endif
			}
		}
		
		public virtual Source CreateSizingSource (bool unevenRows)
		{
			return unevenRows ? new SizingSource (this) : new Source (this);
		}
		
		Source TableSource;
		
		void UpdateSource ()
		{
			if (root == null)
				return;
			
			TableSource = CreateSizingSource (root.UnevenRows);
			tableView.Source = TableSource;
		}

		public void ReloadData ()
		{
			if (root == null)
				return;
			
			if(root.Caption != null) 
				NavigationItem.Title = root.Caption;
			
			root.Prepare ();
			if (tableView != null){
				UpdateSource ();
				tableView.ReloadData ();
			}
			dirty = false;
		}
		
		public event EventHandler ViewDisappearing;
		
		[Obsolete ("Use the ViewDisappearing event instead")]
		public event EventHandler ViewDissapearing {
			add {
				ViewDisappearing += value;
			}
			remove {
				ViewDisappearing -= value;
			}
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (ViewDisappearing != null)
				ViewDisappearing (this, EventArgs.Empty);
		}
		
		public DialogViewController (RootElement root) : base (UITableViewStyle.Grouped)
		{
			this.root = root;
		}
		
		public DialogViewController (UITableViewStyle style, RootElement root) : base (style)
		{
			Style = style;
			this.root = root;
		}
		
		/// <summary>
		///     Creates a new DialogViewController from a RootElement and sets the push status
		/// </summary>
		/// <param name="root">
		/// The <see cref="RootElement"/> containing the information to render.
		/// </param>
		/// <param name="pushing">
		/// A <see cref="System.Boolean"/> describing whether this is being pushed 
		/// (NavigationControllers) or not.   If pushing is true, then the back button 
		/// will be shown, allowing the user to go back to the previous controller
		/// </param>
		public DialogViewController (RootElement root, bool pushing) : base (UITableViewStyle.Grouped)
		{
			this.pushing = pushing;
			this.root = root;
		}

		public DialogViewController (UITableViewStyle style, RootElement root, bool pushing) : base (style)
		{
			Style = style;
			this.pushing = pushing;
			this.root = root;
		}
		public DialogViewController (IntPtr handle) : base(handle)
		{
			this.root = new RootElement ("");
		}
	}
}
