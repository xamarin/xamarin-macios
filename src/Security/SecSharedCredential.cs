#if IOS

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace Security {

	public static partial class SecSharedCredential {

		[iOS (8,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static void SecAddSharedWebCredential (IntPtr /* CFStringRef */ fqdn, IntPtr /* CFStringRef */ account, IntPtr /* CFStringRef */ password,
			IntPtr /* void (^completionHandler)( CFErrorRef error) ) */ completionHandler);
			
		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		internal delegate void DActionArity1V12 (IntPtr block, IntPtr obj);
		
		// This class bridges native block invocations that call into C#
		static internal class ActionTrampoline {
			static internal readonly DActionArity1V12 Handler = Invoke;
			
			[MonoPInvokeCallback (typeof (DActionArity1V12))]
			static unsafe void Invoke (IntPtr block, IntPtr obj) {
				var descriptor = (BlockLiteral *) block;
				var del = (global::System.Action<NSError>) (descriptor->Target);
				if (del != null) {
					del ( Runtime.GetNSObject<NSError> (obj));
				}
			} 
		} 

		[iOS (8,0)]
		public static void AddSharedWebCredential (string domainName, string account, string password, Action<NSError> handler)
		{
			if (domainName == null)
				throw new ArgumentNullException ("domainName");
			if (account == null)
				throw new ArgumentNullException ("account");
			// we need to create our own block literal. We can reuse the SDActionArity1V12 which is generated and takes a
			// NSError because a CFError is a toll-free bridget to CFError
			unsafe {
				BlockLiteral *block_ptr_onComplete;
				BlockLiteral block_onComplete;
				block_onComplete = new BlockLiteral ();
				block_ptr_onComplete = &block_onComplete;
				block_onComplete.SetupBlockUnsafe (ActionTrampoline.Handler, handler);

				using (var nsDomain = new NSString (domainName))
				using (var nsAccount = new NSString (account)) {
					if (password == null) {  // we are removing a password
						SecAddSharedWebCredential (nsDomain.Handle, nsAccount.Handle, IntPtr.Zero, (IntPtr) block_ptr_onComplete);
					} else {
						using (var nsPassword = new NSString (password)) {
							SecAddSharedWebCredential (nsDomain.Handle, nsAccount.Handle, nsPassword.Handle, (IntPtr) block_ptr_onComplete);
						}
					}
					block_ptr_onComplete->CleanupBlock ();
				}
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static void SecRequestSharedWebCredential ( IntPtr /* CFStringRef */ fqdn, IntPtr /* CFStringRef */ account,
			IntPtr /* void (^completionHandler)( CFArrayRef credentials, CFErrorRef error) */ completionHandler);

		[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]
		internal delegate void ArrayErrorAction (IntPtr block, IntPtr array, IntPtr err);

		//
		// This class bridges native block invocations that call into C# because we cannot use the decorator, we have to create
		// it for our use here.
		//
		static internal class ArrayErrorActionTrampoline {
			static internal readonly ArrayErrorAction Handler = Invoke;

			[MonoPInvokeCallback (typeof (ArrayErrorAction))]
			static unsafe void Invoke (IntPtr block, IntPtr array, IntPtr err) {
				var descriptor = (BlockLiteral *) block;
				var del = (global::System.Action<NSArray, NSError>) (descriptor->Target);
				if (del != null)
					del ( Runtime.GetNSObject<NSArray> (array), Runtime.GetNSObject<NSError> (err));
			}
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the overload accepting a 'SecSharedCredentialInfo' argument.")]
		public static void RequestSharedWebCredential (string domainName, string account, Action<string[], NSError> handler)
		{
			throw new InvalidOperationException ("Use correct delegate type");
		}
#endif

		[iOS (8,0)]
		public static void RequestSharedWebCredential (string domainName, string account, Action<SecSharedCredentialInfo[], NSError> handler)
		{
			Action<NSArray, NSError> onComplete = (NSArray a, NSError e) => {
				var creds = new SecSharedCredentialInfo [a.Count];
				int i = 0;
				foreach (var dict in NSArray.FromArrayNative<NSDictionary> (a)) {
					creds [i++] = new SecSharedCredentialInfo (dict);
				}
				handler (creds, e);
			};
			// we need to create our own block literal.
			unsafe {
				BlockLiteral *block_ptr_onComplete;
				BlockLiteral block_onComplete;
				block_onComplete = new BlockLiteral ();
				block_ptr_onComplete = &block_onComplete;
				block_onComplete.SetupBlockUnsafe (ArrayErrorActionTrampoline.Handler, onComplete);

				NSString nsDomain = null;
				if (domainName != null)
					nsDomain = new NSString (domainName);	

				NSString nsAccount = null;
				if (account != null) 
					nsAccount = new NSString (account);
				
				SecRequestSharedWebCredential ((nsDomain == null)? IntPtr.Zero : nsDomain.Handle, (nsAccount == null)? IntPtr.Zero : nsAccount.Handle,
					(IntPtr) block_ptr_onComplete); 
				block_ptr_onComplete->CleanupBlock ();
				if (nsDomain != null)
					nsDomain.Dispose ();
				if (nsAccount != null)
					nsAccount.Dispose ();
			}
		}

		[iOS (8,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* CFStringRef */ SecCreateSharedWebCredentialPassword ();

		[iOS (8,0)]
		public static string CreateSharedWebCredentialPassword ()
		{
			var handle = SecCreateSharedWebCredentialPassword ();
			var str = NSString.FromHandle (handle);
			NSObject.DangerousRelease (handle);
			return str;
		}
	}

}

#endif  // IOS
