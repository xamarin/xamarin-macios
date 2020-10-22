#load "provision-shared.csx"

var dotnet_version = FindVariable("DOTNET_VERSION");

Console.WriteLine($"Provision .NET Core version: {dotnet_version}");
DotNetCoreSdk (dotnet_version);
