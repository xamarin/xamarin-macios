// Copyright 2015 Xamarin, Inc.
using System;

using Foundation;

using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

#if MONOMAC || __MACCATALYST__

	// The kyes are not found in any of the public headers from apple. That is the reason
	// to use this technique.
	public static class NSScriptCommandArgumentDescriptionKeys {
		public static NSString AppleEventCodeKey {
			get { return NSScriptCommonKeys.AppleEventCodeKey; }
		}

		public static NSString TypeKey {
			get { return NSScriptCommonKeys.TypeKey; }
		}

		private static NSString optional = new NSString ("Optional");
		public static NSString OptionalKey {
			get { return optional; }
		}
	}

	public partial class NSScriptCommandArgumentDescription {
		public string Name { get; set; }

		public bool IsOptional { 
			get {
				return Optional is not null && Optional == "Yes";
			}
 			set {
				Optional = (value) ? "Yes" : "No";
			}
		}

		public NSScriptCommandArgumentDescription (string name, string eventCode, string type, bool isOptional=false)
		{
			if (String.IsNullOrEmpty (name))
				throw new ArgumentException ("name cannot be null or empty.");
			if (String.IsNullOrEmpty (eventCode))
				throw new ArgumentException ("eventCode cannot be null or empty");
			if (String.IsNullOrEmpty (type))
				throw new ArgumentException ("type cannot be null or empty.");

			if (eventCode.Length != 4)
				throw new ArgumentException ("eventCode must be a four char string");

			Name = name;
			AppleEventCode = eventCode;
			Type = type;
			IsOptional = isOptional;
		}

	}
	
#endif

}

