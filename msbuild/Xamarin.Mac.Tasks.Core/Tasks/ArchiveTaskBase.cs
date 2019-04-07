using System;
using System.IO;
using System.Diagnostics;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.Mac.Tasks.Tasks
{
    public abstract class ArchiveTaskBase : Xamarin.MacDev.Tasks.ArchiveTaskBase
    {
        public override bool Execute ()
        {
            var archiveDir = CreateArchiveDirectory ();

            try {
                var plist = PDictionary.FromFile (Path.Combine (AppBundleDir.ItemSpec, "Contents", "Info.plist"));
                var productsDir = Path.Combine (archiveDir, "Products");

                // TODO: Archive the OnDemandResources.
                // I don't have any on hand, so I can't use an Xcode-created archive as reference.

                // Archive the Applications...
                var appDestDir = Path.Combine (productsDir, "Applications", Path.GetFileName (AppBundleDir.ItemSpec));
                Ditto (AppBundleDir.ItemSpec, appDestDir);

                // Archive the dSYMs...
                if (Directory.Exists (DSYMDir)) {
                    var destDir = Path.Combine (archiveDir, "dSYMs", Path.GetFileName (DSYMDir));
                    Ditto (DSYMDir, destDir);
                }

                // Generate an archive Info.plist
                var arInfo = new PDictionary ();
                var props = new PDictionary ();
                props.Add ("ApplicationPath", new PString (string.Format ("Applications/{0}", Path.GetFileName (AppBundleDir.ItemSpec))));
                props.Add ("CFBundleIdentifier", new PString (plist.GetCFBundleIdentifier ()));

                var version = plist.GetCFBundleShortVersionString ();
                var build = plist.GetCFBundleVersion ();
                props.Add ("CFBundleShortVersionString", new PString (version ?? (build ?? "1.0")));
                props.Add ("CFBundleVersion", new PString (build ?? "1.0"));
                props.Add ("SigningIdentity", SigningKey);
                // TODO: Add "Team" key (Apple Team ID) here. Extract it from the SigningKey somehow?

                arInfo.Add ("ArchiveVersion", new PNumber (2));
                arInfo.Add ("CreationDate", Now);
                arInfo.Add ("Name", new PString (plist.GetCFBundleName () ?? plist.GetCFBundleDisplayName ()));
                arInfo.Add ("ApplicationProperties", props);

                arInfo.Save (Path.Combine (archiveDir, "Info.plist"));
            } catch (Exception ex) {
                Log.LogErrorFromException (ex);
                Directory.Delete (archiveDir, true);
            }

            return !Log.HasLoggedErrors;
        }
    }
}
