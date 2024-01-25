using AppKit;

using CoreGraphics;

using Foundation;

using SceneKit;

namespace MyCocoaSceneKitApp {
	static class MainClass {
		static void Main (string [] args)
		{
			NSApplication.Init ();
			NSApplication.SharedApplication.Delegate = new AppDelegate ();
			NSApplication.Main (args);
		}
	}

	public partial class AppDelegate : NSApplicationDelegate {
		NSWindow window;
		public override void DidFinishLaunching (NSNotification notification)
		{
			window = new NSWindow (new CGRect (0, 0, 300, 300), NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable | NSWindowStyle.Titled, NSBackingStore.Retained, false);

			SCNScene scene = SCNScene.FromFile ("art.scnassets/scene.scn");
			SCNView scnView = new SCNView (window.ContentView.Bounds);
			scnView.Scene = scene;
			window.ContentView.AddSubview (scnView);

			var size = window.Frame.Size;
			var loc = new CGPoint ((NSScreen.MainScreen.Frame.Width - size.Width) / 2, (NSScreen.MainScreen.Frame.Height - size.Height) / 2);
			window.SetFrameOrigin (loc);
			window.MakeKeyAndOrderFront (null);
		}
	}
}
