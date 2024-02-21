#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;

namespace Xharness.Jenkins {
	public class ErrorKnowledgeBase : IErrorKnowledgeBase {
		static readonly Dictionary<string, KnownIssue> testErrorMaps = new Dictionary<string, KnownIssue> {
			["error HE0038: Failed to launch the app"] = new KnownIssue ("HE0038", issueLink: "https://github.com/xamarin/maccore/issues/581)"),
			["Couldn't establish a TCP connection with any of the hostnames"] = new KnownIssue ("Tcp Connection Error: Tests are reported as crashes when they succeeded.", issueLink: "https://github.com/xamarin/maccore/issues/1741"),
			["BCLTests.TestRunner.Core.TcpTextWriter..ctor"] = new KnownIssue ("Tcp Connection Error: Tests are reported as crashes when they succeeded.", issueLink: "https://github.com/xamarin/maccore/issues/1741"),
		};

		static readonly Dictionary<string, KnownIssue> buildErrorMaps = new Dictionary<string, KnownIssue> {
			["error MT5210: Native linking failed, undefined symbol: ___multi3"] = new KnownIssue ("Undefined symbol ___multi3 on Release Mode.", issueLink: "https://github.com/mono/mono/issues/18560"),
		};

		static bool TryFindErrors (IFileBackedLog? log, Dictionary<string, KnownIssue> errorMap, [NotNullWhen (true)] out KnownIssue? failureMessage)
		{
			failureMessage = null;
			if (log is null) {
				return false;
			}

			if (!File.Exists (log.FullPath) || new FileInfo (log.FullPath).Length <= 0)
				return false;

			using var reader = log.GetReader ();
			while (!reader.EndOfStream) {
				var line = reader.ReadLine ();
				if (line is null)
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

		public bool IsKnownBuildIssue (IFileBackedLog buildLog, [NotNullWhen (true)] out KnownIssue? knownFailureMessage) =>
			TryFindErrors (buildLog, buildErrorMaps, out knownFailureMessage);

		public bool IsKnownTestIssue (IFileBackedLog runLog, [NotNullWhen (true)] out KnownIssue? knownFailureMessage) =>
			TryFindErrors (runLog, testErrorMaps, out knownFailureMessage);

		public bool IsKnownInstallIssue (IFileBackedLog installLog, [NotNullWhen (true)] out KnownIssue? knownFailureMessage)
		{
			// nothing yet that we are aware of
			knownFailureMessage = null;
			return false;
		}
	}
}
