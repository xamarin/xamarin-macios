//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MySingleView
{
	[Register ("MySingleViewViewController")]
	partial class MySingleViewViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton MyButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (MyButton != null) {
				MyButton.Dispose ();
				MyButton = null;
			}
		}
	}
}
