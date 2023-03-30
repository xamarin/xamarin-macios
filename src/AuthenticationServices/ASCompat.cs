//
// ASCompat.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2023 Microsoft Corporation
//
#if !XAMCORE_5_0

#nullable enable

using Foundation;
using ObjCRuntime;
using System;
using System.Threading.Tasks;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AuthenticationServices {
#if MONOMAC
	public partial class ASAuthorizationProviderExtensionRegistrationHandler {
		public override NativeHandle ClassHandle => throw new InvalidOperationException (Constants.BrokenBinding);
#if !NET
		public Task<ASAuthorizationProviderExtensionRegistrationResult> BeginDeviceRegistrationAsync (ASAuthorizationProviderExtensionLoginManager loginManager, ASAuthorizationProviderExtensionRequestOptions options) => throw new InvalidOperationException (Constants.BrokenBinding);
		public Task<ASAuthorizationProviderExtensionRegistrationResult> BeginUserRegistrationAsync (ASAuthorizationProviderExtensionLoginManager loginManager, string userName, ASAuthorizationProviderExtensionAuthenticationMethod authenticationMethod, ASAuthorizationProviderExtensionRequestOptions options) => throw new InvalidOperationException (Constants.BrokenBinding);
#else
		public virtual Task<ASAuthorizationProviderExtensionRegistrationResult> BeginDeviceRegistrationAsync (ASAuthorizationProviderExtensionLoginManager loginManager, ASAuthorizationProviderExtensionRequestOptions options) => throw new InvalidOperationException (Constants.BrokenBinding);
		public virtual Task<ASAuthorizationProviderExtensionRegistrationResult> BeginUserRegistrationAsync (ASAuthorizationProviderExtensionLoginManager loginManager, string userName, ASAuthorizationProviderExtensionAuthenticationMethod authenticationMethod, ASAuthorizationProviderExtensionRequestOptions options) => throw new InvalidOperationException (Constants.BrokenBinding);
#endif // !NET
	}

	public static partial class ASAuthorizationProviderExtensionRegistrationHandler_Extensions {
		public static Task<ASAuthorizationProviderExtensionRegistrationResult> BeginDeviceRegistrationAsync (this IASAuthorizationProviderExtensionRegistrationHandler This, ASAuthorizationProviderExtensionLoginManager loginManager, ASAuthorizationProviderExtensionRequestOptions options) => throw new InvalidOperationException (Constants.BrokenBinding);
		public static Task<ASAuthorizationProviderExtensionRegistrationResult> BeginUserRegistrationAsync (this IASAuthorizationProviderExtensionRegistrationHandler This, ASAuthorizationProviderExtensionLoginManager loginManager, string userName, ASAuthorizationProviderExtensionAuthenticationMethod authenticationMethod, ASAuthorizationProviderExtensionRequestOptions options) => throw new InvalidOperationException (Constants.BrokenBinding);
	}

#endif // MONOMAC
}
#endif // !XAMCORE_5_0
