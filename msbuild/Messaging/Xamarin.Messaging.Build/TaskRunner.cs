using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Build.Properties;
using Xamarin.Messaging.Build.Serialization;

namespace Xamarin.Messaging.Build {
	internal class TaskRunner : ITaskRunner {
		ITaskSerializer serializer;
		List<Type> tasks = new List<Type> ();

		internal TaskRunner (ITaskSerializer serializer)
		{
			this.serializer = serializer;

			var sdkRootPath = Path.Combine (MessagingContext.GetXmaPath (), "SDKs");
			var dotnetPath = Path.Combine (sdkRootPath, "dotnet", "dotnet");

			if (File.Exists (dotnetPath)) {
				Environment.SetEnvironmentVariable ("HOME", Path.Combine (sdkRootPath, ".home"));
			} else {
				//In case the XMA dotnet has not been installed yet
				dotnetPath = "/usr/local/share/dotnet/dotnet";
			}

			Environment.SetEnvironmentVariable ("DOTNET_HOST_PATH", dotnetPath);
		}

		internal IEnumerable<Type> Tasks => tasks.AsReadOnly ();

		internal void LoadTasks (Assembly assembly) => tasks.AddRange (assembly.GetTypes ());

		internal void LoadXamarinTasks () => LoadTasks (typeof (iOS.Tasks.MTouch).Assembly);

		public ExecuteTaskResult Execute (string taskName, string inputs)
		{
			var taskType = tasks.FirstOrDefault (x => string.Equals (x.FullName, taskName, StringComparison.OrdinalIgnoreCase));

			if (taskType == null) {
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
	}
}
