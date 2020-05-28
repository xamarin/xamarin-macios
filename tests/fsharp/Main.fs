namespace Test

open UIKit

#if !__WATCHOS__ && !TODAY_EXTENSION

module Main = 
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, "AppDelegate")
        0

#endif
