using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.BusinessChat {

	[Mac (10,13, onlyOn64: true), iOS (11,3)]
	[Native]
	public enum BCChatButtonStyle : nint {
		Light = 0,
		Dark,
	}

	[Mac (10,13, onlyOn64: true), iOS (11, 3)]
	public enum BCParameterName {

		[Field ("BCParameterNameIntent")]
		Intent,

		[Mac (10, 13, 4, onlyOn64: true), iOS (11, 3)]
		[Field ("BCParameterNameGroup")]
		Group,

		[Mac (10, 13, 4, onlyOn64: true), iOS (11, 3)]
		[Field ("BCParameterNameBody")]
		Body,
	}
}
