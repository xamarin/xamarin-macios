using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.Macios.Generator;

/// <summary>
/// A sample source generator that creates a custom report based on class properties. The target class should be annotated with the 'Generators.ReportAttribute' attribute.
/// When using the source code as a baseline, an incremental source generator is preferable because it reduces the performance overhead.
/// </summary>
[Generator]
public class BindingSourceGeneratorGenerator : IIncrementalGenerator {

	public void Initialize (IncrementalGeneratorInitializationContext context)
	{
		// Add the binding generator attributes to the compilation. This are only available when the
		// generator is used, similar to how bgen works.
		foreach ((string fileName, string content) in ExtraSources.Sources) {
			context.RegisterPostInitializationOutput (ctx => ctx.AddSource (
				fileName, SourceText.From (content, Encoding.UTF8)));
		}
	}

}
