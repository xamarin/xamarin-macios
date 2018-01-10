using System;

namespace HealthKit {

#if !XAMCORE_3_0
	public partial class HKStatisticsCollectionQuery {
		
		[Obsolete ("Use 'InitialResultsHandler'.")]
		public virtual void SetInitialResultsHandler (HKStatisticsCollectionQueryInitialResultsHandler handler)
		{
			InitialResultsHandler = handler;
		}

		[Obsolete ("Use 'StatisticsUpdated', this handler uses an incorrect type and does nothing to avoid a crash.")]
		public virtual void SetStatisticsUpdateHandler (HKStatisticsCollectionQueryInitialResultsHandler handler)
		{
		}

		[Obsolete ("Use 'StatisticsUpdated', this handler uses an incorrect type and does nothing to avoid a crash.")]
		public virtual HKStatisticsCollectionQueryInitialResultsHandler StatisticsUpdateHandler {
			get; set;
		}
	}

	public partial class HKObjectType {

		[Obsolete ("Use 'GetWorkoutType', it will return a valid HKWorkoutType instance.")]
		static public HKWorkout WorkoutType ()
		{
			// would throw an InvalidCastException since the old selector returned a HKWorkoutType
			return null;
		}
	}
#endif
}
