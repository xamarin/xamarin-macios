using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	public partial class TrampolineTestGenerated {
		void AssertIfIgnored ([CallerMemberName] string testCase = null)
		{
			switch (testCase) {
			default:
				break;
			}
		}
	}
}
