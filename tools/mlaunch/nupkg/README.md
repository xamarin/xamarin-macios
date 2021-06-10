Microsoft.DotNet.Mlaunch
------------------------

mlaunch is a closed source internal tool used to interact with Apple simulators and devices.

Inside of this NuGet, you can find a MacOS app. The tool itself is a self-contained (includes the mono runtime) .NET command line application.

When installing the NuGet, you can specify where you want the tool extracted by setting the `MlaunchDestinationDir` property:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <MlaunchDestinationDir>$(IntermediateOutputPath)\mlaunch</MlaunchDestinationDir>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.DotNet.Mlaunch" Version="x.y.z" />
  </ItemGroup>
</Project>
```

When building your project, mlaunch will be extracted to that path (before the `Build` target).
