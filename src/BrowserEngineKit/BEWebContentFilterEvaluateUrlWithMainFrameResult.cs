#nullable enable

namespace BrowserEngineKit {
#if IOS || MACCATALYST
	/// <summary>Contains the result of evaluating a URL for web content filtering.</summary>
	[SupportedOSPlatform ("ios27.0")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class BEWebContentFilterEvaluateUrlWithMainFrameResult {
#if !COREBUILD
		/// <param name="shouldBlock">Whether the URL should be blocked.</param>
		/// <param name="blockPageRepresentation">The optional block page representation.</param>
		/// <summary>Creates a result for a web content filter URL evaluation.</summary>
		public BEWebContentFilterEvaluateUrlWithMainFrameResult (bool shouldBlock, NSData? blockPageRepresentation)
		{
			ShouldBlock = shouldBlock;
			BlockPageRepresentation = blockPageRepresentation;
		}

		/// <summary>Gets or sets whether the URL should be blocked.</summary>
		public bool ShouldBlock { get; set; }

		/// <summary>Gets or sets the optional block page representation.</summary>
		public NSData? BlockPageRepresentation { get; set; }
#endif
	}
#endif
}
