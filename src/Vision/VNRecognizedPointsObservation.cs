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

			if (type == typeof (VNHumanBodyPoseObservationJointName))
				availableKeys = Array.ConvertAll (keys, (v) => (T) (object) VNHumanBodyPoseObservationJointNameExtensions.GetValue (v));
			else if (type == typeof (VNHumanHandPoseObservationJointName))
				availableKeys = Array.ConvertAll (keys, (v) => (T) (object) VNHumanHandPoseObservationJointNameExtensions.GetValue (v));

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

			if (type == typeof (VNHumanBodyPoseObservationJointsGroupName))
				availableGroupKeys = Array.ConvertAll (keys, (v) => (T) (object) VNHumanBodyPoseObservationJointsGroupNameExtensions.GetValue (v));
			else if (type == typeof (VNHumanHandPoseObservationJointsGroupName))
				availableGroupKeys = Array.ConvertAll (keys, (v) => (T) (object) VNHumanHandPoseObservationJointsGroupNameExtensions.GetValue (v));

			return availableGroupKeys;
		}
	}
}
