//
// Exceptions.cs:
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc.

using System;
using Foundation;

namespace ObjCRuntime {
	public delegate void MarshalObjectiveCExceptionHandler (object sender, MarshalObjectiveCExceptionEventArgs args);

	public class MarshalObjectiveCExceptionEventArgs {
		public NSException Exception { get; set; }
		public MarshalObjectiveCExceptionMode ExceptionMode { get; set; }
	}

	public delegate void MarshalManagedExceptionHandler (object sender, MarshalManagedExceptionEventArgs args);

	public class MarshalManagedExceptionEventArgs {
		public Exception Exception { get; set; }
		public MarshalManagedExceptionMode ExceptionMode { get; set; }
	}
}
