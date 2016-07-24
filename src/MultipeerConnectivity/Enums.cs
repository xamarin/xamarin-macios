//
// Enums.cs: enums for MultipeerConnectivity
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, 2016 Xamarin, Inc.

#if XAMCORE_2_0 || !MONOMAC // MultipeerConnectivity is 64-bit only on OS X
using XamCore.ObjCRuntime;

namespace XamCore.MultipeerConnectivity {

	// NSInteger -> MCSession.h
	[TV (10,0)]
	[iOS (7,0)]
	[Native]
	public enum MCSessionSendDataMode : nint {
		Reliable,
		Unreliable
	}

	// NSInteger -> MCSession.h
	[TV (10,0)]
	[iOS (7,0)]
	[Native]
	public enum MCSessionState : nint {
		NotConnected,
		Connecting,
		Connected
	}

	// NSInteger -> MCSession.h
	[TV (10,0)]
	[iOS (7,0)]
	[Native]
	public enum MCEncryptionPreference : nint {
		Optional = 0,
		Required = 1,
		None = 2
	}

	// NSInteger -> MCError.h
	[TV (10,0)]
	[iOS (7,0)]
	[Native]
	[ErrorDomain ("MCErrorDomain")]
	public enum MCError : nint {
		Unknown ,
		NotConnected,
		InvalidParameter,
		Unsupported,
		TimedOut,
		Cancelled,
		Unavailable
	}
}
#endif
