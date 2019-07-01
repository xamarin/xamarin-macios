using System;
using System.IO;
using System.IO.Compression;

using Foundation;
using BackgroundTasks;
using Compression;
using ObjCRuntime;

using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.BackgroundTasks {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BGTaskSchedulerTest {

		static bool taskWasCalled;
		static bool registered;
		static AutoResetEvent autoResetEvent = new AutoResetEvent (false);
		static readonly string taskIdentifier = "com.xamarin.monotouch-test.TesgBackgroundTask";

		public static void RegisterTestTasks ()
		{
			registered = BGTaskScheduler.Shared.Register (taskIdentifier, null, (task) => {
				HandleBackgroudTask (task as BGProcessingTask);
			});
		}

		static void HandleBackgroudTask (BGProcessingTask task)
		{
			// reset the event and state that the task was completed
			taskWasCalled = true;
			autoResetEvent.Set ();
			task.SetTaskCompleted (true);
		}

		void LaunchBGTask ()
		{
			using (var taskId = new NSString (taskIdentifier)) { 
				var method = new Selector ("_simulateLaunchForTaskWithIdentifier:");
				Messaging.void_objc_msgSend_IntPtr (BGTaskScheduler.Shared.Handle, method.Handle, taskId.Handle);
			}
		}

		[Test]
		public void SubmitTaskRequestTest ()
		{
			TestRuntime.AssertDevice ();
			Assert.True (registered, "Task was not registered.");
			// get the shared scheduler, create a request and submit it, this will be called asap
			// and the autoreset event set.
			var request = new BGProcessingTaskRequest (taskIdentifier);
			NSError error;
			BGTaskScheduler.Shared.Submit (request, out error);
			Assert.IsNull (error, $"Error submiting request {error}");
			LaunchBGTask ();
			autoResetEvent.WaitOne ();
			Assert.True (taskWasCalled, "Called task.");
		}
	}
}