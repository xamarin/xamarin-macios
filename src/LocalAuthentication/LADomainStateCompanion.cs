#if !WATCH && !TV
using Foundation;

namespace LocalAuthentication {
	public partial class LADomainStateCompanion {
		/// <summary>Returns all the companions paired with this device, as a bitmask of <see cref="LACompanionType" />.</summary>
		public LACompanionType AvailableCompanionTypes {
			get {
				var setOfCompanions = WeakAvailableCompanionTypes;
				var rv = default (LACompanionType);
				foreach (var value in setOfCompanions) {
					var companion = (LACompanionType) (long) value.LongValue;
					rv |= companion;
				}
				return rv;
			}
		}
	}
}
#endif // !WATCH && !TV
