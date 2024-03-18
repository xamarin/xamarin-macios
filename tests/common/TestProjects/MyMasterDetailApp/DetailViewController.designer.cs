//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using System.CodeDom.Compiler;

using Foundation;

namespace MyMasterDetailApp
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UILabel detailDescriptionLabel { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIToolbar toolbar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (detailDescriptionLabel is not null) {
				detailDescriptionLabel.Dispose ();
				detailDescriptionLabel = null;
			}
			
			if (toolbar is not null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}
}
