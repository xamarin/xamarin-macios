using System;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class EventNotFoundEventArgs : EventArgs {
		public EventNotFoundEventArgs (string originalEvent)
		{
			OriginalEvent = originalEvent;
		}
		public string OriginalEvent { get; init; }
	}

	public class EventFoundEventArgs : EventNotFoundEventArgs {
		public EventFoundEventArgs (string originalEvent, string mappedEvent)
			: base (originalEvent)
		{
			MappedEvent = mappedEvent;
		}
		public string MappedEvent { get; init; }
	}
}
