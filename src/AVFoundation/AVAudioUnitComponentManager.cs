//
// AVAudioUnitComponentManager.cs
//
// Author: Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2018 Microsoft.
//

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AVFoundation {

#if !WATCH
	public partial class AVAudioUnitComponentManager {
#if !XAMCORE_4_0
		[Obsolete ("Please use the static 'SharedInstance' property as this type is not meant to be created.")]
		[CompilerGenerated]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Export ("init")]
		public AVAudioUnitComponentManager () : base (NSObjectFlag.Empty)
		{
			if (IsDirectBinding) {
				Handle = XamCore.ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle ("init"));
			} else {
				Handle = XamCore.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle ("init"));
			}
		}
#endif
	}
#endif
}
