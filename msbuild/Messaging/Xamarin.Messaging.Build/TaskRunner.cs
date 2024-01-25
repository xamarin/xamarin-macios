using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Build.Properties;
using Xamarin.Messaging.Build.Serialization;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.Messaging.Build {
	internal class TaskRunner : ITaskRunner {
		static readonly ITracer tracer = Tracer.Get<TaskRunner> ();

		readonly ITaskSerializer serializer;
		readonly List<Type> tasks = new List<Type> ();

		internal TaskRunner (ITaskSerializer serializer)
		{
			this.serializer = serializer;
			SetDotNetVariables ();
		}

		internal IEnumerable<Type> Tasks => tasks.AsReadOnly ();

		internal void LoadTasks (Assembly assembly) => tasks.AddRange (assembly.GetTypes ());

		internal void LoadXamarinTasks () => LoadTasks (typeof (iOS.Tasks.MTouch).Assembly);

		public ExecuteTaskResult Execute (string taskName, string inputs)
		{
			var taskType = tasks.FirstOrDefault (x => string.Equals (x.FullName, taskName, StringComparison.OrdinalIgnoreCase));

			if (taskType is null) {
				throw new ArgumentException (string.Format (Resources.TaskRunner_Execute_Error, taskName), nameof (taskName));
			}

			var task = serializer.Deserialize (inputs, taskType) as Task;
			var buildEngine = new BuildEngine ();

			task.BuildEngine = buildEngine;

			var result = new ExecuteTaskResult ();

			result.Result = task.Execute ();
			result.Output = serializer.SerializeOutputs (task);
			result.LogEntries = buildEngine.LogEntries;

			return result;
		}

		void SetDotNetVariables ()
		{
			var xmaSdkRootPath = Path.Combine (MessagingContext.GetXmaPath (), "SDKs");
			var xmaDotNetRootPath = Path.Combine (xmaSdkRootPath, "dotnet");
			var xmaDotNetPath = default (string);

			if (IsValidDotNetInstallation (xmaDotNetRootPath)) {
				//If the XMA dotnet is already installed, we use it and also declare a custom home for it (for NuGet restore and caches)
				Environment.SetEnvironmentVariable ("DOTNET_CUSTOM_HOME", Path.Combine (xmaSdkRootPath, ".home"));
				xmaDotNetPath = GetDotNetPath (xmaDotNetRootPath);
			} else {
				//In case the XMA dotnet has not been installed yet, we use the default dotnet installation
				xmaDotNetPath = GetDefaultDotNetPath ();
				xmaDotNetRootPath = Path.GetDirectoryName (xmaDotNetPath);
			}

			var pathContent = GetPathContent ();
			//We add the XMA dotnet path first so it has priority over the default dotnet installation
			var newPathContent = $"{xmaDotNetRootPath}:{pathContent}";

			//Override the PATH with the XMA dotnet in it, just in case it's used internally by dotnet
			Environment.SetEnvironmentVariable ("PATH", newPathContent);
			//Deprecated dotnet environment variable. We still preserve ir for backwards compatibility with other components that haven't deprecated it yet (like dotnet ILLink)
			Environment.SetEnvironmentVariable ("DOTNET_HOST_PATH", xmaDotNetPath);
			//Custom environment variable for internal iOS SDK usage
			Environment.SetEnvironmentVariable ("DOTNET_CUSTOM_PATH", xmaDotNetPath);

			tracer.Info ($"Using dotnet: {xmaDotNetPath}");
			tracer.Info ($"Current PATH: {newPathContent}");
		}

		string GetDefaultDotNetPath ()
		{
			var dotnetRootPath = "/usr/local/share/dotnet";

			if (IsValidDotNetInstallation (dotnetRootPath)) {
				return GetDotNetPath (dotnetRootPath);
			}

			var dotnetPath = "dotnet";
			var pathContent = GetPathContent ();
			var pathElements = pathContent.Split (new string [] { ":" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var pathElement in pathElements) {
				try {
					if (IsValidDotNetInstallation (pathElement)) {
						dotnetPath = GetDotNetPath (pathElement);
						break;
					}
				} catch {
					//If we can't read a directory for any reason just skip it
				}
			}

			return dotnetPath;
		}

		string GetPathContent () => Environment.GetEnvironmentVariable ("PATH") ?? "";

		bool IsValidDotNetInstallation (string rootPath) => File.Exists (GetDotNetPath (rootPath));

		string GetDotNetPath (string rootPath) => Path.Combine (rootPath, "dotnet");
	}
}
