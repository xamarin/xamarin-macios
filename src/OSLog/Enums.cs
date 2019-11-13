using ObjCRuntime;
using Foundation;
using CoreFoundation;
using System;

namespace OSLog {
	[Mac (10,15)]
	[Native]
	public enum EntryCategory : long
	{
		Undefined,
		Metadata,
		ShortTerm,
		LongTermAuto,
		LongTerm1,
		LongTerm3,
		LongTerm7,
		LongTerm14,
		LongTerm30
	}

	[Mac (10,15)]
	[Native]
	public enum LogLevel : long
	{
		Undefined,
		Debug,
		Info,
		Notice,
		Error,
		Fault
	}

	[Mac (10,15)]
	[Native]
	public enum EntrySignpostType : long
	{
		Undefined,
		IntervalBegin,
		IntervalEnd,
		Event
	}

	[Mac (10,15)]
	[Native]
	public enum ComponentArgumentCategory : long
	{
		Undefined,
		Data,
		Double,
		Int64,
		String,
		UInt64
	}

	[Flags, Mac (10,15)]
	[Native]
	public enum EnumeratorOptions : ulong
	{
		Reverse = 0x1
	}

}
