using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Linker.Steps;
using Xamarin;

namespace Mono.Linker;

public class CustomStepHostDriver {
	LinkContext _context;
	public CustomStepHostDriver(ILogger logger, string outputDirectory)
	{
		_context = new LinkContext(new Pipeline(), logger, outputDirectory);
	}

	public void AddAssembly(string pathToAssembly)
	{
		var assembly =_context.Resolver.GetAssembly(pathToAssembly);
		_context.Resolver.CacheAssembly(assembly);
	}

	public void AddStep(IStep step)
	{
		_context.Pipeline.AppendStep(step);
	}

	public void Run()
	{
		_context.Pipeline.Process(_context);
	}

	public static void Main (string[] args)
	{
		throw new NotImplementedException ();
	}

	public class ConsoleLogger : ILogger
	{
		public static ConsoleLogger Instance = new ConsoleLogger ();
		public void LogMessage (MessageContainer message)
		{
			Console.WriteLine ($"{message.Category} {message.Code} at {message.Origin}: {message.Text}");
		}
	}
}
