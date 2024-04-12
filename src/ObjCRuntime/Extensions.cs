namespace ObjCRuntime {
	static class Extensions {
		public static byte AsByte (this bool value)
		{
			return value ? (byte) 1 : (byte) 0;
		}
	}
}
