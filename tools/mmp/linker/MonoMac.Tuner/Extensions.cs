using System;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

namespace MonoMac.Tuner {

	static class Extensions {

		static bool HasGeneratedCodeAttribute (ICustomAttributeProvider provider)
		{
			if (provider == null || !provider.HasCustomAttributes)
				return false;
			
			foreach (CustomAttribute attribute in provider.CustomAttributes) {
				TypeReference tr = attribute.Constructor.DeclaringType;
				if (tr.Is ("System.Runtime.CompilerServices", "CompilerGeneratedAttribute"))
					return true;
			}
			return false;
		}
		
		static PropertyDefinition GetPropertyByAccessor (MethodDefinition method)
		{
			string mname = method.Name;
			foreach (PropertyDefinition property in method.DeclaringType.Properties) {
				// set_ and get_ both have a length equal to 4
				string pname = property.Name;
				if (String.CompareOrdinal (pname, 0, mname, 4, pname.Length) == 0)
					return property;
			}
			return null;
		}
		
		public static bool IsGeneratedCode (this MethodDefinition self)
		{
			// check the property too
			if (self.IsGetter || self.IsSetter) {
				if (HasGeneratedCodeAttribute (GetPropertyByAccessor (self)))
					return true;
			}
			return HasGeneratedCodeAttribute (self);
		}
	}
}