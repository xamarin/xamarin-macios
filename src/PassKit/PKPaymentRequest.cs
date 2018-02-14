using System;
using Foundation;
using ObjCRuntime;

namespace PassKit {

	public partial class PKContactFieldsExtensions {

		static public PKContactFields GetValue (NSSet set)
		{
			var fields = PKContactFields.None;
			if (set == null)
				return fields;

			foreach (PKContactFields value in Enum.GetValues (typeof (PKContactFields))) {
				var constant = value.GetConstant ();
				// None does not have an associated native value and Contains would throw an ANE
				if ((constant != null) && set.Contains (constant))
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
					if (constant != null)
						set.Add (constant);
				}
			}
			return set;
		}
	}

	public partial class PKPaymentRequest {

		[Watch (4,0)][iOS (11,0)]
		public PKContactFields RequiredBillingContactFields {
			get { return PKContactFieldsExtensions.GetValue (WeakRequiredBillingContactFields); }
			set { WeakRequiredBillingContactFields = PKContactFieldsExtensions.GetSet (value); }
		}

		[Watch (4,0)][iOS (11,0)]
		public PKContactFields RequiredShippingContactFields {
			get { return PKContactFieldsExtensions.GetValue (WeakRequiredShippingContactFields); }
			set { WeakRequiredShippingContactFields = PKContactFieldsExtensions.GetSet (value); }
		}
	}
}
