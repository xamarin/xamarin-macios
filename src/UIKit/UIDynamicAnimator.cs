//
// UIDynamicAnimator.cs: Extension methods to improve the API on UIDynamicAnimator
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2013 Xamarin Inc
//

#if !WATCH

using System.Collections;
using System.Collections.Generic;

namespace UIKit {
	public partial class UIDynamicAnimator :
	IEnumerable<UIDynamicBehavior>
	{
		public void AddBehaviors (params UIDynamicBehavior [] behaviors)
		{
			foreach (var behavior in behaviors)
				AddBehavior (behavior);
		}

		public void RemoveBehaviors (params UIDynamicBehavior [] behaviors)
		{
			foreach (var behavior in behaviors)
				RemoveBehavior (behavior);
		}

		public void Add (UIDynamicBehavior behavior)
		{
			AddBehavior (behavior);
		}

		IEnumerator<UIDynamicBehavior> IEnumerable<UIDynamicBehavior>.GetEnumerator ()
		{
			foreach (var behavior in Behaviors)
				yield return behavior;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			foreach (var behavior in Behaviors)
				yield return behavior;
		}
	}
}

#endif // WATCH
