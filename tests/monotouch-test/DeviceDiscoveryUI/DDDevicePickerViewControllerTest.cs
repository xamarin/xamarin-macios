//
// Unit tests for DDDevicePickerViewController
//
// Authors:
//	Israel Soto <issoto@microsoft.com>
//
// Copyright 2022 Microsoft Corporation.
//

#if __TVOS__

using System;
using DeviceDiscoveryUI;
using Foundation;
using ObjCRuntime;
using Network;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.DeviceDiscoveryUI {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DDDevicePickerViewControllerTest {
		
		const string serviceName = "MyAppService";

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (14, 0);

		[Test]
		public void IsSupportedTest ()
		{
			var browserDescriptor = NWBrowserDescriptor.CreateApplicationServiceName (serviceName);
			var parameters = NWParameters.CreateApplicationService ();
			var isSupported = DDDevicePickerViewController.IsSupported (browserDescriptor, parameters);
			
			// DDDevicePickerViewController seems to work only for devices
			if (TestRuntime.IsSimulator)
				Assert.IsFalse (isSupported, "IsSupported");
			else
				Assert.IsTrue (isSupported, "IsSupported");
		}

		[Test]
		public void InitWithBrowseDescriptorAndParametersTest ()
		{
			// DDDevicePickerViewController seems to work only for devices
			TestRuntime.AssertNotSimulator ();

			var browserDescriptor = NWBrowserDescriptor.CreateApplicationServiceName (serviceName);
			var parameters = NWParameters.CreateApplicationService ();
			var isSupported = DDDevicePickerViewController.IsSupported (browserDescriptor, parameters);
			
			// If this fails, please, double check that MyAppService is registered within the Info.plist
			// https://developer.apple.com/documentation/bundleresources/information_property_list/nsapplicationservices
			Assert.IsTrue (isSupported, $"The {serviceName} key might not be registered in the Info.plist.");

			Assert.DoesNotThrow (() => { var devicePicker = new DDDevicePickerViewController (browserDescriptor, parameters); },
				"InitWithBrowseDescriptorAndParameters");
		}

		[Test]
		public void SetDevicePickerTest ()
		{
			// DDDevicePickerViewController seems to work only for devices
			TestRuntime.AssertNotSimulator ();

			var browserDescriptor = NWBrowserDescriptor.CreateApplicationServiceName (serviceName);
			var parameters = NWParameters.CreateApplicationService ();
			var isSupported = DDDevicePickerViewController.IsSupported (browserDescriptor, parameters);
			
			// If this fails, please, double check that MyAppService is registered within the Info.plist
			// https://developer.apple.com/documentation/bundleresources/information_property_list/nsapplicationservices
			Assert.IsTrue (isSupported, $"The {serviceName} key might not be registered in the Info.plist.");
			
			Assert.DoesNotThrow (() => {
				var devicePicker = new DDDevicePickerViewController (browserDescriptor, parameters);
				devicePicker.SetDevicePicker ((endpoint, error) => { });
			},
			"SetDevicePicker");
		}
	}
}

#endif // __TVOS__
