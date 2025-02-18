using ObjCRuntime;
using Foundation;

#nullable enable

namespace BusinessChat {

	[MacCatalyst (13, 1)]
	[Native]
	public enum BCChatButtonStyle : long {
		/// <summary>To be added.</summary>
		Light = 0,
		/// <summary>To be added.</summary>
		Dark,
	}

	[Deprecated (PlatformName.MacOSX, 13, 1)]
	[Deprecated (PlatformName.iOS, 16, 2)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 16, 2)]
	public enum BCParameterName {

		/// <summary>To be added.</summary>
		[Field ("BCParameterNameIntent")]
		Intent,

		/// <summary>To be added.</summary>
		[Field ("BCParameterNameGroup")]
		Group,

		/// <summary>To be added.</summary>
		[Field ("BCParameterNameBody")]
		Body,
	}
}
