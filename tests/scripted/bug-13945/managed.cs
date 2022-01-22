using Foundation;
using ObjCRuntime;

[assembly: LinkWith ("Native", ForceLoad = true, LinkTarget = LinkTarget.ArmV7)]

public class Native {
}
