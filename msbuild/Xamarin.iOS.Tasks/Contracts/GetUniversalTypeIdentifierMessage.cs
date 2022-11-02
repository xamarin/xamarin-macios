namespace Xamarin.Messaging.Build.Contracts {
	[Topic ("xi/build/get-UTI")]
	public class GetUniversalTypeIdentifierMessage {
		public byte [] Payload { get; set; }

		public string FileName { get; set; }
	}
}
