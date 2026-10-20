#!/bin/bash -e

# Go to the top level directory
cd "$(git rev-parse --show-toplevel)"
SRC_DIR=$(pwd)

# Replace:
#     == null     with     is null
#     != null     with     is not null
# except in a few tests files, where we have tests for (in)equality operators, and in that case the '== null' and '!= null' code is correct.
#
IFS=$'\n'

(
	set +x
	export LANG=en
	IFS=$'\n'
	cd "$SRC_DIR"

	if [[ "$OSTYPE" == "darwin"* ]]; then
		SED=(sed -i "")
	else
		SED=(sed -i)
	fi

	echo 'Changing "== null" and "!= null" to "is null" and "is not null"...'
	for file in $(git ls-files -- '*.cs' ':(exclude)tests/monotouch-test/Foundation/UrlTest.cs' ':(exclude)tests/monotouch-test/AVFoundation/AVAudioFormatTest.cs'); do
		if [[ -L "$file" ]]; then
			echo "Skipping $file because it's a symlink"
			continue
		fi

		"${SED[@]}" -e 's/!= null/is not null/g' -e 's/== null/is null/g' "$file"
	done
)

make -C builds dotnet

if test -z "${DOTNET:-}"; then
	DOTNET=dotnet
fi

which $DOTNET
$DOTNET --info

function af_whitespace ()
{
	echo "Processing whitespace in $1..."
	$DOTNET format whitespace "$1"
}

function af ()
{
	echo "Formatting $i"
	$DOTNET format "$1"
}

# Start formatting!
af_whitespace "$SRC_DIR/tests/cecil-tests/cecil-tests.csproj"
af_whitespace "$SRC_DIR/tests/dotnet/UnitTests/DotNetUnitTests.csproj"
af_whitespace "$SRC_DIR/msbuild/Messaging/Xamarin.Messaging.Build/Xamarin.Messaging.Build.csproj"
af_whitespace "$SRC_DIR/msbuild/Xamarin.MacDev.Tasks/Xamarin.MacDev.Tasks.csproj"
af_whitespace "$SRC_DIR/msbuild/Xamarin.iOS.Tasks.Windows/Xamarin.iOS.Tasks.Windows.csproj"
af_whitespace "$SRC_DIR/src/bgen/bgen.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Binding.Common/Microsoft.Macios.Binding.Common.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Bindings.Analyzer.Sample/Microsoft.Macios.Bindings.Analyzer.Sample.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Bindings.Analyzer/Microsoft.Macios.Bindings.Analyzer.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Bindings.CodeFixers/Microsoft.Macios.Bindings.CodeFixers.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Generator.Sample/Microsoft.Macios.Generator.Sample.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Generator/Microsoft.Macios.Generator.csproj"
af_whitespace "$SRC_DIR/src/rgen/Microsoft.Macios.Transformer/Microsoft.Macios.Transformer.csproj"
af_whitespace "$SRC_DIR/tools/dotnet-linker/dotnet-linker.csproj"
af_whitespace "$SRC_DIR/tools/mtouch/mtouch.csproj"
af_whitespace "$SRC_DIR/tests/xharness/xharness.slnx"
af_whitespace "$SRC_DIR/tests/introspection/dotnet/iOS/introspection.csproj"
af_whitespace "$SRC_DIR/tests/introspection/dotnet/MacCatalyst/introspection.csproj"
af_whitespace "$SRC_DIR/tests/introspection/dotnet/macOS/introspection.csproj"
af_whitespace "$SRC_DIR/tests/introspection/dotnet/tvOS/introspection.csproj"
af_whitespace "$SRC_DIR/tests/monotouch-test/dotnet/iOS/monotouch-test.csproj"
af_whitespace "$SRC_DIR/tests/monotouch-test/dotnet/MacCatalyst/monotouch-test.csproj"
af_whitespace "$SRC_DIR/tests/monotouch-test/dotnet/macOS/monotouch-test.csproj"
af_whitespace "$SRC_DIR/tests/monotouch-test/dotnet/tvOS/monotouch-test.csproj"
af_whitespace "$SRC_DIR/tests/xtro-sharpie/xtro-sharpie/xtro-sharpie.csproj"
af_whitespace "$SRC_DIR/tests/xtro-sharpie/u2ignore/u2ignore.csproj"
af_whitespace "$SRC_DIR/tests/xtro-sharpie/u2todo/u2todo.csproj"
af_whitespace "$SRC_DIR/tests/xtro-sharpie/xtro-report/xtro-report.csproj"
af_whitespace "$SRC_DIR/tests/xtro-sharpie/xtro-sanity/xtro-sanity.csproj"
af_whitespace "$SRC_DIR/tools/api-tools/mono-api-html/mono-api-html.csproj"
af_whitespace "$SRC_DIR/tools/api-tools/mono-api-info/mono-api-info.csproj"
af_whitespace "$SRC_DIR/tests/common/MonoTouch.Dialog/MonoTouch.Dialog.iOS.csproj"
af_whitespace "$SRC_DIR/tests/common/MonoTouch.Dialog/MonoTouch.Dialog.MacCatalyst.csproj"
af_whitespace "$SRC_DIR/tests/common/MonoTouch.Dialog/MonoTouch.Dialog.tvOS.csproj"
af_whitespace "$SRC_DIR/tests/common/Touch.Unit/Touch.Client/dotnet/Touch.Client.iOS.csproj"
af_whitespace "$SRC_DIR/tests/common/Touch.Unit/Touch.Client/dotnet/Touch.Client.MacCatalyst.csproj"
af_whitespace "$SRC_DIR/tests/common/Touch.Unit/Touch.Client/dotnet/Touch.Client.macOS.csproj"
af_whitespace "$SRC_DIR/tests/common/Touch.Unit/Touch.Client/dotnet/Touch.Client.tvOS.csproj"

af "$SRC_DIR/tools/sharpie/sharpie.slnx"

echo "Processing $SRC_DIR..."
$DOTNET format whitespace --folder "$SRC_DIR"

for file in "$SRC_DIR"/dotnet/Templates/Microsoft.*.Templates/*/*/.template.config/localize/*.json "$SRC_DIR"/dotnet/Templates/Microsoft.*.Templates/*/.template.config/localize/*.json; do
	tr -d $'\r' < "$file" > "$file".tmp
	mv "$file".tmp "$file"
done

make -C src csproj -j8

FILE=$(pwd)/tmp.txt
make print-variable-value-to-file FILE="$FILE" VARIABLE=DOTNET_PLATFORMS -C tools/devops
DOTNET_PLATFORMS=$(cat "$FILE" | sed 's/.*=//' | xargs)
rm -f "$FILE"

for platform in $(echo "$DOTNET_PLATFORMS" | sed 's/ /\n/g'); do
	af_whitespace src/build/dotnet/$platform/csproj/core/Core.$platform.csproj
	af_whitespace src/build/dotnet/$platform/csproj/api/ApiDefinition.$platform.csproj
	af_whitespace src/build/dotnet/$platform/csproj/platform/Microsoft.$platform.csproj
done

# dotnet format "$SRC_DIR/[...]"
# add more projects here...

cd "$SRC_DIR"
