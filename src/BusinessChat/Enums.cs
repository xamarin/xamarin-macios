using ObjCRuntime;
using Foundation;

#nullable enable

namespace BusinessChat {

	[iOS (11, 3)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum BCChatButtonStyle : long {
		Light = 0,
		Dark,
	}

	[Deprecated (PlatformName.MacOSX, 13, 1)]
	[Deprecated (PlatformName.iOS, 16, 2)]
	[iOS (11, 3)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 16, 2)]
	public enum BCParameterName {

		[Field ("BCParameterNameIntent")]
		Intent,

		[Field ("BCParameterNameGroup")]
		Group,

		[Field ("BCParameterNameBody")]
		Body,
	}
}
