# Scripts

This directory contains numerous short C# scripts, each in their own directory
with its own project file.

To create a new script, the easy way is to run the `new-script.sh` script.

The harder way is to copy an existing script directory, and:

* Rename the csproj file to match the directory name.
* Add your C# code, and document what it's supposed to do in a README.md file.
* Edit the arguments to the `TemplateScript` template in the `fragment.mk`
  file according to how you named your script (directory). The first argument
  will be used in other makefiles that use the script, the second is the name
  of the script (directory). Say your script is `my-script`, then that would be:

  ```make
  $(eval $(call TemplateScript,MY_SCRIPT,my-script))

To use the new script:

1. In the consuming `Makefile`, import the `fragment.mk` file from the script directory:

    ```make
    include $(TOP)/scripts/my-script/fragment.mk
    ```

2. In the target where you want to execute the script, depend on the script executable, which is named `MY_SCRIPT` (from the call to the `TemplateScript` template):

    ```make
    dostuff: $(MY_SCRIPT)
        echo "Doing stuff"
    ```

    This makes sure the script is actually built before you want to execute it.

3. The actual invocation to call the script, is the same variable, but with `_EXEC` appended:

    ```make
    dostuff: $(MY_SCRIPT)
        $(MY_SCRIPT_EXEC) --arguments to/my/script
    ```

Sidenote: if https://github.com/dotnet/designs/pull/296 were ever implemented,
we could dispense with the project file, making these C# files actual scripts.