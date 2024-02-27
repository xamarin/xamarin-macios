using System;
using Mono.Cecil;

#nullable enable

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	public class ItemNotFoundEventArgs<T> : EventArgs {
		public ItemNotFoundEventArgs (string original)
		{
			Original = original;
		}
		public string Original { get; init; }
	}

	public class ItemFoundEventArgs<T> : EventArgs {
		public ItemFoundEventArgs (string original, T mapped)
		{
			Original = original;
			Mapped = mapped;
		}
		public string Original { get; init; }
		public T Mapped { get; init; }
	}

	public class ItemEvents<T> where T : IMemberDefinition {
		public EventHandler<ItemNotFoundEventArgs<T>> NotFound = (s, e) => { };
		public EventHandler<ItemFoundEventArgs<T>> Found = (s, e) => { };

		internal void InvokeFound (object sender, string original, T mapped) =>
			Found.Invoke (sender, new (original, mapped));

		internal void InvokeNotFound (object sender, string original) =>
			NotFound.Invoke (sender, new (original));
	}
}
