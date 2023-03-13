#if NET
#nullable enable

using System;

public static partial class AttributeFactory {
	readonly partial struct ConstructorArguments {
		public object? [] GetCtorValues ()
		{
			if (major is null || minor is null) {
				return new object? [] { (byte) platform, message };
			}

			if (build is null)
				return new object? [] { (byte) platform, major, minor, message };
			return new object? [] { (byte) platform, major, minor, build, message };
		}

		public Type [] GetCtorTypes ()
		{
			if (major is null || minor is null) {
				return new [] { PlatformEnum, typeof (string) };
			}

			if (build is null)
				return new [] { PlatformEnum, typeof (int), typeof (int), typeof (string) };
			return new [] { PlatformEnum, typeof (int), typeof (int), typeof (int), typeof (string) };
		}
	}
}
#endif
