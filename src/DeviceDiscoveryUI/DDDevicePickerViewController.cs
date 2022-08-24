//
// Custom methods for DDDevicePickerViewController
//
// Authors:
//	Israel Soto  <issoto@microsoft.com>
//
// Copyright 2022 Microsoft Corporation.
//

#nullable enable

using ObjCRuntime;
using Foundation;
using Network;
using System;

using OS_nw_endpoint = System.IntPtr;
using OS_nw_error = System.IntPtr;

namespace DeviceDiscoveryUI {

	public partial class DDDevicePickerViewController {
		public delegate void DevicePickerCompletionHandler (NWEndpoint? endpoint, NWError? error);

		public static bool IsSupported (NWBrowserDescriptor browseDescriptor, NWParameters? parameters)
		{
			if (browseDescriptor == null)
				throw new ArgumentNullException (nameof (browseDescriptor));

			return _IsSupported (browseDescriptor.Handle, parameters?.Handle ?? IntPtr.Zero);
		}

		public DDDevicePickerViewController (NWBrowserDescriptor browseDescriptor, NWParameters? parameters) : base (NSObjectFlag.Empty)
		{
			if (browseDescriptor == null)
				throw new ArgumentNullException (nameof (browseDescriptor));

			Handle = _InitWithBrowseDescriptorAndParameters (browseDescriptor.Handle, parameters?.Handle ?? IntPtr.Zero);
		}

		public void SetDevicePicker (DevicePickerCompletionHandler devicePickerCompletionHandler)
		{
			_SetDevicePicker (InternalDevicePickerCompletionHandler);

			void InternalDevicePickerCompletionHandler (OS_nw_endpoint endpoint, OS_nw_error error)
			{
				NWEndpoint? ep = endpoint != IntPtr.Zero ? new NWEndpoint (endpoint, false) : null;
				NWError? er = error != IntPtr.Zero ? new NWError (error, false) : null;

				devicePickerCompletionHandler (ep, er);
			}
		}
	}
}
