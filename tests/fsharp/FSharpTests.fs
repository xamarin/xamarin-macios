namespace fsharp

open System
open System.Reflection

#if __UNIFIED__
open UIKit
open Foundation
#else
open MonoTouch.UIKit
open MonoTouch.Foundation
#endif

open NUnit.Framework

[<TestFixture>]
[<Preserve (AllMembers=true)>]
type FSharpTest () =
    [<Test>]
    member x.Func () =
        ()
