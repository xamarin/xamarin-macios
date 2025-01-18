#if !XAMCORE_5_0

using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;

using Foundation;
using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CloudKit {
	[Register ("CKModifyBadgeOperation", SkipRegistration = true)]
#if NET
	[UnsupportedOSPlatform ("ios", "Modifying badge counts is no longer supported.")]
	[UnsupportedOSPlatform ("macos", "Modifying badge counts is no longer supported.")]
	[UnsupportedOSPlatform ("tvos", "Modifying badge counts is no longer supported.")]
	[UnsupportedOSPlatform ("maccatalyst", "Modifying badge counts is no longer supported.")]
#else
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Modifying badge counts is no longer supported.")]
	[Deprecated (PlatformName.iOS, 18, 0, message: "Modifying badge counts is no longer supported.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Modifying badge counts is no longer supported.")]
#endif
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class CKModifyBadgeOperation : CKOperation {
		public override NativeHandle ClassHandle { get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms); }

		public CKModifyBadgeOperation () : base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		protected CKModifyBadgeOperation (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		protected internal CKModifyBadgeOperation (NativeHandle handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public CKModifyBadgeOperation (nuint badgeValue)
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public virtual nuint BadgeValue {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}

		public unsafe virtual global::System.Action<NSError>? Completed {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}
	} /* class CKModifyBadgeOperation */
}
#endif // !XAMCORE_5_0
