using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

[assembly: LinkWith ("Native", ForceLoad = true, LinkTarget = LinkTarget.ArmV7)]

public class Native {
}