#load "provision-shared.csx"

if (!ProvisionXcode ())
	return 1;
ProvisionMono ();
ProvisionSharpie ();
ProvisionBrewPackages ();
SetDefaultXcodeInVSMac ();
