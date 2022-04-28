using System;

#nullable enable

namespace Microsoft.MaciOS.AssemblyComparator {
	public class ItemNotFoundEventArgs<T> {
		public ItemNotFoundEventArgs (string original)
		{
			Original = original;
		}
		public string Original { get; init; }
	}

	public class ItemFoundEventArgs<T> {
		public ItemFoundEventArgs (string original, string mapped)
		{
			Original = original;
			Mapped = mapped;
		}
		public string Original { get; init; }
		public string Mapped { get; init; }
	}

	public class ItemEvents<T> {
		public EventHandler<ItemNotFoundEventArgs<T>> NotFound = (s, e) => { };
		public EventHandler<ItemFoundEventArgs<T>> Found = (s, e) => { };

		internal void InvokeFound (object sender, string original, string mapped) =>
			Found.Invoke (sender, new (original, mapped));

		internal void InvokeNotFound (object sender, string original) =>
			NotFound.Invoke (sender, new (original));
	}
}
