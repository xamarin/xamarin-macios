#!/bin/bash -e

report_error ()
{
	printf "🔥 [Failed to compare API and create generator diff]($BUILD_URL/console) 🔥\\n" >> $WORKSPACE/jenkins/pr-comments.md
	touch $WORKSPACE/jenkins/failure-stamp
	exit 0
}
trap report_error ERR

cd $WORKSPACE

if ./jenkins/fetch-pr-labels.sh --check=skip-api-comparison; then
	printf "❎ Skipped API comparison because the PR has the label 'skip-api-comparison'\\n" >> $WORKSPACE/jenkins/pr-comments.md
	exit 0
fi

BASE=origin/pr/$ghprbPullId/merge
if ! git rev-parse $BASE >/dev/null 2>&1; then
	echo "Can't compare API and create generator diff because the pull request has conflicts that must be resolved first (the branch '$BASE' doesn't exist)."
	printf "🔥 [Failed to compare API and create generator diff because the pull request has conflicts that must be resolved first]($BUILD_URL/console) 🔥\\n" >> $WORKSPACE/jenkins/pr-comments.md
	exit 0
fi

./tools/compare-commits.sh --base=$BASE^1

mkdir -p jenkins-results/apicomparison

cp -R tools/comparison/apidiff/diff jenkins-results/apicomparison/
cp    tools/comparison/apidiff/*.html jenkins-results/apicomparison/
cp -R tools/comparison/generator-diff jenkins-results/generator-diff

printf "✅ [API Diff (from PR only)]($BUILD_URL/API_20diff_20_28PR_20only_29)" >> $WORKSPACE/jenkins/pr-comments.md
if ! grep "href=" jenkins-results/apicomparison/api-diff.html >/dev/null 2>&1; then
	printf " (no change)" >> "$WORKSPACE/jenkins/pr-comments.md"
elif grep "</script>" jenkins-results/apicomparison/*.html | grep data-is-breaking; then
	printf " (🔥 breaking changes 🔥)" >> "$WORKSPACE/jenkins/pr-comments.md"
else
	printf " (please review changes)" >> "$WORKSPACE/jenkins/pr-comments.md"
fi
printf "\\n" >> "$WORKSPACE/jenkins/pr-comments.md"

printf "✅ [Generator Diff]($BUILD_URL/Generator_20Diff)" >> $WORKSPACE/jenkins/pr-comments.md
if grep "^[+-][^+-]" jenkins-results/generator-diff/generator.diff | grep -v "^.[[]assembly: AssemblyInformationalVersion" | grep -v "^[+-][[:space:]]*internal const string Revision =" >/dev/null 2>&1; then
	printf " (please review changes)" >> "$WORKSPACE/jenkins/pr-comments.md"
else
	printf " (only version changes)" >> "$WORKSPACE/jenkins/pr-comments.md"
fi
printf "\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
