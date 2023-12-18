## Debugging


One of the main problems when adding changes to the generator was that before the generator was moved to dotnet, we could not attach the dotnet debugger to it. 
Things have changed, and now if you want to debug the generator, you can do it with Visual Studio. To do so, you need to follow these steps:

1. Export the environment variable `XAMMACIOS_DEBUGGER` with any value. For example:

    ```bash
    export XAMMACIOS_DEBUGGER=1
    ```

2. Open the src/src/bgen.csproj solution with Visual Studio.
3. Add a break point at the end of the `Main` method in the BindingTouch.cs file.
4. Rebuild the generator.


Now, when the project is rebuilt you will have to pay attention to the output of the build. You will see something like this:

```bash
Waiting for debugger to attach: (55644) dotnet
Waiting for debugger to attach: (55646) dotnet
```

These messages are telling you that the generator is waiting for the debugger to be attach to the process. To do so, you need to go to the Debug menu and select 
the Attach to Process option. In the dialog that will appear, you will need to select the dotnet process that is running the generator. In this case, the process
is the one with the PID 55644. Once you have selected the process, you can click on the Attach button.

Because the generator is called several times, one per platform, you can attach to the different processes in case the issue you are trying to debug
is platform specific.
