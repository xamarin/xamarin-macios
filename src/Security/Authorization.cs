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

#if MONOMAC

using ObjCRuntime;
using Foundation;
using System;
using System.Runtime.InteropServices;

namespace Security {
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

	// typedef UInt32 AuthorizationFlags;
	[Flags]
	public enum AuthorizationFlags : int {
		Defaults,
		InteractionAllowed = 1 << 0,
		ExtendRights = 1 << 1,
		PartialRights = 1 << 2,
		DestroyRights = 1 << 3,
		PreAuthorize = 1 << 4,
	}

	//
	// For ease of use, we let the user pass the AuthorizationParameters, and we
	// create the structure for them with the proper data
	//
	public class AuthorizationParameters {
		public string PathToSystemPrivilegeTool;
		public string Prompt;
		public string IconPath;
	}

	public class AuthorizationEnvironment {
		public string Username;
		public string Password;
		public bool   AddToSharedCredentialPool;
	}

	[StructLayout (LayoutKind.Sequential)]
	struct AuthorizationItem {
		public IntPtr /* AuthorizationString = const char * */ name;
#if XAMCORE_2_0
		public nint /* size_t */ valueLen;
#else
		public IntPtr /* size_t */ valueLen;
#endif
		public IntPtr /* void* */ value;
		public int /* UInt32 */ flags;  // zero
	}

	unsafe struct AuthorizationItemSet {
		public int /* UInt32 */ count;
		public AuthorizationItem * /* AuthorizationItem* */ ptrToAuthorization;
	}

	public unsafe class Authorization : INativeObject, IDisposable {
		IntPtr handle;

		public IntPtr Handle { get { return handle; } }
		
		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationCreate (AuthorizationItemSet *rights, AuthorizationItemSet *environment, AuthorizationFlags flags, out IntPtr auth);

		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationExecuteWithPrivileges (IntPtr handle, string pathToTool, AuthorizationFlags flags, string [] args, IntPtr FILEPtr);

		[DllImport (Constants.SecurityLibrary)]
		extern static int /* OSStatus = int */ AuthorizationFree (IntPtr handle, AuthorizationFlags flags);
		
		internal Authorization (IntPtr handle)
		{
			this.handle = handle;
		}

		public int ExecuteWithPrivileges (string pathToTool, AuthorizationFlags flags, string [] args)
		{
			return AuthorizationExecuteWithPrivileges (handle, pathToTool, flags, args, IntPtr.Zero);
		}

		public void Dispose ()
		{
			GC.SuppressFinalize (this);
			Dispose (0, true);
		}

		~Authorization ()
		{
			Dispose (0, false);
		}
		
		public virtual void Dispose (AuthorizationFlags flags, bool disposing)
		{
			if (handle != IntPtr.Zero){
				AuthorizationFree (handle, flags);
				handle = IntPtr.Zero;
			}
		}
		
		public static Authorization Create (AuthorizationFlags flags)
		{
			return Create (null, null, flags);
		}
		
		static void EncodeString (ref AuthorizationItem item, string key, string value)
		{
			item.name = Marshal.StringToHGlobalAuto (key);
			if (value != null){
				item.value = Marshal.StringToHGlobalAuto (value);
#if XAMCORE_2_0
				item.valueLen = value.Length;
#else
				item.valueLen = (IntPtr) value.Length;
#endif
			}
		}
		
		public static Authorization Create (AuthorizationParameters parameters, AuthorizationEnvironment environment, AuthorizationFlags flags)
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
					if (parameters != null){
						ppars = &pars;
						pars.ptrToAuthorization = (AuthorizationItem *) Marshal.AllocHGlobal (sizeof (AuthorizationItem) * 3);
						if (parameters.PathToSystemPrivilegeTool != null)
							EncodeString (ref pars.ptrToAuthorization [pars.count++], "system.privilege.admin", parameters.PathToSystemPrivilegeTool);
						if (parameters.Prompt != null)
							EncodeString (ref pars.ptrToAuthorization [pars.count++], "prompt", parameters.Prompt);
						if (parameters.IconPath != null)
							EncodeString (ref pars.ptrToAuthorization [pars.count++], "prompt", parameters.IconPath);
					}
					if (environment != null){
						penv = &env;
						env.ptrToAuthorization = (AuthorizationItem *) Marshal.AllocHGlobal (sizeof (AuthorizationItem) * 3);
						if (environment.Username != null)
							EncodeString (ref env.ptrToAuthorization [env.count++], "username", environment.Username);
						if (environment.Password != null)
							EncodeString (ref env.ptrToAuthorization [env.count++], "password", environment.Password);
						if (environment.AddToSharedCredentialPool)
							EncodeString (ref env.ptrToAuthorization [env.count++], "shared", null);
					}
					code = AuthorizationCreate (ppars, penv, flags, out auth);
					if (code != 0)
						return null;
					return new Authorization (auth);
				}
			} finally {
				if (ppars != null){
					for (int i = 0; i < pars.count; i++){
						Marshal.FreeHGlobal (pars.ptrToAuthorization [i].name);
						Marshal.FreeHGlobal (pars.ptrToAuthorization [i].value);
					}
					Marshal.FreeHGlobal ((IntPtr)pars.ptrToAuthorization);
				}
				if (penv != null){
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
