#!/bin/bash -e

cd "$(dirname "${BASH_SOURCE[0]}")/.."
WORKSPACE=$(pwd)

report_error ()
{
	printf "🔥 [Failed to compare API and create generator diff](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
	touch "$WORKSPACE/jenkins/failure-stamp"
	exit 0
}
trap report_error ERR


if ./jenkins/fetch-pr-labels.sh --check=skip-api-comparison; then
	printf "❎ Skipped API comparison because the PR has the label 'skip-api-comparison'\\n" >> "$WORKSPACE/jenkins/pr-comments.md"
	exit 0
fi

# SC2154: ghprbPullId is referenced but not assigned.
# shellcheck disable=SC2154
BASE="origin/pr/$ghprbPullId/merge"
if ! git rev-parse "$BASE" >/dev/null 2>&1; then
	echo "Can't compare API and create generator diff because the pull request has conflicts that must be resolved first (the branch '$BASE' doesn't exist)."
	printf "🔥 [Failed to compare API and create generator diff because the pull request has conflicts that must be resolved first](%s/console) 🔥\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
	exit 0
fi

./tools/compare-commits.sh --base="$BASE^1"

mkdir -p jenkins-results/apicomparison

cp -R tools/comparison/apidiff/diff jenkins-results/apicomparison/
cp    tools/comparison/apidiff/*.html jenkins-results/apicomparison/
cp -R tools/comparison/generator-diff jenkins-results/generator-diff

printf "✅ [API Diff (from PR only)](%s/API_20diff_20_28PR_20only_29)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
printf "✅ [Generator Diff](%s/Generator_20Diff)\\n" "$BUILD_URL" >> "$WORKSPACE/jenkins/pr-comments.md"
