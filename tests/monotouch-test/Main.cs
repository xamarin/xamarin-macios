#if !MONOMAC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using ObjCRuntime;
using System.Runtime.InteropServices;

#nullable enable

// This prevents the need for putting lots of #ifdefs inside the list of usings.
#if __WATCHOS__
namespace System.Drawing {}
namespace OpenTK {}
#endif
#endif
