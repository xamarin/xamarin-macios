namespace Test

#if __UNIFIED__
open UIKit
#else
open MonoTouch.UIKit
#endif

#if !__WATCHOS__

module Main = 
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, "AppDelegate")
        0

#endif
