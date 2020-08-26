//
// VNRecognizedPointsObservation.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;

namespace Vision {
	public partial class VNRecognizedPointsObservation {

		public T [] GetAvailableKeys<T> () where T : Enum
		{
			var type = typeof (T);
			if (!(type == typeof (VNHumanBodyPoseObservationJointName) || type == typeof (VNHumanHandPoseObservationJointName)))
				throw new InvalidOperationException ($"Only '{nameof (VNHumanBodyPoseObservationJointName)}' and '{nameof (VNHumanHandPoseObservationJointName)}' are supported.");

			NSString[] keys = AvailableKeys;
			if (keys == null)
				return null;

			var count = keys.Length;
			var availableKeys = new T [count];

			if (count == 0)
				return availableKeys;

			if (availableKeys is VNHumanBodyPoseObservationJointName) {
				for (int i = 0; i < count; i++)
					availableKeys[i] = (T) (object) VNHumanBodyPoseObservationJointNameExtensions.GetValue (keys[i]);
			} else if (availableKeys is VNHumanHandPoseObservationJointName) {
				for (int i = 0; i < count; i++)
					availableKeys[i] = (T) (object) VNHumanHandPoseObservationJointNameExtensions.GetValue (keys[i]);
			}

			return availableKeys;
		}

		public T [] GetAvailableGroupKeys<T> () where T : Enum
		{
			var type = typeof (T);
			if (!(type == typeof (VNHumanBodyPoseObservationJointsGroupName) || type == typeof (VNHumanHandPoseObservationJointsGroupName)))
				throw new InvalidOperationException ($"Only '{nameof (VNHumanBodyPoseObservationJointsGroupName)}' and '{nameof (VNHumanHandPoseObservationJointsGroupName)}' are supported.");

			NSString[] keys = AvailableGroupKeys;
			if (keys == null)
				return null;

			var count = keys.Length;
			var availableGroupKeys = new T [count];

			if (count == 0)
				return availableGroupKeys;

			if (availableGroupKeys is VNHumanBodyPoseObservationJointsGroupName) {
				for (int i = 0; i < count; i++)
					availableGroupKeys[i] = (T) (object) VNHumanBodyPoseObservationJointsGroupNameExtensions.GetValue (keys[i]);
			} else if (availableGroupKeys is VNHumanHandPoseObservationJointsGroupName) {
				for (int i = 0; i < count; i++)
					availableGroupKeys[i] = (T) (object) VNHumanHandPoseObservationJointsGroupNameExtensions.GetValue (keys[i]);
			}

			return availableGroupKeys;
		}
	}
}
