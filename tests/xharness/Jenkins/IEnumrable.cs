using System;

namespace Xharness.Jenkins {
	public interface IEnumrable<T> {
		object Where (Func<object, object> p);
	}
}