#if __WATCHOS__

using System;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using UIKit;
using CoreML;
using Network;
using SceneKit;
using CoreVideo;
using CoreMedia;
using SpriteKit;
using Foundation;
using ObjCRuntime;
using MediaPlayer;
using CoreGraphics;
using CoreLocation;
using AVFoundation;
using CoreFoundation;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace UIKit {
	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("This type is not available on this platform.")]
	public partial interface INSTextAttachmentContainer : INativeObject, IDisposable
	{
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("This type is not available on this platform.")]
	public class NSTextAttachmentContainer : NSObject, INSTextAttachmentContainer {
		public NSTextAttachmentContainer () : base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		protected NSTextAttachmentContainer (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		protected internal NSTextAttachmentContainer (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

	}
}

#endif // __WATCHOS__
