using System;
using System.Collections.Generic;

namespace Microsoft.Macios.Generator.DataModel;

class MethodReturnTypeComparer : IComparer<ReturnType> {

	/// <inheritdoc/>
	public int Compare (ReturnType x, ReturnType y)
	{
		var returnTypeComparison = String.Compare (x.Type, y.Type, StringComparison.Ordinal);
		if (returnTypeComparison != 0)
			return returnTypeComparison;
		var isNullableComparison = x.IsNullable.CompareTo (y.IsNullable);
		if (isNullableComparison != 0)
			return isNullableComparison;
		var isBlittableComparison = x.IsBlittable.CompareTo (y.IsBlittable);
		if (isBlittableComparison != 0)
			return isBlittableComparison;
		var isSmartEnumComparison = x.IsSmartEnum.CompareTo (y.IsSmartEnum);
		if (isSmartEnumComparison != 0)
			return isSmartEnumComparison;
		var isArrayComparison = x.IsArray.CompareTo (y.IsArray);
		if (isArrayComparison != 0)
			return isArrayComparison;
		var isReferenceTypeComparison = x.IsReferenceType.CompareTo (y.IsReferenceType);
		if (isReferenceTypeComparison != 0)
			return isReferenceTypeComparison;
		return x.IsVoid.CompareTo (y.IsVoid);
	}

}
