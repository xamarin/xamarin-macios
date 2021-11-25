using System;
using System.Threading.Tasks;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace WebKit {

#if !XAMCORE_4_0
	public partial class WKWebsiteDataStore {

		[Obsolete ("This constructor does not create a valid instance of the type.")]
		public WKWebsiteDataStore ()
		{
		}
	}

	public partial class WKWebView {

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
		[Obsolete ("Use 'CloseAllMediaPresentations (Action completionHandler)' instead.")]
		public virtual void CloseAllMediaPresentations () {
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					CloseAllMediaPresentationsAsync ().Wait();
				else
					_OldCloseAllMediaPresentations ();
		}

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
 		public virtual void PauseAllMediaPlayback (Action? completionHandler)
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					_NewPauseAllMediaPlayback (completionHandler);
				else
					_OldPauseAllMediaPlayback (completionHandler);
		}

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
 		public virtual Task PauseAllMediaPlaybackAsync ()
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					return _NewPauseAllMediaPlaybackAsync ();
				else
					return _OldPauseAllMediaPlaybackAsync ();
		}

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
		[Obsolete ("Use 'SetAllMediaPlaybackSuspended' instead.")]
 		public virtual void SuspendAllMediaPlayback (Action? completionHandler)
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					SetAllMediaPlaybackSuspended (true, completionHandler);
				else
					_OldSuspendAllMediaPlayback (completionHandler);
		}

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
 		public virtual Task SuspendAllMediaPlaybackAsync ()
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					return SetAllMediaPlaybackSuspendedAsync (true);
				else
					return _OldSuspendAllMediaPlaybackAsync ();
		}

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
		[Obsolete ("Use 'SetAllMediaPlaybackSuspended' instead.")]
 		public virtual void ResumeAllMediaPlayback (Action? completionHandler)
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					SetAllMediaPlaybackSuspended (false, completionHandler);
				else
					_OldResumeAllMediaPlayback (completionHandler);
		}

#if !NET
		[Mac (11,3), iOS (14,5), MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
 		public virtual Task ResumeAllMediaPlaybackAsync ()
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
					return SetAllMediaPlaybackSuspendedAsync (false);
				else
					return _OldResumeAllMediaPlaybackAsync ();
		}

	}
#endif
}
