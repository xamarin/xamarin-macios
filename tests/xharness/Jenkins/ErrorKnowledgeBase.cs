using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

#nullable enable

namespace Xharness.Jenkins {
	public class ErrorKnowledgeBase : IErrorKnowledgeBase{
		static bool IsHE0038Error (ILog log) {
			if (log == null)
				return false;
			if (File.Exists (log.FullPath) && new FileInfo (log.FullPath).Length > 0) {
				using (var reader = log.GetReader ()) {
					while (!reader.EndOfStream) {
						string line = reader.ReadLine ();
						if (line == null)
							continue;
						if (line.Contains ("error HE0038: Failed to launch the app"))
							return true;
					}
				}
			}
			return false;
		}

		static bool IsMonoMulti3Issue (ILog log) {
			if (log == null)
				return false;
			if (File.Exists (log.FullPath) && new FileInfo (log.FullPath).Length > 0) {
				using var reader = log.GetReader ();
				while (!reader.EndOfStream) {
					string line = reader.ReadLine ();
					if (line == null)
						continue;
					if (line.Contains ("error MT5210: Native linking failed, undefined symbol: ___multi3"))
						return true;
				}
			}
			return false;
		}

		public bool IsKnownBuildIssue (ILog buildLog, out string? knownFailureMessage)
		{
			knownFailureMessage = null;
			if (IsMonoMulti3Issue (buildLog)) {
				knownFailureMessage = $"<a href='https://github.com/mono/mono/issues/18560'>Undefined symbol ___multi3 on Release Mode</a>";
				return true;
			}
			return false;
		}

		public bool IsKnownTestIssue (ILog runLog, out string? knownFailureMessage)
		{
			knownFailureMessage = null;
			if (IsHE0038Error (runLog)) {
				knownFailureMessage = $"<a href='https://github.com/xamarin/maccore/issues/581'>HE0038</a>";
				return true;
			}
			return false;
		}

		public bool IsKnownInstallIssue (ILog installLog, out string? knownFailureMessage)
		{
			// nothing yet that we are aware of
			knownFailureMessage = null;
			return false;
		}
	}
}
