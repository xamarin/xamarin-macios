using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

readonly struct ExportedMember<T, R> where T : ISymbol {
	public T Symbol { get; }
	public R ExportData { get; }

	public ExportedMember (T symbol, R exportData)
	{
		Symbol = symbol;
		ExportData = exportData;
	}
}
