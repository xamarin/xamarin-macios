using System;
using System.Collections.Generic;

namespace Extrospection {
	
	// Stolen from Sebastien's introspection work because it is awesome
	public static class Helpers {

		// the original name can be lost and, if not registered (e.g. enums), might not be available
		static Dictionary<string,string> map = new Dictionary<string, string> () {
			{ "GKErrorCode", "GKError" },
			{ "HMCharacteristicValueLockMechanismLastKnownAction", "HMCharacteristicValueLockMechanism" },
			{ "HMErrorCode", "HMError" },
			{ "LAError", "LAStatus" },
			{ "MCErrorCode", "MCError" },
			{ "MTLCPUCacheMode", "MTLCpuCacheMode" },
			{ "NEVPNError", "NEVpnError" },
			{ "NEVPNIKEAuthenticationMethod", "NEVpnIkeAuthenticationMethod" },
			{ "NEVPNIKEv2CertificateType", "NEVpnIke2CertificateType" },
			{ "NEVPNIKEv2DeadPeerDetectionRate", "NEVpnIke2DeadPeerDetectionRate" },
			{ "NEVPNIKEv2DiffieHellmanGroup", "NEVpnIke2DiffieHellman" },
			{ "NEVPNIKEv2EncryptionAlgorithm", "NEVpnIke2EncryptionAlgorithm" },
			{ "NEVPNIKEv2IntegrityAlgorithm", "NEVpnIke2IntegrityAlgorithm" },
			{ "NEVPNStatus", "NEVpnStatus" },
			{ "NSLineBreakMode", "UILineBreakMode" },
			{ "NSTextAlignment", "UITextAlignment" },
			{ "NSURLBookmarkCreationOptions", "NSUrlBookmarkCreationOptions" },
			{ "NSURLBookmarkResolutionOptions", "NSUrlBookmarkResolutionOptions" },
			{ "NSURLCacheStoragePolicy", "NSUrlCacheStoragePolicy" },
			{ "NSURLCredentialPersistence", "NSUrlCredentialPersistence" },
			{ "NSURLRelationship", "NSUrlRelationship" },
			{ "NSURLRequestCachePolicy", "NSUrlRequestCachePolicy" },
			{ "NSURLRequestNetworkServiceType", "NSUrlRequestNetworkServiceType" },
			{ "NSURLSessionAuthChallengeDisposition", "NSUrlSessionAuthChallengeDisposition" },
			{ "NSURLSessionResponseDisposition", "NSUrlSessionResponseDisposition" },
			{ "NSURLSessionTaskState", "NSUrlSessionTaskState" },
			{ "SSReadingListErrorCode", "SSReadingListError" },
			{ "UIDataDetectorTypes", "UIDataDetectorType" },
			{ "WatchKitErrorCode", "WKErrorCode" }, // WebKit already had that name
		};

		public static string GetManagedName (string nativeName)
		{
			string result;
			map.TryGetValue (nativeName, out result);
			return result ?? nativeName;
		}
	}
}

