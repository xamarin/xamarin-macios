using System;
using System.Runtime.Serialization;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {
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
