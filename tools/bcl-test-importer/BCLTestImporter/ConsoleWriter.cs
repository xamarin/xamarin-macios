using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace BCLTestImporter {
	class ConsoleWriter {
		ApplicationOptions options;

		public ConsoleWriter (ApplicationOptions o)
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));
			options = o;
		}
		public void WriteWarning (string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine (message);
			Console.ResetColor ();
		}

		public void WriteError (string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine (message);
			Console.ResetColor ();
		}
		public void WriteLine (string message)
		{
			if (options.Verbose)
				Console.WriteLine (message);
		}
	}
}
