
#if !MONOMAC

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
using Registrar;
using UIKit;

namespace ObjCRuntime {

	public static partial class Runtime {
#if !COREBUILD
#if WATCH
		internal const string ProductName = "Xamarin.Watch";
#elif TVOS
		internal const string ProductName = "Xamarin.TVOS";
#elif IOS
		internal const string ProductName = "Xamarin.iOS";
#else
		#error Unknown platform
#endif
		internal const string CompatNamespace = "MonoTouch";
#if XAMCORE_2_0
#if WATCH
		internal const string AssemblyName = "Xamarin.Watch.dll";
#elif TVOS
		internal const string AssemblyName = "Xamarin.TVOS.dll";
#elif IOS
		internal const string AssemblyName = "Xamarin.iOS.dll";
#else
		#error Unknown platform
#endif
#else
		internal const string AssemblyName = "monotouch.dll";
#endif

		public static Arch Arch; // default: = Arch.DEVICE;

		unsafe static void InitializePlatform (InitializationOptions* options)
		{
			if (options->IsSimulator)
				Arch = Arch.SIMULATOR;

			UIApplication.Initialize ();
		}

		// This method is documented to be for diagnostic purposes only,
		// and should not be considered stable API.
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		static public List<WeakReference> GetSurfacedObjects ()
		{
			lock (lock_obj){
				var list = new List<WeakReference> (object_map.Count);

				foreach (var kv in object_map)
					list.Add (kv.Value);

				return list;
			}
		}
			
#if TVOS || WATCH
		[Advice ("This method is present only to help porting code.")]
		public static void StartWWAN (Uri uri, Action<Exception> callback)
		{
			NSRunLoop.Main.BeginInvokeOnMainThread (() => callback (null));
		}

		[Advice ("This method is present only to help porting code.")]
		public static void StartWWAN (Uri uri)
		{
		}
#else
		public static void StartWWAN (Uri uri, Action<Exception> callback)
		{
			DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => 
			{
				Exception ex = null;
				try {
					StartWWAN (uri);
				} catch (Exception x) {
					ex = x;
				}

				NSRunLoop.Main.BeginInvokeOnMainThread (() => callback (ex));
			});
		}

		[DllImport ("__Internal")]
		static extern void xamarin_start_wwan (string uri);

		public static void StartWWAN (Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException ("uri");

			if (uri.Scheme != "http" && uri.Scheme != "https")
				throw new ArgumentException ("uri is not a valid http or https uri", uri.ToString ());

			if (Runtime.Arch == Arch.SIMULATOR)
				return;

			xamarin_start_wwan (uri.ToString ());
		}
#endif // !TVOS && !WATCH
#endif // !COREBUILD
	}

	public enum Arch {
		DEVICE,
		SIMULATOR
	}
}

#endif // MONOMAC
