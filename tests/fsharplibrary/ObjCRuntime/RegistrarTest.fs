namespace MonoTouchFixtures

open System
open Foundation

type Registrar_OutExportClass() = 
    inherit NSObject()

    abstract member Func: NSObject byref -> IntPtr

    [<Export ("func:")>]
    default this.Func ([<System.Runtime.InteropServices.OutAttribute>] value: NSObject byref) =
        let tmp = if Object.ReferenceEquals (value, null) then IntPtr.Zero else value.Handle
        let value = new NSObject ()
        tmp

type Registrar_OutExportDerivedClass() = 
    inherit Registrar_OutExportClass()

    override this.Func (value: NSObject byref) = 
        let tmp = if Object.ReferenceEquals (value, null) then IntPtr.Zero else value.Handle
        let value = new NSDate ()
        tmp

type ClassWithAnonymousFSharpClass() =
    let anonymous = {
        new NSObject () with
            member x.ToString() = "ToString"
   }
