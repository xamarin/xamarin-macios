using System;
using System.Runtime.Serialization;

namespace Xharness {
	public class NoDeviceFoundException : Exception {
		public NoDeviceFoundException ()
		{
		}

		public NoDeviceFoundException (string message) : base (message)
		{
		}

		public NoDeviceFoundException (string message, Exception innerException) : base (message, innerException)
		{
		}

		protected NoDeviceFoundException (SerializationInfo info, StreamingContext context) : base (info, context)
		{
		}
	}
}
