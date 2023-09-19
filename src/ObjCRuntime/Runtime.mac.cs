//
// Copyright 2010, Novell, Inc.
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

#nullable enable

#if MONOMAC

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using AppKit;
using Foundation;
using Registrar;

namespace ObjCRuntime {

	public static partial class Runtime {
#if !COREBUILD
#if NET
		internal const string ProductName = "Microsoft.macOS";
		internal const string AssemblyName = "Microsoft.macOS.dll";
#else
		internal const string ProductName = "Xamarin.Mac";
		internal const string AssemblyName = "Xamarin.Mac.dll";
#endif

		public static string? FrameworksPath {
			get; set;
		}

		public static string? ResourcesPath {
			get; set;
		}
			
		delegate void initialize_func ();
		unsafe delegate sbyte *get_sbyteptr_func ();

#if !NET // There's a different implementation for other platforms + .NET macOS in Runtime.cs
		static volatile bool originalWorkingDirectoryIsSet;
		static string? originalWorkingDirectory;

		public unsafe static string? OriginalWorkingDirectory {
			get {
				if (originalWorkingDirectoryIsSet)
					return originalWorkingDirectory;

				originalWorkingDirectoryIsSet = true;

				var pathPtr = LookupInternalFunction<get_sbyteptr_func> (
						"xamarin_get_original_working_directory_path") ();
				if (pathPtr == (sbyte *)0 || *pathPtr == 0)
					return null;

				return originalWorkingDirectory = new string (pathPtr);
			}
		}

		public static void ChangeToOriginalWorkingDirectory ()
		{
			Directory.SetCurrentDirectory (OriginalWorkingDirectory!);
		}
#endif // !NET

#if NET
		[DllImport ("__Internal")]
		extern static void xamarin_initialize ();
#else
		static IntPtr runtime_library;

		internal static T LookupInternalFunction<T> (string name) where T: class
		{
			IntPtr rv;

			if (name is null)
				throw new ArgumentNullException (nameof (name));

			if (runtime_library == IntPtr.Zero) {
				runtime_library = new IntPtr (-2 /* RTLD_DEFAULT */);
				rv = Dlfcn.dlsym (runtime_library, name);
				if (rv == IntPtr.Zero) {
					runtime_library = Dlfcn.dlopen ("libxammac.dylib", 0, false);
					if (runtime_library == IntPtr.Zero)
						runtime_library = Dlfcn.dlopen (Path.Combine (Path.GetDirectoryName (typeof (NSApplication).Assembly.Location)!, "libxammac.dylib"), 0, false);
					if (runtime_library == IntPtr.Zero)
						throw new DllNotFoundException ("Could not find the runtime library libxammac.dylib");
					rv = Dlfcn.dlsym (runtime_library, name);
				}
			} else {
				rv = Dlfcn.dlsym (runtime_library, name);
			}
			if (rv == IntPtr.Zero)
				throw new EntryPointNotFoundException (string.Format ("Could not find the runtime method '{0}'", name));
			return (T) (object) Marshal.GetDelegateForFunctionPointer (rv, typeof (T));
		}
#endif

		internal static void EnsureInitialized ()
		{
			if (initialized)
				return;

#if !NET
			if (GC.MaxGeneration <= 0)
				throw ErrorHelper.CreateError (8017, "The Boehm garbage collector is not supported. Please use SGen instead.");

			VerifyMonoVersion ();
#endif

#if NET
			xamarin_initialize ();
#else
			LookupInternalFunction<initialize_func> ("xamarin_initialize") ();
#endif
		}

#if !NET
		static void VerifyMonoVersion ()
		{
			// Verify that the system mono we're running against is of a supported version.
			// Only verify if we're able to get the mono version (we don't want to fail if the Mono.Runtime type was linked away for instance).
			var type = Type.GetType ("Mono.Runtime");
			if (type is null)
				return;

			var displayName = type.GetMethod ("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
			if (displayName is null)
				return;

			var actual = displayName.Invoke (null, null) as string;
			if (string.IsNullOrEmpty (actual))
				return;
			// The version string looks something like this:
			// "5.16.0.209 (2018-06/709b46e3338 Wed Oct 31 09:14:07 EDT 2018)"
			// We only want the first part up until the first space.
			var spaceIndex = actual!.IndexOf (' ');
			if (spaceIndex > 0)
				actual = actual.Substring (0, spaceIndex);
			if (!Version.TryParse (actual, out var actual_version))
				return;

			if (!Version.TryParse (Constants.MinMonoVersion, out var required_version))
				return;

			if (required_version <= actual_version)
				return;

			throw new NotSupportedException ($"This version of Xamarin.Mac requires Mono {required_version}, but found Mono {actual_version}.");
		}
#endif

		unsafe static void InitializePlatform (InitializationOptions* options)
		{
#if !NET
			// BaseDirectory may not be set in some Mono embedded environments
			// so try some reasonable fallbacks in these cases.
#endif
			string basePath = AppDomain.CurrentDomain.BaseDirectory;
#if !NET
			if(!string.IsNullOrEmpty(basePath))
				basePath = Path.Combine (basePath, "..");
			else {
				basePath = Assembly.GetExecutingAssembly().Location;
				if(!string.IsNullOrEmpty(basePath)) {
					basePath = Path.Combine (Path.GetDirectoryName(basePath)!, "..");
				}
				else {
					// The executing assembly location may be null if loaded from
					// memory so the final fallback is the current directory
					basePath = Path.Combine (Environment.CurrentDirectory, "..");
				}
			}
#endif

			ResourcesPath = Path.Combine (basePath, "Resources");
			FrameworksPath = Path.Combine (basePath, "Frameworks");
		}

		[Preserve]
		static IntPtr GetNullableType (IntPtr type)
		{
			return AllocGCHandle (Registrar.GetNullableType ((Type) GetGCHandleTarget (type)!));
		}
#endif // !COREBUILD
	}
}

#endif // MONOMAC
