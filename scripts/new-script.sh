#!/bin/bash -e

echo 'Run this script to create a new script!'

echo ""
if test -z "$SCRIPT_NAME"; then
	read -p "What's the name of your new script? " SCRIPT_NAME
	if test -z "$SCRIPT_NAME"; then
		echo "Exiting, no script name provided"
		exit 0
	fi
fi

echo "Name of the new script: $SCRIPT_NAME"

UPPER_NAME=$(echo "$SCRIPT_NAME" | tr 'a-z' "A-Z" | tr '-' '_')
echo "Variable name: $UPPER_NAME"

mkdir -p "$SCRIPT_NAME"

printf 'include $(TOP)/scripts/template.mk\n' > "$SCRIPT_NAME/fragment.mk"
printf "\$(eval \$(call TemplateScript,$UPPER_NAME,$SCRIPT_NAME))\n" >> "$SCRIPT_NAME/fragment.mk"

printf '<Project Sdk="Microsoft.NET.Sdk">\n' > "$SCRIPT_NAME/$SCRIPT_NAME.csproj"
printf '	<PropertyGroup>\n' >> "$SCRIPT_NAME/$SCRIPT_NAME.csproj"
printf '		<TargetFramework>net$(BundledNETCoreAppTargetFrameworkVersion)</TargetFramework>\n' >> "$SCRIPT_NAME/$SCRIPT_NAME.csproj"
printf '	</PropertyGroup>\n' >> "$SCRIPT_NAME/$SCRIPT_NAME.csproj"
printf '</Project>\n' >> "$SCRIPT_NAME/$SCRIPT_NAME.csproj"

printf "Console.WriteLine (\"Hello $SCRIPT_NAME\");\n" > "$SCRIPT_NAME/$SCRIPT_NAME.cs"

printf "# $SCRIPT_NAME\n" > "$SCRIPT_NAME/README.md"

echo "Your new script is located in ./$SCRIPT_NAME. Read the README.md in this directory for how to use it."
