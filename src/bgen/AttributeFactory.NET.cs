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

		public static bool TryGetCtorArguments (object [] constructorArguments, PlatformName platform, out object? []? ctorValues, out Type []? ctorTypes)
		{
			ctorValues = null;
			ctorTypes = null;

			switch (constructorArguments.Length) {
			case 2:
				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte) {
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), typeof (string) };
					return true;
				}

				return false;
			case 3:
				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte &&
					constructorArguments [2] is byte) {
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), typeof (int), typeof (string) };
					return true;
				}
				return false;
			default:
				return false;
			}
		}
	}
}
#endif
