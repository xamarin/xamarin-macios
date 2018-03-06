//
// Unit tests for UISearchDisplayController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	class SearchDisplayControllerPoker : UISearchDisplayController {
		
		static FieldInfo bkSearchBar;
		static FieldInfo bkSearchContentsController;
		static FieldInfo bkSearchResultsTableView;
		
		static SearchDisplayControllerPoker ()
		{
			var t = typeof (UISearchDisplayController);
			bkSearchBar = t.GetField ("__mt_SearchBar_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkSearchContentsController = t.GetField ("__mt_SearchContentsController_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkSearchResultsTableView = t.GetField ("__mt_SearchResultsTableView_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		
		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}
		
		public SearchDisplayControllerPoker ()
		{
		}
		
		public SearchDisplayControllerPoker (UISearchBar searchBar, UIViewController viewController) : base (searchBar, viewController)
		{
		}
		
		public UISearchBar SearchBarBackingField {
			get {
				return (UISearchBar) bkSearchBar.GetValue (this);
			}
		}

		public UIViewController SearchContentsControllerBackingField {
			get {
				return (UIViewController) bkSearchContentsController.GetValue (this);
			}
		}

		public UITableView SearchResultsTableViewBackingField {
			get {
				return (UITableView) bkSearchResultsTableView.GetValue (this);
			}
		}
	}
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SearchDisplayControllerTest {
		
		[Test]
		public void Ctor_Default_BackingFields ()
		{
			if (SearchDisplayControllerPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			using (var sc = new SearchDisplayControllerPoker ()) {
				// default constructor does not set any UIViewController so the backing fields are null
				Assert.Null (sc.SearchBarBackingField, "1a");
				Assert.Null (sc.SearchContentsControllerBackingField, "2a");
				Assert.Null (sc.SearchResultsTableViewBackingField, "3a");

				Assert.Null (sc.SearchBar, "1b");
				Assert.Null (sc.SearchContentsController, "2b");
				// not an issue (backing field being null before calling the getter) 
				// since it's not something we supplied to the instance
				Assert.NotNull (sc.SearchResultsTableView, "3b");
				// the backing field will be set afterward
				Assert.NotNull (sc.SearchResultsTableViewBackingField, "3c");
			}
		}

		[Test]
		public void Ctor_BackingFields ()
		{
			if (SearchDisplayControllerPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");
			
			using (var sb = new UISearchBar ())
			using (var vc = new UIViewController ())
			using (var sc = new SearchDisplayControllerPoker (sb, vc)) {
				// default constructor does not set any UIViewController so the backing fields are null
				Assert.AreSame (sb, sc.SearchBarBackingField, "1a");
				Assert.AreSame (vc, sc.SearchContentsControllerBackingField, "2a");
				Assert.Null (sc.SearchResultsTableViewBackingField, "3a");

				Assert.AreSame (sb, sc.SearchBar, "1b");
				Assert.AreSame (vc, sc.SearchContentsController, "2b");
				// not an issue (backing field being null before calling the getter) 
				// since it's not something we supplied to the instance
				Assert.NotNull (sc.SearchResultsTableView, "3b");
				// the backing field will be set afterward
				Assert.NotNull (sc.SearchResultsTableViewBackingField, "3c");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__

