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

namespace CoreBluetooth {

	internal static class CFUUID {

		// CFUUID.h
		[DllImport (Constants.CoreFoundationLibrary)]
		public extern static /* CFUUIDRef */ IntPtr CFUUIDCreateFromString ( /* CFAllocatorRef */ IntPtr alloc, /* CFStringRef */ IntPtr uuidStr);
	}		
	
	public partial class CBCentralManager {

		public void ConnectPeripheral (CBPeripheral peripheral, PeripheralConnectionOptions options = null)
		{
			ConnectPeripheral (peripheral, options == null ? null : options.Dictionary);
		}

#if !TVOS && !WATCH
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0, message : "Use 'RetrievePeripheralsWithIdentifiers' instead.")]
		public void RetrievePeripherals (CBUUID [] peripheralUuids)
		{
			if (peripheralUuids == null)
				throw new ArgumentNullException ("peripheralUuids");
			var ptrs = new IntPtr [peripheralUuids.Length];	
			for (int i =0; i < peripheralUuids.Length; i++) {
				using (var s = new NSString (peripheralUuids[i].ToString(true)))
					ptrs [i] = CFUUID.CFUUIDCreateFromString (IntPtr.Zero, s.Handle);
			}	
			using (var arr = NSArray.FromIntPtrs (ptrs)) 
				RetrievePeripherals (arr);

			foreach (var p in ptrs) 
				CFObject.CFRelease (p);
		}

		public void RetrievePeripherals (CBUUID peripheralUuid)
		{
			RetrievePeripherals (new [] { peripheralUuid });
		}
#endif
		public void ScanForPeripherals (CBUUID [] peripheralUuids, NSDictionary options)
		{
			if (peripheralUuids == null)
				ScanForPeripherals ((NSArray) null, options);
			else
				ScanForPeripherals (NSArray.FromObjects (peripheralUuids), options);
		}

		public void ScanForPeripherals (CBUUID[] peripheralUuids, PeripheralScanningOptions options = null)
		{
			ScanForPeripherals (peripheralUuids, options == null ? null : options.Dictionary);
		}

		public void ScanForPeripherals (CBUUID [] peripheralUuids)
		{
			ScanForPeripherals (peripheralUuids, null as NSDictionary);
		}

		public void ScanForPeripherals (CBUUID serviceUuid, NSDictionary options)
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
			DiscoverServices ((NSArray) null);
		}

		public void DiscoverServices (CBUUID [] services)
		{
			if (services == null)
				DiscoverServices ((NSArray) null);
			else
				DiscoverServices (NSArray.FromObjects (services));
		}

		public void DiscoverIncludedServices (CBUUID [] includedServiceUUIDs, CBService forService)
		{
			if (includedServiceUUIDs == null)
				DiscoverIncludedServices ((NSArray) null, forService);
			else
				DiscoverIncludedServices (NSArray.FromObjects (includedServiceUUIDs), forService);
		}

		public void DiscoverCharacteristics (CBService forService)
		{
			DiscoverCharacteristics ((NSArray)null, forService);
		}

		public void DiscoverCharacteristics (CBUUID [] charactersticUUIDs, CBService forService)
		{
			if (charactersticUUIDs == null)
				DiscoverCharacteristics ((NSArray) null, forService);
			else
				DiscoverCharacteristics (NSArray.FromObjects (charactersticUUIDs), forService);
		}
	}
}
