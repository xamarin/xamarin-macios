//
// Copyright 2010, Novell, Inc.
// Copyright 2012 - 2013, Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if MONOMAC

using System;
using System.Reflection;

using ObjCRuntime;

namespace Foundation {
	public partial class NSObject {
#if !COREBUILD

		// note: the linker will remove the unrequired `dlopen` calls
		// Used to force the loading of AppKit and Foundation
		// ** DO NOT rename the fields as mmp's linker can remove them when not required **
		static IntPtr fl = Dlfcn.dlopen (Constants.FoundationLibrary, 1);
		static IntPtr al = Dlfcn.dlopen (Constants.AppKitLibrary, 1);
		static IntPtr ab = Dlfcn.dlopen (Constants.AddressBookLibrary, 1);
		static IntPtr ct = Dlfcn.dlopen (Constants.CoreTextLibrary, 1);
		static IntPtr wl = Dlfcn.dlopen (Constants.WebKitLibrary, 1);
		static IntPtr zl = Dlfcn.dlopen (Constants.QuartzLibrary, 1);
#if !NET
		static IntPtr ql = Dlfcn.dlopen (Constants.QTKitLibrary, 1);
#endif
		static IntPtr cl = Dlfcn.dlopen (Constants.CoreLocationLibrary, 1);
		static IntPtr ll = Dlfcn.dlopen (Constants.SecurityLibrary, 1);
		static IntPtr zc = Dlfcn.dlopen (Constants.QuartzComposerLibrary, 1);
		static IntPtr cw = Dlfcn.dlopen (Constants.CoreWlanLibrary, 1);
		static IntPtr pk = Dlfcn.dlopen (Constants.PdfKitLibrary, 1);
		static IntPtr ik = Dlfcn.dlopen (Constants.ImageKitLibrary, 1);
		static IntPtr sb = Dlfcn.dlopen (Constants.ScriptingBridgeLibrary, 1);
		static IntPtr av = Dlfcn.dlopen (Constants.AVFoundationLibrary, 1);
		static IntPtr js = Dlfcn.dlopen (Constants.JavaScriptCoreLibrary, 1);
		static IntPtr sk = Dlfcn.dlopen (Constants.SceneKitLibrary, 1);
		static IntPtr bl = Dlfcn.dlopen (Constants.CoreBluetoothLibrary, 1);
		static IntPtr st = Dlfcn.dlopen (Constants.StoreKitLibrary, 1);
		static IntPtr gk = Dlfcn.dlopen (Constants.GameKitLibrary, 1);
		static IntPtr ib = Dlfcn.dlopen (Constants.IOBluetoothLibrary, 1);
		static IntPtr bu = Dlfcn.dlopen (Constants.IOBluetoothUILibrary, 1);
		static IntPtr ca = Dlfcn.dlopen (Constants.CoreAudioKitLibrary, 1);
		static IntPtr ol = Dlfcn.dlopen (Constants.OpenALLibrary, 1);
		static IntPtr ma = Dlfcn.dlopen (Constants.MediaAccessibilityLibrary, 1);
		static IntPtr mi = Dlfcn.dlopen (Constants.CoreMidiLibrary, 1);
		static IntPtr ic = Dlfcn.dlopen (Constants.ImageCaptureCoreLibrary, 1);
		static IntPtr it = Dlfcn.dlopen (Constants.IntentsLibrary, 1);
		static IntPtr me = Dlfcn.dlopen (Constants.MediaLibraryLibrary, 1);
		static IntPtr gl = Dlfcn.dlopen (Constants.GLKitLibrary, 1);
		static IntPtr sp = Dlfcn.dlopen (Constants.SpriteKitLibrary, 1);
		static IntPtr ck = Dlfcn.dlopen (Constants.CloudKitLibrary, 1);
		static IntPtr la = Dlfcn.dlopen (Constants.LocalAuthenticationLibrary, 1);
		static IntPtr ac = Dlfcn.dlopen (Constants.AccountsLibrary, 1);
		// Contacts must come before MapKit to fix 33576 due to spam in MKContactsShim
		static IntPtr cn = Dlfcn.dlopen (Constants.ContactsLibrary, 1);
		static IntPtr cu = Dlfcn.dlopen (Constants.ContactsUILibrary, 1);
		static IntPtr mk = Dlfcn.dlopen (Constants.MapKitLibrary, 1);
		static IntPtr ek = Dlfcn.dlopen (Constants.EventKitLibrary, 1);
		static IntPtr so = Dlfcn.dlopen (Constants.SocialLibrary, 1);
		static IntPtr gc = Dlfcn.dlopen (Constants.GameControllerLibrary, 1);
		static IntPtr ak = Dlfcn.dlopen (Constants.AVKitLibrary, 1);
		static IntPtr vt = Dlfcn.dlopen (Constants.VideoToolboxLibrary, 1);
		static IntPtr gp = Dlfcn.dlopen (Constants.GameplayKitLibrary, 1);
		static IntPtr ne = Dlfcn.dlopen (Constants.NetworkExtensionLibrary, 1);
		static IntPtr mc = Dlfcn.dlopen (Constants.MultipeerConnectivityLibrary, 1);
		static IntPtr fs = Dlfcn.dlopen (Constants.FinderSyncLibrary, 1);
		static IntPtr ml = Dlfcn.dlopen (Constants.MetalKitLibrary, 1);
		static IntPtr io = Dlfcn.dlopen (Constants.ModelIOLibrary, 1);
		static IntPtr nc = Dlfcn.dlopen (Constants.NotificationCenterLibrary, 1);
		static IntPtr pl = Dlfcn.dlopen (Constants.PhotosLibrary, 1);
		static IntPtr pu = Dlfcn.dlopen (Constants.PhotosUILibrary, 1);
		static IntPtr mp = Dlfcn.dlopen (Constants.MediaPlayerLibrary, 1);
		static IntPtr pc = Dlfcn.dlopen (Constants.PrintCoreLibrary, 1);
		static IntPtr cml = Dlfcn.dlopen (Constants.CoreMLLibrary, 1);
		static IntPtr vn = Dlfcn.dlopen (Constants.VisionLibrary, 1);
		static IntPtr ios = Dlfcn.dlopen (Constants.IOSurfaceLibrary, 1);
		static IntPtr ex = Dlfcn.dlopen (Constants.ExternalAccessoryLibrary, 1);
		static IntPtr ms = Dlfcn.dlopen (Constants.MetalPerformanceShadersLibrary, 1);
		static IntPtr msg = Dlfcn.dlopen (Constants.MetalPerformanceShadersGraphLibrary, 1);
		static IntPtr bc = Dlfcn.dlopen (Constants.BusinessChatLibrary, 1);
		static IntPtr ad = Dlfcn.dlopen (Constants.AdSupportLibrary, 1);
		static IntPtr nl = Dlfcn.dlopen (Constants.NaturalLanguageLibrary, 1);
		static IntPtr vs = Dlfcn.dlopen (Constants.VideoSubscriberAccountLibrary, 1);
		static IntPtr un = Dlfcn.dlopen (Constants.UserNotificationsLibrary, 1);
		static IntPtr il  = Dlfcn.dlopen (Constants.iTunesLibraryLibrary, 1);
		static IntPtr exl = Dlfcn.dlopen (Constants.ExtensionKitLibrary, 1);
		static IntPtr sw = Dlfcn.dlopen (Constants.SharedWithYouLibrary, 1);
		static IntPtr swc = Dlfcn.dlopen (Constants.SharedWithYouCoreLibrary, 1);
		static IntPtr th = Dlfcn.dlopen (Constants.ThreadNetworkLibrary, 1);
		static IntPtr ni = Dlfcn.dlopen (Constants.NearbyInteractionLibrary, 1);
		static IntPtr sm = Dlfcn.dlopen (Constants.ServiceManagementLibrary, 1);
		static IntPtr sa = Dlfcn.dlopen (Constants.SafetyKitLibrary, 1);

#if !NET
		[Obsolete ("Use PlatformAssembly for easier code sharing across platforms.")]
		public static readonly Assembly MonoMacAssembly = typeof (NSObject).Assembly;
#endif
#endif // !COREBUILD
	}
}

#endif // MONOMAC
