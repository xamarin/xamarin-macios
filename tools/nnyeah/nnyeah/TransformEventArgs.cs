using System;
using System.Text;

#nullable enable

namespace nnyeah {
	public class TransformEventArgs : BaseTransformEventArgs {
		public TransformEventArgs (string containerName, string methodName, string targetOperand, int addedCount, int removedCount)
			: base (containerName, methodName, targetOperand)
		{
			if (addedCount < 0)
				throw new ArgumentOutOfRangeException (nameof (addedCount));
			if (removedCount < 0)
				throw new ArgumentOutOfRangeException (nameof (removedCount));
			AddedCount = addedCount;
			RemovedCount = removedCount;
		}

		public int AddedCount { get; private set; }
		public int RemovedCount { get; private set; }

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
