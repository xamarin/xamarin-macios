//
// UIGuidedAccessRestriction
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyrigh 2013-2014 Xamarin Inc.
//

#if !WATCH

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using UIKit;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

	public static partial class UIGuidedAccessRestriction {
#if !COREBUILD
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static /* UIGuidedAccessRestrictionState */ nint UIGuidedAccessRestrictionStateForIdentifier (/* NSString */ IntPtr restrictionIdentifier);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIGuidedAccessRestrictionState GetState (string restrictionIdentifier)
		{
			IntPtr p = NSString.CreateNative (restrictionIdentifier);
			var result = UIGuidedAccessRestrictionStateForIdentifier (p);
			NSString.ReleaseNative (p);
			return (UIGuidedAccessRestrictionState) (int) result;
		}

#if IOS
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.UIKitLibrary)]
		static unsafe extern void UIGuidedAccessConfigureAccessibilityFeatures (/* UIGuidedAccessAccessibilityFeature */ nuint features, byte enabled, BlockLiteral* completion);

#if NET
		// [SupportedOSPlatform ("ios")] -- Not valid for Delegates
		// [SupportedOSPlatform ("maccatalyst")]
		// [SupportedOSPlatform ("tvos")]
#endif
		public delegate void UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler (bool success, NSError error);

#if !NET
		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		internal delegate void DUIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler (IntPtr block, byte success, IntPtr error);
#endif

		static internal class UIGuidedAccessConfigureAccessibilityFeaturesTrampoline {
#if !NET
			static internal readonly DUIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler Handler = Invoke;
#endif

#if NET
			[UnmanagedCallersOnlyAttribute]
#else
			[MonoPInvokeCallback (typeof (DUIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler))]
#endif
			internal static unsafe void Invoke (IntPtr block, byte success, IntPtr error)
			{
				var descriptor = (BlockLiteral*) block;
				var del = (UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler) (descriptor->Target);
				if (del is not null)
					del (success != 0, Runtime.GetNSObject<NSError> (error));
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void ConfigureAccessibilityFeatures (UIGuidedAccessAccessibilityFeature features, bool enabled, UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler completionHandler)
		{
			if (completionHandler is null)
				throw new ArgumentNullException (nameof (completionHandler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, byte, IntPtr, void> trampoline = &UIGuidedAccessConfigureAccessibilityFeaturesTrampoline.Invoke;
				using var block = new BlockLiteral (trampoline, completionHandler, typeof (UIGuidedAccessConfigureAccessibilityFeaturesTrampoline), nameof (UIGuidedAccessConfigureAccessibilityFeaturesTrampoline.Invoke));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (UIGuidedAccessConfigureAccessibilityFeaturesTrampoline.Handler, completionHandler);
#endif
				UIGuidedAccessConfigureAccessibilityFeatures ((nuint) (ulong) features, enabled ? (byte) 1 : (byte) 0, &block);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static Task<(bool Success, NSError Error)> ConfigureAccessibilityFeaturesAsync (UIGuidedAccessAccessibilityFeature features, bool enabled)
		{
			var tcs = new TaskCompletionSource<(bool, NSError)> ();
			ConfigureAccessibilityFeatures (features, enabled, (success_, error_) => tcs.SetResult ((success_, error_)));
			return tcs.Task;
		}
#endif
#endif // !COREBUILD
	}
}

#endif // !WATCH
