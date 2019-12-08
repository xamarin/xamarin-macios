#if XAMCORE_2_0 || !MONOMAC

using System;

namespace AudioUnit
{
	public partial class AUParameterNode
	{
		public AUParameterObserverToken CreateTokenByAddingParameterObserver (AUParameterObserver observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

#pragma warning disable CS0618
			IntPtr observerToken = TokenByAddingParameterObserver (observer);
#pragma warning restore CS0618

			return new AUParameterObserverToken {
				ObserverToken = observerToken
			};
		}

		public AUParameterObserverToken CreateTokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

#pragma warning disable CS0618
			IntPtr observerToken = TokenByAddingParameterRecordingObserver (observer);
#pragma warning restore CS0618

			return new AUParameterObserverToken {
				ObserverToken = observerToken
			};
		}

		public void RemoveParameterObserver (AUParameterObserverToken token)
		{
#pragma warning disable CS0618
			RemoveParameterObserver (token.ObserverToken);
#pragma warning restore CS0618
		}
	}
}

#endif
