//
// CPCompat.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#nullable enable

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CarPlay {
#if !NET
	[Register (SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("This API has been removed from the native SDK.")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class CPEntity : NSObject, INSSecureCoding {

		public CPEntity () => throw new NotSupportedException ();

		public CPEntity (NSCoder coder) => throw new NotSupportedException ();

		protected CPEntity (NSObjectFlag t) => throw new NotSupportedException ();

		protected internal CPEntity (NativeHandle handle) : base (handle) => throw new NotSupportedException ();

		public virtual void EncodeTo (NSCoder encoder) => throw new NotSupportedException ();

		public override NativeHandle ClassHandle => throw new NotSupportedException ();
	}
#endif
#if !XAMCORE_5_0 && __IOS__
	public partial class CPListItem {
#if NET
		[ObsoletedOSPlatform ("ios14.0", "Do not use; this API was removed.")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios")]
#else
		[Deprecated (PlatformName.iOS, 14, 0, message: "Do not use; this API was removed.")]
#endif
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static CGSize MaximumListItemImageSize {
			get {
				return default (CGSize);
			}
		}
	}
#endif // !XAMCORE_5_0 && __IOS__
}
