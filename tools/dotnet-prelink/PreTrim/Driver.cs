using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Linker.Steps;
using Xamarin;

namespace Mono.Linker;

public class CustomStepHostDriver {

	public static void Run(IEnumerable<IStep> steps, ILogger logger, string outputDirectory)
	{
		Pipeline pipeline = new ();
		foreach(var step in steps)
		{
			pipeline.AppendStep(step);
		}
		LinkContext context = new LinkContext(pipeline, logger, outputDirectory);
		pipeline.Process(context);
	}

	public static void Main (string[] args)
	{
	}

	private class DefaultLogger : ILogger
	{
		public static DefaultLogger Instance = new DefaultLogger ();
		public void LogMessage (MessageContainer message)
		{
			Console.WriteLine ($"{message.Category} {message.Code} at {message.Origin}: {message.Text}");
		}
	}
}
