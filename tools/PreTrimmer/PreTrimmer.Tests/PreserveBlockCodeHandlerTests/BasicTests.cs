using Mono.Linker;
using Mono.Linker.Steps;
using Xamarin.Linker.Steps;

namespace PreTrimmer.Tests.PreserveBlockCodeHandlerTests;

public class BasicTests
{
	[Fact]
	public void SDInnerBlockIsKept()
	{
		// assembly with SDInnerBlock and the fields has them kept via the descriptors file
		string currentAssemblyPath = Path.GetDirectoryName (typeof (BasicTests).Assembly.Location)!;
		string basicAssemblyPath = Path.Combine (currentAssemblyPath, "SDInnerBlock.dll");
		string outputFolder = Path.Combine (Path.GetTempPath (), nameof (SDInnerBlockIsKept));
		Console.WriteLine(outputFolder);
		CustomStepHostDriver driver = new (CustomStepHostDriver.ConsoleLogger.Instance, outputFolder);
		driver.AddAssembly (basicAssemblyPath);
		driver.AddStep (new PreserveBlockCodeHandler ());
		driver.AddStep (new OutputStep ());
		driver.Run ();
		const string expectedDescriptorContents = $$$"""
			<linker>
				<assembly fullname="SDInnerBlock, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
					<type fullname="ObjCRuntime.Trampolines/SDInnerBlock" preserve="nothing" required="false">
						<method signature="System.Void Invoke(System.IntPtr,System.Int32)" required="false" />
						<field signature="ObjCRuntime.Trampolines/SDInnerBlock/DInnerBlock Handler" required="false" />
					</type>
				</assembly>
			</linker>
			""";
		string descriptorPath = Path.Combine (outputFolder, "ILLink.Descriptors.xml");
		string actualDescriptorContents = File.ReadAllText (descriptorPath);
		Assert.Equal (expectedDescriptorContents, actualDescriptorContents);
	}
}
