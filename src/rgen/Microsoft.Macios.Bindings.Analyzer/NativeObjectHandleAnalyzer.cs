using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using static Microsoft.Macios.Generator.RgenDiagnostics;

namespace Microsoft.Macios.Bindings.Analyzer;

/// <summary>
/// Analyzer to ensure that native objects are kept alive for the duration of accessing them by their Handle.
/// </summary>
/// <remarks>
/// Common pattern used in the manual bindings is to call a native method and pass objects using their
/// handle like this:
/// <code>
/// var x = nw_framer_create_options (protocolDefinition.Handle);
/// </code>
/// If <c>protocolDefinition</c> is a local variable or method argument that is no longer accessed after
/// the call then the .NET runtime may consider it eligible for garbage collection. Such garbage collection
/// could run the object finalizer and render the <c>Handle</c> invalid while the native call is still in
/// progress. In order to avoid this race condition the bindings have to keep the <c>protocolDefinition</c>
/// variable alive for the garbage collector. This can be done by either accessing the same variable
/// later in the method, or explicitly calling <c>GC.KeepAlive (protocolDefinition);</c>. This analyzer
/// detects cases where such access is not performed and issues an error.
/// 
/// Current shortcommings of the analyzer include:
/// <list type="bullet">
/// <item><description>Handles stored inside an array are not tracked</description></item>
/// <item><description>Only nearest scope is considered when checking if the variable is kept alive</description></item>
/// </list>
/// </remarks>
[DiagnosticAnalyzer (LanguageNames.CSharp)]
public class NativeObjectHandleAnalyzer : DiagnosticAnalyzer {

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create (RBI0014);

