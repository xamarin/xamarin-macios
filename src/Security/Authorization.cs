// 
// Authorization.cs: 
//
// Authors: Miguel de Icaza
//     
// Copyright 2012 Xamarin Inc
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

#if MONOMAC || __MACCATALYST__

using ObjCRuntime;
using Foundation;
using System;
using System.Runtime.InteropServices;
#if NET
#else
using NativeHandle = System.IntPtr;
#endif

namespace Security {

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	// Untyped enum in ObjC
	public enum AuthorizationStatus {
		Success                 = 0,
		InvalidSet              = -60001,
		InvalidRef              = -60002,
		InvalidTag              = -60003,
		InvalidPointer          = -60004,
		Denied                  = -60005,
		Canceled                = -60006,
		InteractionNotAllowed   = -60007,
		Internal                = -60008,
		ExternalizeNotAllowed   = -60009,
		InternalizeNotAllowed   = -60010,
		InvalidFlags            = -60011,
		ToolExecuteFailure      = -60031,
		ToolEnvironmentError    = -60032,
		BadAddress              = -60033,
	}

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	// typedef UInt32 AuthorizationFlags;
	[Flags]
	public enum AuthorizationFlags : int {
		Defaults,
		InteractionAllowed = 1 << 0,
		ExtendRights = 1 << 1,
		PartialRights = 1 << 2,
		DestroyRights = 1 << 3,
		PreAuthorize = 1 << 4,
#if NET
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[Mac(14, 0), MacCatalyst(17, 0)]
#endif
		SkipInternalAuth = 1 << 9,
		NoData = 1 << 20,
	}

	//
	// For ease of use, we let the user pass the AuthorizationParameters, and we
	// create the structure for them with the proper data
	//
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	public class AuthorizationParameters {
		public string? PathToSystemPrivilegeTool;
		public string? Prompt;
		public string? IconPath;
	}

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	public class AuthorizationEnvironment {
		public string? Username;
		public string? Password;
		public bool   AddToSharedCredentialPool;
	}

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	struct AuthorizationItem {
		public IntPtr /* AuthorizationString = const char * */ name;
		public nint /* size_t */ valueLen;
		public IntPtr /* void* */ value;
		public int /* UInt32 */ flags;  // zero
	}

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	unsafe struct AuthorizationItemSet {
		public int /* UInt32 */ count;
		public AuthorizationItem * /* AuthorizationItem* */ ptrToAuthorization;
	}

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (15,0)]
#endif
	public unsafe class Authorization : DisposableObject {
		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationCreate (AuthorizationItemSet *rights, AuthorizationItemSet *environment, AuthorizationFlags flags, out IntPtr auth);

#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.7", "Use the Service Management framework or the launchd-launched helper tool instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10,7)]
#endif
#if NET
		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationExecuteWithPrivileges (IntPtr handle, IntPtr pathToTool, AuthorizationFlags flags, IntPtr args, IntPtr FILEPtr);
#else
		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationExecuteWithPrivileges (IntPtr handle, string pathToTool, AuthorizationFlags flags, string? []? args, IntPtr FILEPtr);
#endif

		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationFree (IntPtr handle, AuthorizationFlags flags);
		
		[Preserve (Conditional = true)]
		internal Authorization (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos10.7", "Use the Service Management framework or the launchd-launched helper tool instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10,7)]
