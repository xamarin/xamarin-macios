// Copyright 2014 Xamarin, Inc.
#if IOS || TVOS
using System;
using System.Runtime.InteropServices;
using Foundation;
#if HAS_NOTIFICATIONCENTER
using NotificationCenter;
#endif
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

	public partial class UIVibrancyEffect {

#if HAS_NOTIFICATIONCENTER
		// This code comes from NotificationCenter
		// This is a [Category] -> C# extension method (see adlib.cs) but it targets on static selector
		// the resulting syntax does not look good in user code so we provide a better looking API
		// https://trello.com/c/iQpXOxCd/227-category-and-static-methods-selectors
		// note: we cannot reuse the same method name - as it would break compilation of existing apps
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios10.0", "Use 'CreatePrimaryVibrancyEffectForNotificationCenter' instead.")]
#else
		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CreatePrimaryVibrancyEffectForNotificationCenter' instead.")]
#endif
		static public UIVibrancyEffect CreateForNotificationCenter ()
		{
			return (null as UIVibrancyEffect).NotificationCenterVibrancyEffect ();
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'UIVibrancyEffect.CreateWidgetEffectForNotificationCenter' instead.")]
#else
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'UIVibrancyEffect.CreateWidgetEffectForNotificationCenter' instead.")]
#endif
		static public UIVibrancyEffect CreatePrimaryVibrancyEffectForNotificationCenter ()
		{
			return (null as UIVibrancyEffect).GetWidgetPrimaryVibrancyEffect ();
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'UIVibrancyEffect.CreateWidgetEffectForNotificationCenter' instead.")]
#else
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'UIVibrancyEffect.CreateWidgetEffectForNotificationCenter' instead.")]
#endif
		static public UIVibrancyEffect CreateSecondaryVibrancyEffectForNotificationCenter ()
		{
			return (null as UIVibrancyEffect).GetWidgetSecondaryVibrancyEffect ();
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
#endif
		static public UIVibrancyEffect CreateWidgetEffectForNotificationCenter (UIVibrancyEffectStyle vibrancyStyle)
		{
			return (null as UIVibrancyEffect).GetWidgetEffect (vibrancyStyle);
		}
#endif // HAS_NOTIFICATIONCENTER
	}
}
#endif // IOS || TVOS
