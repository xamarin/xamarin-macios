using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Logging;

namespace Xamarin.MacDev.Tasks {

	public class TestEngine : IBuildEngine, IBuildEngine2, IBuildEngine3, IBuildEngine4 {
		public Logger Logger {
			get; set;
		}

		public TestEngine ()
		{
			Logger = new Logger ();
			ProjectCollection = new ProjectCollection ();
			ProjectCollection.RegisterLogger (Logger);
		}

		public bool BuildProjectFile (string projectFileName, string [] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
		{
			throw new NotImplementedException ();
		}

		bool Verbose => false; // change to true while debugging output

		public void LogCustomEvent (CustomBuildEventArgs e)
		{
			if (Verbose)
				Console.WriteLine (e.Message);
			Logger.CustomEvents.Add (e);
		}
		public void LogErrorEvent (BuildErrorEventArgs e)
		{
			if (Verbose)
				Console.WriteLine (e.Message);
			Logger.ErrorEvents.Add (e);
		}
		public void LogMessageEvent (BuildMessageEventArgs e)
		{
			if (Verbose)
				Console.WriteLine (e.Message);
			Logger.MessageEvents.Add (e);
		}
		public void LogWarningEvent (BuildWarningEventArgs e)
		{
			if (Verbose)
				Console.WriteLine (e.Message);
			Logger.WarningsEvents.Add (e);
		}

		public int ColumnNumberOfTaskNode {
			get { return 0; }
		}
		public bool ContinueOnError {
			get { return true; }
		}
		public int LineNumberOfTaskNode {
			get { return 0; }
		}
		public string ProjectFileOfTaskNode {
			get { return ""; }
		}

		public ProjectCollection ProjectCollection { get; set; }

		private Dictionary<object, object> Tasks = new Dictionary<object, object> ();

		void IBuildEngine4.RegisterTaskObject (object key, object obj, RegisteredTaskObjectLifetime lifetime, bool allowEarlyCollection)
		{
			Tasks [key] = obj;
		}

		object IBuildEngine4.GetRegisteredTaskObject (object key, RegisteredTaskObjectLifetime lifetime)
		{
			Tasks.TryGetValue (key, out object value);
			return value;
		}

		object IBuildEngine4.UnregisterTaskObject (object key, RegisteredTaskObjectLifetime lifetime)
		{
			if (Tasks.TryGetValue (key, out object value)) {
				Tasks.Remove (key);
			}
			return value;
		}

		BuildEngineResult IBuildEngine3.BuildProjectFilesInParallel (string [] projectFileNames, string [] targetNames, IDictionary [] globalProperties, IList<string> [] removeGlobalProperties, string [] toolsVersion, bool returnTargetOutputs)
		{
			throw new NotImplementedException ();
		}

		void IBuildEngine3.Yield () { }

		void IBuildEngine3.Reacquire () { }

		bool IBuildEngine2.BuildProjectFile (string projectFileName, string [] targetNames, IDictionary globalProperties, IDictionary targetOutputs, string toolsVersion) => true;

		bool IBuildEngine2.BuildProjectFilesInParallel (string [] projectFileNames, string [] targetNames, IDictionary [] globalProperties, IDictionary [] targetOutputsPerProject, string [] toolsVersion, bool useResultsCache, bool unloadProjectsOnCompletion) => true;

		bool IBuildEngine2.IsRunningMultipleNodes => false;
	}
}
