using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Contracts;

namespace Xamarin.Messaging.Build {
	internal class BuildEngine : IBuildEngine, IBuildEngine2, IBuildEngine3, IBuildEngine4 {
		List<LogEntry> logEntries = new List<LogEntry> ();

		public List<LogEntry> LogEntries => logEntries;

		public int ColumnNumberOfTaskNode => 0;

		public bool ContinueOnError => throw new NotImplementedException ();

		public int LineNumberOfTaskNode => 0;

		public string ProjectFileOfTaskNode => string.Empty;

		public bool IsRunningMultipleNodes => throw new NotImplementedException ();

		public bool BuildProjectFile (string projectFileName, string [] targetNames, System.Collections.IDictionary globalProperties, System.Collections.IDictionary targetOutputs)
		{
			throw new NotImplementedException ();
		}

		public void LogCustomEvent (CustomBuildEventArgs e)
		{
		}

		public void LogErrorEvent (BuildErrorEventArgs e)
		{
			logEntries.Add (
				new LogEntry {
					MessageType = LogEntryType.Error,
					Message = e.Message,
					Code = e.Code
				});
		}

		public void LogMessageEvent (BuildMessageEventArgs e)
		{
			logEntries.Add (
				new LogEntry {
					MessageType = LogEntryType.Message,
					Message = e.Message,
					Importance = e.Importance
				});
		}

		public void LogWarningEvent (BuildWarningEventArgs e)
		{
			logEntries.Add (
				new LogEntry {
					MessageType = LogEntryType.Warning,
					Message = e.Message,
					Code = e.Code
				});
		}

		public bool BuildProjectFile (string projectFileName, string [] targetNames, System.Collections.IDictionary globalProperties, System.Collections.IDictionary targetOutputs, string toolsVersion)
		{
			throw new NotImplementedException ();
		}

		public bool BuildProjectFilesInParallel (string [] projectFileNames, string [] targetNames, System.Collections.IDictionary [] globalProperties, System.Collections.IDictionary [] targetOutputsPerProject, string [] toolsVersion, bool useResultsCache, bool unloadProjectsOnCompletion)
		{
			throw new NotImplementedException ();
		}

		public BuildEngineResult BuildProjectFilesInParallel (string [] projectFileNames, string [] targetNames, System.Collections.IDictionary [] globalProperties, System.Collections.Generic.IList<string> [] removeGlobalProperties, string [] toolsVersion, bool returnTargetOutputs)
		{
			throw new NotImplementedException ();
		}

		public void Reacquire ()
		{
			throw new NotImplementedException ();
		}

		public void Yield ()
		{
			throw new NotImplementedException ();
		}

		public object GetRegisteredTaskObject (object key, RegisteredTaskObjectLifetime lifetime)
		{
			throw new NotImplementedException ();
		}

		public void RegisterTaskObject (object key, object obj, RegisteredTaskObjectLifetime lifetime, bool allowEarlyCollection)
		{
			throw new NotImplementedException ();
		}

		public object UnregisterTaskObject (object key, RegisteredTaskObjectLifetime lifetime)
		{
			throw new NotImplementedException ();
		}
	}
}
