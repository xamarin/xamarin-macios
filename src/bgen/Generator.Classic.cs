#if !NET
using System.Linq;
using System.Reflection;
using ObjCRuntime;

#nullable enable

public partial class Generator {

	AvailabilityBaseAttribute [] GetPlatformAttributesToPrint (MemberInfo mi, MemberInfo? context, MemberInfo? inlinedType)
		=> AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (mi).ToArray ();
}
#endif
