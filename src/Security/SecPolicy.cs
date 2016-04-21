// 
// SecPolicy.cs: Implements the managed SecPolicy wrapper.
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;
using XamCore.CoreFoundation;
using XamCore.Foundation;

namespace XamCore.Security {

	// untyped enum in Security.framework/Headers/SecPolicy.h but the API use CFOptionFlags
	// which is defined as in CFBase.h (do not trust Apple web documentation)
	[iOS (7,0)]
	[Flags]
	[Native]
	public enum SecRevocation : nuint_compat_int {
		None,
		OCSPMethod = 1,
		CRLMethod = 2,
		PreferCRL = 4,
		RequirePositiveResponse = 8,
		NetworkAccessDisabled = 16,
		UseAnyAvailableMethod = OCSPMethod | CRLMethod
	}

	public partial class SecPolicy {

		[iOS (7,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* CFDictionaryRef */ SecPolicyCopyProperties (IntPtr /* SecPolicyRef */ policyRef);

		[iOS (7,0)]
		public NSDictionary GetProperties ()
		{
			return new NSDictionary (SecPolicyCopyProperties (Handle), true);
		}

		[Mac (10,9)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* SecPolicyRef */ SecPolicyCreateRevocation (/* CFOptionFlags */ nuint revocationFlags);

		[Mac (10,9)][iOS (7,0)]
		static public SecPolicy CreateRevocationPolicy (SecRevocation revocationFlags)
		{
			return new SecPolicy (SecPolicyCreateRevocation ((nuint)(ulong) revocationFlags), true);
		}

		[Mac (10,9)][iOS (7,0)]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr /* SecPolicyRef */ SecPolicyCreateWithProperties (IntPtr /* CFTypeRef */ policyIdentifier, IntPtr /* CFDictionaryRef */ properties);

		[Mac (10,9)][iOS (7,0)]
		static public SecPolicy CreatePolicy (NSString policyIdentifier, NSDictionary properties)
		{
			if (policyIdentifier == null)
				throw new ArgumentNullException ("policyIdentifier");
			IntPtr dh = properties == null ? IntPtr.Zero : properties.Handle;

			// note: only accept known OIDs or return null (unit test will alert us if that change, FIXME in Apple code)
			// see: https://github.com/Apple-FOSS-Mirror/libsecurity_keychain/blob/master/lib/SecPolicy.cpp#L245
			IntPtr ph = SecPolicyCreateWithProperties (policyIdentifier.Handle, dh);
			if (ph == IntPtr.Zero)
				throw new ArgumentException ("Unknown policyIdentifier");
			return new SecPolicy (ph, true);
		}
	}
}