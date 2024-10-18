using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Bindings.Analyzer;

public struct DiagnosticInfo (
	string id,
	LocalizableResourceString title,
	LocalizableResourceString messageFormat,
	LocalizableResourceString description,
	string category) {

	public string Id { get; } = id;
	public LocalizableResourceString Title { get; } = title;
	public LocalizableResourceString MessageFormat { get; } = messageFormat;
	public LocalizableResourceString Description { get; } = description;
	public string Category { get; } = category;
}
