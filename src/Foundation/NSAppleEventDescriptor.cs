//
// NSAppleEventDescriptor.cs
//
// Copyright 2015 Xamarin Inc

#if MONOMAC

using System;

using AppKit;

namespace Foundation
{
	public enum NSAppleEventDescriptorType {
		Record,
		List,
	}

	public partial class NSAppleEventDescriptor
	{
		public NSAppleEventDescriptor (NSAppleEventDescriptorType type)
		{
			switch (type) {
			case NSAppleEventDescriptorType.List:
				InitializeHandle (_InitListDescriptor (), "listDescriptor");
				break;
			case NSAppleEventDescriptorType.Record:
				InitializeHandle (_InitRecordDescriptor (), "recordDescriptor");
				break;
			default:
				throw new ArgumentOutOfRangeException ("type");
			}
		}
	}
}

#endif // MONOMAC
