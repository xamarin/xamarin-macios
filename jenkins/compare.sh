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
#    c. Execute 'ghprbPullId=4614 ./compare.sh'
#

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

MARKDOWN_INDENT="&nbsp;&nbsp;&nbsp;&nbsp;"
echo "*** Comparing API & creating generator diff... ***"
export COMPARE_FAILURE_FILE=$TMPDIR/api-diff-compare-failures.txt
report_error ()
{
	printf "🔥 [Failed to compare API and create generator diff](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
	if test -f "$COMPARE_FAILURE_FILE"; then
		sed "s/^/${MARKDOWN_INDENT//&/\\&}/" "$COMPARE_FAILURE_FILE" >> "$WORKSPACE/jenkins/pr-comments.md"
	fi
	printf "${MARKDOWN_INDENT}Search for \`Comparing API & creating generator diff\` in the log to view the complete log.\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
	touch "$WORKSPACE/jenkins/failure-stamp"
	rm -f "$COMPARE_FAILURE_FILE"
	echo "*** Comparing API & creating generator diff failed ***"
	exit 0
}
trap report_error ERR

# SC2154: ghprbPullId is referenced but not assigned.
# shellcheck disable=SC2154
if test -n "$ghprbPullId"; then
	if ./jenkins/fetch-pr-labels.sh --check=skip-api-comparison; then
		printf "❎ Skipped API comparison because the PR has the label 'skip-api-comparison'\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
		exit 0
	fi
	if ./jenkins/fetch-pr-labels.sh --check=skip-public-jenkins; then
		echo "Skipping API comparison because the label 'skip-public-jenkins' was found."
		exit 0
	fi
fi

if test -z "$ghprbPullId"; then
	BASE=HEAD
else
	BASE="origin/pr/$ghprbPullId/merge"
fi

if ! git rev-parse "$BASE" >/dev/null 2>&1; then
	echo "Can't compare API and create generator diff because the pull request has conflicts that must be resolved first (the branch '$BASE' doesn't exist)."
	printf "🔥 [Failed to compare API and create generator diff because the pull request has conflicts that must be resolved first](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
	exit 0
fi

./tools/compare-commits.sh --base="$BASE^1" "--failure-file=$COMPARE_FAILURE_FILE"

mkdir -p jenkins-results/apicomparison

cp -R tools/comparison/apidiff/diff jenkins-results/apicomparison/
cp    tools/comparison/apidiff/*.html jenkins-results/apicomparison/
cp -R tools/comparison/generator-diff jenkins-results/generator-diff

if [[ "x$1" == "x--publish" ]]; then
	URL_PREFIX=$(./jenkins/publish-results.sh | grep "^Url Prefix: " | sed 's/^Url Prefix: //')
	URL_API="$URL_PREFIX/apicomparison/api-diff.html"
	URL_GENERATOR="$URL_PREFIX/generator-diff/index.html"
else
	URL_API="$BUILD_URL/API_20diff_20_28PR_20only_29"
	URL_GENERATOR="$BUILD_URL/Generator_20Diff"
fi

if ! grep "href=" jenkins-results/apicomparison/api-diff.html >/dev/null 2>&1; then
	printf "✅ [API Diff (from PR only)](%s) (no change)" "$URL_API" >> "$WORKSPACE/jenkins/pr-comments.md"
elif perl -0777 -pe 's/<script type="text\/javascript">.*?<.script>/script removed/gs' jenkins-results/apicomparison/*.html | grep data-is-breaking; then
	printf "⚠️ [API Diff (from PR only)](%s) (🔥 breaking changes 🔥)" "$URL_API" >> "$WORKSPACE/jenkins/pr-comments.md"
else
	printf "ℹ️ [API Diff (from PR only)](%s) (please review changes)" "$URL_API" >> "$WORKSPACE/jenkins/pr-comments.md"
fi
printf "\\n" >> "$WORKSPACE/jenkins/pr-comments.md"

if ! test -s jenkins-results/generator-diff/generator.diff; then
	printf "✅ [Generator Diff](%s) (no change)" "$URL_GENERATOR" >> "$WORKSPACE/jenkins/pr-comments.md"
elif grep "^[+-][^+-]" jenkins-results/generator-diff/generator.diff | grep -v "^.[[]assembly: AssemblyInformationalVersion" | grep -v "^[+-][[:space:]]*internal const string Revision =" >/dev/null 2>&1; then
	printf "ℹ️ [Generator Diff](%s) (please review changes)" "$URL_GENERATOR" >> "$WORKSPACE/jenkins/pr-comments.md"
else
	printf "✅ [Generator Diff](%s) (only version changes)" "$URL_GENERATOR" >> "$WORKSPACE/jenkins/pr-comments.md"
fi
printf "\\n" >> "$WORKSPACE/jenkins/pr-comments.md"

echo "*** Comparing API & creating generator diff completed ***"
