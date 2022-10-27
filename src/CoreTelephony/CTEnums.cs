using ObjCRuntime;
using System;

namespace CoreTelephony {

	// untyped enum -> CoreTelephonyDefines.h
	// in header file this is used inside a CTError structure where the domain is a SInt32
	public enum CTErrorDomain {
		NoError = 0,
		Posix = 1,
		Mach = 2,
	}

	[NoMacCatalyst]
	[iOS (9,0)]
	[Native]
	public enum CTCellularDataRestrictedState : ulong {
		Unknown,
		Restricted,
		NotRestricted,
	}

	[NoMacCatalyst]
	[iOS (12,0)]
	[Native]
	public enum CTCellularPlanProvisioningAddPlanResult : long {
		Unknown,
		Fail,
		Success,
	}
}
