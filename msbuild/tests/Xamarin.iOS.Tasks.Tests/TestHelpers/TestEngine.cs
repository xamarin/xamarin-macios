using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
//using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Logging;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.iOS.Tasks
{

	public class TestEngine : /*Engine, */IBuildEngine
	{
		public Logger Logger {
			get; set;
		}
	
		public TestEngine ()
		{
			Logger = new Logger ();
			ProjectCollection = new ProjectCollection ();

			ProjectCollection.RegisterLogger (Logger);

			var printer = new ConsoleReportPrinter ();
			var cl = new ConsoleLogger (LoggerVerbosity.Diagnostic, printer.Print, printer.SetForeground, printer.ResetColor);
			ProjectCollection.RegisterLogger (cl);
		}

		public bool BuildProjectFile (string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
		{
			throw new NotImplementedException ();
		}

		public bool BuildProject (Project project, string[] targetNames, IDictionary globalProperties)
		{
			foreach (DictionaryEntry de in globalProperties)
				project.SetGlobalProperty ((string)de.Key, (string)de.Value);

			return project.Build (targetNames);
		}

		public void LogCustomEvent (CustomBuildEventArgs e)
		{
			Logger.CustomEvents.Add (e);
		}
		public void LogErrorEvent (BuildErrorEventArgs e)
		{
			Logger.ErrorEvents.Add (e);
		}
		public void LogMessageEvent (BuildMessageEventArgs e)
		{
			Logger.MessageEvents.Add (e);
		}
		public void LogWarningEvent (BuildWarningEventArgs e)
		{
			Logger.WarningsEvents.Add (e);
		}

		public void UnloadAllProjects ()
		{
			ProjectCollection.UnloadAllProjects ();
		}

		public int ColumnNumberOfTaskNode {
			get { return 0; }
		}
		public bool ContinueOnError {
			get { return true; }
		}
		public int LineNumberOfTaskNode {
			get {return 0;}
		}
		public string ProjectFileOfTaskNode {
			get { return ""; }
		}

		public ProjectCollection ProjectCollection { get; set; }
	}
	
}
