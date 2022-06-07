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

#if !NET
#nullable enable
namespace HomeKit {

	[Obsolete ("Use 'HMFetchRoomHandler' instead.")]
	public delegate void FetchRoomHandler (NSArray<HMChipServiceRoom> rooms, NSError error);

	[Obsolete ("Use 'Action<Error>' instead.")]
	public delegate void HMErrorHandler (NSError error);

	[Obsolete ("This class is removed, use 'HMMatterRequestHandler' instead.")]
	[Register ("HMCHIPServiceRequestHandler", SkipRegistration = true)]
	public class HMChipServiceRequestHandler : NSObject, INSExtensionRequestHandling {

		public override IntPtr ClassHandle => throw new InvalidOperationException ();

		public HMChipServiceRequestHandler () => throw new InvalidOperationException ();
		protected HMChipServiceRequestHandler (NSObjectFlag t) => throw new InvalidOperationException ();
		protected HMChipServiceRequestHandler (IntPtr handle) => throw new InvalidOperationException ();

		public virtual void BeginRequestWithExtensionContext (NSExtensionContext context) => throw new InvalidOperationException ();
		public virtual void ConfigureAccessory (string accessoryName, HMChipServiceRoom accessoryRoom, Action<NSError> completion) => throw new InvalidOperationException ();
		public virtual Task ConfigureAccessoryAsync (string accessoryName, HMChipServiceRoom accessoryRoom) => throw new InvalidOperationException ();
		public virtual void FetchRooms (HMChipServiceHome home, FetchRoomHandler completion) => throw new InvalidOperationException ();
		public virtual Task<NSArray<HMChipServiceRoom>> FetchRoomsAsync (HMChipServiceHome home) => throw new InvalidOperationException ();
		public virtual void PairAccessory (HMChipServiceHome home, string onboardingPayload, Action<NSError> completion) => throw new InvalidOperationException ();
		public virtual Task PairAccessoryAsync (HMChipServiceHome home, string onboardingPayload) => throw new InvalidOperationException ();

	} /* class HMChipServiceRequestHandler */

	[Obsolete ("This class is removed, use 'HMMatterTopology' instead.")]
	[Register ("HMCHIPServiceTopology", SkipRegistration = true)]
	public class HMChipServiceTopology : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException ();

		protected HMChipServiceTopology (IntPtr handle) => throw new InvalidOperationException ();
		public HMChipServiceTopology (NSCoder coder) => throw new InvalidOperationException ();
		protected HMChipServiceTopology (NSObjectFlag t) => throw new InvalidOperationException ();
		public HMChipServiceTopology (HMChipServiceHome[] homes) => throw new InvalidOperationException ();

		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException ();
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException ();
		public virtual HMChipServiceHome[] Homes => throw new InvalidOperationException ();

	} /* class HMChipServiceTopology */

	[Obsolete ("This class is removed, use 'HMMatterRoom' instead.")]
	[Register("HMCHIPServiceRoom", SkipRegistration = true)]
	public class HMChipServiceRoom : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException ();

		protected HMChipServiceRoom (IntPtr handle) => throw new InvalidOperationException ();
		public HMChipServiceRoom (NSCoder coder) => throw new InvalidOperationException ();
		protected HMChipServiceRoom (NSObjectFlag t) => throw new InvalidOperationException ();
		public HMChipServiceRoom (NSUuid uuid, string name) => throw new InvalidOperationException ();

		public virtual NSObject Copy (NSZone? zone)=> throw new InvalidOperationException ();
		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException ();
		public virtual string Name => throw new InvalidOperationException ();
		public virtual NSUuid Uuid => throw new InvalidOperationException ();

	} /* class HMChipServiceRoom */

	[Obsolete ("This class is removed, use 'HMMatterHome' instead.")]
	[Register("HMCHIPServiceHome", SkipRegistration = true)]
	public partial class HMChipServiceHome : NSObject, INSCoding, INSCopying, INSSecureCoding {

		public override IntPtr ClassHandle => throw new InvalidOperationException ();

		protected HMChipServiceHome (IntPtr handle) => throw new InvalidOperationException ();
		public HMChipServiceHome (NSCoder coder) => throw new InvalidOperationException ();
		protected HMChipServiceHome (NSObjectFlag t) => throw new InvalidOperationException ();

		public HMChipServiceHome (NSUuid uuid, string name) => throw new InvalidOperationException ();
		public virtual NSObject Copy (NSZone? zone) => throw new InvalidOperationException ();

		public virtual void EncodeTo (NSCoder encoder) => throw new InvalidOperationException ();
		public virtual string Name => throw new InvalidOperationException ();
		public virtual NSUuid Uuid => throw new InvalidOperationException ();

	} /* class HMChipServiceHome */

	public partial class HMAccessorySetupManager {

#pragma warning disable CS0618 // HMChipServiceTopology and HMErrorHandler is obsolete
		public virtual void AddAndSetUpAccessories (HMChipServiceTopology topology, HMErrorHandler completion) => throw new InvalidOperationException ();
		public virtual Task AddAndSetUpAccessoriesAsync (HMChipServiceTopology topology) => throw new InvalidOperationException ();
#pragma warning restore CS0618 // HMChipServiceTopology and HMErrorHandler is obsolete

	} /* class HMAccessorySetupManager */

}
#endif // !NET