#endif
		public int ExecuteWithPrivileges (string pathToTool, AuthorizationFlags flags, string []? args)
		{
			string?[]? arguments = args!;

			if (arguments is not null) {
				// The arguments array must be null-terminated, so make sure that's the case
				if (arguments.Length == 0) {
					arguments = new string? [] { null };
				} else if (arguments [arguments.Length - 1] is not null) {
					var array = new string? [arguments.Length + 1];
					arguments.CopyTo (array, 0);
					arguments = array;
				}
			}
#if NET
			using var pathToToolStr = new TransientString (pathToTool);
			var argsPtr = TransientString.AllocStringArray (arguments);
			var retval = AuthorizationExecuteWithPrivileges (Handle, pathToToolStr, flags, argsPtr, IntPtr.Zero);
			TransientString.FreeStringArray (argsPtr, args is null ? 0 : args.Length);
			return retval;
#else
			return AuthorizationExecuteWithPrivileges (Handle, pathToTool, flags, arguments, IntPtr.Zero);
#endif
		}

		protected override void Dispose (bool disposing)
		{
			Dispose (0, disposing);
		}
		
		public virtual void Dispose (AuthorizationFlags flags, bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns)
				AuthorizationFree (Handle, flags);
			base.Dispose (disposing);
		}
		
		public static Authorization? Create (AuthorizationFlags flags)
		{
			return Create (null, null, flags);
		}
		
		static void EncodeString (ref AuthorizationItem item, string key, string? value)
		{
			item.name = Marshal.StringToHGlobalAuto (key);
			if (value is not null) {
				item.value = Marshal.StringToHGlobalAuto (value);
				item.valueLen = value.Length;
			}
		}
		
		public static Authorization? Create (AuthorizationParameters? parameters, AuthorizationEnvironment? environment, AuthorizationFlags flags)
		{
			AuthorizationItemSet pars = new AuthorizationItemSet ();
			AuthorizationItemSet *ppars = null;
			AuthorizationItem *pitems = null;
			AuthorizationItemSet env = new AuthorizationItemSet ();
			AuthorizationItemSet *penv = null;
			AuthorizationItem *eitems = null;
			int code;
			IntPtr auth;

			try {
				unsafe {
					if (parameters is not null){
						ppars = &pars;
						pars.ptrToAuthorization = (AuthorizationItem *) Marshal.AllocHGlobal (sizeof (AuthorizationItem) * 2);
						if (parameters.PathToSystemPrivilegeTool is not null)
							EncodeString (ref pars.ptrToAuthorization [pars.count++], "system.privilege.admin", parameters.PathToSystemPrivilegeTool);
						if (parameters.IconPath is not null)
							EncodeString (ref pars.ptrToAuthorization [pars.count++], "icon", parameters.IconPath);
					}
					if (environment is not null || (parameters is not null && parameters.Prompt is not null)){
						penv = &env;
						env.ptrToAuthorization = (AuthorizationItem *) Marshal.AllocHGlobal (sizeof (AuthorizationItem) * 4);
						if (environment is not null){
							if (environment.Username is not null)
								EncodeString (ref env.ptrToAuthorization [env.count++], "username", environment.Username);
							if (environment.Password is not null)
								EncodeString (ref env.ptrToAuthorization [env.count++], "password", environment.Password);
							if (environment.AddToSharedCredentialPool)
								EncodeString (ref env.ptrToAuthorization [env.count++], "shared", null);
						}
						if (parameters is not null){
							if (parameters.Prompt is not null)
								EncodeString (ref env.ptrToAuthorization [env.count++], "prompt", parameters.Prompt);
						}
					}
					code = AuthorizationCreate (ppars, penv, flags, out auth);
					if (code != 0)
						return null;
					return new Authorization (auth, true);
				}
			} finally {
				if (ppars is not null){
					for (int i = 0; i < pars.count; i++){
						Marshal.FreeHGlobal (pars.ptrToAuthorization [i].name);
						Marshal.FreeHGlobal (pars.ptrToAuthorization [i].value);
					}
					Marshal.FreeHGlobal ((IntPtr)pars.ptrToAuthorization);
				}
				if (penv is not null){
					for (int i = 0; i < env.count; i++){
						Marshal.FreeHGlobal (env.ptrToAuthorization [i].name);
						if (env.ptrToAuthorization [i].value != IntPtr.Zero)
							Marshal.FreeHGlobal (env.ptrToAuthorization [i].value);
					}
					Marshal.FreeHGlobal ((IntPtr)env.ptrToAuthorization);
				}
			}
		}
	}
}

#endif // MONOMAC
