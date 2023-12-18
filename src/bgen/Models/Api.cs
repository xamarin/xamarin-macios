#nullable enable

using System;
using System.Reflection;

// contains all the types defined in the API to generate. This could have
// been a (Type [] Types, Type [] StrongDictionaries) but we have to keep 
// backcompat before dotnet. records do work ;)
public record Api (Type [] Types, Type [] StrongDictionaries, Assembly Assembly) {

	public Type [] Types { get; } = Types;
	public Type [] StrongDictionaries { get; } = StrongDictionaries;
	public Assembly Assembly { get; } = Assembly;
}
