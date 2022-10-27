# Editors 

This directory contains a list of useful settings and snippets to make your life easier. There is a directory per editor that contains recommendations
and snippets to be used.

## VS Code

You can find the snippets for csharp in the VSCode directory. There are two possible scenarios:

1. You do no have snippets setup in your system.
2. You do alrady use snippets.

### New to snippets 

In order to be able to use snippets you need to enable tab completion on vscode, you do so by following the instructions 
you can find in the [vscode documentation](https://code.visualstudio.com/docs/editor/userdefinedsnippets).

Copy the snippets file

```bash
mkdir -p "~/Library/Application Support/Code/User/snippets" 
cp VSCode/* "~/Library/Application Support/Code/User/snippets" 
```

### Already have snippets

You are all setup, you just need to merge the contents of the file VSCode/csharp.json with the file in "~/Library/Application Support/Code/User/snippets" 

