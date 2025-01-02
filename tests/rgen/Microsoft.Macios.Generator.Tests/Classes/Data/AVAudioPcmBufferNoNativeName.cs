using System;
using Foundation;
using ObjCBindings;

namespace TestNamespace;

[BindingType<Class> (Flags=Class.DisableDefaultCtor)]
public partial class AVAudioPcmBuffer {
}
