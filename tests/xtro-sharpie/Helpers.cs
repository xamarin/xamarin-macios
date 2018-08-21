using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {
	
	public static class Helpers {

		// the original name can be lost and, if not registered (e.g. enums), might not be available
		static Dictionary<string,string> map = new Dictionary<string, string> () {
			{ "EABluetoothAccessoryPickerErrorCode", "EABluetoothAccessoryPickerError" },
			{ "EKCalendarEventAvailabilityMask", "EKCalendarEventAvailability" },
			{ "GKErrorCode", "GKError" },
			{ "HMCharacteristicValueAirParticulateSize", "HMCharacteristicValueAirParticulate" },
			{ "HMCharacteristicValueLockMechanismLastKnownAction", "HMCharacteristicValueLockMechanism" },
			{ "HMErrorCode", "HMError" },
			{ "LAError", "LAStatus" },
			{ "MCErrorCode", "MCError" },
			{ "MPMovieMediaTypeMask", "MPMovieMediaType" },
			{ "NEVPNIKEv2CertificateType", "NEVpnIke2CertificateType" },
			{ "NEVPNIKEv2DeadPeerDetectionRate", "NEVpnIke2DeadPeerDetectionRate" },
			{ "NEVPNIKEv2DiffieHellmanGroup", "NEVpnIke2DiffieHellman" },
			{ "NEVPNIKEv2EncryptionAlgorithm", "NEVpnIke2EncryptionAlgorithm" },
			{ "NEVPNIKEv2IntegrityAlgorithm", "NEVpnIke2IntegrityAlgorithm" },
			{ "NSAttributedStringEnumerationOptions", "NSAttributedStringEnumeration" },
			{ "NSFileProviderErrorCode", "NSFileProviderError" },
			{ "NSUbiquitousKeyValueStoreChangeReason", "NSUbiquitousKeyValueStore" },
			{ "PHLivePhotoEditingErrorCode", "PHLivePhotoEditingError" },
			{ "RPRecordingErrorCode", "RPRecordingError" },
			{ "SecTrustResultType", "SecTrustResult" },
			{ "SKErrorCode", "SKError" },
			{ "SSReadingListErrorCode", "SSReadingListError" },
			{ "UIDataDetectorTypes", "UIDataDetectorType" },
			{ "UIControlEvents", "UIControlEvent" },
			{ "UITableViewCellAccessoryType", "UITableViewCellAccessory" },
			{ "UITableViewCellStateMask", "UITableViewCellState" },
			{ "WatchKitErrorCode", "WKErrorCode" }, // WebKit already had that name
			// not enums
		};

		public static string GetManagedName (string nativeName)
		{
			map.TryGetValue (nativeName, out var result);
			return result ?? nativeName;
		}

		public static string ReplaceFirstInstance (this string source, string find, string replace)
		{
			int index = source.IndexOf (find, StringComparison.Ordinal);
			return index < 0 ? source : source.Substring (0, index) + replace + source.Substring (index + find.Length);
		}

		public enum Platforms
		{
			macOS,
			iOS,
			watchOS,
			tvOS,
		}

		public static Platforms Platform { get; set; }

		public static bool IsAvailable (this Decl decl)
		{
			// there's no doubt we need to ask for the current platform
			var result = decl.IsAvailable (Platform);

			// some categories are not decorated (as not available) but they extend types that are
			if (!result.HasValue) {
				// first check if we're checking the category itself
				var category = decl as ObjCCategoryDecl;
				if (category != null)
					result = category.ClassInterface.IsAvailable (Platform);

				if (!result.HasValue) {
					// then check if we're a method inside a category
					category = (decl.DeclContext as ObjCCategoryDecl);
					if (category != null)
						result = category.ClassInterface.IsAvailable (Platform);
				}
			}
				
			// but right now most frameworks consider tvOS and watchOS like iOS unless 
			// decorated otherwise so we must check again if we do not get a definitve answer
			if ((result == null) && ((Platform == Platforms.tvOS) || (Platform == Platforms.watchOS)))
				result = decl.IsAvailable (Platforms.iOS);
			return !result.HasValue ? true : result.Value;
		}

		static bool? IsAvailable (this Decl decl, Platforms platform_value)
		{
			var platform = platform_value.ToString ().ToLowerInvariant ();
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

		public static bool RequiresSuper (this MethodDefinition self)
		{
			return self.HasAttribute ("RequiresSuperAttribute");
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
			if (self.DeclContext is ObjCCategoryDecl category) {
				sb.Append (category.ClassInterface.Name);
			} else {
				sb.Append ((self.DeclContext as NamedDecl).Name);
			}
			sb.Append ("::");
			var sel = self.Selector.ToString ();
			sb.Append (string.IsNullOrEmpty (sel) ? self.Name : sel);
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
					} else if (ca.Constructor.DeclaringType.Name == "ProtocolAttribute") {
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
			return self.Selector.ToString () ?? self.Name;
		}

		public static bool IsObsolete (this ICustomAttributeProvider provider)
		{
			if (provider.HasCustomAttributes) {
				foreach (var attrib in provider.CustomAttributes) {
					var attribType = attrib.Constructor.DeclaringType;
					if (attribType.Namespace == "System" && attribType.Name == "ObsoleteAttribute")
						return true;
				}
			}

			// If we're a property accessor, check the property as well.
			var prop = FindProperty (provider as MethodReference);
			if (prop != null)
				return IsObsolete (prop);

			return false;
		}

		public static PropertyDefinition FindProperty (this MethodReference method)
		{
			var def = method?.Resolve ();
			if (def == null)
				return null;

			if (!def.IsSpecialName)
				return null;

			if (!def.DeclaringType.HasProperties)
				return null;

			if (!method.Name.StartsWith ("get_", StringComparison.Ordinal) && !method.Name.StartsWith ("set_", StringComparison.Ordinal))
				return null;

			var propName = method.Name.Substring (4);
			foreach (var prop in def.DeclaringType.Properties) {
				if (prop.Name == propName)
					return prop;
			}

			return null;
		}

		public static string GetFramework (TypeReference type)
		{
			var framework = type.Namespace;
			if (String.IsNullOrEmpty (framework) && type.IsNested)
				framework = type.DeclaringType.Namespace;
			return MapFramework (framework);
		}

		public static string GetFramework (MethodDefinition method)
		{
			string framework = null;
			if (method.HasPInvokeInfo)
				framework = Path.GetFileNameWithoutExtension (method.PInvokeInfo.Module.Name);
			else
				framework = GetFramework (method.DeclaringType);
			return MapFramework (framework);
		}

		public static string GetFramework (MemberReference member)
		{
			string framework = GetFramework (member.DeclaringType);
			return MapFramework (framework);
		}

		public static string GetFramework (Decl decl)
		{
			var header_file = decl.PresumedLoc.FileName;
			var fxh = header_file.IndexOf (".framework/Headers/", StringComparison.Ordinal);
			if (fxh <= 0)
				return null;
			
			var start = header_file.LastIndexOf ('/', fxh) + 1;
			return MapFramework (header_file.Substring (start, fxh - start));
		}

		public static string MapFramework (string candidate)
		{
			switch (candidate) {
			case "AVFAudio":
				return "AVFoundation";
			case "libc": // dispatch_*
				return "CoreFoundation";
			case "libobjc":
			case "libSystem": // dlopen, dlerror, dlsym, dlclose
				return "ObjCRuntime";
			case "libsystem_kernel": // getxattr, removexattr and setxattr
				return "Foundation";
			case "MPSCore":
			case "MPSImage":
			case "MPSMatrix":
			case "MPSNeuralNetwork":
			case "MPSRayIntersector":
				return "MetalPerformanceShaders";
			case "QuartzCore":
				return "CoreAnimation";
			case "OpenAL":
			case "OpenGL":
			case "OpenGLES":
			case "OpenTK.Platform.iPhoneOS":
				return "OpenGL[ES]";
			case "vImage":
				return "Accelerate";
			default:
				return candidate;
			}
		}

		public static (T, T) Sort<T> (T o1, T o2)
		{
			if (StringComparer.Ordinal.Compare (o1.ToString (), o2.ToString ()) < 0)
				return (o2, o1);
			else
				return (o1, o2);
		}
	}
}