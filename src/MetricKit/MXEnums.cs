//
// MetricKit enumerations
//
// Copyright 2022 Microsoft Inc.
//

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace MetricKit {
	[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	[ErrorDomain ("MXErrorDomain")]
	[Native]
	public enum MXErrorCode : long {
		InvalidId,
		MaxCount,
		PastDeadline,
		Duplicated,
		Unknown,
		InternalFailure,
	}
}
