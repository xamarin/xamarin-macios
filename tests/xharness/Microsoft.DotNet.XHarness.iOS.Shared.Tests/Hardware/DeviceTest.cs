using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using NUnit.Framework;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Hardware {
	[TestFixture]
	public class DeviceTest {

		class DevicesDataTestSource {
			public static IEnumerable DebugSpeedDevices {
				get {
					var data = new [] {
						(interfaceType: "usb", result: 0),
						(interfaceType: "USB", result: 0),
						(interfaceType: "wifi", result:  2),
						(interfaceType: "WIFI", result: 2),
						(interfaceType: (string) null, result:  1),
						(interfaceType: "HOLA", result: 3),
					};
					foreach (var (interfaceType, result) in data) {
						yield return new TestCaseData (new Device { InterfaceType = interfaceType }).Returns (result);
					}
				}
			}

			public static IEnumerable DevicePlatformDevices {
				get {
					var data = new [] {
						(deviceClass: DeviceClass.iPhone, result: DevicePlatform.iOS),
						(deviceClass: DeviceClass.iPod, result: DevicePlatform.iOS),
						(deviceClass: DeviceClass.iPad, result: DevicePlatform.iOS),
						(deviceClass: DeviceClass.AppleTV, result: DevicePlatform.tvOS),
						(deviceClass: DeviceClass.Watch, result: DevicePlatform.watchOS),
						(deviceClass: DeviceClass.Unknown, result: DevicePlatform.Unknown),
					};
					foreach (var (deviceClass, result) in data) {
						yield return new TestCaseData (new Device { DeviceClass = deviceClass }).Returns (result);

					}
				}
			}

			public static IEnumerable Supports64bDevices {
				get {
					var data = new Dictionary<string, (string version, bool result) []> () {
						["iPhone"] = new [] {
							(version: "1,1", result: false),
							(version: "1,2", result: false),
							(version: "2,1", result: false),
							(version: "3,1", result: false),
							(version: "3,2", result: false),
							(version: "3,3", result: false),
							(version: "4,1", result: false),
							(version: "5,1", result: false),
							(version: "5,2", result: false),
							(version: "5,3", result: false),
							(version: "6,1", result: true),
							(version: "6,2", result: true),
							(version: "7,1", result: true),
							(version: "7,2", result: true),
							(version: "8,4", result: true),
							(version: "9,1", result: true),
							(version: "9,2", result: true),
							(version: "10,1", result: true),
							(version: "11,1", result: true),
							(version: "12,1", result: true),
						},
						["iPad"] = new [] {
							(version: "1,1", result: false),
							(version: "1,2", result: false),
							(version: "2,1", result: false),
							(version: "3,1", result: false),
							(version: "3,2", result: false),
							(version: "3,3", result: false),
							(version: "4,1", result: true),
							(version: "4,2", result: true),
							(version: "5,1", result: true),
							(version: "6,1", result: true),
							(version: "6,3", result: true),
							(version: "7,1", result: true),
						},
						["iPod"] = new [] {
							(version: "1,1", result: false),
							(version: "1,2", result: false),
							(version: "2,1", result: false),
							(version: "3,3", result: false),
							(version: "4,1", result: false),
							(version: "5,1", result: false),
							(version: "5,2", result: false),
							(version: "7,1", result: true),
							(version: "7,2", result: true),
						},
						["AppleTV"] = new [] {
							(version: "1,1", result: true),
							(version: "2,1", result: true),
							(version: "3,1", result: true),
						},
						["Watch"] = new [] {
							(version: "1,1", result: false),
							(version: "1,2", result: false),
							(version: "2,1", result: false),
							(version: "3,1", result: false),
							(version: "3,2", result: false),
							(version: "3,3", result: false),
							(version: "4,1", result: false),
							(version: "4,2", result: false),

						}
					};


					foreach (var product in data.Keys) {
						foreach (var (version, result) in data [product])
							yield return new TestCaseData (new Device { ProductType = $"{product}{version}" }).Returns (result).SetDescription ($"{product} {version}");
					}
				}
			}

			public static IEnumerable Supports32bDevices {
				get {
					var iOSCommon = new [] {
							(version: new Version (1,1), result: true),
							(version: new Version (2,1), result: true),
							(version: new Version (3,1), result: true),
							(version: new Version (4,1), result: true),
							(version: new Version (5,1), result: true),
							(version: new Version (6,1), result: true),
							(version: new Version (7,1), result: true),
							(version: new Version (8,1), result: true),
							(version: new Version (11,1), result: false),
							(version: new Version (11,2), result: false),
							(version: new Version (12,1), result: false),
						};
					var data = new Dictionary<DeviceClass, (Version version, bool result) []> {
						[DeviceClass.iPhone] = iOSCommon,
						[DeviceClass.iPad] = iOSCommon,
						[DeviceClass.iPod] = iOSCommon,
						[DeviceClass.AppleTV] = new [] {
							(version: new Version (1,1), result: false),
							(version: new Version (2,1), result: false),
							(version: new Version (3,1), result: false),
							(version: new Version (4,1), result: false),
						},
						[DeviceClass.Watch] = new [] {
							(version: new Version (1,1), result: true),
							(version: new Version (2,1), result: true),
							(version: new Version (3,1), result: true),
							(version: new Version (4,1), result: true),
						}
					};

					foreach (var deviceClass in data.Keys) {
						foreach (var (version, result) in data [deviceClass]) {
							yield return new TestCaseData (new Device { DeviceClass = deviceClass, ProductVersion = version.ToString () }).Returns (result);
						}
					}


				}
			}

			[TestCaseSource (typeof (DevicesDataTestSource), "DebugSpeedDevices")]
			public int DebugSpeedTest (Device device) => device.DebugSpeed;

			[TestCaseSource (typeof (DevicesDataTestSource), "DevicePlatformDevices")]
			public DevicePlatform DevicePlatformTest (Device device) => device.DevicePlatform;

			[TestCaseSource (typeof (DevicesDataTestSource), "Supports64bDevices")]
			public bool Supports64bTest (Device device) => device.Supports64Bit;

			[TestCaseSource (typeof (DevicesDataTestSource), "Supports32bDevices")]
			public bool Supports32BTest (Device device) => device.Supports32Bit;
		}
	}
}
