using System;

#nullable enable

namespace nnyeah {
	public class WarningEventArgs : BaseTransformEventArgs {
		public WarningEventArgs (string containerName, string methodName, string targetOperand, string message)
			: base (containerName, methodName, targetOperand)
		{
			Message = message;
		}

		public string Message { get; set; }

		public override string HelpfulMessage ()
		{
			return $"In {ContainerName}.{MethodName}, found reference to {TargetOperand} - this is not transformable and will likely not work if invoked.";
		}
	}
}
