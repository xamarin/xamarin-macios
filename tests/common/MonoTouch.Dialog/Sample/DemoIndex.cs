//
// This sample shows how to present an index.
//
// This requires the user to create two subclasses for the
// internal model used in DialogViewController and a new
// subclass of DialogViewController that activates it.
//
// See the source in IndexedViewController
//
// The reason for Source and SourceSizing derived classes is
// that MonoTouch.Dialog will create one or the other based on
// whether there are elements with uniform sizes or not.  This
// imrpoves performance by avoiding expensive computations.
//
using System;
using System.Drawing;
using System.Linq;
using UIKit;
using MonoTouch.Dialog;
using System.Collections.Generic;

namespace Sample
{
	class IndexedViewController : DialogViewController {
		public IndexedViewController (RootElement root, bool pushing) : base (root, pushing)
		{
			// Indexed tables require this style.
			Style = UITableViewStyle.Plain;
			EnableSearch = true;
			SearchPlaceholder = "Find item";
			AutoHideSearch = true;
		}
		
		string [] GetSectionTitles ()
		{
			return (
				from section in Root
				where !String.IsNullOrEmpty(section.Caption)
				select section.Caption.Substring(0,1)
			).ToArray ();
		}
		
		class IndexedSource : Source {
        	IndexedViewController parent;

	        public IndexedSource (IndexedViewController parent) : base (parent)
	        {
	            this.parent = parent;
	        }
	
#if !__TVOS__
	        public override string[] SectionIndexTitles (UITableView tableView)
	        {
				var j = parent.GetSectionTitles ();
				return j;
	        }
#endif
	    }

		class SizingIndexedSource : Source {
        	IndexedViewController parent;

	        public SizingIndexedSource (IndexedViewController parent) : base (parent)
	        {
	            this.parent = parent;
	        }
	
#if !__TVOS__
	        public override string[] SectionIndexTitles (UITableView tableView)
	        {
				var j = parent.GetSectionTitles ();
				return j;
	        }
#endif // !__TVOS__
	    }

		public override Source CreateSizingSource (bool unevenRows)
	    {
			if (unevenRows)
				return new SizingIndexedSource (this);
			else
	        	return new IndexedSource (this);
	    }
	}
	
	public partial class AppDelegate
	{
		public void DemoIndex () 
		{
			var root = new RootElement ("Container Style") {
				from sh in "ABCDEFGHIJKLMNOPQRSTUVWXYZ" 
				    select new Section (sh + " - Section") {
					   from filler in "12345" 
						select (Element) new StringElement (sh + " - " + filler)
				}
			};
			var dvc = new IndexedViewController (root, true);
			navigation.PushViewController (dvc, true);
		}

	}
}

