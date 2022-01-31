//
// ChipCompat.cs
//
// Authors:
//	Rachel Kang  <rachelkang@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
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
	public class ChipTrustedRootCertificates : NSObject
	{
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
}
#endif // !NET
