using System;
using System.IO;

namespace Xamarin.Bundler {
	public partial class Assembly
	{
		// returns false if the assembly was not copied (because it was already up-to-date).
		bool CopyAssembly (string source, string target)
		{
			// TODO - We should move to mtouch's code, shared in common
			var copied = false;

			try {
				if (!Application.IsUptodate (source, target)) {
					copied = true;
					Application.CopyFile (source, target);
				}
			} catch (Exception e) {
				throw new ProductException (1009, true, e, Errors.MX1009, source, target, e.Message);
			}

			return copied;
		}
	}
}
