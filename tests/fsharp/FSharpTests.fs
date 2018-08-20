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

    [<Test>]
    member x.SprintfTest () =
        // Bug: https://bugzilla.xamarin.com/show_bug.cgi?id=52866
        let a = 1111
        let b = 2222
        let c = 3333
        let d = 4444
        let e = 5555

        let pr = sprintf "%d %d %d %d %d" a b c d e
        Assert.AreEqual ("1111 2222 3333 4444 5555", pr)
