using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Build.Framework;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.iOS.Tasks
{
	public class Logger : ILogger
	{
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

			eventSource.CustomEventRaised += (object sender, CustomBuildEventArgs e) => CustomEvents.Add (e);
			eventSource.ErrorRaised += (object sender, BuildErrorEventArgs e) => ErrorEvents.Add (e);
			eventSource.MessageRaised += (object sender, BuildMessageEventArgs e) => MessageEvents.Add (e);
			eventSource.WarningRaised += (object sender, BuildWarningEventArgs e) => WarningsEvents.Add (e);
		}

		public void Shutdown ()
		{
			throw new NotImplementedException ();
		}

		public string Parameters {
			get; set;
		}

		public LoggerVerbosity Verbosity {
			get; set;
		}
	}

	// Stolen from xbuild.
	class ConsoleReportPrinter
	{
		string prefix, postfix;
		bool color_supported;
		TextWriter writer;
		string [] colorPrefixes;

		public ConsoleReportPrinter ()
			: this (Console.Out)
		{
		}

		public ConsoleReportPrinter (TextWriter writer)
		{
			this.writer = writer;

			string term = Environment.GetEnvironmentVariable ("TERM");
			bool xterm_colors = false;

			color_supported = false;
			switch (term){
			case "xterm":
			case "rxvt":
			case "rxvt-unicode":
				if (Environment.GetEnvironmentVariable ("COLORTERM") != null){
					xterm_colors = true;
				}
				break;

			case "xterm-color":
			case "xterm-256color":
				xterm_colors = true;
				break;
			}
			if (!xterm_colors)
				return;

			if (!(UnixUtils.isatty (1) && UnixUtils.isatty (2)))
				return;

			color_supported = true;
			PopulateColorPrefixes ();
			postfix = "\x001b[0m";
		}

		void PopulateColorPrefixes ()
		{
			colorPrefixes = new string [16];

			colorPrefixes [(int)ConsoleColor.Black] = GetForeground ("black");
			colorPrefixes [(int)ConsoleColor.DarkBlue] = GetForeground ("blue");
			colorPrefixes [(int)ConsoleColor.DarkGreen] = GetForeground ("green");
			colorPrefixes [(int)ConsoleColor.DarkCyan] = GetForeground ("cyan");
			colorPrefixes [(int)ConsoleColor.DarkRed] = GetForeground ("red");
			colorPrefixes [(int)ConsoleColor.DarkMagenta] = GetForeground ("magenta");
			colorPrefixes [(int)ConsoleColor.DarkYellow] = GetForeground ("yellow");
			colorPrefixes [(int)ConsoleColor.DarkGray] = GetForeground ("grey");

			colorPrefixes [(int)ConsoleColor.Gray] = GetForeground ("brightgrey");
			colorPrefixes [(int)ConsoleColor.Blue] = GetForeground ("brightblue");
			colorPrefixes [(int)ConsoleColor.Green] = GetForeground ("brightgreen");
			colorPrefixes [(int)ConsoleColor.Cyan] = GetForeground ("brightcyan");
			colorPrefixes [(int)ConsoleColor.Red] = GetForeground ("brightred");
			colorPrefixes [(int)ConsoleColor.Magenta] = GetForeground ("brightmagenta");
			colorPrefixes [(int)ConsoleColor.Yellow] = GetForeground ("brightyellow");

			colorPrefixes [(int)ConsoleColor.White] = GetForeground ("brightwhite");
		}

		public void SetForeground (ConsoleColor color)
		{
			if (color_supported)
				prefix = colorPrefixes [(int)color];
		}

		public void ResetColor ()
		{
			prefix = "\x001b[0m";
		}

		static int NameToCode (string s)
		{
			switch (s) {
			case "black":
				return 0;
			case "red":
				return 1;
			case "green":
				return 2;
			case "yellow":
				return 3;
			case "blue":
				return 4;
			case "magenta":
				return 5;
			case "cyan":
				return 6;
			case "grey":
			case "white":
				return 7;
			}
			return 7;
		}

		//
		// maps a color name to its xterm color code
		//
		static string GetForeground (string s)
		{
			string highcode;

			if (s.StartsWith ("bright")) {
				highcode = "1;";
				s = s.Substring (6);
			} else
				highcode = "";

			return "\x001b[" + highcode + (30 + NameToCode (s)).ToString () + "m";
		}

		static string GetBackground (string s)
		{
			return "\x001b[" + (40 + NameToCode (s)).ToString () + "m";
		}

		string FormatText (string txt)
		{
			if (prefix != null && color_supported)
				return prefix + txt + postfix;

			return txt;
		}

		public void Print (string message)
		{
			writer.WriteLine (FormatText (message));
		}

	}

	class UnixUtils {
		[System.Runtime.InteropServices.DllImport ("libc", EntryPoint="isatty")]
		extern static int _isatty (int fd);

		public static bool isatty (int fd)
		{
			try {
				return _isatty (fd) == 1;
			} catch {
				return false;
			}
		}
	}
}
