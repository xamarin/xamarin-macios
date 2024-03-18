using System;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {
	public static class TaskExtensions {
		public static bool ShouldExecuteRemotely (this Task task, string sessionId)
			=> Environment.OSVersion.Platform == PlatformID.Win32NT && !string.IsNullOrEmpty (sessionId);

		public static string GetDotNetPath (this Task task)
		{
			//Custom environment variable set by the XMA Build Agent
			var dotnetPath = Environment.GetEnvironmentVariable ("DOTNET_CUSTOM_PATH");

			if (string.IsNullOrEmpty (dotnetPath)) {
				//Deprecated dotnet environment variable used for backwards compatibility
				dotnetPath = Environment.GetEnvironmentVariable ("DOTNET_HOST_PATH");
			}

			if (string.IsNullOrEmpty (dotnetPath)) {
				dotnetPath = Environment.OSVersion.Platform == PlatformID.Win32NT ? "dotnet.exe" : "dotnet";
			}

			return dotnetPath;
		}
	}
}
