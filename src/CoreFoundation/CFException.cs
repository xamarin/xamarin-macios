// 
// CFException.cs: Convert CFError into an CFException
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
// Copyright 2012 Xamarin Inc.
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

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace CoreFoundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class CFErrorDomain {

		public static readonly NSString? Cocoa;
		public static readonly NSString? Mach;
		public static readonly NSString? OSStatus;
		public static readonly NSString? Posix;

		static CFErrorDomain ()
		{
			var handle = Libraries.CoreFoundation.Handle;
			Cocoa = Dlfcn.GetStringConstant (handle, "kCFErrorDomainCocoa");
			Mach = Dlfcn.GetStringConstant (handle, "kCFErrorDomainMach");
			OSStatus = Dlfcn.GetStringConstant (handle, "kCFErrorDomainOSStatus");
			Posix = Dlfcn.GetStringConstant (handle, "kCFErrorDomainPosix");
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class CFExceptionDataKey {

		public static readonly NSString? Description;
		public static readonly NSString? LocalizedDescription;
		public static readonly NSString? LocalizedFailureReason;
		public static readonly NSString? LocalizedRecoverySuggestion;
		public static readonly NSString? UnderlyingError;

		static CFExceptionDataKey ()
		{
			var handle = Libraries.CoreFoundation.Handle;
			Description = Dlfcn.GetStringConstant (handle, "kCFErrorDescriptionKey");
			LocalizedDescription = Dlfcn.GetStringConstant (handle, "kCFErrorLocalizedDescriptionKey");
			LocalizedFailureReason = Dlfcn.GetStringConstant (handle, "kCFErrorLocalizedFailureReasonKey");
			LocalizedRecoverySuggestion = Dlfcn.GetStringConstant (handle, "kCFErrorLocalizedRecoverySuggestionKey");
			UnderlyingError = Dlfcn.GetStringConstant (handle, "kCFErrorUnderlyingErrorKey");
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFException : Exception {

		public CFException (string? description, NSString? domain, nint code, string? failureReason, string? recoverySuggestion)
			: base (description)
		{
			Code = code;
			Domain = domain;
			FailureReason = failureReason;
			RecoverySuggestion = recoverySuggestion;
		}

		public static CFException FromCFError (IntPtr cfErrorHandle)
		{
			return FromCFError (cfErrorHandle, true);
		}

		public static CFException FromCFError (IntPtr cfErrorHandle, bool release)
		{
			if (cfErrorHandle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (cfErrorHandle));

			var e = new CFException (
					CFString.FromHandle (CFErrorCopyDescription (cfErrorHandle), releaseHandle: true),
					Runtime.GetNSObject (CFErrorGetDomain (cfErrorHandle)) as NSString,
					CFErrorGetCode (cfErrorHandle),
					CFString.FromHandle (CFErrorCopyFailureReason (cfErrorHandle), releaseHandle: true),
					CFString.FromHandle (CFErrorCopyRecoverySuggestion (cfErrorHandle), releaseHandle: true));

			var cfUserInfo = CFErrorCopyUserInfo (cfErrorHandle);
			if (cfUserInfo != IntPtr.Zero) {
				using (var userInfo = new NSDictionary (cfUserInfo)) {
					foreach (var i in userInfo) {
						if (i.Key is not null)
							e.Data.Add (i.Key.ToString (), i.Value?.ToString ());
					}
				}
			}
			if (release)
				CFObject.CFRelease (cfErrorHandle);
			return e;
		}

		public nint Code { get; private set; }
		public NSString? Domain { get; private set; }
		public string? FailureReason { get; private set; }
		public string? RecoverySuggestion { get; private set; }

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFErrorCopyDescription (IntPtr err);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFErrorCopyFailureReason (IntPtr err);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFErrorCopyRecoverySuggestion (IntPtr err);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFErrorCopyUserInfo (IntPtr err);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern nint CFErrorGetCode (IntPtr err);

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern IntPtr CFErrorGetDomain (IntPtr err);
	}
}
