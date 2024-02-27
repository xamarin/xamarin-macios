using System;
using System.Diagnostics.CodeAnalysis;
using ObjCRuntime;

#nullable enable

public static partial class AttributeFactory {
	public static readonly Type PlatformEnum = typeof (PlatformName);
#if !NET
	public static readonly Type PlatformArch = typeof (PlatformArchitecture);
#endif

	public readonly struct ConstructorArguments {
		readonly PlatformName platform;
		readonly int? major;
		readonly int? minor;
		readonly int? build;
		readonly string? message;

		public ConstructorArguments (PlatformName platformIn, int majorIn, int minorIn, int buildIn, string? messageIn)
		{
			platform = platformIn;
			major = majorIn;
			minor = minorIn;
			build = buildIn;
			message = messageIn;
		}

		public ConstructorArguments (PlatformName platformIn, int majorIn, int minorIn, string? messageIn)
		{
			platform = platformIn;
			major = majorIn;
			minor = minorIn;
			build = null;
			message = messageIn;
		}

		public ConstructorArguments (PlatformName platformIn, string? messageIn)
		{
			platform = platformIn;
			major = null;
			minor = null;
			build = null;
			message = messageIn;
		}

#if NET
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

		public static bool TryGetCtorArguments (object [] constructorArguments, PlatformName platform, [NotNullWhen (true)] out object? []? ctorValues, [NotNullWhen (true)] out Type []? ctorTypes)
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
#else
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
			return new [] { PlatformEnum, typeof (int), typeof (int), typeof (int), PlatformArch, typeof (string) };
		}

		public static bool TryGetCtorArguments (object [] constructorArguments, PlatformName platform, out object? []? ctorValues, out Type []? ctorTypes)
		{
			ctorValues = null;
			ctorTypes = null;

			switch (constructorArguments.Length) {
			case 2:
				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte) {
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (byte) 0xff, null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), PlatformArch, typeof (string) };
					return true;
				}

				return false;
			case 3:
				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte &&
					constructorArguments [2] is byte) {
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], (byte) 0xff, null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), typeof (int), PlatformArch, typeof (string) };
					return true;
				}

				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte &&
					constructorArguments [2] is bool) {
					byte arch = (bool) constructorArguments [2] ? (byte) 2 : (byte) 0xff;
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], arch, null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), PlatformArch, typeof (string) };
					return true;
				}
				return false;
			case 4:
				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte &&
					constructorArguments [2] is byte &&
					constructorArguments [3] is bool) {
					byte arch = (bool) constructorArguments [3] ? (byte) 2 : (byte) 0xff;
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], arch, null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), typeof (int), PlatformArch, typeof (string) };
					return true;
				}
				if (constructorArguments [0] is byte &&
					constructorArguments [1] is byte &&
					constructorArguments [2] is byte &&
					constructorArguments [3] is byte /* ObjCRuntime.PlatformArchitecture */) {
					ctorValues = new object? [] { (byte) platform, (int) (byte) constructorArguments [0], (int) (byte) constructorArguments [1], (int) (byte) constructorArguments [2], constructorArguments [3], null };
					ctorTypes = new [] { PlatformEnum, typeof (int), typeof (int), typeof (int), PlatformArch, typeof (string) };
					return true;
				}

				return false;
			default:
				return false;
			}
		}
#endif
	}
}
