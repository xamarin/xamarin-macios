#nullable enable

using System;
using Foundation;
using ObjCRuntime;

namespace PassKit {

	public partial class PKContactFieldsExtensions {

		static public PKContactFields GetValue (NSSet set)
		{
			var fields = PKContactFields.None;
			if (set is null)
				return fields;

			foreach (PKContactFields value in Enum.GetValues (typeof (PKContactFields))) {
				var constant = value.GetConstant ();
				// None does not have an associated native value and Contains would throw an ANE
				if ((constant is not null) && set.Contains (constant))
				    fields |= value;
			}
			return fields;
		}

		static public NSSet GetSet (PKContactFields values)
		{
			var set = new NSMutableSet ();
			if (values == PKContactFields.None)
				return set;
			
			foreach (PKContactFields value in Enum.GetValues (typeof (PKContactFields))) {
				if (values.HasFlag (value)) {
					var constant = value.GetConstant ();
					// None does not have an associated native value and Contains would throw an ANE
					if (constant is not null)
						set.Add (constant);
				}
			}
			return set;
		}
	}

	public partial class PKPaymentRequest {

#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (4,0)]
		[iOS (11,0)]
#endif
		public PKContactFields RequiredBillingContactFields {
			get { return PKContactFieldsExtensions.GetValue (WeakRequiredBillingContactFields); }
			set { WeakRequiredBillingContactFields = PKContactFieldsExtensions.GetSet (value); }
		}

#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (4,0)]
		[iOS (11,0)]
#endif
		public PKContactFields RequiredShippingContactFields {
			get { return PKContactFieldsExtensions.GetValue (WeakRequiredShippingContactFields); }
			set { WeakRequiredShippingContactFields = PKContactFieldsExtensions.GetSet (value); }
		}
	}
}
