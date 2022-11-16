using System;
namespace Microsoft.MaciOS.Nnyeah {
	public class MemberNotFoundException : ConversionException {
		public MemberNotFoundException (string memberName)
			: base ($"Member {memberName} not found.")
		{
			MemberName = memberName;
		}

		public string MemberName { get; init; }
	}
}
