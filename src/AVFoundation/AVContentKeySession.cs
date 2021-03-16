//
// Copyright 2021 Microsoft Corp
//
// Authors:
//   Alex Soto (alexsoto@microsoft.com)
//   Rachel Kang (rachelkang@microsoft.com)
//

#if false
using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using ObjCRuntime;

namespace AVFoundation {
    public partial class AVContentKeySession {

        [iOS (14, 5), Mac (11, 3), TV (14, 5), Watch (7,4)]
        [DllImport (Constants.AVFoundationLibrary)]
        [return: MarshalAs (UnmanagedType.I1)]
        static extern /* BOOL */ bool AVSampleBufferAttachContentKey (
            /* CMSampleBufferRef */ IntPt sbuf,
            /* AVContentKey */ IntPt contentKey,
            /* NSError * _Nullable * _Nullable */ out IntPtr outError);
        
        [iOS (14, 5), Mac (11, 3), TV (14, 5), Watch (7,4)]
        public static bool AttachContentKey (CMSampleBuffer sampleBuffer, AVContentKey contentKey, out NSError error)
        {
            IntPtr outerr;
            var retVal = AVSampleBufferAttachContentKey (sampleBuffer.Handle, contentKey.Handle, out outerr);
            error = Runtime.GetNSObject<NSError> (outerr);
            return retVal;
        }
    }
}
#endif