using System;
using System.Collections.Generic;

#nullable enable

public class GeneratedTypes {
	public Generator Generator;

	readonly Dictionary<Type, GeneratedType> knownTypes = new ();

	public GeneratedTypes (Generator generator)
	{
		this.Generator = generator;
	}

	public GeneratedType Lookup (Type t)
	{
		if (knownTypes.TryGetValue (t, out var n))
			return n;
		n = new (t, this);
		knownTypes [t] = n;
		return n;
	}
}
