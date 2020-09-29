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
	[Obsoleted (PlatformName.iOS, 14, 2, message: "API removed.")]
	public class CPEntity : NSObject, INSSecureCoding {

		public CPEntity () => throw new NotImplementedException ();

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected CPEntity (NSObjectFlag t) => throw new NotImplementedException ();

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected internal CPEntity (IntPtr handle) => throw new NotImplementedException ();

		public virtual void EncodeTo (NSCoder coder) => throw new NotImplementedException ();
	}
}
#endif
