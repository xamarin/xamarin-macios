// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MySceneKitGame {
	partial class GameViewController {
		[Outlet]
		SceneKit.SCNView MyGameView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (MyGameView != null) {
				MyGameView.Dispose ();
				MyGameView = null;
			}
		}
	}
}
