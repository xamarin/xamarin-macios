#!/bin/bash -e

#
# How to run API comparison locally:
#
# 1. To compare a specific commit to the previous one, just run this script (./compare.sh).
# 2. Replicating what happens for a specific PR is a bit more complicated:
#    a. Fetch the source code for that PR, using "git fetch --no-tags --progress https://github.com/xamarin/xamarin-macios +refs/pull/<PR>/*:refs/remotes/origin/pr/<PR>/*",
#       so for example this for PR #4614:
#
#            git fetch --no-tags --progress https://github.com/xamarin/xamarin-macios +refs/pull/4614/*:refs/remotes/origin/pr/4614/*
#
#    b. Checkout the commit you want to compare (typically the branch you created the PR from if it's your own PR).
#       i) This isn't 100% accurate, since GitHub will build a merged version
#          of your branch + the target branch. If you want to test _exactly_
#          the same code as GitHub, checkout the 'origin/pr/<merge>/merge'
#          branch the previous git fetch command created.
#
#    c. Execute 'PR_ID=4614 ./compare.sh'
#

API_COMPARISON="$WORKSPACE/apicomparison"
BUILD_URL="$SYSTEM_TEAMFOUNDATIONCOLLECTIONURI$SYSTEM_TEAMPROJECT/_build/index?buildId=$BUILD_BUILDID&view=ms.vss-test-web.test-result-details"
URL_PREFIX="$VSDROPSPREFIX/$BUILD_BUILDNUMBER/$BUILD_BUILDID/apigeneratordiff/;/"
API_URL="$URL_PREFIX/api-diff.html"
GENERATOR_URL="$URL_PREFIX/generator-diff/index.html"

# env var should have been defined by the CI
if test -z "$XAM_TOP"; then
    echo "Variable XAM_TOP is missing."
    exit 1
fi

if test -z "$WORKSPACE"; then
	echo "Variable WORKSPACE is missing."
	exit 1
fi

cd $XAM_TOP

MARKDOWN_INDENT="&nbsp;&nbsp;&nbsp;&nbsp;"
echo "*** Comparing API & creating generator diff... ***"
export COMPARE_FAILURE_FILE=$TMPDIR/api-diff-compare-failures.txt
report_error ()
{
	printf ":fire: [Failed to compare API and create generator diff](%s/console) :fire:\\n" "$BUILD_URL" >> "$WORKSPACE/api-diff-comments.md"
	if test -f "$COMPARE_FAILURE_FILE"; then
		sed "s/^/${MARKDOWN_INDENT//&/\\&}/" "$COMPARE_FAILURE_FILE" >> "$WORKSPACE/api-diff-comments.md"
	fi
	printf "${MARKDOWN_INDENT}Search for \`Comparing API & creating generator diff\` in the log to view the complete log.\\n" >> "$WORKSPACE/api-diff-comments.md"
	touch "$WORKSPACE/failure-stamp"
	rm -f "$COMPARE_FAILURE_FILE"
	echo "*** Comparing API & creating generator diff failed ***"
	MESSAGE="*** Comparing API & creating generator diff failed ***"
	exit 0
}
trap report_error ERR

if test -z "$PR_ID"; then
	BASE=HEAD
else
	BASE="origin/pr/$PR_ID/merge"
fi

if ! git rev-parse "$BASE" >/dev/null 2>&1; then
	echo "Can't compare API and create generator diff because the pull request has conflicts that must be resolved first (the branch '$BASE' doesn't exist)."
	MESSAGE="Can't compare API and create generator diff because the pull request has conflicts that must be resolved first (the branch '$BASE' doesn't exist)."
	printf ":fire: [Failed to compare API and create generator diff because the pull request has conflicts that must be resolved first](%s/console) :fire:\\n" "$BUILD_URL" >> "$WORKSPACE/api-diff-comments.md"
	exit 0
fi

./tools/compare-commits.sh --base="$BASE^1" "--failure-file=$COMPARE_FAILURE_FILE"

mkdir -p "$API_COMPARISON"

cp -R ./tools/comparison/apidiff/diff "$API_COMPARISON"
cp -R ./tools/comparison/apidiff/dotnet "$API_COMPARISON"
cp    ./tools/comparison/apidiff/*.html "$API_COMPARISON"
cp    ./tools/comparison/apidiff/*.md "$API_COMPARISON"
cp -R ./tools/comparison/generator-diff "$API_COMPARISON"

if ! grep "href=" "$API_COMPARISON/api-diff.html" >/dev/null 2>&1; then
	STATUS_MESSAGE=":white_check_mark: API Diff (from PR only) (no change)"
	echo "##vso[task.setvariable variable=API_GENERATOR_DIFF_STATUS_MESSAGE;isOutput=true]$STATUS_MESSAGE"
elif perl -0777 -pe 's/<script type="text\/javascript">.*?<.script>/script removed/gs' "$API_COMPARISON"/*.html | grep data-is-breaking; then
	STATUS_MESSAGE=":warning: API Diff (from PR only) (:fire: breaking changes :fire:)"
	echo "##vso[task.setvariable variable=API_GENERATOR_DIFF_STATUS_MESSAGE;isOutput=true]$STATUS_MESSAGE"
else
	STATUS_MESSAGE=":information_source: API Diff (from PR only) (please review changes)"
	echo "##vso[task.setvariable variable=API_GENERATOR_DIFF_STATUS_MESSAGE;isOutput=true]$STATUS_MESSAGE"
fi

if ! test -s $API_COMPARISON/generator-diff/generator.diff; then
	printf ":white_check_mark: [Generator Diff](%s) (no change)" "$GENERATOR_URL" >> "$WORKSPACE/api-diff-comments.md"
elif grep "^[+-][^+-]" $API_COMPARISON/generator-diff/generator.diff | grep -v "^.[[]assembly: AssemblyInformationalVersion" | grep -v "^[+-][[:space:]]*internal const string Revision =" >/dev/null 2>&1; then
	printf ":information_source: [Generator Diff](%s) (please review changes)" "$GENERATOR_URL" >> "$WORKSPACE/api-diff-comments.md"
else
	printf ":white_check_mark: [Generator Diff](%s) (only version changes)" "$GENERATOR_URL" >> "$WORKSPACE/api-diff-comments.md"
fi
printf "\\n" >> "$WORKSPACE/api-diff-comments.md"

cp "$WORKSPACE/api-diff-comments.md" "$API_COMPARISON"
if test -f "$COMPARE_FAILURE_FILE"; then
	cp "$COMPARE_FAILURE_FILE" "$API_COMPARISON"
fi
MESSAGE="*** Comparing API & creating generator diff completed ***"
echo "##vso[task.setvariable variable=API_GENERATOR_DIFF_MESSAGE;isOutput=true]$MESSAGE"
echo "##vso[task.setvariable variable=API_GENERATOR_BUILT;isOutput=true]True"
