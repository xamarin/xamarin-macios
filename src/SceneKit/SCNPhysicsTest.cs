//
// SCNPhysicsTest.cs: Strong type helper members
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;

#nullable enable

namespace SceneKit {
	public partial class SCNPhysicsTest {
		public SCNPhysicsSearchMode SearchMode {
			get {
				var k = _SearchMode;

				if (k == SCNPhysicsTestSearchModeKeys.Any)
					return SCNPhysicsSearchMode.Any;
				if (k == SCNPhysicsTestSearchModeKeys.Closest)
					return SCNPhysicsSearchMode.Closest;
				if (k == SCNPhysicsTestSearchModeKeys.All)
					return SCNPhysicsSearchMode.All;

				return SCNPhysicsSearchMode.Unknown;
			}

			set {
				switch (value) {
				case SCNPhysicsSearchMode.All:
					_SearchMode = SCNPhysicsTestSearchModeKeys.All;
					break;
				case SCNPhysicsSearchMode.Closest:
					_SearchMode = SCNPhysicsTestSearchModeKeys.Closest;
					break;
				case SCNPhysicsSearchMode.Any:
					_SearchMode = SCNPhysicsTestSearchModeKeys.Any;
					break;
				default:
					throw new ArgumentException ("Invalid value passed to SearchMode property");
				}
			}
		}

	}
}
