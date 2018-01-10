// Copyright 2015 Xamarin, Inc.

using System;
using Foundation;
using ObjCRuntime;

namespace Foundation {

#if MONOMAC

	// The kyes are not found in any of the public headers from apple. That is the reason
	// to use this technique.
	static class NSScriptCommonKeys {
		private static NSString appEventCode = new NSString ("AppleEventCode"); 
		public static NSString AppleEventCodeKey {
			get { return appEventCode; }
		}
		
		private static NSString typeKey = new NSString ("Type");
		public static NSString TypeKey {
			get { return typeKey; }
		}
	}

	public partial class NSScriptCommandDescription {

		NSScriptCommandDescriptionDictionary description = null;

		static string ToFourCCString (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}
		
		static int ToIntValue (string fourCC)
		{
			if (fourCC.Length != 4)
				throw new FormatException (string.Format ("{0} must have a lenght of 4", nameof (fourCC)));
			int ret = 0;
			for (int i = 0; i < 4; i++) {
				ret <<= 8;
				ret |= fourCC[i];
			}
			return ret;
		}

		public static NSScriptCommandDescription Create (string suiteName, string commandName, NSScriptCommandDescriptionDictionary commandDeclaration)
		{
			if (String.IsNullOrEmpty (suiteName))
				throw new ArgumentException ("suiteName cannot be null or empty");
			if (String.IsNullOrEmpty (commandName))
				throw new ArgumentException ("commandName cannot be null or empty");
			if (commandDeclaration == null)
				throw new ArgumentNullException ("commandDeclaration");

			// ensure that the passed description is well formed
			if (String.IsNullOrEmpty (commandDeclaration.CommandClass))
				throw new ArgumentException ("cmdClass");
			if (String.IsNullOrEmpty (commandDeclaration.AppleEventCode))
				throw new ArgumentException ("eventCode");
			if (commandDeclaration.AppleEventCode.Length != 4)
				throw new ArgumentException ("eventCode must be a four characters string.");
			if (String.IsNullOrEmpty (commandDeclaration.AppleEventClassCode))
				throw new ArgumentException ("eventClass");
			if (commandDeclaration.AppleEventClassCode.Length != 4)
				throw new ArgumentException ("eventClass must be a four characters string.");
			if (commandDeclaration.ResultAppleEventCode != null && commandDeclaration.ResultAppleEventCode.Length != 4)
				throw new ArgumentException ("resultAppleEvent must be a four characters string.");
			
			using (var nsSuitName = new NSString (suiteName))
			using (var nsCommandName = new NSString (commandName)) {
				try {
					var cmd = new NSScriptCommandDescription (nsSuitName, nsCommandName, commandDeclaration.Dictionary);
					cmd.description = commandDeclaration;
					return cmd;
				} catch (Exception e) {
					// this exception is raised by the platform because the internal constructor returns a nil
					// from the docs we know:
					// 
					// Returns nil if the event constant or class name for the command description is missing; also returns nil
					// if the return type or argument values are of the wrong type.
					// 
					// the conclusion is that the user created a wrong description dict, we let him know
					throw new ArgumentException ("commandDeclaration",
						"Wrong description dictionary: Check that the event constant, class name and argument definitions are well formed as per apple documentation.", e);
				}
			}
		}

		public string AppleEventClassCode {
			get { return ToFourCCString (FCCAppleEventClassCode); }
		}

		public string AppleEventCode {
			get { return ToFourCCString (FCCAppleEventCode); }
		}

		public string GetTypeForArgument (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
				
			using (var nsName = new NSString(name))
			using (var nsType = GetNSTypeForArgument (nsName)) {
				return nsType?.ToString ();
			}
		}

		public string GetAppleEventCodeForArgument (string name)
		{
			if (name == null)
				throw new ArgumentNullException (name);

			using (var nsName = new NSString (name)) {
				return ToFourCCString (FCCAppleEventCodeForArgument (nsName));
			}
		}
		
		public bool IsOptionalArgument (string name) 
		{
			using (var nsName = new NSString (name)) {
				return NSIsOptionalArgument (nsName);
			}
		}

		public string AppleEventCodeForReturnType {
			get { return ToFourCCString (FCCAppleEventCodeForReturnType); }
		}

		public NSScriptCommand CreateCommand ()
		{
			return new NSScriptCommand (CreateCommandInstancePtr ());
		}

		public NSDictionary Dictionary {
			get { return description.Dictionary; }
		}
	}
#endif

}
