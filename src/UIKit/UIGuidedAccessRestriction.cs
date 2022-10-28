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

namespace UIKit {

	public static partial class UIGuidedAccessRestriction {
#if !COREBUILD
#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7,0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		extern static /* UIGuidedAccessRestrictionState */ nint UIGuidedAccessRestrictionStateForIdentifier (/* NSString */ IntPtr restrictionIdentifier);

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7,0)]
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
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (12,2)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		static extern void UIGuidedAccessConfigureAccessibilityFeatures (/* UIGuidedAccessAccessibilityFeature */ nuint features, [MarshalAs (UnmanagedType.I1)] bool enabled, IntPtr completion);

#if NET
		// [SupportedOSPlatform ("ios12.2")] -- Not valid for Delegates
		// [SupportedOSPlatform ("maccatalyst")]
		// [SupportedOSPlatform ("tvos")]
#else
		[iOS (12,2)]
#endif
		public delegate void UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler (bool success, NSError error);

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		internal delegate void DUIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler (IntPtr block, bool success, IntPtr error);

		static internal class UIGuidedAccessConfigureAccessibilityFeaturesTrampoline {
			static internal readonly DUIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler Handler = Invoke;

			[MonoPInvokeCallback (typeof (DUIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler))]
			static unsafe void Invoke (IntPtr block, bool success, IntPtr error)
			{
				var descriptor = (BlockLiteral*) block;
				var del = (UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler) (descriptor->Target);
				if (del != null)
					del (success, Runtime.GetNSObject<NSError> (error));
			}
		}

#if NET
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (12,2)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void ConfigureAccessibilityFeatures (UIGuidedAccessAccessibilityFeature features, bool enabled, UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler completionHandler)
		{
			if (completionHandler == null)
				throw new ArgumentNullException (nameof (completionHandler));

			unsafe {
				BlockLiteral* block_ptr_completionHandler;
				BlockLiteral block_completionHandler;
				block_completionHandler = new BlockLiteral ();
				block_ptr_completionHandler = &block_completionHandler;
				block_completionHandler.SetupBlockUnsafe (UIGuidedAccessConfigureAccessibilityFeaturesTrampoline.Handler, completionHandler);

				UIGuidedAccessConfigureAccessibilityFeatures ((nuint) (ulong) features, enabled, (IntPtr) block_ptr_completionHandler);
				block_ptr_completionHandler->CleanupBlock ();
			}
		}

#if NET
		[SupportedOSPlatform ("ios12.2")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (12,2)]
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
