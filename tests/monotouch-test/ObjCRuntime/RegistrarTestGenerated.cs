using ObjCRuntime;
using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	public partial class RegistrarTestGenerated {
		void AssertIfIgnored ([CallerMemberName] string testCase = null)
		{
			switch (testCase) {
#if __MACCATALYST__ || __IOS__ || __TVOS__
			case "NSNumberBindAs_Boolean_Array_Overrides":
			case "NSNumberBindAs_Byte_Array_Overrides":
			case "NSNumberBindAs_Double_Array_Overrides":
			case "NSNumberBindAs_Int16_Array_Overrides":
			case "NSNumberBindAs_Int32_Array_Overrides":
			case "NSNumberBindAs_Int64_Array_Overrides":
			case "NSNumberBindAs_nint_Array_Overrides":
			case "NSNumberBindAs_NSStreamStatus_Array_Overrides":
			case "NSNumberBindAs_nuint_Array_Overrides":
			case "NSNumberBindAs_SByte_Array_Overrides":
			case "NSNumberBindAs_Single_Array_Overrides":
			case "NSNumberBindAs_UInt16_Array_Overrides":
			case "NSNumberBindAs_UInt32_Array_Overrides":
			case "NSNumberBindAs_UInt64_Array_Overrides":
				// https://github.com/xamarin/xamarin-macios/issues/19781
#if __MACCATALYST__
				if (Runtime.IsARM64CallingConvention)
#elif __IOS__ || __TVOS__
				if (Runtime.IsARM64CallingConvention && Runtime.Arch == Arch.SIMULATOR)
#endif
					Assert.Ignore ("https://github.com/xamarin/xamarin-macios/issues/19781");
				break;
#endif
			default:
				break;
			}
		}
	}
}
