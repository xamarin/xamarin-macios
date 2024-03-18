using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks {
	public static class LoggingExtensions {
		const MessageImportance TaskPropertyImportance = MessageImportance.Normal;
		internal static readonly string ErrorPrefix;

		static LoggingExtensions ()
		{
			var name = typeof (LoggingExtensions).Assembly.GetName ();

			switch (name.Name) {
			case "Xamarin.Mac.Tasks":
				ErrorPrefix = "MM";
				break;
			default:
				ErrorPrefix = "MT";
				break;
			}
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, ITaskItem [] items)
		{
			if (items is null) {
				log.LogMessage (TaskPropertyImportance, "  {0}: <null>", propertyName);
				return;
			}

			log.LogMessage (TaskPropertyImportance, "  {0}:", propertyName);

			for (int i = 0; i < items.Length; i++)
				log.LogMessage (TaskPropertyImportance, "    {0}", items [i].ItemSpec);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, ITaskItem item)
		{
			if (item is not null)
				log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, item.ItemSpec);
			else
				log.LogMessage (TaskPropertyImportance, "  {0}: ", propertyName);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, string [] items)
		{
			if (items is null) {
				log.LogMessage (TaskPropertyImportance, "  {0}: <null>", propertyName);
				return;
			}

			log.LogMessage (TaskPropertyImportance, "  {0}:", propertyName);

			for (int i = 0; i < items.Length; i++)
				log.LogMessage (TaskPropertyImportance, "    {0}", items [i]);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, string value)
		{
			log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, value ?? "<null>");
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, bool value)
		{
			log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, value);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, int value)
		{
			log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, value);
		}

		/// <summary>
		/// Creates an MSBuild error following our MTErrors convention.</summary>
		/// <remarks>
		/// For every new error we need to update "docs/website/mtouch-errors.md" and "tools/mtouch/error.cs".</remarks>
		/// <param name="errorCode">In the 7xxx range for MSBuild error.</param>
		/// <param name="message">The error's message to be displayed in the error pad.</param>
		/// <param name="fileName">Path to the known guilty file or null.</param>
		public static void LogError (this TaskLoggingHelper log, int errorCode, string fileName, string message, params object [] args)
		{
			log.LogError (null, $"{ErrorPrefix}{errorCode}", null, fileName ?? "MSBuild", 0, 0, 0, 0, message, args);
		}

		public static void LogWarning (this TaskLoggingHelper log, int errorCode, string fileName, string message, params object [] args)
		{
			log.LogWarning (null, $"{ErrorPrefix}{errorCode}", null, fileName ?? "MSBuild", 0, 0, 0, 0, message, args);
		}

		public static bool LogErrorsFromException (this TaskLoggingHelper log, Exception exception, bool showStackTrace = true, bool showDetail = true)
		{
			var exceptions = new List<Exception> ();
			if (exception is AggregateException ae) {
				exceptions.AddRange (ae.InnerExceptions);
			} else {
				exceptions.Add (exception);
			}
			foreach (var e in exceptions) {
				log.LogErrorFromException (e, showStackTrace, showDetail, null);
			}
			return false;
		}
	}
}
