#if !XAMCORE_2_0

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.PassKit {

	public partial class PKContactFieldsExtensions {

		static public PKContactFields GetValue (NSSet set)
		{
			var fields = PKContactFields.None;
			foreach (PKContactFields value in Enum.GetValues (typeof (PKContactFields))) {
				if (set.Contains (value.GetConstant ()))
				    fields |= value;
			}
			return fields;
		}

		static public NSSet GetSet (PKContactFields values)
		{
			var set = new NSMutableSet ();
			foreach (PKContactFields value in Enum.GetValues (typeof (PKContactFields))) {
				if (values.HasFlag (value))
					set.Add (value.GetConstant ());
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

#endif
