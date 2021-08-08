namespace Xamarin.Mac.Tests

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

module PInvokes =
    [<DllImport ("/usr/lib/libSystem.dylib")>]
    extern void _exit (int exit_code)

type MainClass =
    static member ThreadMonitor(obj : System.Object) =
        let exit_code = obj :?> int
        Thread.Sleep (3000)
        Console.WriteLine ($"The process didn't exist within 3s of returning from Main. Assuming something is deadlocked, and will now exit immediately and forcefully (with exit code {exit_code}).")
        PInvokes._exit (exit_code)

    static member asyncMainTask (args: string[]) : Async<int> =
        async {
            // Skip arguments added by VSfM/macOS when running from the IDE
            let mutable arguments = new List<string> (args)
            arguments.RemoveAll (fun (arg) -> arg.StartsWith ("-psn_", StringComparison.Ordinal)) |> ignore

            let! exit_code = MonoTouch.NUnit.UI.MacRunner.MainAsync (arguments, true, PInvokes._exit, typedefof<MainClass>.Assembly) |> Async.AwaitTask

            let del = ParameterizedThreadStart(MainClass.ThreadMonitor)
            let exit_monitor = new Thread (del)
            exit_monitor.Name <- "Exit monitor"
            exit_monitor.IsBackground <- true
            exit_monitor.Start (exit_code)

            return exit_code
        }

module Program =
    [<EntryPoint>]
    let main args =
        async {
            return! MainClass.asyncMainTask (args)
        } |> Async.RunSynchronously
