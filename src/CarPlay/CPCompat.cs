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
using Foundation;
using ObjCRuntime;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if !NET
namespace CarPlay {
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
}
#endif
