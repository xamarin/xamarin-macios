//
// Copyright 2021 Microsoft Corp
//
// Authors:
//   Alex Soto (alexsoto@microsoft.com)
//   Rachel Kang (rachelkang@microsoft.com)
//

#if false // requires tests
using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using ObjCRuntime;

using CMSampleBufferRef = System.IntPtr;
using AVContentKey = System.IntPtr;
using NSErrorPtr = System.IntPtr;

#nullable enable

namespace AVFoundation {
    public static class AVSampleBufferExtensions {

        [iOS (14, 5), Mac (11, 3), TV (14, 5), Watch (7,4)]
        [DllImport (Constants.AVFoundationLibrary)]
        [return: MarshalAs (UnmanagedType.I1)]
        static extern /* BOOL */ bool AVSampleBufferAttachContentKey (
            /* CMSampleBufferRef */ CMSampleBufferRef sbuf,
            /* AVContentKey */ AVContentKey contentKey,
            /* NSError * _Nullable * _Nullable */ out NSErrorPtr outError);
        
        [iOS (14, 5), Mac (11, 3), TV (14, 5), Watch (7,4)]
        public static bool AttachContentKey (this CMSampleBuffer sampleBuffer, AVContentKey contentKey, out NSError error)
        {
            if (sampleBuffer is null)
                ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sampleBuffer));

            if (contentKey is null)
                ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (contentKey));
            
            IntPtr outerr;
            var retVal = AVSampleBufferAttachContentKey (sampleBuffer.Handle, contentKey.Handle, out outerr);
            error = Runtime.GetNSObject<NSError> (outerr);
            return retVal;
        }
    }
}
#endif
