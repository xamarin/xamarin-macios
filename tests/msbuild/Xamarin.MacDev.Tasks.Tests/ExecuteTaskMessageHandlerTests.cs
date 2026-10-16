// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Messaging;
using Xamarin.Messaging.Build;
using Xamarin.Messaging.Build.Contracts;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ExecuteTaskMessageHandlerTests {
		class TestTaskRunner : ITaskRunner {
			public bool Executed { get; private set; }
			public string TaskName { get; private set; } = "";
			public string Inputs { get; private set; } = "";
			public string WorkingDirectory { get; private set; } = "";
			public ExecuteTaskResult Result { get; } = new ExecuteTaskResult ();

			public ExecuteTaskResult Execute (string taskName, string inputs)
			{
				Executed = true;
				TaskName = taskName;
				Inputs = inputs;
				WorkingDirectory = Directory.GetCurrentDirectory ();
				return Result;
			}
		}

		class TestMessageHandler : ExecuteTaskMessageHandler {
			public TestMessageHandler (ITaskRunner runner) : base (runner)
			{
			}

			public Task<ExecuteTaskResult> Execute (ExecuteTaskMessage message) => ExecuteAsync (message);
		}

		[Test]
		public void MissingAppName ()
		{
			var runner = new TestTaskRunner ();
			var handler = new TestMessageHandler (runner);
			var message = new ExecuteTaskMessage {
				TaskName = "Xamarin.MacDev.Tasks.ReadAppManifest",
				SessionId = "test-build-session",
				AppName = null,
			};
			var currentDirectory = Directory.GetCurrentDirectory ();

			var exception = Assert.ThrowsAsync<ArgumentNullException> (async () => await handler.Execute (message));

			Assert.Multiple (() => {
				Assert.That (exception?.ParamName, Is.EqualTo ("AppName"));
				Assert.That (exception?.Message, Does.Contain ($"Cannot execute task '{message.TaskName}' remotely"));
				Assert.That (exception?.Message, Does.Contain ("application name (AppName) is missing"));
				Assert.That (exception?.Message, Does.Contain ($"build session '{message.SessionId}'"));
				Assert.That (exception?.Message, Does.Contain ("Ensure the task runs while its remote build session is connected."));
				Assert.That (runner.Executed, Is.False, "The runner must not execute an invalid request.");
				Assert.That (Directory.GetCurrentDirectory (), Is.EqualTo (currentDirectory), "The working directory must not change.");
			});
		}

		[Test]
		public async Task ValidAppName ()
		{
			var runner = new TestTaskRunner ();
			var handler = new TestMessageHandler (runner);
			var message = new ExecuteTaskMessage {
				TaskName = "Xamarin.MacDev.Tasks.ReadAppManifest",
				SessionId = "test-build-session",
				AppName = "test-application",
				Inputs = "test-inputs",
			};
			var currentDirectory = Directory.GetCurrentDirectory ();
			var previousBasePath = MessagingContext.BasePath;
			var basePath = Cache.CreateTemporaryDirectory ();

			try {
				MessagingContext.BasePath = basePath;
				var buildDirectory = Path.Combine (MessagingContext.BuildsPath, message.AppName, message.SessionId);
				var result = await handler.Execute (message);

				Assert.Multiple (() => {
					Assert.That (result, Is.SameAs (runner.Result));
					Assert.That (runner.Executed, Is.True);
					Assert.That (runner.TaskName, Is.EqualTo (message.TaskName));
					Assert.That (runner.Inputs, Is.EqualTo (message.Inputs));
					Assert.That (runner.WorkingDirectory, Is.EqualTo (buildDirectory));
					Assert.That (Directory.GetCurrentDirectory (), Is.EqualTo (currentDirectory), "The working directory must be restored.");
				});
			} finally {
				MessagingContext.BasePath = previousBasePath;
				Directory.Delete (basePath, true);
			}
		}
	}
}
