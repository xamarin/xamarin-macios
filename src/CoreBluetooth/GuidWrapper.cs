//
// GuidWrapper.cs: Support for treating C# Guids as UUids
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com
//
// Copyright 2011-2014 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;

#nullable enable

namespace CoreBluetooth {

	internal static class CFUUID {

		// CFUUID.h
		[DllImport (Constants.CoreFoundationLibrary)]
		public extern static /* CFUUIDRef */ IntPtr CFUUIDCreateFromString ( /* CFAllocatorRef */ IntPtr alloc, /* CFStringRef */ IntPtr uuidStr);
	}

	public partial class CBCentralManager {

		public void ConnectPeripheral (CBPeripheral peripheral, PeripheralConnectionOptions? options = null)
		{
			ConnectPeripheral (peripheral, options?.Dictionary);
		}
#if !NET && !TVOS && !WATCH
		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public void RetrievePeripherals (CBUUID [] peripheralUuids)
			=> throw new NotSupportedException ();

		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public void RetrievePeripherals (CBUUID peripheralUuid)
			=> throw new NotSupportedException ();
#endif
		public void ScanForPeripherals (CBUUID []? peripheralUuids, NSDictionary? options)
		{
			if (peripheralUuids is null)
				ScanForPeripherals ((NSArray?) null, options);
			else
				ScanForPeripherals (NSArray.FromObjects (peripheralUuids), options);
		}

		public void ScanForPeripherals (CBUUID [] peripheralUuids, PeripheralScanningOptions? options = null)
		{
			ScanForPeripherals (peripheralUuids, options?.Dictionary);
		}

		public void ScanForPeripherals (CBUUID [] peripheralUuids)
		{
			ScanForPeripherals (peripheralUuids, null as NSDictionary);
		}

		public void ScanForPeripherals (CBUUID serviceUuid, NSDictionary? options)
		{
			ScanForPeripherals (new [] { serviceUuid }, options);
		}

		public void ScanForPeripherals (CBUUID serviceUuid)
		{
			ScanForPeripherals (new [] { serviceUuid }, null as NSDictionary);
		}
	}

	public partial class CBPeripheral {

		public void DiscoverServices ()
		{
			DiscoverServices ((NSArray?) null);
		}

		public void DiscoverServices (CBUUID []? services)
		{
			if (services is null)
				DiscoverServices ((NSArray?) null);
			else
				DiscoverServices (NSArray.FromObjects (services));
		}

		public void DiscoverIncludedServices (CBUUID []? includedServiceUUIDs, CBService forService)
		{
			if (includedServiceUUIDs is null)
				DiscoverIncludedServices ((NSArray?) null, forService);
			else
				DiscoverIncludedServices (NSArray.FromObjects (includedServiceUUIDs), forService);
		}

		public void DiscoverCharacteristics (CBService forService)
		{
			DiscoverCharacteristics ((NSArray?) null, forService);
		}

		public void DiscoverCharacteristics (CBUUID []? charactersticUUIDs, CBService forService)
		{
			if (charactersticUUIDs is null)
				DiscoverCharacteristics ((NSArray?) null, forService);
			else
				DiscoverCharacteristics (NSArray.FromObjects (charactersticUUIDs), forService);
		}
	}
}
