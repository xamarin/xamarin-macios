// warning FS3391: This expression uses the implicit conversion 'ObjCRuntime.NativeHandle.op_Implicit(value: ObjCRuntime.NativeHandle) : nativeint' to convert type 'ObjCRuntime.NativeHandle' to type 'nativeint'.
// Just disable for the whole file instead of for each expression, since this is just test code anyways.
#nowarn "3391"

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
