#if IOS
#if !__MACOS__

using Foundation;
using Intents;
using ObjCRuntime;

#nullable enable

namespace Intents {

	public partial class INStartWorkoutIntent {

		public INStartWorkoutIntent (INSpeakableString workoutName, NSNumber goalValue, INWorkoutGoalUnitType workoutGoalUnitType, INWorkoutLocationType workoutLocationType, bool? isOpenEnded) :
			this (workoutName, goalValue, workoutGoalUnitType, workoutLocationType, isOpenEnded.HasValue ? new NSNumber (isOpenEnded.Value) : null)
		{
		}

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public bool? IsOpenEnded {
			get { return _IsOpenEnded is null ? null : (bool?) _IsOpenEnded.BoolValue; }
		}
	}
}

#endif // !__MACOS__
#endif
