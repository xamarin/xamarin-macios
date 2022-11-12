#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using Foundation;
using SceneKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SCNMaterialTests {
		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureMavericks ();
			if (Asserts.IsAtLeastElCapitan)
				Asserts.Ensure64Bit ();
		}

		[Test]
		public void SCNMaterial_ShaderModifierTest_Weak ()
		{
			if (IntPtr.Size == 8) // API is 64-bit only
			{
				SCNMaterial m = new SCNMaterial ();
				m.WeakShaderModifiers = new NSDictionary ();
			}
		}

		[Test]
		public void SCNMaterial_ShaderModifierTest ()
		{
			if (IntPtr.Size == 8) // API is 64-bit only
			{
				SCNMaterial m = new SCNMaterial ();
				m.ShaderModifiers = new SCNShaderModifiers ();
			}
		}
	}
}
#endif // __MACOS__
