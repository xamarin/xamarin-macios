// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.Messaging.Build.Contracts {
	[Topic ("xi/build/get-UTI")]
	public class GetUniversalTypeIdentifierMessage {
		public byte [] Payload { get; set; }

		public string FileName { get; set; }
	}
}
