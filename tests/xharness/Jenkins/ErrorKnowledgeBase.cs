#nullable enable

using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins {
	public class ErrorKnowledgeBase : IErrorKnowledgeBase {
		static readonly Dictionary<string, string> testErrorMaps = new Dictionary<string, string> {
			["error HE0038: Failed to launch the app"] = "<a href='https://github.com/xamarin/maccore/issues/581'>HE0038</a>",
		};
		
		static readonly Dictionary<string, string> buildErrorMaps = new Dictionary<string, string> {
			["error MT5210: Native linking failed, undefined symbol: ___multi3"] = "<a href='https://github.com/mono/mono/issues/18560'>Undefined symbol ___multi3 on Release Mode</a>",
		};
		
		static bool TryFindErrors (ILog? log, Dictionary<string, string> errorMap, out string? failureMessage)
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

		public bool IsKnownBuildIssue (ILog buildLog, out string? knownFailureMessage) => 
			TryFindErrors (buildLog, buildErrorMaps, out knownFailureMessage);

		public bool IsKnownTestIssue (ILog runLog, out string? knownFailureMessage) =>
			TryFindErrors (runLog, testErrorMaps, out knownFailureMessage);

		public bool IsKnownInstallIssue (ILog installLog, out string? knownFailureMessage)
		{
			// nothing yet that we are aware of
			knownFailureMessage = null;
			return false;
		}
	}
}
