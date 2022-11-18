namespace iOSApp1

open UIKit

module Main =
    // This is the main entry point of the application.
    // If you want to use a different Application Delegate class from "AppDelegate"
    // you can specify it here.
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, typeof<AppDelegate>)
        0
