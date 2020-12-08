# BCL assembly fixer

This is a tool that fixes:

* InternalsVisibleTo attributes to Xamarin.iOS
* Assembly references to Xamarin.iOS

for Mac Catalyst, and changes these to reference Xamarin.MacCatalyst instead.

This is a temporary workaround until we have a mono archive with properly
built BCL assemblies.
