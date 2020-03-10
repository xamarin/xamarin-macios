using Foundation;
using System.CodeDom.Compiler;

namespace MySpriteKitGame {
	[Register ("AppDelegate")]
	partial class AppDelegate {
		[Outlet]
		SpriteKit.SKView MyGameView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (MyGameView != null) {
				MyGameView.Dispose ();
				MyGameView = null;
			}
		}
	}
}
