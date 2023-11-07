namespace fsharp

open UIKit

#if !__WATCHOS__ && !TODAY_EXTENSION

module Main = 
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, typedefof<AppDelegate>)
        0

#endif
