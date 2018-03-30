using ObjCRuntime;
using Foundation;

namespace BusinessChat {

	[Mac (10,13,4, onlyOn64: true), iOS (11,3)]
	[Native]
	public enum BCChatButtonStyle : long {
		Light = 0,
		Dark,
	}

	[Mac (10,13,4, onlyOn64: true), iOS (11,3)]
	public enum BCParameterName {

		[Field ("BCParameterNameIntent")]
		Intent,

		[Field ("BCParameterNameGroup")]
		Group,

		[Field ("BCParameterNameBody")]
		Body,
	}
}
