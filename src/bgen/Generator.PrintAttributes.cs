#if NET

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjCRuntime;

#nullable enable

public partial class Generator {

	AvailabilityBaseAttribute [] GetPlatformAttributesToPrint (MemberInfo mi, MemberInfo? context, MemberInfo? inlinedType)
	{
		// Attributes are directly on the member
		List<AvailabilityBaseAttribute> memberAvailability = AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (mi).ToList ();

		// Due to differences between Xamarin and NET6 availability attributes, we have to synthesize many duplicates for NET6
		// See https://github.com/xamarin/xamarin-macios/issues/10170 for details
		if (context is null)
			context = FindContainingContext (mi);
		// Attributes on the _target_ context, the class itself or the target of the protocol inlining
		List<AvailabilityBaseAttribute> parentContextAvailability = GetAllParentAttributes (context);
		// (Optional) Attributes from the inlined protocol type itself
		var inlinedTypeAvailability = inlinedType is not null ? GetAllParentAttributes (inlinedType) : null;

		// We must consider attributes if we have any on our type, or if we're inlining and that inlined type has attributes
		// If neither are true, we have zero attributes that are relevant
		bool shouldConsiderAttributes = memberAvailability.Any () || inlinedTypeAvailability is not null && inlinedTypeAvailability.Any ();
		if (shouldConsiderAttributes) {
			// We will consider any inlinedType attributes first, if any, before any from our parent context
			List<AvailabilityBaseAttribute> availabilityToConsider = new List<AvailabilityBaseAttribute> ();
			if (inlinedTypeAvailability is not null) {
				availabilityToConsider.AddRange (inlinedTypeAvailability);
				// Don't copy parent attributes if the conflict with the type we're inlining members into
				// Example: don't copy Introduced on top of Unavailable.
				AttributeFactory.CopyValidAttributes (availabilityToConsider, parentContextAvailability);
				AttributeFactory.FindHighestIntroducedAttributes (availabilityToConsider, parentContextAvailability);
			} else {
				availabilityToConsider.AddRange (parentContextAvailability);
			}

			// We do not support Watch, so strip from both our input sources before any processing
			memberAvailability = memberAvailability.Where (x => x.Platform != PlatformName.WatchOS).ToList ();
			availabilityToConsider = availabilityToConsider.Where (x => x.Platform != PlatformName.WatchOS).ToList ();

			// Add any implied non-catalyst introduced (Catalyst will come later)
			AddUnlistedAvailability (context, availabilityToConsider);

			// Copy down any unavailable from the parent before expanding, since a [NoMacCatalyst] on the type trumps [iOS] on a member
			AttributeFactory.CopyValidAttributes (memberAvailability, availabilityToConsider.Where (attr => attr.AvailabilityKind != AvailabilityKind.Introduced));

			if (inlinedType is not null && inlinedType != mi.DeclaringType && memberAvailability.Count > 1) {
				// We might have gotten conflicting availability attributes for inlined members, where the inlined member
				// might be available on a platform the target type isn't. The target type's unavailability will come
				// later in the list, which means that if we have an unavailable attribute after an introduced attribute,
				// then we need to remove the introduced attribute.
				for (var i = memberAvailability.Count - 1; i >= 0; i--) {
					if (memberAvailability [i].AvailabilityKind != AvailabilityKind.Unavailable)
						continue;
					for (var k = i - 1; k >= 0; k--) {
						if (memberAvailability [k].AvailabilityKind == AvailabilityKind.Introduced && memberAvailability [k].Platform == memberAvailability [i].Platform) {
							memberAvailability.RemoveAt (k);
							i--;
						}
					}
				}
			}

			// Now copy it down introduced from the parent
			AttributeFactory.FindHighestIntroducedAttributes (memberAvailability, availabilityToConsider.Where (attr => attr.AvailabilityKind == AvailabilityKind.Introduced));

			if (!BindThirdPartyLibrary) {
				// If all of this implication gives us something silly, like being introduced
				// on a type that is on a namespace we don't support, ignore those Supported
				StripIntroducedOnNamespaceNotIncluded (memberAvailability, context);
				if (inlinedType is not null) {
					StripIntroducedOnNamespaceNotIncluded (memberAvailability, inlinedType);
				}
			}

			// Remove any duplicates attributes as well
			memberAvailability = memberAvailability.Distinct ().ToList ();
		}
		return memberAvailability.ToArray ();
	}
}

#else

using System.Linq;
using System.Reflection;
using ObjCRuntime;

#nullable enable

public partial class Generator {

	AvailabilityBaseAttribute [] GetPlatformAttributesToPrint (MemberInfo mi, MemberInfo? context, MemberInfo? inlinedType)
		=> AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (mi).ToArray ();
}
#endif
