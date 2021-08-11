namespace monotouchtestWatchKitExtension

open System
open System.Collections.Generic
open System.Threading

open WatchKit
open Foundation

open MonoTouch.NUnit.UI

[<Register ("InterfaceController")>]
type InterfaceController (handle: IntPtr) =
    inherit WKInterfaceController (handle)

    let mutable runner = Unchecked.defaultof<WatchOSRunner>
    let mutable running = false

    [<Outlet ("lblStatus")>]
    member val lblStatus = Unchecked.defaultof<WKInterfaceLabel> with get, set

    [<Outlet ("lblSuccess")>]
    member val lblSuccess = Unchecked.defaultof<WKInterfaceLabel> with get, set

    [<Outlet ("lblFailed")>]
    member val lblFailed = Unchecked.defaultof<WKInterfaceLabel> with get, set

    [<Outlet ("lblIgnInc")>]
    member val lblIgnInc = Unchecked.defaultof<WKInterfaceLabel> with get, set

    [<Outlet ("cmdRun")>]
    member val cmdRun = Unchecked.defaultof<WKInterfaceButton> with get, set

    member this.LoadTests () =
        let excludeCategories =
            [|
                "MobileNotWorking"
                "NotOnMac"
                "NotWorking"
                "ValueAdd"
                "CAS"
                "InetAccess"
                "NotWorkingLinqInterpreter"
                "RequiresBSDSockets"
                "BitcodeNotSupported"
            |]
        runner <- new WatchOSRunner ()
        runner.ExcludedCategories <- new HashSet<string> (excludeCategories :> IEnumerable<string>)
        let tp = this.GetType ()
        runner.Add (tp.Assembly)
        ThreadPool.QueueUserWorkItem (fun v ->
            runner.LoadSync ()
            this.BeginInvokeOnMainThread (fun x ->
                this.lblStatus.SetText (String.Format ("{0} tests", runner.TestCount))
                this.RenderResults ()
                this.cmdRun.SetEnabled (true)
                this.cmdRun.SetHidden (false)

                runner.AutoRun ()
            )
        )
        |> ignore

    override this.Awake (context: NSObject) =
        base.Awake (context)
        this.BeginInvokeOnMainThread (fun x ->
            this.LoadTests ()
            ()
        )

    member this.RenderResults () =
        if runner.TestCount > 0 then
            this.lblSuccess.SetText (String.Format ("P: {0}/{1} {2}%", runner.PassedCount, runner.TestCount, 100 * runner.PassedCount / runner.TestCount))
            this.lblFailed.SetText (String.Format ("F: {0}/{1} {2}%", runner.FailedCount, runner.TestCount, 100 * runner.FailedCount / runner.TestCount))
            this.lblIgnInc.SetText (String.Format ("I: {0}/{1} {2}%", (runner.IgnoredCount + runner.InconclusiveCount), runner.TestCount, 100 * (runner.IgnoredCount + runner.InconclusiveCount) / runner.TestCount))

            if running && runner.PassedCount > 0 then
                if runner.FailedCount = 0 then
                    this.lblSuccess.SetTextColor (UIKit.UIColor.Green);
                    this.lblStatus.SetTextColor (UIKit.UIColor.Green);
                    this.lblStatus.SetText ("Success");
                
                if runner.FailedCount > 0 then
                    this.lblFailed.SetTextColor (UIKit.UIColor.Red);
                    this.lblStatus.SetTextColor (UIKit.UIColor.Red);
                    this.lblStatus.SetText ("Failed");

    member this.RunTests () = 
        if running then
            printf "Already running"
        else
            let running = true
            this.cmdRun.SetEnabled (false)
            this.lblStatus.SetText ("Running")
            this.BeginInvokeOnMainThread (fun v ->
                runner.Run ()
                this.lblStatus.SetText ("Done")
                let running = false
                this.RenderResults ()
            )

    [<Action ("runTests:")>]
    member this.RunTests (obj: NSObject) =
        this.RunTests ()

