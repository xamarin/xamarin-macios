#if !NET

using System;
using ObjCRuntime;

#nullable enable

public static partial class AttributeFactory {
	public static readonly Type PlatformArch = typeof (PlatformArchitecture);

	readonly partial struct ConstructorArguments {
		public object? [] GetCtorValues ()
		{
			if (major is null || minor is null) {
				return new object? [] { (byte) platform, (byte) 0xff, message };
			}

			if (build is null)
				return new object? [] { (byte) platform, major, minor, (byte) 0xff, message };
			return new object? [] { (byte) platform, major, minor, build, (byte) 0xff, message };
		}

		public Type [] GetCtorTypes ()
		{
			if (major is null || minor is null) {
				return new [] { PlatformEnum, PlatformArch, typeof (string) };
			}

			if (build is null)
				return new [] { PlatformEnum, typeof (int), typeof (int), PlatformArch, typeof (string) };
			return new [] { PlatformEnum, typeof (int), typeof (int), typeof(int), PlatformArch, typeof (string) };
		}
	}

}
#endif
