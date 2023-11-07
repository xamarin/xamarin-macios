//
// HMCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable
namespace HomeKit {
#if !NET
	[Obsolete ("Use 'HMFetchRoomHandler' instead.")]
	public delegate void FetchRoomHandler (NSArray<HMChipServiceRoom> rooms, NSError error);

	[Obsolete ("Use 'Action<Error>' instead.")]
	public delegate void HMErrorHandler (NSError error);

	[Obsolete ("This class is removed, use 'HMMatterRequestHandler' instead.")]
	[Register ("HMCHIPServiceRequestHandler", SkipRegistration = true)]
	public class HMChipServiceRequestHandler : NSObject, INSExtensionRequestHandling {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public HMChipServiceRequestHandler () => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceRequestHandler (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceRequestHandler (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void BeginRequestWithExtensionContext (NSExtensionContext context) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void ConfigureAccessory (string accessoryName, HMChipServiceRoom accessoryRoom, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task ConfigureAccessoryAsync (string accessoryName, HMChipServiceRoom accessoryRoom) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void FetchRooms (HMChipServiceHome home, FetchRoomHandler completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task<NSArray<HMChipServiceRoom>> FetchRoomsAsync (HMChipServiceHome home) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void PairAccessory (HMChipServiceHome home, string onboardingPayload, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task PairAccessoryAsync (HMChipServiceHome home, string onboardingPayload) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	} /* class HMChipServiceRequestHandler */

	[Obsolete ("This class is removed.")]
	[Register ("HMCHIPServiceTopology", SkipRegistration = true)]
	public class HMChipServiceTopology : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMChipServiceTopology (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceTopology (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceTopology (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceTopology (HMChipServiceHome [] homes) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual HMChipServiceHome [] Homes => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	} /* class HMChipServiceTopology */

	[Obsolete ("This class is removed, use 'HMMatterRoom' instead.")]
	[Register ("HMCHIPServiceRoom", SkipRegistration = true)]
	public class HMChipServiceRoom : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMChipServiceRoom (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceRoom (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceRoom (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceRoom (NSUuid uuid, string name) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual string Name => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSUuid Uuid => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	} /* class HMChipServiceRoom */

	[Obsolete ("This class is removed.")]
	[Register ("HMCHIPServiceHome", SkipRegistration = true)]
	public partial class HMChipServiceHome : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMChipServiceHome (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceHome (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceHome (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public HMChipServiceHome (NSUuid uuid, string name) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual string Name => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSUuid Uuid => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	} /* class HMChipServiceHome */

	public partial class HMAccessorySetupManager {

#pragma warning disable CS0618 // HMChipServiceTopology and HMErrorHandler is obsolete
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.RemovedFromHomeKit)]
		public virtual void AddAndSetUpAccessories (HMChipServiceTopology topology, HMErrorHandler completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.RemovedFromHomeKit)]
		public virtual Task AddAndSetUpAccessoriesAsync (HMChipServiceTopology topology) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
#pragma warning restore CS0618 // HMChipServiceTopology and HMErrorHandler is obsolete
	}
#endif // !NET

#if !XAMCORE_5_0
#if __IOS__ && !__MACCATALYST__
	public unsafe partial class HMAccessorySetupManager {

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.RemovedFromHomeKit)]
		public virtual void AddAndSetUpAccessories (HMMatterTopology topology, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.RemovedFromHomeKit)]
		public virtual Task AddAndSetUpAccessoriesAsync (HMMatterTopology topology) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.RemovedFromHomeKit)]
		public virtual void PerformMatterEcosystemAccessorySetup (HMAccessorySetupRequest request, HMMatterTopology topology, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.RemovedFromHomeKit)]
		public virtual Task PerformMatterEcosystemAccessorySetupAsync (HMAccessorySetupRequest request, HMMatterTopology topology) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
	}

#endif

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("This class is removed.")]
#if NET
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
#endif
	[Register ("HMMatterRoom", SkipRegistration = true)]
	public partial class HMMatterRoom : NSObject, INSCoding, INSCopying, INSSecureCoding {
		public override NativeHandle ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMMatterRoom (NativeHandle handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMMatterRoom (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMMatterRoom (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMMatterRoom (NSUuid uuid, string name) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual string Name => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSUuid Uuid => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("This class is removed.")]
#if NET
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
#endif
	[Register ("HMMatterHome", SkipRegistration = true)]
	public partial class HMMatterHome : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override NativeHandle ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMMatterHome (NativeHandle handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMMatterHome (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMMatterHome (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public HMMatterHome (NSUuid uuid, string name) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual string Name => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSUuid Uuid => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("This class is removed.")]
#if NET
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
#endif
	[Register ("HMMatterTopology", SkipRegistration = true)]
	public partial class HMMatterTopology : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override NativeHandle ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMMatterTopology (NativeHandle handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMMatterTopology (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMMatterTopology (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public HMMatterTopology (HMMatterHome [] homes) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual HMMatterHome [] Homes => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
	}

	public delegate void HMFetchRoomHandler (HMMatterRoom [] rooms, NSError error);

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("This class is removed.")]
#if NET
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
#endif
	[Register ("HMMatterRequestHandler", SkipRegistration = true)]
	public partial class HMMatterRequestHandler : NSObject, INSExtensionRequestHandling {
		public override NativeHandle ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public HMMatterRequestHandler () => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMMatterRequestHandler (NativeHandle handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMMatterRequestHandler (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void FetchRooms (HMMatterHome home, HMFetchRoomHandler completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task<HMMatterRoom []> FetchRoomsAsync (HMMatterHome home) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void PairAccessory (HMMatterHome home, string onboardingPayload, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task PairAccessoryAsync (HMMatterHome home, string onboardingPayload) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void ConfigureAccessory (string accessoryName, HMMatterRoom accessoryRoom, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task ConfigureAccessoryAsync (string accessoryName, HMMatterRoom accessoryRoom) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void BeginRequestWithExtensionContext (NSExtensionContext context) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
	}

#endif
}
