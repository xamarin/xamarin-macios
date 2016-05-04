using System;

namespace Xamarin.Utils {
	// Boolean variable that defaults to false, unless a environmental variable is set. Setting value explictly overrides all other values.
	class LatchedEnvironmentVariable {
		bool envVariable;
		bool? setValue;

		internal LatchedEnvironmentVariable (string variableName)
		{
			envVariable = !String.IsNullOrEmpty (Environment.GetEnvironmentVariable (variableName));
		}

		internal bool Value {
			set { setValue = value; }
			get { return setValue.HasValue ? setValue.Value : envVariable; }
		}
	}
}
