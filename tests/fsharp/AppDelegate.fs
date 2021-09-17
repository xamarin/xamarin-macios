namespace fsharp

#if !__WATCHOS__
open System
open System.Reflection

open UIKit
open Foundation

open MonoTouch.NUnit.UI

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    let mutable window = Unchecked.defaultof<UIWindow>
    let mutable runner = Unchecked.defaultof<TouchRunner>

    // This method is invoked when the application is ready to run.
    override this.FinishedLaunching (app, options) =
        window <- new UIWindow (UIScreen.MainScreen.Bounds)
        runner <- new TouchRunner (window)

        runner.Add (Assembly.GetExecutingAssembly ())

        window.RootViewController <- new UINavigationController (runner.GetViewController ())
        window.MakeKeyAndVisible ()

        true
#endif // !__WATCHOS__
