#if XAMCORE_2_0 || !MONOMAC

using System;

namespace XamCore.AudioUnit
{
	public partial class AUParameterNode
	{
		public AUParameterObserverToken CreateTokenByAddingParameterObserver (AUParameterObserver observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

			IntPtr observerToken = _TokenByAddingParameterObserver (observer);

			return new AUParameterObserverToken {
				ObserverToken = observerToken
			};
		}

		public AUParameterObserverToken CreateTokenByAddingParameterRecordingObserver (AUParameterRecordingObserver observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

			IntPtr observerToken = _TokenByAddingParameterRecordingObserver (observer);

			return new AUParameterObserverToken {
				ObserverToken = observerToken
			};
		}

		public void RemoveParameterObserver (AUParameterObserverToken token)
		{
			_RemoveParameterObserver (token.ObserverToken);
		}
	}
}

#endif
