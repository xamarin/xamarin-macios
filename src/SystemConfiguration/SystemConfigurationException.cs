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

		/// <summary>The <see cref="T:SystemConfiguration.StatusCode" /> wrapped in this <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:SystemConfigurtion.SystemConfigurationException&amp;scope=Xamarin" title="T:SystemConfigurtion.SystemConfigurationException">T:SystemConfigurtion.SystemConfigurationException</a></format>.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public StatusCode StatusErrorCode { get; private set; }

		internal static SystemConfigurationException FromMostRecentCall ()
		{
			var code = StatusCodeError.SCError ();
			return new SystemConfigurationException (code);
		}
	}
}
