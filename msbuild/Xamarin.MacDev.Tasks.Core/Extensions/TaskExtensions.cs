using System;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks
{
	public static class TaskExtensions
	{
		public static bool ShouldExecuteRemotely (this Task task, string sessionId)
			=> Environment.OSVersion.Platform == PlatformID.Win32NT && !string.IsNullOrEmpty (sessionId);
	}
}
