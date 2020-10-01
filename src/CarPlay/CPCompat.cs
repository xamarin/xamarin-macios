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

#if !XAMCORE_4_0
namespace CarPlay {
	[Register (SkipRegistration = true)]
	[Introduced (PlatformName.iOS, 14, 0)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("This API has been removed from the native SDK.")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class CPEntity : NSObject, INSSecureCoding {

		public CPEntity () => throw new NotSupportedException ();

		public CPEntity (NSCoder coder) => throw new NotSupportedException ();

		protected CPEntity (NSObjectFlag t) => throw new NotSupportedException ();

		protected internal CPEntity (IntPtr handle) => throw new NotSupportedException ();

		public virtual void EncodeTo (NSCoder encoder) => throw new NotSupportedException ();

		public override IntPtr ClassHandle => throw new NotSupportedException ();
	}
}
#endif
