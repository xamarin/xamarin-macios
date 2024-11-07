//
// VNVector.cs
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
	public partial class VNVector {
		/// <summary>Create a new <see cref="VNVector" /> instance using polar coordinates.</summary>
		/// <param name="polarCoordinates">The r and theta values of the vector.</param>
		public VNVector ((double R, double Theta) polarCoordinates)
			: base (NSObjectFlag.Empty)
		{
			InitializeHandle (_InitWithRTheta (polarCoordinates.R, polarCoordinates.Theta));
		}

		/// <summary>Create a new <see cref="VNVector" /> instance using polar coordinates.</summary>
		/// <param name="r">The r value (length) of the vector.</param>
		/// <param name="theta">The theta value (angle) of the vector.</param>
		public static VNVector Create (double r, double theta)
		{
			return new VNVector ((r, theta));
		}
	}
}
