using System;
using System.ComponentModel;
// the linker will remove the attributes
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace ObjCRuntime {

	[EditorBrowsable (EditorBrowsableState.Never)]
	public static class ThrowHelper {

		[DoesNotReturn]
		public static void ThrowArgumentException (string argumentName)
		{
			throw new ArgumentException (argumentName);
		}

		[DoesNotReturn]
		public static void ThrowArgumentException (string argumentName, string message)
		{
			throw new ArgumentException (message, argumentName);
		}

		[DoesNotReturn]
		public static void ThrowArgumentNullException (string argumentName)
		{
			throw new ArgumentNullException (argumentName);
		}

		[DoesNotReturn]
		public static void ThrowArgumentNullException (string argumentName, string message)
		{
			throw new ArgumentNullException (argumentName, message);
		}

		[DoesNotReturn]
		public static void ThrowArgumentOutOfRangeException (string argumentName, string message)
		{
			throw new ArgumentOutOfRangeException (argumentName, message);
		}

		[DoesNotReturn]
		public static void ThrowArgumentOutOfRangeException (string argumentName, object actualValue, string message)
		{
			throw new ArgumentOutOfRangeException (argumentName, actualValue, message);
		}

		[DoesNotReturn]
		public static void ThrowObjectDisposedException (object o)
		{
			throw new ObjectDisposedException (o.GetType ().ToString ());
		}
	}
}
