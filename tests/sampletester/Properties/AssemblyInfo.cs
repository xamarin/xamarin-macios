using NUnit.Framework;

// Two tests at the same time is enough: mtouch already parallelizes things, so we don't want to overload the bots either.
[assembly: LevelOfParallelism (2)]
