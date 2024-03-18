using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev {
	/// <summary>
	/// Builds a process argument string.
	/// </summary>
	public class CommandLineArgumentBuilder {
		static readonly char [] QuoteSpecials = new char [] { ' ', '\\', '\'', '"', ',', ';' };

		readonly HashSet<string> hash = new HashSet<string> ();
		readonly StringBuilder builder = new StringBuilder ();

		public string ProcessPath {
			get; private set;
		}

		public CommandLineArgumentBuilder ()
		{
		}

		public CommandLineArgumentBuilder (string processPath)
		{
			ProcessPath = processPath;
		}

		public int Length {
			get { return builder.Length; }
		}

		/// <summary>
		/// Adds an argument without escaping or quoting.
		/// </summary>
		public void Add (string argument, bool appendLine = false)
		{
			if (builder.Length > 0 && !appendLine)
				builder.Append (' ');

			builder.Append (argument);

			if (appendLine)
				builder.AppendLine ();

			hash.Add (argument);
		}

		public void AddQuotedSwitchIfNotNull (string name, string value)
		{
			if (value is null)
				return;
			AddQuoted (name + value);
		}

		/// <summary>
		/// Adds an argument without escaping or quoting and goes to the next line
		/// </summary>
		public void AddLine (string argument)
		{
			Add (argument, true);
		}

		/// <summary>
		/// Adds multiple arguments without escaping or quoting.
		/// </summary>
		public void Add (params string [] args)
		{
			foreach (var a in args)
				Add (a);
		}

		/// <summary>
		/// Adds a formatted argument, quoting and escaping as necessary.
		/// </summary>
		public void AddQuotedFormat (string argumentFormat, params object [] values)
		{
			AddQuoted (string.Format (argumentFormat, values));
		}

		public void AddQuotedFormat (string argumentFormat, object val0)
		{
			AddQuoted (string.Format (argumentFormat, val0));
		}

		static void AppendQuoted (StringBuilder quoted, string text, bool appendLine = false)
		{
			if (text.IndexOfAny (QuoteSpecials) != -1) {
				quoted.Append ("\"");

				for (int i = 0; i < text.Length; i++) {
					if (text [i] == '\\' || text [i] == '"')
						quoted.Append ('\\');
					quoted.Append (text [i]);
				}

				quoted.Append ("\"");
			} else {
				quoted.Append (text);
			}

			if (appendLine)
				quoted.AppendLine ();
		}

		/// <summary>Adds an argument, quoting and escaping as necessary.</summary>
		/// <remarks>The .NET process class does not support escaped 
		/// arguments, only quoted arguments with escaped quotes.</remarks>
		public void AddQuoted (string argument, bool appendLine = false)
		{
			if (argument is null)
				return;

			if (builder.Length > 0 && !appendLine)
				builder.Append (' ');

			AppendQuoted (builder, argument, appendLine);
			hash.Add (argument);
		}

		/// <summary>
		/// Adds an argument, quoting, escaping as necessary, and goes to the next line
		/// </summary>
		public void AddQuotedLine (string argument)
		{
			AddQuoted (argument, true);
		}

		/// <summary>
		/// Adds multiple arguments, quoting and escaping each as necessary.
		/// </summary>
		public void AddQuoted (params string [] args)
		{
			foreach (var a in args)
				AddQuoted (a);
		}

		/// <summary>
		/// Contains the specified argument.
		/// </summary>
		/// <param name="argument">Argument.</param>
		public bool Contains (string argument)
		{
			return hash.Contains (argument);
		}

		/// <summary>Quotes a string, escaping if necessary.</summary>
		/// <remarks>The .NET process class does not support escaped 
		/// arguments, only quoted arguments with escaped quotes.</remarks>
		public static string Quote (string text)
		{
			var quoted = new StringBuilder ();

			AppendQuoted (quoted, text);

			return quoted.ToString ();
		}

		public override string ToString ()
		{
			return builder.ToString ();
		}

		static bool TryParse (string commandline, out string [] argv, out Exception ex)
		{
			return StringUtils.TryParseArguments (commandline, out argv, out ex);
		}

		public static bool TryParse (string commandline, out string [] argv)
		{
			Exception ex;

			return TryParse (commandline, out argv, out ex);
		}

		public IList<string> ToList ()
		{
			return Parse (ToString ());
		}

		public static string [] Parse (string commandline)
		{
			string [] argv;
			Exception ex;

			if (!TryParse (commandline, out argv, out ex))
				throw ex;

			return argv;
		}

		// Creates a response file for the given arguments, and returns the command line
		// with the response file and any arguments that can't go into the response file.
		public string CreateResponseFile (Task task, string responseFilePath, IList<string> nonResponseArguments)
		{
			// Generate a response file
			var responseFile = Path.GetFullPath (responseFilePath);

			if (File.Exists (responseFile))
				File.Delete (responseFile);

			try {
				using (var fs = File.Create (responseFile)) {
					using (var writer = new StreamWriter (fs))
						writer.Write (this);
				}
			} catch (Exception ex) {
				task.Log.LogWarning ("Failed to create response file '{0}': {1}", responseFile, ex);
			}

			// Some arguments can not safely go in the response file and are 
			// added separately. They must go _after_ the response file
			// as they may override options passed in the response file
			var actualArgs = new CommandLineArgumentBuilder ();

			actualArgs.AddQuoted ($"@{responseFile}");

			if (nonResponseArguments is not null) {
				foreach (var arg in nonResponseArguments)
					actualArgs.AddQuoted (arg);
			}

			// Generate the command line
			return actualArgs.ToString ();
		}
	}
}
