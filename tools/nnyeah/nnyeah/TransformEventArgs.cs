using System;
using System.Text;

#nullable enable

namespace nnyeah {
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
			var sb = new StringBuilder ();
			sb.Append ($"In {ContainerName}.{MethodName}, found reference to {TargetOperand}. Added {AddedCount} IL instructions");
			if (RemovedCount > 0)
				sb.Append ($" and removed {RemovedCount} IL instructions");
			sb.Append ('.');
			return sb.ToString ();
		}
	}
}
