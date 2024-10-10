#load "provision-shared.csx"

if (!ProvisionXcode ())
	return 1;
ProvisionSharpie ();
ProvisionBrewPackages ();
SetDefaultXcodeInVSMac ();
