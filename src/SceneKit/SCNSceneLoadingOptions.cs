using System;

#nullable enable

namespace SceneKit {
	public partial class SCNSceneLoadingOptions {
		public SCNAnimationImportPolicy AnimationImportPolicy {
			get {
				var k = _AnimationImportPolicyKey;

				if (k == SCNSceneSourceLoading.AnimationImportPolicyPlay)
					return SCNAnimationImportPolicy.Play;
				if (k == SCNSceneSourceLoading.AnimationImportPolicyPlayRepeatedly)
					return SCNAnimationImportPolicy.PlayRepeatedly;
				if (k == SCNSceneSourceLoading.AnimationImportPolicyDoNotPlay)
					return SCNAnimationImportPolicy.DoNotPlay;
				if (k == SCNSceneSourceLoading.AnimationImportPolicyPlayUsingSceneTimeBase)
					return SCNAnimationImportPolicy.PlayUsingSceneTimeBase;

				return SCNAnimationImportPolicy.Unknown;
			}

			set {
				switch (value) {
				case SCNAnimationImportPolicy.Play:
					_AnimationImportPolicyKey = SCNSceneSourceLoading.AnimationImportPolicyPlay;
					break;
				case SCNAnimationImportPolicy.PlayRepeatedly:
					_AnimationImportPolicyKey = SCNSceneSourceLoading.AnimationImportPolicyPlayRepeatedly;
					break;
				case SCNAnimationImportPolicy.DoNotPlay:
					_AnimationImportPolicyKey = SCNSceneSourceLoading.AnimationImportPolicyDoNotPlay;
					break;
				case SCNAnimationImportPolicy.PlayUsingSceneTimeBase:
					_AnimationImportPolicyKey = SCNSceneSourceLoading.AnimationImportPolicyPlayUsingSceneTimeBase;
					break;
				default:
					throw new ArgumentException ("Invalid value passed to AnimationImportPolicy property");
				}
			}
		}
	}
}
