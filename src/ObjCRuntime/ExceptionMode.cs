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

	/// <summary>This enum is used to specify what to do when an Objective-C exception is thrown, and that exception reaches managed code.</summary>
	/// <seealso href="https://learn.microsoft.com/dotnet/ios/platform/exception-marshaling">Exception marshaling</seealso>
	public enum MarshalObjectiveCExceptionMode {
		/// <summary>The default mode.</summary>
		/// <remarks>This is currently the same as <see cref="ThrowManagedException" />.</remarks>
		Default = 0,

		/// <summary>Let the Objective-C runtime unwind managed frames.</summary>
		/// <remarks>
		///		This option is not recommended, because it leads to undefined behavior (the app may crash, leak memory, deadlock, etc.)
		///		if Objective-C exceptions are thrown (the Objective-C runtime does not know how to unwind managed frames, so anything
		///     can happen). The recommended option is <see cref="ThrowManagedException" />.
		/// </remarks>
		UnwindManagedCode = 1,

		/// <summary>Convert the Objective-C exception to a managed exception.</summary>
		/// <remarks>This is the recommended option (and also the default behavior).</remarks>
		ThrowManagedException = 2,

		/// <summary>Abort when an Objective-C exception reaches managed code.</summary>
		/// <remarks>This may be useful during debugging to easily detect when Objective-C exceptions are thrown.</remarks>
		Abort = 3,

		/// <summary>Disable marshalling Objective-C exceptions.</summary>
		/// <remarks>This is effectively the same as <see cref="UnwindManagedCode" />, except that no events will be raised.</remarks>
		Disable = 4, // this will also prevent the corresponding event from working
	}

	/* This enum must always match the identical enum in runtime/xamarin/main.h */

	/// <summary>This enum is used to specify what to do when an managed exception is thrown, and that exception reaches native code.</summary>
	/// <seealso href="https://learn.microsoft.com/dotnet/ios/platform/exception-marshaling">Exception marshaling</seealso>
	public enum MarshalManagedExceptionMode {
		/// <summary>The default mode.</summary>
		/// <remarks>This is currently the same as <see cref="ThrowObjectiveCException" />.</remarks>
		Default = 0,

		/// <summary>Let the runtime unwind native frames.</summary>
		/// <remarks>
		///   <para>
		///       This option is only available when using the MonoVM runtime, not when using CoreCLR runtime. The CoreCLR runtime does
		///       not support unwinding native frames, and will just abort the process instead. The CoreCLR runtime is used in a macOS
		///       (not Mac Catalyst) app, or when using NativeAOT.
		///   </para>
		///   <para>
		///       This option is not recommended, because it leads to undefined behavior (the app may crash, leak memory, deadlock, etc.)
		///       if managed exceptions reaches native code (the MonoVM runtime does not know how to unwind native frames, so anything can
		///       happen). The recommended option is <see cref="ThrowObjectiveCException" />.
		///   </para>
		/// </remarks>
		UnwindNativeCode = 1,

		/// <summary>Convert the managed exception to an Objective-C exception.</summary>
		/// <remarks>This is the recommended option (and also the default behavior).</remarks>
		ThrowObjectiveCException = 2,

		/// <summary>Abort when a managed exception reaches native code.</summary>
		/// <remarks>This may be useful during debugging to easily detect when managed exceptions reaches native code.</remarks>
		Abort = 3,

		/// <summary>Disable marshalling managed exceptions.</summary>
		/// <remarks>This is effectively the same as <see cref="UnwindNativeCode" />, except that no events will be raised.</remarks>
		Disable = 4, // this will also prevent the corresponding event from working
	}
}
