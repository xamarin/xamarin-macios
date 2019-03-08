using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	public partial class RegistrarTestGenerated {
		void AssertIfIgnored ([CallerMemberName] string testCase = null)
		{
			switch (testCase) {
#if __WATCHOS__
			case "Test_c":
			case "Test_cc":
			case "Test_ccc":
			case "Test_s":
			case "Test_sss":
				Assert.Ignore ("https://github.com/mono/mono/issues/8486");
				break;
#endif
			default:
				break;
			}
		}
	}
}
