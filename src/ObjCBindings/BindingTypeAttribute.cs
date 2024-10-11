using System;
using System.Reflection;

namespace ObjCBindings {

	[AttributeUsage (AttributeTargets.Class | System.AttributeTargets.Enum, AllowMultiple = false)]
	public class BindingTypeAttribute : Attribute {
		public string Name { get; set; } = string.Empty;
	}

}
