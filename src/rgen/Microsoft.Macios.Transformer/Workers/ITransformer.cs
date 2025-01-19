// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Marille;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Transformer.Workers;

interface ITransformer<T> : IWorker<T>, IErrorWorker<T> where T : struct {

	T CreateMessage (SyntaxTree treeNode, ISymbol symbol);
}
