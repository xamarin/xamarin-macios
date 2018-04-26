namespace todayextensiontests

open System
open System.Threading

open NotificationCenter
open Foundation
open UIKit

open MonoTouch.NUnit.UI
open NUnit.Framework
open NUnit.Framework.Internal
open NUnit.Framework.Internal.Filters

[<Register ("TodayViewController")>]
type TodayViewController() =
    inherit UIViewController()

    let mutable runner = Unchecked.defaultof<ConsoleRunner>

    [<Export ("widgetPerformUpdateWithCompletionHandler:")>]
    member this.WidgetPerformUpdate (completionHandler: Action<NCUpdateResult>) =
        runner <- new ConsoleRunner ()
        let ce = new CategoryExpression ("MobileNotWorking,NotOnMac,NotWorking,ValueAdd,CAS,InetAccess,NotWorkingLinqInterpreter")
        runner.Filter <- new NotFilter (ce.Filter)
        let tp = this.GetType ()
        runner.Add (tp.Assembly)
        ThreadPool.QueueUserWorkItem (fun v ->
            runner.LoadSync ()
            this.BeginInvokeOnMainThread (fun x ->
                runner.AutoStart <- true
                runner.AutoRun ()
            )
        )
        |> ignore
        completionHandler.Invoke (NCUpdateResult.NewData)

    interface INCWidgetProviding
