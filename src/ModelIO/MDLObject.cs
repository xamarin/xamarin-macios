#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.ObjCRuntime;
namespace XamCore.ModelIO {
	public partial class MDLObject {
		[iOS (10,3), TV (10,2), Mac (10,12,4)]
		public IMDLComponent this [Protocol key] {
			get {
				return ObjectForKeyedSubscript (key);
			}
			set {
				SetObject (value, key);
			}
		}
	}
}
#endif