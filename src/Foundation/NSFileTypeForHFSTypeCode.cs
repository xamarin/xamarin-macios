//
// NSFileTypeForHFSTypeCode.cs
//
// Author:
//  Whitney Schmidt <whschm@microsoft.com>
//
// Copyright 2019 Microsoft Corporation. All right reserved.


using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation
{
    public static class NSFileTypeForHFSType {

        [DllImport (Constants.FoundationLibrary)]
        extern static IntPtr NSFileTypeForHFSTypeCode (uint /* OSType = int32_t */ hfsFileTypeCode);

        [DllImport (Constants.FoundationLibrary)]
        extern static int UTGetOSTypeFromString (IntPtr str);

        public static IntPtr GetNSFileType (uint fourCcTypeCode)
        {
		return NSFileTypeForHFSTypeCode (fourCcTypeCode);
        }
    }
}