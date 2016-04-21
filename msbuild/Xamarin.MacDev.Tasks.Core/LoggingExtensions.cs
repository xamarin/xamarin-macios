using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public static class LoggingExtensions
	{
		const MessageImportance TaskPropertyImportance = MessageImportance.Normal;

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, ITaskItem[] items)
		{
			if (items == null) {
				log.LogMessage (TaskPropertyImportance, "  {0}: <null>", propertyName);
				return;
			}

			log.LogMessage (TaskPropertyImportance, "  {0}:", propertyName);

			for (int i = 0; i < items.Length; i++)
				log.LogMessage (TaskPropertyImportance, "    {0}", items[i].ItemSpec);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, ITaskItem item)
		{
			if (item != null)
				log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, item.ItemSpec);
			else
				log.LogMessage (TaskPropertyImportance, "  {0}: ", propertyName);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, string[] items)
		{
			if (items == null) {
				log.LogMessage (TaskPropertyImportance, "  {0}: <null>", propertyName);
				return;
			}

			log.LogMessage (TaskPropertyImportance, "  {0}:", propertyName);

			for (int i = 0; i < items.Length; i++)
				log.LogMessage (TaskPropertyImportance, "    {0}", items[i]);
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, string value)
		{
			log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, value ?? "<null>");
		}

		public static void LogTaskProperty (this TaskLoggingHelper log, string propertyName, bool value)
		{
			log.LogMessage (TaskPropertyImportance, "  {0}: {1}", propertyName, value);
		}

		public static void LogTaskName (this TaskLoggingHelper log, string taskName)
		{
			log.LogMessage (TaskPropertyImportance, "{0} Task", taskName);
		}
	}
}
