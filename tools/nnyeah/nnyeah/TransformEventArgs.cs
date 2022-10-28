using System;
using System.Text;

namespace Microsoft.MaciOS.Nnyeah {
	public class TransformEventArgs : BaseTransformEventArgs {
		public TransformEventArgs (string containerName, string methodName, string targetOperand, uint addedCount, uint removedCount)
			: base (containerName, methodName, targetOperand)
		{
			AddedCount = addedCount;
			RemovedCount = removedCount;
		}

		public uint AddedCount { get; init; }
		public uint RemovedCount { get; init; }

		public override string HelpfulMessage ()
		{
			if (RemovedCount > 0)
				return string.Format (Errors.N0004, ContainerName, MethodName, TargetOperand, AddedCount, RemovedCount);

			return string.Format (Errors.N0005, ContainerName, MethodName, TargetOperand, AddedCount);
		}
	}
}
