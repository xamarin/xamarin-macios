//
// SystemConfigurationException.cs: SystemConfiguration error handling
//
// Authors:
//    Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;

namespace SystemConfiguration {

	public class SystemConfigurationException : Exception {
		public SystemConfigurationException (StatusCode statusErrorCode)
			: base (StatusCodeError.GetErrorDescription (statusErrorCode))
		{
			StatusErrorCode = statusErrorCode;
		}

		public StatusCode StatusErrorCode { get; private set; }

		internal static SystemConfigurationException FromMostRecentCall ()
		{
			var code = StatusCodeError.SCError ();
			return new SystemConfigurationException (code);
		}
	}
}
