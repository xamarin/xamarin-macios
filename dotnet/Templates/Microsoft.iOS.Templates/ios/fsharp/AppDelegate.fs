namespace iOSApp1

open UIKit
open Foundation

[<Register(nameof AppDelegate)>]
type AppDelegate() =
    inherit UIApplicationDelegate()
       
    override val Window = null with get, set

    override this.FinishedLaunching(application: UIApplication, launchOptions: NSDictionary) =
        // create a new window instance based on the screen size
        this.Window <- new UIWindow(UIScreen.MainScreen.Bounds)

        // create a UIViewController with a single UILabel
        let vc = new UIViewController()
        vc.View.AddSubview(
            new UILabel(
                this.Window.Frame,
                BackgroundColor = UIColor.SystemBackground,
                TextAlignment = UITextAlignment.Center,
                Text = "Hello, iOS!",
                AutoresizingMask = UIViewAutoresizing.All
            )
        )
        this.Window.RootViewController <- vc

        // make the window visible
        this.Window.MakeKeyAndVisible()
        
        true
