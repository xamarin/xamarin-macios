using System.Collections.Generic;

using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public class Logger : ILogger {
		public List<CustomBuildEventArgs> CustomEvents {
			get; set;
		}

		public List<BuildErrorEventArgs> ErrorEvents {
			get; set;
		}

		public List<BuildMessageEventArgs> MessageEvents {
			get; set;
		}

		public List<BuildWarningEventArgs> WarningsEvents {
			get; set;
		}

		public void Initialize (IEventSource eventSource)
		{
			CustomEvents = new List<CustomBuildEventArgs> ();
			ErrorEvents = new List<BuildErrorEventArgs> ();
			MessageEvents = new List<BuildMessageEventArgs> ();
			WarningsEvents = new List<BuildWarningEventArgs> ();
		}

		public void Shutdown ()
		{
			Clear ();
		}

		public string Parameters {
			get; set;
		}

		public LoggerVerbosity Verbosity {
			get; set;
		}

		public void Clear ()
		{
			CustomEvents.Clear ();
			ErrorEvents.Clear ();
			MessageEvents.Clear ();
			WarningsEvents.Clear ();
		}
	}
}
