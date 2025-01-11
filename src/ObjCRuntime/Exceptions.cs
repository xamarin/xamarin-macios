//
// Exceptions.cs:
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc.

using System;
using Foundation;

#nullable enable

namespace ObjCRuntime {

	/// <summary>The event delegate for the <see cref="Runtime.MarshalObjectiveCException" /> event.</summary>
	/// <param name="sender">Always null.</param>
	/// <param name="args">The exception data for the Objective-C exception.</param>
	/// <seealso href="https://learn.microsoft.com/dotnet/ios/platform/exception-marshaling">Exception marshaling</seealso>
	public delegate void MarshalObjectiveCExceptionHandler (object sender, MarshalObjectiveCExceptionEventArgs args);

	/// <summary>The event args for the <see cref="Runtime.MarshalObjectiveCException" /> event.</summary>
	/// <seealso href="https://learn.microsoft.com/dotnet/ios/platform/exception-marshaling">Exception marshaling</seealso>
	public class MarshalObjectiveCExceptionEventArgs {
#if !XAMCORE_5_0
		/// <summary>Creates a new <see cref="MarshalObjectiveCExceptionEventArgs" /> instance.</summary>
#pragma warning disable CS8618 // Non-nullable property 'Exception' must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring the property as nullable.
		public MarshalObjectiveCExceptionEventArgs () { }
#pragma warning restore CS8618
#endif

		/// <param name="exception">The <see cref="Exception" /> value for this instance.</param>
		/// <param name="mode">The <see cref="ExceptionMode" /> value for this instance.</param>
		public MarshalObjectiveCExceptionEventArgs (NSException exception, MarshalObjectiveCExceptionMode mode)
		{
			Exception = exception;
			ExceptionMode = mode;
		}

		/// <summary>The Objective-C exception that must be marshalled.</summary>
		/// <value>The Objective-C exception that must be marshalled.</value>
		public NSException Exception { get; set; }

		/// <summary>Specify how to marshal the Objective-C exception.</summary>
		/// <value>A value that specifies how to marshal the Objective-C exception.</value>
		public MarshalObjectiveCExceptionMode ExceptionMode { get; set; }
	}

	/// <summary>The event delegate for the <see cref="Runtime.MarshalManagedException" /> event.</summary>
	/// <param name="sender">Always null.</param>
	/// <param name="args">The exception data for the managed exception.</param>
	/// <seealso href="https://learn.microsoft.com/dotnet/ios/platform/exception-marshaling">Exception marshaling</seealso>
	public delegate void MarshalManagedExceptionHandler (object sender, MarshalManagedExceptionEventArgs args);

	/// <summary>The event args for the <see cref="Runtime.MarshalManagedException" /> event.</summary>
	/// <seealso href="https://learn.microsoft.com/dotnet/ios/platform/exception-marshaling">Exception marshaling</seealso>
	public class MarshalManagedExceptionEventArgs {
#if !XAMCORE_5_0
		/// <summary>Creates a new <see cref="MarshalManagedExceptionEventArgs" /> instance.</summary>
#pragma warning disable CS8618 // Non-nullable property 'Exception' must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring the property as nullable.
		public MarshalManagedExceptionEventArgs () { }
#pragma warning restore CS8618
#endif

		/// <summary>Creates a new <see cref="MarshalManagedExceptionEventArgs" /> instance.</summary>
		/// <param name="exception">The <see cref="Exception" /> value for this instance.</param>
		/// <param name="mode">The <see cref="ExceptionMode" /> value for this instance.</param>
		public MarshalManagedExceptionEventArgs (Exception exception, MarshalManagedExceptionMode mode)
		{
			Exception = exception;
			ExceptionMode = mode;
		}

		/// <summary>The managed exception that must be marshalled.</summary>
		/// <value>The managed exception that must be marshalled.</value>
		public Exception Exception { get; set; }

		/// <summary>Specify how to marshal the managed exception.</summary>
		/// <value>A value that specifies how to marshal the Objective-C exception.</value>
		public MarshalManagedExceptionMode ExceptionMode { get; set; }
	}
}