	public override void Initialize (AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis (GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution ();
		context.RegisterSyntaxNodeAction (AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
	}

	private bool IsINativeObject (ITypeSymbol typeSymbol)
	{
		if (typeSymbol.TypeKind == TypeKind.TypeParameter)
			return ((ITypeParameterSymbol) typeSymbol).ConstraintTypes.Any (t => IsINativeObject (t));
		if (typeSymbol.TypeKind == TypeKind.Interface && typeSymbol.ToDisplayString () == "ObjCRuntime.INativeObject")
			return true;
		return typeSymbol.AllInterfaces.Any (i => i.ToDisplayString () == "ObjCRuntime.INativeObject");
	}

	private bool IsHandleAccessor (MemberAccessExpressionSyntax memberAccess)
	{
		string name = memberAccess.Name.Identifier.Text;
		switch (name) {
		case "Handle":
		case "GetHandle":
		case "GetNonNullHandle":
		case "GetCheckedHandle":
			return true;
		default:
			return false;
		}
	}

	private bool IsSafeMethod (IMethodSymbol methodSymbol)
	{
		var name = methodSymbol.Name;
		// White-list couple of methods that return one of the input parameters or a static
		// object.
		switch (name) {
		case "DangerousAutorelease":
		case "GetConstant":
			return true;
		default:
			return false;
		}
	}

	private ITypeSymbol GetRealReturnType (IMethodSymbol methodSymbol)
	{
		if (methodSymbol.MethodKind == MethodKind.Constructor)
			return methodSymbol.ContainingType;
		return methodSymbol.ReturnType;
	}

	private void AnalyzeNode (SyntaxNodeAnalysisContext context)
	{
		if (context.Node is not MemberAccessExpressionSyntax memberAccess)
			return;

		if (!IsHandleAccessor (memberAccess))
			return;

		ExpressionSyntax expressionToCheck = memberAccess.Expression;

		var symbol = context.SemanticModel.GetSymbolInfo (expressionToCheck).Symbol;
		if (symbol is null)
			return;

		if ((symbol as IParameterSymbol)?.IsThis == true)
			return;

		if (symbol is IMethodSymbol methodSymbol) {
			if (IsINativeObject (GetRealReturnType (methodSymbol))) {
				// Ignore methods that return a handle that is guaranteed to stay
				// alive.
				if (methodSymbol.Name is "DangerousAutorelease" or "GetConstant") {
					return;
				}

				// Calling Handle directly on a value returned from method is wrong because
				// the object is immediately collectible.                
				var diagnostic = Diagnostic.Create (RBI0014, memberAccess.GetLocation (), symbol.Name);
				context.ReportDiagnostic (diagnostic);
			}
			return;
		}

		ITypeSymbol varType;
		if (symbol is ILocalSymbol localSymbol) {
			varType = localSymbol.Type;
			// Ignore variables that are wrapped in using block
			if (localSymbol.IsUsing) {
				return;
			}
		} else if (symbol is IParameterSymbol parameterSymbol) {
			varType = parameterSymbol.Type;
		} else {
			return;
		}

		if (!IsINativeObject (varType))
			return;

		// Exclude Class and Selector types, they are implicitly kept alive
		if (varType.ContainingNamespace.Name == "ObjCRuntime" && varType.Name is "Class" or "Selector") {
			return;
		}

		// Skip over this.Handle == other.Handle checks
		if (memberAccess.Parent is BinaryExpressionSyntax binaryParent &&
			binaryParent.Kind () is SyntaxKind.EqualsExpression or SyntaxKind.NotEqualsExpression) {
			return;
		}

		// Ignore variables that are wrapped in `using (existingVariable)` block. These are
		// not excluded by the `localSymbol.IsUsing` condition above.
		var usingStatement = memberAccess.Parent?.FirstAncestorOrSelf<UsingStatementSyntax> ();
		while (usingStatement is not null) {
			if (usingStatement.Expression is not null) {
				var resourceSymbol = context.SemanticModel.GetSymbolInfo (usingStatement.Expression).Symbol;
				if (resourceSymbol is not null && SymbolEqualityComparer.Default.Equals (resourceSymbol, symbol))
					return;
			}
			usingStatement = usingStatement.Parent?.FirstAncestorOrSelf<UsingStatementSyntax> ();
		}

		bool accessedAfter = false;
		var block = memberAccess.FirstAncestorOrSelf<BlockSyntax> ();
		if (block is null) {
			// We are not in a block. This can happen when handles are accessed in calls in constructor
			// parameters, or if we are in an expression body. For constructors we have to check the
			// method body for GC.KeepAlive or other references.
			var constructor = memberAccess.FirstAncestorOrSelf<ConstructorDeclarationSyntax> ();
			if (constructor is not null &&
				constructor.Body?.Statements is { } statements &&
				statements.Count > 0) {
				var df = context.SemanticModel.AnalyzeDataFlow (statements.First (), statements.Last ());
				if (df is null)
					return;
				accessedAfter = df.ReadInside.Contains (symbol);
			} else {
				// We end up here for empty constructor bodies or bodies written as an
				// expression. Assume that expressions don't access the variable and
				// produce an error.
				accessedAfter = false;
			}
		} else {
			// Search just the immediate containing block for variable access
			var statement = memberAccess.FirstAncestorOrSelf<StatementSyntax> ();
			if (statement is null)
				return;

			accessedAfter = statement.DescendantNodes ()
				.OfType<IdentifierNameSyntax> ()
				.Where (identifier => identifier.SpanStart > memberAccess.Span.End)
				.Any (identifier => SymbolEqualityComparer.Default.Equals (
					context.SemanticModel.GetSymbolInfo (identifier, context.CancellationToken).Symbol,
					symbol));

			int index = block.Statements.IndexOf (statement);

			// we have to deal with two possible scenarions:
			// 1. The symbol is accessed in the same block after it was used 
			// 2. The symbol was used in an outer block and after it was used. This happens when we call the 
			//    GC.KeepAlive method outside an if statement

			// case 1 check in the enclosing block
			DataFlowAnalysis? df;
			if (!accessedAfter && index >= 0 && index + 1 < block.Statements.Count) {
				df = context.SemanticModel.AnalyzeDataFlow (block.Statements [index + 1], block.Statements.Last ());
				if (df is null)
					return;
				accessedAfter = df.ReadInside.Contains (symbol);
			}
			// case 2
			// check if the usage is inside a if statement, only do the check if we did not find a usage in the block
			var enclosingIfStatement = statement.FirstAncestorOrSelf<IfStatementSyntax> ();
			if (!accessedAfter && enclosingIfStatement is not null) {
				// bind the index to the if statement
				var methodBlock = enclosingIfStatement.FirstAncestorOrSelf<BlockSyntax> ();
				if (methodBlock is null)
					return;
				var ifStatementIndex = methodBlock.Statements.IndexOf (enclosingIfStatement);
				if (ifStatementIndex >= 0 && ifStatementIndex + 1 < methodBlock.Statements.Count) {
					var dataFlowAnalysis = context.SemanticModel.AnalyzeDataFlow (methodBlock.Statements [ifStatementIndex + 1], methodBlock.Statements.Last ());
					if (dataFlowAnalysis is null)
						return;
					accessedAfter = dataFlowAnalysis.ReadInside.Contains (symbol);
				}
			}
		}

		if (!accessedAfter) {
			var diagnostic = Diagnostic.Create (RBI0014, memberAccess.GetLocation (), symbol.Name);
			context.ReportDiagnostic (diagnostic);
		}
	}
}
