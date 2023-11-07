//
// VNRecognizedPointsObservation.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace Vision {
	public partial class VNRecognizedPointsObservation {

		public T []? GetAvailableKeys<T> () where T : Enum
		{
			var type = typeof (T);
			if (!(type == typeof (VNHumanBodyPoseObservationJointName) || type == typeof (VNHumanHandPoseObservationJointName)))
				throw new InvalidOperationException ($"Only '{nameof (VNHumanBodyPoseObservationJointName)}' and '{nameof (VNHumanHandPoseObservationJointName)}' are supported.");

			NSString [] keys = AvailableKeys;
			if (keys is null)
				return null;

			if (keys.Length == 0)
				return Array.Empty<T> ();

			if (type == typeof (VNHumanBodyPoseObservationJointName))
				return Array.ConvertAll (keys, (v) => (T) (object) VNHumanBodyPoseObservationJointNameExtensions.GetValue (v));
			else if (type == typeof (VNHumanHandPoseObservationJointName))
				return Array.ConvertAll (keys, (v) => (T) (object) VNHumanHandPoseObservationJointNameExtensions.GetValue (v));
			return null;
		}

		public T []? GetAvailableGroupKeys<T> () where T : Enum
		{
			var type = typeof (T);
			if (!(type == typeof (VNHumanBodyPoseObservationJointsGroupName) || type == typeof (VNHumanHandPoseObservationJointsGroupName)))
				throw new InvalidOperationException ($"Only '{nameof (VNHumanBodyPoseObservationJointsGroupName)}' and '{nameof (VNHumanHandPoseObservationJointsGroupName)}' are supported.");

			NSString [] keys = AvailableGroupKeys;
			if (keys is null)
				return null;

			if (keys.Length == 0)
				return Array.Empty<T> ();

			if (type == typeof (VNHumanBodyPoseObservationJointsGroupName))
				return Array.ConvertAll (keys, (v) => (T) (object) VNHumanBodyPoseObservationJointsGroupNameExtensions.GetValue (v));
			else if (type == typeof (VNHumanHandPoseObservationJointsGroupName))
				return Array.ConvertAll (keys, (v) => (T) (object) VNHumanHandPoseObservationJointsGroupNameExtensions.GetValue (v));
			return null;
		}
	}
}
