using Foundation;
using ObjCRuntime;

[assembly: LinkWith ("Native", ForceLoad = true, LinkTarget = LinkTarget.ArmV7, IsCxx = true)]

public class Native {
}
