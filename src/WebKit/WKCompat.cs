using System;
using System.Threading.Tasks;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace WebKit {

#if !NET
	public partial class WKWebsiteDataStore {

		[Obsolete ("This constructor does not create a valid instance of the type.")]
		public WKWebsiteDataStore ()
		{
		}
	}

	public partial class WKWebView {

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
		[Obsolete ("Use 'CloseAllMediaPresentations (Action completionHandler)' instead.")]
		public virtual void CloseAllMediaPresentations ()
		{
#if IOS || __MACCATALYST__
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
#endif
			CloseAllMediaPresentationsAsync ().Wait ();
				else
				_OldCloseAllMediaPresentations ();
		}

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
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

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
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

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
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

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
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

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
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

		[Mac (11, 3), iOS (14, 5), MacCatalyst (14, 5)]
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
#endif // !NET
}
