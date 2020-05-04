#nullable enable
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared {
	/// <summary>
	/// Interface to be implemented by those classes that know about common errors that will be reporter to the 
	/// harness runner. This allows to store certain problems that we know are common and that we can skip, helping
	/// those that are monitoring the result.
	/// </summary>
	public interface IErrorKnowledgeBase {
		bool IsKnownBuildIssue (ILog buildLog, out string? knownFailureMessage);
		bool IsKnownTestIssue (ILog runLog, out string? knownFailureMessage);
		bool IsKnownInstallIssue (ILog installLog, out string? knownFailureMessage);
	}
}
