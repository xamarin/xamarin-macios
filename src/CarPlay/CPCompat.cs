//
// CPCompat.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;
using System.ComponentModel;
using System.Runtime.Versioning;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if !XAMCORE_4_0
namespace CarPlay {
	[Register (SkipRegistration = true)]
#if !NET
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("This API has been removed from the native SDK.")]
#else
	[UnsupportedOSPlatform ("ios")]
	[Obsolete ("This API has been removed from the native SDK.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class CPEntity : NSObject, INSSecureCoding {

		public CPEntity () => throw new NotSupportedException ();

		public CPEntity (NSCoder coder) => throw new NotSupportedException ();

		protected CPEntity (NSObjectFlag t) => throw new NotSupportedException ();

		protected internal CPEntity (NativeHandle handle) : base (handle) => throw new NotSupportedException ();

		public virtual void EncodeTo (NSCoder encoder) => throw new NotSupportedException ();

		public override NativeHandle ClassHandle => throw new NotSupportedException ();
	}
}
#endif
