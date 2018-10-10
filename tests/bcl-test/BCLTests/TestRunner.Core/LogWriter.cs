using System;
using Microsoft.Extensions.Logging;

namespace Xamarin.iOS.UnitTests
{
	public class LogWriter
	{
		static ILoggerFactory LoggerFactory {get;} = new LoggerFactory();
		static ILogger Logger;
		
		const string Tag = "LogWriter";

		public MinimumLogLevel MinimumLogLevel { get; set; } = MinimumLogLevel.Info;

		static LogWriter ()
		{
			LoggerFactory.AddConsole ();
			Logger = LoggerFactory.CreateLogger<LogWriter> ();
		}

		public void OnError (string tag, string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Error)
				return;
			Logger.Log (LogLevel.Error, $"{tag} {message}");
		}

		public void OnWarning (string tag, string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Warning)
				return;
			Logger.Log (LogLevel.Warning, $"{tag} {message}");
		}

		public void OnDebug (string tag, string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Debug)
				return;
			Logger.Log (LogLevel.Debug, $"{tag} {message}");
		}

		public void OnDiagnostic (string tag, string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Verbose)
				return;
			Logger.Log (LogLevel.Trace, $"{tag} {message}");
		}

		public void OnInfo (string tag, string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Info)
				return;
			Logger.Log (LogLevel.Information, $"{tag} {message}");
		}
		
		public void Info (string tag, string message)
		{
			Logger.Log (LogLevel.Information, $"{tag} {message}");
		}

		public void SetMinimuLogLevelFromString (string minimumLevel)
		{
			// Be forgiving... :P
			if (String.IsNullOrEmpty (minimumLevel))
				return;

			MinimumLogLevel level;
			if (!Enum.TryParse (minimumLevel, true, out level)) {
				Logger.Log (LogLevel.Warning, $"{Tag} Unknown log level name: {minimumLevel}");
				return;
			}

			Logger.Log (LogLevel.Information, $"{Tag} Setting log level to {level}");
			MinimumLogLevel = level;
		}
	}
}