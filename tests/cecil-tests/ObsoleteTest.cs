using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class ObsoleteTest {

        // Find (fail by default) every property/method/event that has [Obsolete]
        // search through search code or look at binaries, latter of which is easier via couple lines of reflection vs. sorting through a gazillion source files
        //     mono.cecil library allows you to treat a .net mono library like it's data. walk through code to verify certain things easily



		[TestCaseSource (typeof (Helper), "Net6PlatformAssemblies")] // call this method with every .net6 library
		public void GetAllObsoletedThings (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}
			Console.WriteLine(assemblyPath);

			// Make a list of Obsolete things
			HashSet<string> found = new HashSet<string> ();
			int count = 0;
			foreach (var prop in Helper.FilterProperties(assembly, a => HasObsoleteAttribute(a))) {
                found.Add(prop.FullName);
			}
			
            foreach (var meth in Helper.FilterMethods(assembly, a => HasObsoleteAttribute(a))) {
                found.Add(meth.FullName);
			}
			
            foreach (var type in Helper.FilterTypes(assembly, a => HasObsoleteAttribute(a))) {
                found.Add(type.FullName);
			}

            // TODO: Events?
            
            
            Console.WriteLine(found.Count);
            //Assert.That (found, Is.Empty, string.Join (", ", found));
		}

        // bool FirstContainsAllOfSecond<T> (IEnumerable<T> first, IEnumerable<T> second)
		// {
		// 	HashSet<T> firstSet = new HashSet<T>(first);
		// 	return second.All(s => firstSet.Contains(s));
		// }

        // IEnumerable<string> GetObsoleteParent(PropertyDefinition prop) => GetObsolete(prop.DeclaringType);
		// IEnumerable<string> GetObsoleteParent(MethodDefinition meth) => GetObsolete(meth.DeclaringType);

        // IEnumerable<string> GetObsolete(TypeDefinition type)
        // {
        //     var obsoleted = new List<string>();
		// 	foreach (var type in types.Where(a => IsObsoleteAttribute(a))) {
		// 		if (type != null) {
		// 			obsoleted.Add(type);
		// 		}
		// 	}
		// 	return obsoleted;
        // }
		// IEnumerable<string> GetObsolete(PropertyDefinition prop)
        // {
        //     var obsoleted = new List<string>();
		// 	foreach (var prop in props.Where(a => IsObsoleteAttribute(a))) {
		// 		if (prop != null) {
		// 			obsoleted.Add(prop);
		// 		}
		// 	}
		// 	return obsoleted;
        // }
        // IEnumerable<string> GetObsolete(MethodDefinition meth)
        // {
        //     var obsoleted = new List<string>();
		// 	foreach (var meth in methods.Where(a => IsObsoleteAttribute(a))) {
		// 		if (meth != null) {
		// 			obsoleted.Add(meth);
		// 		}
		// 	}
		// 	return obsoleted;
        // }

		bool HasObsoleteAttribute (PropertyDefinition prop) => HasObsoleteAttribute(prop.CustomAttributes);
		bool HasObsoleteAttribute (MethodDefinition meth) => HasObsoleteAttribute(meth.CustomAttributes);
		bool HasObsoleteAttribute (TypeDefinition type) => HasObsoleteAttribute(type.CustomAttributes);

		bool HasObsoleteAttribute (IEnumerable<CustomAttribute> attributes) => attributes.Any (a => IsObsoleteAttribute(a));

		bool IsObsoleteAttribute (CustomAttribute attribute)
		{
			return attribute.AttributeType.Name == "Obsolete" ||
				(attribute.AttributeType.Name == "ObsoleteAttribute");
		}
	}
}
