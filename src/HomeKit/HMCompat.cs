//
// HMCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
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

	[Obsolete ("This class is removed, use 'HMMatterTopology' instead.")]
	[Register ("HMCHIPServiceTopology", SkipRegistration = true)]
	public class HMChipServiceTopology : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMChipServiceTopology (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceTopology (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceTopology (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceTopology (HMChipServiceHome[] homes) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual HMChipServiceHome[] Homes => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	} /* class HMChipServiceTopology */

	[Obsolete ("This class is removed, use 'HMMatterRoom' instead.")]
	[Register("HMCHIPServiceRoom", SkipRegistration = true)]
	public class HMChipServiceRoom : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		protected HMChipServiceRoom (IntPtr handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceRoom (NSCoder coder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMChipServiceRoom (NSObjectFlag t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public HMChipServiceRoom (NSUuid uuid, string name) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual NSObject Copy (NSZone? zone)=> throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual string Name => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual NSUuid Uuid => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

	} /* class HMChipServiceRoom */

	[Obsolete ("This class is removed, use 'HMMatterHome' instead.")]
	[Register("HMCHIPServiceHome", SkipRegistration = true)]
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

	[Obsolete ("This class is removed.")]
	public partial class HMAccessorySetupManager {

#pragma warning disable CS0618 // HMChipServiceTopology and HMErrorHandler is obsolete
		public virtual void AddAndSetUpAccessories (HMChipServiceTopology topology, HMErrorHandler completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task AddAndSetUpAccessoriesAsync (HMChipServiceTopology topology) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
#pragma warning restore CS0618 // HMChipServiceTopology and HMErrorHandler is obsolete
	}	
#endif // !NET

#if __IOS__ && !__MACCATALYST__
#if NET
	[Obsolete ("This class is removed.")]
#endif
	public unsafe partial class HMAccessorySetupManager : NSObject {
		public override NativeHandle ClassHandle => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public HMAccessorySetupManager () : base (NSObjectFlag.Empty) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected HMAccessorySetupManager (NSObjectFlag t) : base (t) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		protected internal HMAccessorySetupManager (NativeHandle handle) : base (handle) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);

		public virtual void AddAndSetUpAccessories (HMMatterTopology topology, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task AddAndSetUpAccessoriesAsync (HMMatterTopology topology) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void PerformAccessorySetup (HMAccessorySetupRequest request, Action<HMAccessorySetupResult, NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task<HMAccessorySetupResult> PerformAccessorySetupAsync (HMAccessorySetupRequest request) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual void PerformMatterEcosystemAccessorySetup (HMAccessorySetupRequest request, HMMatterTopology topology, Action<NSError> completion) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
		public virtual Task PerformMatterEcosystemAccessorySetupAsync (HMAccessorySetupRequest request, HMMatterTopology topology) => throw new InvalidOperationException (Constants.RemovedFromHomeKit);
	}
#endif
}
