using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.iOS.Tasks
{

	public class TestEngine : IBuildEngine
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

		public bool BuildProjectFile (string projectFileName, string [] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
		{
			throw new NotImplementedException ();
		}

		public bool BuildProject (ProjectInstance instance, string [] targetNames, IDictionary globalProperties)
		{
			if (globalProperties != null) {
				foreach (DictionaryEntry de in globalProperties) {
					//Note: trying to set this on the project causes the project to be added to the PC
					//      again, which of course, fails
					instance.SetProperty ((string)de.Key, (string)de.Value);
				}
			}

			//FIXME: assumption that we are still using the same PC!
			return instance.Build (targetNames, ProjectCollection.Loggers);
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
