#nullable enable

using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins {
	public class ErrorKnowledgeBase : IErrorKnowledgeBase {
		static readonly Dictionary<string, (string HumanMessage, string IssueLink)> testErrorMaps = new Dictionary<string, (string HumanMessage, string IssueLink)> {
			["error HE0038: Failed to launch the app"] = (HumanMessage: "HE0038", IssueLink: "https://github.com/xamarin/maccore/issues/581)"),
			["Couldn't establish a TCP connection with any of the hostnames"] = (HumanMessage: "Tcp Connection Error: Tests are reported as crashes when they succeeded.", IssueLink: "https://github.com/xamarin/maccore/issues/1741"),
		};
		
		static readonly Dictionary<string, (string HumanMessage, string IssueLink)> buildErrorMaps = new Dictionary<string, (string HumanMessage, string IssueLink)> {
			["error MT5210: Native linking failed, undefined symbol: ___multi3"] = (HumanMessage: "Undefined symbol ___multi3 on Release Mode.", IssueLink:"https://github.com/mono/mono/issues/18560"),
		};
		
		static bool TryFindErrors (ILog? log, Dictionary<string, (string HumanMessage, string IssueLink)> errorMap, out (string HumanMessage, string IssueLink)? failureMessage)
		{
			failureMessage = null;
			if (log == null) {
				return false;
			}

			if (!File.Exists (log.FullPath) || new FileInfo (log.FullPath).Length <= 0)
				return false;
			
			using var reader = log.GetReader ();
			while (!reader.EndOfStream) {
				string line = reader.ReadLine ();
				if (line == null)
					continue;
				//go over errors and return true as soon as we find one that matches
				foreach (var error in errorMap.Keys) {
					if (!line.Contains (error))
						continue;
					failureMessage = errorMap [error];
					return true;
				}
			}
			
			return false;
		}

		public bool IsKnownBuildIssue (ILog buildLog, out (string HumanMessage, string IssueLink)? knownFailureMessage) => 
			TryFindErrors (buildLog, buildErrorMaps, out knownFailureMessage);

		public bool IsKnownTestIssue (ILog runLog, out (string HumanMessage, string IssueLink)? knownFailureMessage) =>
			TryFindErrors (runLog, testErrorMaps, out knownFailureMessage);

		public bool IsKnownInstallIssue (ILog installLog, out (string HumanMessage, string IssueLink)? knownFailureMessage)
		{
			// nothing yet that we are aware of
			knownFailureMessage = null;
			return false;
		}
	}
}
