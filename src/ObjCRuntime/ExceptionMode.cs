//
// ExceptionMode.cs:
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc.

using System;

namespace ObjCRuntime {
	/* This enum must always match the identical enum in runtime/xamarin/main.h */
	public enum MarshalObjectiveCExceptionMode {
		Default = 0,
		UnwindManagedCode = 1, // not available for watchOS/COOP, default for the other platforms
		ThrowManagedException = 2, // default for watchOS/COOP
		Abort = 3,
		Disable = 4, // this will also prevent the corresponding event from working
	}

	/* This enum must always match the identical enum in runtime/xamarin/main.h */
	public enum MarshalManagedExceptionMode {
		Default = 0,
		UnwindNativeCode = 1, // not available for watchOS/COOP, default for the other platforms
		ThrowObjectiveCException = 2, // default for watchOS/COOP
		Abort = 3,
		Disable = 4, // this will also prevent the corresponding event from working
	}
}
