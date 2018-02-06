//
// UISearchController.cs: Simplify the UISearchController
//

#if !WATCH

using System;

namespace UIKit {

	public partial class UISearchController {
		class __Xamarin_UISearchResultsUpdating : UISearchResultsUpdating {
			Action<UISearchController> cback;
			
			public __Xamarin_UISearchResultsUpdating (Action<UISearchController> cback)
			{
				this.cback = cback;
				IsDirectBinding = false;
			}
			public override void UpdateSearchResultsForSearchController (UISearchController searchController)
			{
				cback (searchController);
			}
		}
		
		public void SetSearchResultsUpdater (Action<UISearchController> updateSearchResults)
		{
			if (updateSearchResults == null){
				WeakSearchResultsUpdater = null;
				return;
			}

			WeakSearchResultsUpdater = new __Xamarin_UISearchResultsUpdating (updateSearchResults);
		}
	}
}

#endif // !WATCH
