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

#if !XAMCORE_2_0
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUUIDRef */ IntPtr CFUUIDCreateWithBytes (/* CFAllocatorRef */ IntPtr alloc, 
			/* Uint8 */ byte byte0, /* Uint8 */ byte byte1, /* Uint8 */ byte byte2, /* Uint8 */ byte byte3, 
			/* Uint8 */ byte byte4, /* Uint8 */ byte byte5, /* Uint8 */ byte byte6, /* Uint8 */ byte byte7,
			/* Uint8 */ byte byte8, /* Uint8 */ byte byte9, /* Uint8 */ byte byte10, /* Uint8 */ byte byte11,
			/* Uint8 */byte  byte12, /* Uint8 */ byte byte13, /* Uint8 */ byte byte14, /* Uint8 */ byte byte15);
		
		internal unsafe static void WithArray (Guid [] uuids, string argName, Action<NSArray> action)
		{
			if (uuids == null)
				throw new ArgumentNullException (argName);
			var ptrs = new IntPtr [uuids.Length];
			for (int i = 0; i < uuids.Length; i++){
				var ba = uuids [i].ToByteArray ();
				ptrs [i] = CFUUID.CFUUIDCreateWithBytes (IntPtr.Zero, ba [0], ba [1], ba [2], ba [3], ba [4], 
					ba [5], ba [6], ba [7], ba [8], ba [9], ba [10], ba [11], ba [12], ba [13], ba [14], ba [15]);
			}
			using (var arr = NSArray.FromIntPtrs (ptrs))
				action (arr);
			
			foreach (var h in ptrs){
				if (h == IntPtr.Zero)
					continue;
				CFObject.CFRelease (h);
			}
		}
#endif
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

#if !XAMCORE_2_0
		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void RetrievePeripherals (Guid [] peripheralUuids)
		{
			CFUUID.WithArray (peripheralUuids, "peripheralUuids", x => RetrievePeripherals (x));
		}

		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void RetrievePeripherals (Guid peripheralUuid)
		{
			RetrievePeripherals (new [] { peripheralUuid });
		}

		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void ScanForPeripherals (Guid [] serviceUuids, NSDictionary options)
		{
			if (serviceUuids == null)
				ScanForPeripherals ((NSArray) null, options);
			else
				CFUUID.WithArray (serviceUuids, "serviceUuids", x => ScanForPeripherals (x, options));
		}

		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void ScanForPeripherals (Guid[] serviceUuids, PeripheralScanningOptions options = null)
		{
			ScanForPeripherals (serviceUuids, options == null ? null : options.Dictionary);
		}

		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void ScanForPeripherals (Guid [] serviceUuids)
		{
			ScanForPeripherals (serviceUuids, null as NSDictionary);
		}

		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void ScanForPeripherals (Guid serviceUuid, NSDictionary options)
		{
			ScanForPeripherals (new [] { serviceUuid }, options);
		}

		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void ScanForPeripherals (Guid serviceUuid)
		{
			ScanForPeripherals (new [] { serviceUuid }, null as NSDictionary);
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

#if !XAMCORE_2_0
		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void DiscoverServices (Guid [] services)
		{
			if (services == null)
				DiscoverServices ((NSArray) null);
			else
				CFUUID.WithArray (services, "services", x => DiscoverServices (x));
		}
#endif

		public void DiscoverServices (CBUUID [] services)
		{
			if (services == null)
				DiscoverServices ((NSArray) null);
			else
				DiscoverServices (NSArray.FromObjects (services));
		}
		
#if !XAMCORE_2_0
		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void DiscoverIncludedServices (Guid [] includedServiceUUIDs, CBService forService)
		{
			if (includedServiceUUIDs == null)
				DiscoverIncludedServices ((NSArray) null, forService);
			else
				CFUUID.WithArray (includedServiceUUIDs, "includedServiceUUIDs", x => DiscoverIncludedServices (x, forService));
		}
#endif

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

#if !XAMCORE_2_0
		[Obsolete ("Use the 'CBUUID' overload since Guid internal memory representation is different.")]
		public void DiscoverCharacteristics (Guid [] charactersticUUIDs, CBService forService)
		{
			if (charactersticUUIDs == null)
				DiscoverCharacteristics ((NSArray) null, forService);
			else
				CFUUID.WithArray (charactersticUUIDs, "charactersticUUIDs", x => DiscoverCharacteristics (x, forService));
		}
#endif
		
		public void DiscoverCharacteristics (CBUUID [] charactersticUUIDs, CBService forService)
		{
			if (charactersticUUIDs == null)
				DiscoverCharacteristics ((NSArray) null, forService);
			else
				DiscoverCharacteristics (NSArray.FromObjects (charactersticUUIDs), forService);
		}
	}
}
