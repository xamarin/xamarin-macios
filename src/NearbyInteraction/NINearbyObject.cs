//
// NearbyInteraction manual bindings
//
// Authors:
//	Whitney Schmidt  <whschm@microsoft.com>
//
// Copyright 2020 Microsoft Inc.
//

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
using Vector3 = global::OpenTK.Vector3;

#if !MONOMAC
namespace NearbyInteraction {
    partial class NINearbyObject
    {
        static Vector3? _NINearbyObjectDirectionNotAvailable;

        // TODO: https://github.com/xamarin/maccore/issues/2274
        // We do not have generator support to trampoline Vector3 -> vector_float3 for Fields
        [Field ("NINearbyObjectDirectionNotAvailable",  "NearbyInteraction")]
        public static Vector3 NINearbyObjectDirectionNotAvailable {
            get {
                if (_NINearbyObjectDirectionNotAvailable == null) {
                    unsafe {
                        Vector3 *pointer = (Vector3 *) Dlfcn.GetIndirect (Libraries.NearbyInteraction.Handle, "NINearbyObjectDirectionNotAvailable");
                        _NINearbyObjectDirectionNotAvailable = *pointer;
                    }
                }
                return (OpenTK.Vector3)_NINearbyObjectDirectionNotAvailable;
            }
        }
    }

}
#endif //!MONOMAC
