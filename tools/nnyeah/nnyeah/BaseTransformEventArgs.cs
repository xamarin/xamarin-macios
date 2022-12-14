using System;

namespace Microsoft.MaciOS.Nnyeah {
	public abstract class BaseTransformEventArgs : EventArgs {
		public BaseTransformEventArgs (string containerName, string methodName, string targetOperand)
		{
			ContainerName = containerName;
			MethodName = methodName;
			TargetOperand = targetOperand;
		}

		public string ContainerName { get; init; }
		public string MethodName { get; init; }
		public string TargetOperand { get; init; }

		public abstract string HelpfulMessage ();
	}
}
