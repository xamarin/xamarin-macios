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
		/// <summary>
		/// The type of the delegate that will be called when the user selects a device in the list of devices.
		/// </summary>
		public delegate void DevicePickerCompletionHandler (NWEndpoint? endpoint, NWError? error);

		/// <summary>
		/// Checks if device discovery is supported for the current device
		/// </summary>
		public static bool IsSupported (NWBrowserDescriptor browseDescriptor, NWParameters? parameters) =>
			_IsSupported (browseDescriptor.GetNonNullHandle (nameof (browseDescriptor)), parameters.GetHandle ());

		/// <summary>
		/// A view controller that lists other devices on your network.
		/// </summary>
		/// <remarks>
		/// Verify if this controller is supported by calling DDDevicePickerViewController.IsSupported before creating an instance, as this will crash if not supported.
		/// </remarks>
		[DesignatedInitializer]
		public DDDevicePickerViewController (NWBrowserDescriptor browseDescriptor, NWParameters? parameters) : base (NSObjectFlag.Empty) =>
			InitializeHandle (
				_InitWithBrowseDescriptorAndParameters (browseDescriptor.GetNonNullHandle (nameof (browseDescriptor)), parameters.GetHandle ()),
				"initWithBrowseDescriptor:parameters:");

		/// <summary>
		/// Set the delegate that will be called when the user selects a device in list of devices.
		/// </summary>
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
