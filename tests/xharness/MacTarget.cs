using System;

namespace xharness
{
	public class MacTarget : Target
	{
		public string SimplifiedName {
			get {
				return Name.EndsWith ("-mac", StringComparison.Ordinal) ? Name.Substring (0, Name.Length - 4) : Name;
			}
		}
	}
}

