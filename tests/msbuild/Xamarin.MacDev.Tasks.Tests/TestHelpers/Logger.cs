using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<BuildEventArgs> AllEvents {
			get {
				var rv = new List<BuildEventArgs> ();
				rv.AddRange (CustomEvents);
				rv.AddRange (ErrorEvents);
				rv.AddRange (MessageEvents);
				rv.AddRange (WarningsEvents);
				return rv;
			}
		}
	}

	public static class BuildEventArgsExtensions {
		public static string AsString (this BuildEventArgs ea)
		{
			if (ea is BuildErrorEventArgs eea) {
				return $"{eea.Code}: error: {eea.Message}";
			} else if (ea is BuildMessageEventArgs bmea) {
				return $"{bmea.Code}: {bmea.Message}";
			} else if (ea is BuildWarningEventArgs bwea) {
				return $"{bwea.Code}: warning: {bwea.Message}";
			} else if (ea is CustomBuildEventArgs cbea) {
				return cbea.Message;
			} else {
				return ea.Message;
			}
		}
	}
}
