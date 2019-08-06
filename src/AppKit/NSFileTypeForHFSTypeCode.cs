//
// NSFileTypeForHFSTypeCode.cs
//
// Author:
//  Whitney Schmidt <whschm@microsoft.com>
//
// Copyright 2019 Microsoft Corporation. All right reserved.


using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace AppKit
{

    public static class NSFileTypeForHFSType {

        [DllImport ("/System/Library/Frameworks/Foundation.framework/Foundation")]
        extern static IntPtr NSFileTypeForHFSTypeCode (int /* OSType = int32_t */ hfsFileTypeCode);

        [DllImport ("/System/Library/Frameworks/Foundation.framework/Foundation")]
        extern static int UTGetOSTypeFromString (IntPtr str);

        public static string GetHFSTypeCodeString (string type)
        {
            int code;
            using (NSString ns = (NSString)type)
                code = UTGetOSTypeFromString (ns.Handle);

            using (var s = Runtime.GetNSObject<NSString> (NSFileTypeForHFSTypeCode (code))) {
                return (string)s;
            }
        }
    }
}