//
// ChipCompat.cs
//
// Authors:
//	Rachel Kang  <rachelkang@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

#nullable enable

#if !NET
namespace Chip {

#if !MONOMAC
	public partial class ChipReadAttributeResult { }
#endif // !MONOMAC

	[Obsolete ("This class is removed.")]
	[Register ("CHIPError", SkipRegistration = true)]
	public class ChipError : NSObject {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected ChipError (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromChip);
		protected ChipError (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public static int ConvertToChipErrorCode (NSError errorCode) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public static NSError? Create (int errorCode) => throw new InvalidOperationException (Constants.RemovedFromChip);

	} /* class ChipError */

#if !MONOMAC
	[Obsolete ("This class is removed, use 'ChipContentLauncher' instead.")]
	[Register ("CHIPContentLaunch", SkipRegistration = true)]
	public class ChipContentLaunch : NSObject {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected ChipContentLaunch (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromChip);
		protected ChipContentLaunch (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public ChipContentLaunch (ChipDevice device, byte endpoint, DispatchQueue queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void LaunchContent (byte autoPlay, string data, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> LaunchContentAsync (byte autoPlay, string data) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual void LaunchUrl (string contentUrl, string displayString, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> LaunchUrlAsync (string contentUrl, string displayString) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual void ReadAttributeAcceptsHeaderList (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> ReadAttributeAcceptsHeaderListAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual void ReadAttributeSupportedStreamingTypes (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> ReadAttributeSupportedStreamingTypesAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual void ReadAttributeClusterRevision (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> ReadAttributeClusterRevisionAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

	} /* class ChipContentLaunch */
#endif // !MONOMAC

#if !MONOMAC
	[Obsolete ("This class is removed.")]
	[Register ("CHIPTrustedRootCertificates", SkipRegistration = true)]
	public class ChipTrustedRootCertificates : NSObject {
		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected ChipTrustedRootCertificates (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromChip);
		protected ChipTrustedRootCertificates (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public ChipTrustedRootCertificates (ChipDevice device, byte endpoint, DispatchQueue queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void AddTrustedRootCertificate (NSData rootCertificate, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> AddTrustedRootCertificateAsync (NSData rootCertificate) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual void RemoveTrustedRootCertificate (NSData trustedRootIdentifier, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> RemoveTrustedRootCertificateAsync (NSData trustedRootIdentifier) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual void ReadAttributeClusterRevision (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> ReadAttributeClusterRevisionAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

	}
#endif // !MONOMAC

	public partial class ChipWindowCovering {

#if !MONOMAC
		static bool CheckSystemVersion ()
		{
#if NET || IOS || __MACCATALYST__ || TVOS
			return SystemVersion.CheckiOS (15, 2);
#elif WATCH
			return SystemVersion.CheckwatchOS (8, 3);
#else
#error Unknown platform
#endif
		}
#endif

#if !MONOMAC
		public virtual void GoToLiftValue (ushort liftValue, ChipResponseHandler responseHandler)
		{
			if (CheckSystemVersion ())
				_OldGoToLiftValue (liftValue, responseHandler);
			else
				_NewGoToLiftValue (liftValue, responseHandler);
		}
#endif

#if !MONOMAC
		public virtual Task<ChipReadAttributeResult> GoToLiftValueAsync (ushort liftValue)
		{
			if (CheckSystemVersion ())
				return _OldGoToLiftValueAsync (liftValue);
			else
				return _NewGoToLiftValueAsync (liftValue);
		}
#endif

#if !MONOMAC
		public virtual void GoToTiltValue (ushort tiltValue, ChipResponseHandler responseHandler)
		{
			if (CheckSystemVersion ())
				_OldGoToTiltValue (tiltValue, responseHandler);
			else
				_NewGoToTiltValue (tiltValue, responseHandler);
		}
#endif

#if !MONOMAC
		public virtual Task<ChipReadAttributeResult> GoToTiltValueAsync (ushort tiltValue)
		{
			if (CheckSystemVersion ())
				return _OldGoToTiltValueAsync (tiltValue);
			else
				return _NewGoToTiltValueAsync (tiltValue);
		}
#endif

	}

	[Mac (12, 1), Watch (8, 3), TV (15, 2), iOS (15, 2), MacCatalyst (15, 2)]
	public delegate void ChipDeviceConnectionCallback (ChipDevice device, NSError error);

	[Obsolete ("This class is removed.")]
	[Register ("CHIPDeviceController", true)]
	public partial class ChipDeviceController : NSObject {
		public override IntPtr ClassHandle { get { throw new InvalidOperationException (Constants.RemovedFromChip); } }

		protected ChipDeviceController (NSObjectFlag t) : base (t) => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected internal ChipDeviceController (IntPtr handle) : base (handle) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool GetConnectedDevice (ulong deviceID, global::CoreFoundation.DispatchQueue queue, ChipDeviceConnectionCallback completionHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipDevice> GetConnectedDeviceAsync (ulong deviceID, global::CoreFoundation.DispatchQueue queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipDevice> GetConnectedDeviceAsync (ulong deviceID, global::CoreFoundation.DispatchQueue queue, out bool result) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual ChipDevice? GetPairedDevice (ulong deviceId, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool IsDevicePaired (ulong deviceID, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

#if !NET && !__MACOS__
		[Obsolete ("This method is removed.", false)]
		public virtual bool PairDevice (ulong deviceId, ushort discriminator, uint setupPinCode, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);
#endif

#if !__MACOS__
		public virtual bool PairDevice (ulong deviceID, ushort discriminator, uint setupPINCode, NSData? csrNonce, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);
#endif

		public virtual bool PairDevice (ulong deviceId, string address, ushort port, ushort discriminator, uint setupPinCode, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool PairDevice (ulong deviceId, string onboardingPayload, ChipOnboardingPayloadType onboardingPayloadType, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool PairDeviceWithoutSecurity (ulong deviceId, string address, ushort port, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void SetListenPort (ushort port) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void SetPairingDelegate (IChipDevicePairingDelegate @delegate, global::CoreFoundation.DispatchQueue queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool Shutdown () => throw new InvalidOperationException (Constants.RemovedFromChip);

		[Obsolete ("This method is removed.", false)]
		public virtual bool Startup (IChipPersistentStorageDelegate? storageDelegate) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool Startup (IChipPersistentStorageDelegate? storageDelegate, ushort vendorId, IChipKeypair? nocSigner) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool StopDevicePairing (ulong deviceId, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual bool UnpairDevice (ulong deviceId, out NSError? error) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void UpdateDevice (ulong deviceId, ulong fabricId) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual NSNumber ControllerNodeId => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual bool IsRunning => throw new InvalidOperationException (Constants.RemovedFromChip);
		public static ChipDeviceController SharedController => throw new InvalidOperationException (Constants.RemovedFromChip);
	}

	[Obsolete ("This class is removed.")]
	[Register ("CHIPLowPower", true)]
	public partial class ChipLowPower : ChipCluster {
		public override IntPtr ClassHandle { get { throw new InvalidOperationException (Constants.RemovedFromChip); } }

		protected ChipLowPower (NSObjectFlag t) : base (t) => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected internal ChipLowPower (IntPtr handle) : base (handle) => throw new InvalidOperationException (Constants.RemovedFromChip);

		[DesignatedInitializer]
		public ChipLowPower (ChipDevice device, ushort endpoint, global::CoreFoundation.DispatchQueue queue)
			: base (NSObjectFlag.Empty) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public ChipLowPower (ChipDevice device, byte endpoint, global::CoreFoundation.DispatchQueue queue)
			: this (device, (ushort) endpoint, queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeClusterRevision (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeClusterRevisionAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void Sleep (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);
		public virtual Task<ChipReadAttributeResult> SleepAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);
	}

	[Obsolete ("This class is removed.")]
	[Register ("CHIPTestCluster", true)]
	public partial class ChipTestCluster : ChipCluster {
		public override IntPtr ClassHandle { get { throw new InvalidOperationException (Constants.RemovedFromChip); } }

		protected ChipTestCluster (NSObjectFlag t) : base (t) => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected internal ChipTestCluster (IntPtr handle) : base (handle) => throw new InvalidOperationException (Constants.RemovedFromChip);

		[DesignatedInitializer]
		public ChipTestCluster (ChipDevice device, ushort endpoint, global::CoreFoundation.DispatchQueue queue)
			: base (NSObjectFlag.Empty) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public ChipTestCluster (ChipDevice device, byte endpoint, global::CoreFoundation.DispatchQueue queue)
			: this (device, (ushort) endpoint, queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBitmap16 (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBitmap16Async () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBitmap32 (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBitmap32Async () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBitmap64 (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBitmap64Async () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBitmap8 (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBitmap8Async () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBoolean (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBooleanAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeCharString (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeCharStringAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeClusterRevision (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeClusterRevisionAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeEnum16 (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeEnum16Async () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeEnum8 (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeEnum8Async () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt16s (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt16sAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt16u (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt16uAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt32s (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt32sAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt32u (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt32uAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt64s (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt64sAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt64u (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt64uAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt8s (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt8sAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeInt8u (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeInt8uAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeListInt8u (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeListInt8uAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeListOctetString (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeListOctetStringAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeListStructOctetString (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeListStructOctetStringAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeLongCharString (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeLongCharStringAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeLongOctetString (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeLongOctetStringAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeOctetString (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeOctetStringAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeUnsupported (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeUnsupportedAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void Test (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> TestAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void TestAddArguments (byte arg1, byte arg2, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> TestAddArgumentsAsync (byte arg1, byte arg2) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void TestNotHandled (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> TestNotHandledAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void TestSpecific (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> TestSpecificAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void TestUnknownCommand (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> TestUnknownCommandAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBitmap16 (ushort value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBitmap16Async (ushort value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBitmap32 (uint value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBitmap32Async (uint value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBitmap64 (ulong value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBitmap64Async (ulong value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBitmap8 (byte value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBitmap8Async (byte value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBoolean (byte value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBooleanAsync (byte value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBoolean (bool boolValue, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBooleanAsync (bool boolValue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeCharString (string value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeCharStringAsync (string value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeEnum16 (ushort value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeEnum16Async (ushort value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeEnum8 (byte value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeEnum8Async (byte value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt16s (short value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt16sAsync (short value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt16u (ushort value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt16uAsync (ushort value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt32s (int value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt32sAsync (int value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt32u (uint value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt32uAsync (uint value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt64s (long value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt64sAsync (long value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt64u (ulong value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt64uAsync (ulong value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt8s (sbyte value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt8sAsync (sbyte value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeInt8u (byte value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeInt8uAsync (byte value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeLongCharString (string value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeLongCharStringAsync (string value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeLongOctetString (NSData value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeLongOctetStringAsync (NSData value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeOctetString (NSData value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeOctetStringAsync (NSData value) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeUnsupported (bool boolValue, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeUnsupportedAsync (bool boolValue) => throw new InvalidOperationException (Constants.RemovedFromChip);
	}

	[Obsolete ("This class is removed.")]
	[Register ("CHIPGeneralCommissioning", true)]
	public partial class ChipGeneralCommissioning : ChipCluster {
		public override IntPtr ClassHandle { get { throw new InvalidOperationException (Constants.RemovedFromChip); } }

		protected ChipGeneralCommissioning (NSObjectFlag t) : base (t) => throw new InvalidOperationException (Constants.RemovedFromChip);

		protected internal ChipGeneralCommissioning (IntPtr handle) : base (handle) => throw new InvalidOperationException (Constants.RemovedFromChip);

		[DesignatedInitializer]
		public ChipGeneralCommissioning (ChipDevice device, ushort endpoint, global::CoreFoundation.DispatchQueue queue)
			: base (NSObjectFlag.Empty) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public ChipGeneralCommissioning (ChipDevice device, byte endpoint, global::CoreFoundation.DispatchQueue queue)
			: this (device, (ushort) endpoint, queue) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ArmFailSafe (ushort expiryLengthSeconds, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ArmFailSafeAsync (ushort expiryLengthSeconds, ulong breadcrumb, uint timeoutMs) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void CommissioningComplete (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> CommissioningCompleteAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBasicCommissioningInfoList (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBasicCommissioningInfoListAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeBreadcrumb (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeBreadcrumbAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void ReadAttributeClusterRevision (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> ReadAttributeClusterRevisionAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		[Obsolete ("This method is removed.", false)]
		public virtual void ReadAttributeFabricId (ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		[Obsolete ("This method is removed.", false)]
		public virtual Task<ChipReadAttributeResult> ReadAttributeFabricIdAsync () => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void SetRegulatoryConfig (byte location, string countryCode, ulong breadcrumb, uint timeoutMs, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> SetRegulatoryConfigAsync (byte location, string countryCode, ulong breadcrumb, uint timeoutMs) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual void WriteAttributeBreadcrumb (ulong value, ChipResponseHandler responseHandler) => throw new InvalidOperationException (Constants.RemovedFromChip);

		public virtual Task<ChipReadAttributeResult> WriteAttributeBreadcrumbAsync (ulong value)
 => throw new InvalidOperationException (Constants.RemovedFromChip);
	}
}
#endif // !NET
