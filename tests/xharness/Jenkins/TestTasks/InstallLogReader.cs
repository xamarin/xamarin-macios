// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Microsoft.DotNet.XHarness.Common.Logging;

namespace Xharness.Jenkins.TestTasks {
	static class InstallLogReader {
		public static TextReader Open (IFileBackedLog installLog, ILog mainLog)
		{
			installLog.Flush ();
			try {
				return installLog.GetReader ();
			} catch (IOException e) when (e is FileNotFoundException || e is DirectoryNotFoundException) {
				var message = $"Install log '{installLog.FullPath}' is unavailable: {e.Message}";
				mainLog.WriteLine (message);
				return new StringReader (message);
			}
		}
	}
}
