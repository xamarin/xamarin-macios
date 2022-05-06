using System;

namespace Microsoft.MaciOS.Nnyeah {
	public class WarningEventArgs : BaseTransformEventArgs {
		public WarningEventArgs (string containerName, string methodName, string targetOperand, string message)
			: base (containerName, methodName, targetOperand)
		{
			Message = message;
		}

		public string Message { get; init; }

		public override string HelpfulMessage ()
		{
			return string.Format (Errors.N0006, ContainerName, MethodName, TargetOperand);
		}
	}
}
