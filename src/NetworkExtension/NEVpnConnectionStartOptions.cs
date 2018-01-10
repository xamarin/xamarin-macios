#if XAMCORE_2_0 || !MONOMAC

using Foundation;

namespace NetworkExtension {

	public class NEVpnConnectionStartOptions : DictionaryContainer {

#if !COREBUILD
		public NEVpnConnectionStartOptions () : base (new NSMutableDictionary ()) {}
		public NEVpnConnectionStartOptions (NSDictionary dictionary) : base (dictionary) {}

		public NSString Username {
			get {
				return GetNSStringValue (NEVpnConnectionStartOptionInternal.Username);
			}
			set {
				SetStringValue (NEVpnConnectionStartOptionInternal.Username, value);
			}
		}

		public NSString Password {
			get {
				return GetNSStringValue (NEVpnConnectionStartOptionInternal.Password);
			}
			set {
				SetStringValue (NEVpnConnectionStartOptionInternal.Password, value);
			}
		}
#endif
	}
}

#endif
