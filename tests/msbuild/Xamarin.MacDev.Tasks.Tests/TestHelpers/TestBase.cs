using System.IO;
using System.Linq;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.MacDev.Tasks {
	public abstract class TestBase {
		TestEngine engine;
		public TestEngine Engine {
			get {
				if (engine is null)
					engine = new TestEngine ();
				return engine;
			}
		}

		ProjectPaths paths;
		ProjectPaths MonoTouchProject {
			get {
				if (paths is null) {
					var platform = "iPhoneSimulator";
					var config = "Debug";
					var projectPath = Path.Combine (Configuration.TestProjectsDirectory, "MySingleView");
					var binPath = Path.Combine (projectPath, "bin", platform, config);
					var objPath = Path.Combine (projectPath, "obj", platform, config);

					paths = new ProjectPaths {
						ProjectPath = projectPath,
						ProjectObjPath = objPath,
						AppBundlePath = Path.Combine (binPath, "MySingleView.app"),
					};
				}
				return paths;
			}
		}

		public string MonoTouchProjectObjPath => MonoTouchProject.ProjectObjPath;
		public string MonoTouchProjectPath => MonoTouchProject.ProjectPath;
		public string AppBundlePath => MonoTouchProject.AppBundlePath;

		[SetUp]
		public virtual void Setup ()
		{
			engine = null;
			paths = null;
		}

		public T CreateTask<T> () where T : Task, new()
		{
			var t = new T ();
			t.BuildEngine = Engine;
			return t;
		}

		/// <summary>
		/// Executes the task and log its error messages.</summary>
		/// <remarks>
		/// This is the prefered way to run tasks as we want error messages to show up in the test results.</remarks>
		/// <param name="task">An msbuild task.</param>
		/// <param name="expectedErrorCount">Expected error count. 0 by default.</param>
		public void ExecuteTask (Task task, int expectedErrorCount = 0)
		{
			var rv = task.Execute ();
			if (expectedErrorCount != Engine.Logger.ErrorEvents.Count) {
				string messages = string.Empty;
				if (Engine.Logger.ErrorEvents.Count > 0) {
					messages = "\n\t" + string.Join ("\n\t", Engine.Logger.ErrorEvents.Select ((v) => v.Message).ToArray ());
				}
				Assert.AreEqual (expectedErrorCount, Engine.Logger.ErrorEvents.Count, "#RunTask-ErrorCount" + messages);
			}
			Assert.AreEqual (expectedErrorCount == 0, rv, "Failure");
		}

		protected string CreateTempFile (string path)
		{
			path = Path.Combine (Cache.CreateTemporaryDirectory ("msbuild-tests"), path);
			using (new FileStream (path, FileMode.CreateNew)) { }
			return path;
		}
	}

	class ProjectPaths {
		public string ProjectPath { get; set; }
		public string ProjectObjPath { get; set; }
		public string AppBundlePath { get; set; }
	}
}
