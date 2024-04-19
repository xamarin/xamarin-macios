using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if !XAMCORE_5_0 && !__MACOS__
namespace GameplayKit {
	[Register ("GKHybridStrategist", SkipRegistration = true)]
#if NET
	[UnsupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
#endif
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class GKHybridStrategist : NSObject, IGKStrategist {
		/// <summary>Do not use</summary>
		public override NativeHandle ClassHandle => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		public GKHybridStrategist () : base (NSObjectFlag.Empty) => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		/// <summary>Do not use</summary>
		protected GKHybridStrategist (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		/// <summary>Do not use</summary>
		protected internal GKHybridStrategist (NativeHandle handle) : base (handle) => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		public virtual IGKGameModelUpdate GetBestMoveForActivePlayer () => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		public virtual nuint Budget {
			get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			set => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}
		public virtual nuint ExplorationParameter {
			get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			set => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}
		public virtual IGKGameModel? GameModel {
			get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			set => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}
		public virtual nuint MaxLookAheadDepth {
			get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			set => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}
		public virtual IGKRandom? RandomSource {
			get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			set => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}
	}
}
#endif // !XAMCORE_5_0 && !__MACOS__
