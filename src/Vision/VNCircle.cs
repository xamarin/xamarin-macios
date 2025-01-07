//
// VNCircle.cs
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
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="VNCircle" />.</summary>
	public enum VNCircleInitializationOption {
		/// <summary>The <c>radiusOrDiameter</c> parameter passed to the constructor is the radius of the circle.</summary>
		Radius,
		/// <summary>The <c>radiusOrDiameter</c> parameter passed to the constructor is the diameter of the circle.</summary>
		Diameter,
	}

	public partial class VNCircle {
		/// <summary>Create a new <see cref="VNCircle" /> instance.</summary>
		/// <param name="center">The center of the circle.</param>
		/// <param name="radiusOrDiameter">The radius or diameter of the circle. Use <paramref name="option" /> to specify which.</param>
		/// <param name="option">Specifies whether <paramref name="radiusOrDiameter" /> is a neutral or a camera value.</param>
		public VNCircle (VNPoint center, double radiusOrDiameter, VNCircleInitializationOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case VNCircleInitializationOption.Radius:
				InitializeHandle (_InitWithCenterRadius (center, radiusOrDiameter));
				break;
			case VNCircleInitializationOption.Diameter:
				InitializeHandle (_InitWithCenterDiameter (center, radiusOrDiameter));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}

		/// <summary>Create a new <see cref="VNCircle" /> instance.</summary>
		/// <param name="center">The center of the circle.</param>
		/// <param name="radius">The radius of the circle.</param>
		public static VNCircle CreateUsingRadius (VNPoint center, double radius)
		{
			return new VNCircle (center, radius, VNCircleInitializationOption.Radius);
		}

		/// <summary>Create a new <see cref="VNCircle" /> instance.</summary>
		/// <param name="center">The center of the circle.</param>
		/// <param name="diameter">The diameter of the circle.</param>
		public static VNCircle CreateUsingDiameter (VNPoint center, double diameter)
		{
			return new VNCircle (center, diameter, VNCircleInitializationOption.Diameter);
		}
	}
}
