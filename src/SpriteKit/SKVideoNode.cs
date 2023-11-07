//
// SKVideoNode.cs: extensions to SKVideoNode
//
// Authors:
//   Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2016 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

#nullable enable

#if !WATCH
namespace SpriteKit {
	public partial class SKVideoNode : SKNode {

		static bool CheckSystemVersion ()
		{
#if MONOMAC
			return SystemVersion.CheckmacOS (10, 10);
#elif TVOS || IOS
			return SystemVersion.CheckiOS (8, 0);
#else
#error Unknown platform
#endif
		}

		// Apple deprecated videoNodeWithVideoFileNamed: in 10.10/8.0
		// and made available videoNodeWithFileNamed: so we invoke
		// the right one at runtime depending on which OS version we are running
		// https://bugzilla.xamarin.com/show_bug.cgi?id=37727
		public static SKVideoNode FromFile (string videoFile)
		{
			if (CheckSystemVersion ())
				return VideoNodeWithFileNamed (videoFile);
			else
				return VideoNodeWithVideoFileNamed (videoFile);
		}

		// Apple deprecated videoNodeWithVideoURL: in 10.10/8.0
		// and made available videoNodeWithURL: so we invoke
		// the right one at runtime depending on which OS version we are running
		// https://bugzilla.xamarin.com/show_bug.cgi?id=37727
		public static SKVideoNode FromUrl (NSUrl videoUrl)
		{
			if (CheckSystemVersion ())
				return VideoNodeWithURL (videoUrl);
			else
				return VideoNodeWithVideoURL (videoUrl);
		}

		// Apple deprecated initWithVideoFileNamed: in 10.10/8.0
		// and made available initWithFileNamed: so we invoke
		// the right one at runtime depending on which OS version we are running
		// https://bugzilla.xamarin.com/show_bug.cgi?id=37727
		[DesignatedInitializer]
		public SKVideoNode (string videoFile)
		{
			if (CheckSystemVersion ())
				Handle = InitWithFileNamed (videoFile);
			else
				Handle = InitWithVideoFileNamed (videoFile);
		}

		// Apple deprecated initWithVideoURL: in 10.10/8.0
		// and made available initWithURL: so we invoke
		// the right one at runtime depending on which OS version we are running
		// https://bugzilla.xamarin.com/show_bug.cgi?id=37727
		[DesignatedInitializer]
		public SKVideoNode (NSUrl url)
		{
			if (CheckSystemVersion ())
				Handle = InitWithURL (url);
			else
				Handle = InitWithVideoURL (url);
		}
	}
}
#endif // !WATCH
