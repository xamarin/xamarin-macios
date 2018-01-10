using System;

namespace AppKit
{
	public class AppKitThreadAccessException : Exception
	{
		public AppKitThreadAccessException() : base("AppKit Consistency error: you are calling a method that can only be invoked from the UI thread.")
		{
		}
	}
}

