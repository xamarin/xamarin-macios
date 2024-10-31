
using Mono.Cecil;
using Mono.Linker.Steps;

namespace PreTrimmer.Tests.TestSteps;
class BasicStep : BaseStep
{
	protected override void ProcessAssembly (AssemblyDefinition assembly)
	{
		foreach (var type in assembly.MainModule.Types)
		{
			MarkType(type);
		}

		void MarkType(TypeDefinition type)
		{
			foreach(var member in type.Methods.Concat<IMemberDefinition>(type.Properties).Concat(type.Fields).Concat(type.Events))
			{
				Context.Annotations.Mark(member);
			}
			Context.Annotations.Mark (type);
			foreach(var nestedType in type.NestedTypes)
			{
				MarkType(nestedType);
			}
		}
	}
}
