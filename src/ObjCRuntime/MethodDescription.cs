using System;
using System.Reflection;

#if !XAMCORE_2_0
namespace XamCore.ObjCRuntime {
	public struct MethodDescription {
		public MethodBase method;
		public ArgumentSemantic semantic;

		public MethodDescription (MethodBase method, ArgumentSemantic semantic) {
			this.method = method;
			this.semantic = semantic;
		}
	}
}
#endif // !XAMCORE_2_0
