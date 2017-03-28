using System;
using System.Collections.Generic;
using System.Text;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {
	
	public static class Helpers {

		// the original name can be lost and, if not registered (e.g. enums), might not be available
		static Dictionary<string,string> map = new Dictionary<string, string> () {
			{ "CFURLPathStyle", "CFUrlPathStyle" },
			{ "CXPlayDTMFCallActionType", "CXPlayDtmfCallActionType" },
			{ "EABluetoothAccessoryPickerErrorCode", "EABluetoothAccessoryPickerError" },
			{ "EKCalendarEventAvailabilityMask", "EKCalendarEventAvailability" },
			{ "GKErrorCode", "GKError" },
			{ "HMCharacteristicValueAirParticulateSize", "HMCharacteristicValueAirParticulate" },
			{ "HMCharacteristicValueLockMechanismLastKnownAction", "HMCharacteristicValueLockMechanism" },
			{ "HMErrorCode", "HMError" },
			{ "LAError", "LAStatus" },
			{ "MCErrorCode", "MCError" },
			{ "MIDINotificationMessageID", "MidiNotificationMessageId" },
			{ "MIDIObjectType", "MidiObjectType" },
			{ "MIDITransformControlType", "MidiTransformControlType" },
			{ "MIDITransformType", "MidiTransformType" },
			{ "MIDINetworkConnectionPolicy", "MidiNetworkConnectionPolicy" },
			{ "MPMovieMediaTypeMask", "MPMovieMediaType" },
			{ "MPSCNNConvolutionFlags", "MPSCnnConvolutionFlags" },
			{ "MTLCPUCacheMode", "MTLCpuCacheMode" },
			{ "NEVPNError", "NEVpnError" },
			{ "NEVPNIKEAuthenticationMethod", "NEVpnIkeAuthenticationMethod" },
			{ "NEVPNIKEv2CertificateType", "NEVpnIke2CertificateType" },
			{ "NEVPNIKEv2DeadPeerDetectionRate", "NEVpnIke2DeadPeerDetectionRate" },
			{ "NEVPNIKEv2DiffieHellmanGroup", "NEVpnIke2DiffieHellman" },
			{ "NEVPNIKEv2EncryptionAlgorithm", "NEVpnIke2EncryptionAlgorithm" },
			{ "NEVPNIKEv2IntegrityAlgorithm", "NEVpnIke2IntegrityAlgorithm" },
			{ "NEVPNStatus", "NEVpnStatus" },
			{ "NSAttributedStringEnumerationOptions", "NSAttributedStringEnumeration" },
			{ "NSHTTPCookieAcceptPolicy", "NSHttpCookieAcceptPolicy" },
			{ "NSISO8601DateFormatOptions", "NSIso8601DateFormatOptions" },
			{ "NSJSONReadingOptions", "NSJsonReadingOptions" },
			{ "NSJSONWritingOptions", "NSJsonWritingOptions" },
			{ "NSUbiquitousKeyValueStoreChangeReason", "NSUbiquitousKeyValueStore" },
			{ "NSURLBookmarkCreationOptions", "NSUrlBookmarkCreationOptions" },
			{ "NSURLBookmarkResolutionOptions", "NSUrlBookmarkResolutionOptions" },
			{ "NSURLCacheStoragePolicy", "NSUrlCacheStoragePolicy" },
			{ "NSURLCredentialPersistence", "NSUrlCredentialPersistence" },
			{ "NSURLRelationship", "NSUrlRelationship" },
			{ "NSURLRequestCachePolicy", "NSUrlRequestCachePolicy" },
			{ "NSURLRequestNetworkServiceType", "NSUrlRequestNetworkServiceType" },
			{ "NSURLSessionAuthChallengeDisposition", "NSUrlSessionAuthChallengeDisposition" },
			{ "NSURLSessionResponseDisposition", "NSUrlSessionResponseDisposition" },
			{ "NSURLSessionTaskMetricsResourceFetchType", "NSUrlSessionTaskMetricsResourceFetchType" },
			{ "NSURLSessionTaskState", "NSUrlSessionTaskState" },
			{ "NWTCPConnectionState", "NWTcpConnectionState" },
			{ "NWUDPSessionState", "NWUdpSessionState" },
			{ "RPRecordingErrorCode", "RPRecordingError" },
			{ "SecTrustResultType", "SecTrustResult" },
			{ "SKErrorCode", "SKError" },
			{ "SSReadingListErrorCode", "SSReadingListError" },
			{ "SCNRenderingAPI", "SCNRenderingApi" },
			{ "UIDataDetectorTypes", "UIDataDetectorType" },
			{ "UIControlEvents", "UIControlEvent" },
			{ "UITableViewCellAccessoryType", "UITableViewCellAccessory" },
			{ "UITableViewCellStateMask", "UITableViewCellState" },
			{ "WatchKitErrorCode", "WKErrorCode" }, // WebKit already had that name
			// not enums
			{ "NSMutableURLRequest", "NSMutableUrlRequest" },
		};

		public static string GetManagedName (string nativeName)
		{
			string result;
			map.TryGetValue (nativeName, out result);
			return result ?? nativeName;
		}

		public static string ReplaceFirstInstance (this string source, string find, string replace)
		{
			int index = source.IndexOf (find, StringComparison.Ordinal);
			return index < 0 ? source : source.Substring (0, index) + replace + source.Substring (index + find.Length);
		}

		public static string Platform { get; set; }

		public static bool IsAvailable (this Decl decl)
		{
			// there's no doubt we need to ask for the current platform
			var result = decl.IsAvailable (Platform);

			// some categories are not decorated (as not available) but they extend types that are
			if (!result.HasValue) {
				var category = (decl.DeclContext as ObjCCategoryDecl);
				if (category != null)
					result = category.ClassInterface.IsAvailable (Platform);
			}
				
			// but right now most frameworks consider tvOS and watchOS like iOS unless 
			// decorated otherwise so we must check again if we do not get a definitve answer
			if ((result == null) && ((Platform == "tvos") || (Platform == "watchos")))
				result = decl.IsAvailable ("ios");
			return !result.HasValue ? true : result.Value;
		}

		static bool? IsAvailable (this Decl decl, string platform)
		{
			bool? result = null;
			foreach (var attr in decl.Attrs) {
				// NS_UNAVAILABLE
				if (attr is UnavailableAttr)
					return false;
				var avail = (attr as AvailabilityAttr);
				if (avail == null)
					continue;
				// if the headers says it's not available then we won't report it as missing
				if (avail.Unavailable && (avail.Platform.Name == platform))
					return false;
				// for iOS we won't report missing members that were deprecated before 5.0
				if (!avail.Deprecated.IsEmpty && avail.Platform.Name == "ios" && avail.Deprecated.Major < 5)
					return false;
				// can't return true right away as it can be deprecated too
				if (!avail.Introduced.IsEmpty && (avail.Platform.Name == platform))
					result = true;
			}
			return result;
		}

		public static bool IsDesignatedInitializer (this MethodDefinition self)
		{
			return self.HasAttribute ("DesignatedInitializerAttribute");
		}

		public static bool IsProtocol (this TypeDefinition self)
		{
			return self.HasAttribute ("ProtocolAttribute");
		}

		static bool HasAttribute (this ICustomAttributeProvider self, string attributeName)
		{
			if (!self.HasCustomAttributes)
				return false;

			foreach (var ca in self.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Name == attributeName)
					return true;
			}
			return false;
		}

		static bool IsStatic (this TypeDefinition self)
		{
			return (self.IsSealed && self.IsAbstract);
		}

		public static string GetName (this ObjCMethodDecl self)
		{
			if (self == null)
				return null;
			
			var sb = new StringBuilder ();
			if (self.IsClassMethod)
				sb.Append ('+');
			sb.Append ((self.DeclContext as NamedDecl).Name);
			sb.Append ("::");
			sb.Append (self.Selector);
			return sb.ToString ();
		}

		public static string GetName (this TypeDefinition self)
		{
			if ((self == null) || !self.HasCustomAttributes)
				return null;

			if (self.IsStatic ()) {
				// static types, e.g. categories, won't have a [Register] attribute
				foreach (var ca in self.CustomAttributes) {
					if (ca.Constructor.DeclaringType.Name == "CategoryAttribute") {
						if (ca.HasProperties)
							return (ca.Properties [0].Argument.Value as string);
						return self.Name;
					}
				}
			} else {
				foreach (var ca in self.CustomAttributes) {
					if (ca.Constructor.DeclaringType.Name == "RegisterAttribute") {
						if (ca.HasConstructorArguments)
							return (ca.ConstructorArguments [0].Value as string);
						return self.Name;
					}
				}
			}
			return null;
		}

		public static string GetName (this MethodDefinition self)
		{
			if (self == null)
				return null;

			var type = self.DeclaringType;
			string tname = self.DeclaringType.GetName ();
			// a static type is not used for static selectors
			bool is_static = !type.IsStatic () && self.IsStatic;

			// static types, e.g. categories, won't have a [Register] attribute
			if (type.IsStatic ()) {
				if (self.HasParameters)
					tname = self.Parameters [0].ParameterType.Name; // extension method
			}
			if (tname == null)
				return null;

			var selector = self.GetSelector ();
			if (selector == null)
				return null;

			var sb = new StringBuilder ();
			if (is_static)
				sb.Append ('+');
			sb.Append (tname);
			sb.Append ("::");
			sb.Append (selector);
			return sb.ToString ();
		}

		public static string GetSelector (this MethodDefinition self)
		{
			if ((self == null) || !self.HasCustomAttributes)
				return null;

			foreach (var ca in self.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Name == "ExportAttribute")
					return ca.ConstructorArguments [0].Value as string;
			}
			return null;
		}

		public static string GetSelector (this ObjCMethodDecl self)
		{
			return self.Selector.ToString () ?? (self.IsPropertyAccessor ? self.Name : null);
		}
	}
}