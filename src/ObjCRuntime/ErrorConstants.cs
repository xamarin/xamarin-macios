using System;

namespace ObjCRuntime {

	// we use this to avoid multiple similar strings for the same purpose
	// which reduce the size of the metadata inside our platform assemblies
	// once adopted everywhere then updating  strings will be much easier
	partial class Constants {

		internal const string PleaseFileBugReport = "Please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new.";

		internal const string SetThrowOnInitFailureToFalse = "It is possible to ignore this condition by setting ObjCRuntime.Class.ThrowOnInitFailure to false";
	}
}
