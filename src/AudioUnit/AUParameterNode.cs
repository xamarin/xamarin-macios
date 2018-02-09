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

			IntPtr observerToken = TokenByAddingParameterObserver (observer);

			return new AUParameterObserverToken {
				ObserverToken = observerToken
			};
		}

		public AUParameterObserverToken CreateTokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

			IntPtr observerToken = TokenByAddingParameterRecordingObserver (observer);

			return new AUParameterObserverToken {
				ObserverToken = observerToken
			};
		}

		public void RemoveParameterObserver (AUParameterObserverToken token)
		{
			RemoveParameterObserver (token.ObserverToken);
		}
	}
}

#endif
