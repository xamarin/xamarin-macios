using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.Foundation;
using MonoMac.SceneKit;

#else
using Foundation;
using SceneKit;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class SCNSceneTests
	{
		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureYosemite ();
			if (Asserts.IsAtLeastElCapitan)
				Asserts.Ensure64Bit ();
		}

		[Test]
		public void SCNSceneLoadingOptions_AnimationImportPolicyTest ()
		{
			SCNSceneLoadingOptions o = new SCNSceneLoadingOptions ();
			RoundTrip (o, SCNAnimationImportPolicy.Play);
			RoundTrip (o, SCNAnimationImportPolicy.PlayRepeatedly);
			RoundTrip (o, SCNAnimationImportPolicy.DoNotPlay);
			RoundTrip (o, SCNAnimationImportPolicy.PlayUsingSceneTimeBase);
		}

		[Test]
		public void SCNSceneLoadingOptions_AnimationImportPolicyTestKeysNonNull ()
		{
			Assert.IsNotNull (SCNSceneSourceLoading.AnimationImportPolicyPlay);
			Assert.IsNotNull (SCNSceneSourceLoading.AnimationImportPolicyPlayRepeatedly);
			Assert.IsNotNull (SCNSceneSourceLoading.AnimationImportPolicyDoNotPlay);
			Assert.IsNotNull (SCNSceneSourceLoading.AnimationImportPolicyPlayUsingSceneTimeBase);
		}

		void RoundTrip (SCNSceneLoadingOptions o, SCNAnimationImportPolicy policy)
		{
			o.AnimationImportPolicy = policy;
			Assert.IsTrue (o.AnimationImportPolicy == policy);
		}
	}
}