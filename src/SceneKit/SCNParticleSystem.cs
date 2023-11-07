//
// SCNParticleSystem.cs: extensions to SCNParticleSystem
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright Xamarin Inc.
//

using System;
using System.Collections;
using System.Collections.Generic;

using Foundation;
using System.Runtime.Versioning;

#nullable enable

namespace SceneKit {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class SCNPropertyControllers {
		NSMutableDictionary? mutDict;
		internal NSDictionary dict;

		internal SCNPropertyControllers (NSDictionary dict)
		{
			this.dict = dict;
			if (dict is NSMutableDictionary)
				mutDict = (NSMutableDictionary) dict;
		}

		public SCNPropertyControllers ()
		{
			mutDict = new NSMutableDictionary ();
			dict = mutDict;
		}

		internal void Set (NSString key, SCNParticlePropertyController? value)
		{
			if (mutDict is null) {
				mutDict = new NSMutableDictionary (dict);
				dict = mutDict;
			}
			mutDict [key] = value;
		}

		public SCNParticlePropertyController? Position {
			get {
				return dict [SCNParticleProperty.Position] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Position, value);
			}
		}

		public SCNParticlePropertyController? Angle {
			get {
				return dict [SCNParticleProperty.Angle] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Angle, value);
			}
		}

		public SCNParticlePropertyController? RotationAxis {
			get {
				return dict [SCNParticleProperty.RotationAxis] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.RotationAxis, value);
			}
		}

		public SCNParticlePropertyController? Velocity {
			get {
				return dict [SCNParticleProperty.Velocity] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Velocity, value);
			}
		}

		public SCNParticlePropertyController? AngularVelocity {
			get {
				return dict [SCNParticleProperty.AngularVelocity] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.AngularVelocity, value);
			}
		}

		public SCNParticlePropertyController? Life {
			get {
				return dict [SCNParticleProperty.Life] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Life, value);
			}
		}

		public SCNParticlePropertyController? Color {
			get {
				return dict [SCNParticleProperty.Color] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Color, value);
			}
		}

		public SCNParticlePropertyController? Opacity {
			get {
				return dict [SCNParticleProperty.Opacity] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Opacity, value);
			}
		}

		public SCNParticlePropertyController? Size {
			get {
				return dict [SCNParticleProperty.Size] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Size, value);
			}
		}

		public SCNParticlePropertyController? Frame {
			get {
				return dict [SCNParticleProperty.Frame] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Frame, value);
			}
		}

		public SCNParticlePropertyController? FrameRate {
			get {
				return dict [SCNParticleProperty.FrameRate] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.FrameRate, value);
			}
		}

		public SCNParticlePropertyController? Bounce {
			get {
				return dict [SCNParticleProperty.Bounce] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Bounce, value);
			}
		}

		public SCNParticlePropertyController? Charge {
			get {
				return dict [SCNParticleProperty.Charge] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Charge, value);
			}
		}

		public SCNParticlePropertyController? Friction {
			get {
				return dict [SCNParticleProperty.Friction] as SCNParticlePropertyController;
			}
			set {
				Set (SCNParticleProperty.Friction, value);
			}
		}

	}

	public partial class SCNParticleSystem {
		public SCNPropertyControllers? PropertyControllers {
			get {
				var weak = WeakPropertyControllers;
				if (weak is null)
					return null;
				return new SCNPropertyControllers (weak);
			}
			set {
				WeakPropertyControllers = value?.dict;
			}
		}
	}
}
