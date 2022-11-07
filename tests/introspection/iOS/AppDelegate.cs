#if !__WATCHOS__
using System;
using System.Reflection;
using System.Collections.Generic;

using Foundation;

public partial class AppDelegate
{
	public partial IEnumerable<Assembly> GetTestAssemblies ()
	{
		return new Assembly [] {
			Assembly.GetExecutingAssembly (),
		};
	}
}

#endif // !__WATCHOS__
