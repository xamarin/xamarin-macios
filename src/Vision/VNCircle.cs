//
// VNCircle.cs
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
	public partial class VNCircle {

		public static VNCircle CreateUsingRadius (VNPoint center, double radius)
		{
			var circle = new VNCircle (NSObjectFlag.Empty);
			circle.InitializeHandle (circle.InitWithCenterRadius (center, radius));
			return circle;
		}

		public static VNCircle CreateUsingDiameter (VNPoint center, double diameter)
		{
			var circle = new VNCircle (NSObjectFlag.Empty);
			circle.InitializeHandle (circle.InitWithCenterDiameter (center, diameter));
			return circle;
		}
	}
}