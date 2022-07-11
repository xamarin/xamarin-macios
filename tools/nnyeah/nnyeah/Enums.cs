using System;
namespace Microsoft.MaciOS.Nnyeah {
	public enum TransformationAction {
		Remove,
		Replace,
		Insert,
		Append,
		Warn,
	}

	public enum PlatformName {
		None, // desktop managed executable
		macOS, // Xamarin.Mac app
		iOS,
		watchOS,
		tvOS,
	}
}
