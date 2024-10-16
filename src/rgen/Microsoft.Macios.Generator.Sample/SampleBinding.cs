namespace Microsoft.Macios.Generator.Sample;

// This code will not compile until you build the project with the Source Generators

[BindingType]
public partial class SampleBinding {
	public int Id { get; } = 42;
	public string? Name { get; } = "Sample";
}
