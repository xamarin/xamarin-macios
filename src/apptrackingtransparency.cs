//
// AppTrackingTransparency.cs
//
// Authors:
//   Dorothy Tam (dottam@gmail.com)
//

namespace AppTrackingTransparency {

	[iOS (14, 0), TV (14, 0)]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Native]
	public enum ATTrackingManagerAuthorizationStatus : ulong {
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Authorized = 3,
	}

	[iOS (14, 0), TV (14, 0)]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ATTrackingManager {
		[Static, Export ("requestTrackingAuthorizationWithCompletionHandler:")]
		[Async]
		void RequestTrackingAuthorization (Action<ATTrackingManagerAuthorizationStatus> completion);

		/// <summary>Requests tracking authorization, optionally preferring the expanded consent interface.</summary>
		/// <param name="preferExpandedInterface">Whether to prefer the expanded interface where available.</param>
		/// <param name="additionalInformationAction">An action for the additional-information button, or <see langword="null" /> to omit the button.</param>
		/// <param name="completion">Receives the resulting tracking authorization status.</param>
		/// <remarks>The app must declare NSUserTrackingUsageDescription. Selecting the additional-information button dismisses the prompt without recording a decision and completes with NotDetermined.</remarks>
		[iOS (27, 2), MacCatalyst (27, 2), NoMac, NoTV]
		[Static, Export ("requestTrackingAuthorizationPreferringExpandedInterface:additionalInformationAction:completionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously requests tracking authorization, optionally preferring the expanded consent interface.</summary>
			<param name="preferExpandedInterface">Whether to prefer the expanded interface where available.</param>
			<param name="additionalInformationAction">An action for the additional-information button, or <see langword="null" /> to omit the button.</param>
			<returns>A task that returns the resulting tracking authorization status.</returns>
			<remarks>The app must declare NSUserTrackingUsageDescription. Selecting the additional-information button dismisses the prompt without recording a decision and completes with NotDetermined.</remarks>
			""")]
		void RequestTrackingAuthorization (bool preferExpandedInterface, [NullAllowed] Action additionalInformationAction, Action<ATTrackingManagerAuthorizationStatus> completion);

		[Static, Export ("trackingAuthorizationStatus", ArgumentSemantic.Assign)]
		ATTrackingManagerAuthorizationStatus TrackingAuthorizationStatus { get; }
	}
}
