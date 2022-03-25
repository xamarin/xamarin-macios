using System;

#nullable enable

namespace nnyeah {
	public abstract class BaseTransformEventArgs : EventArgs {
		public BaseTransformEventArgs (string containerName, string methodName, string targetOperand)
		{
			ContainerName = containerName;
			MethodName = methodName;
			TargetOperand = targetOperand;
		}

		public string ContainerName { get; private set; }
		public string MethodName { get; private set; }
		public string TargetOperand { get; private set; }

		public abstract string HelpfulMessage ();
	}
}
