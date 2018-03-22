#!/bin/bash -e

report_error ()
{
	printf "🔥 [Failed to compare API and create generator diff]($BUILD_URL/console) 🔥\\n" >> $WORKSPACE/jenkins/pr-comments.md
}
trap report_error ERR

cd $WORKSPACE
./tools/compare-commits.sh --base=origin/pr/$ghprbPullId/merge^1

mkdir -p jenkins-results/apicomparison

cp -R tools/comparison/apidiff/diff jenkins-results/apicomparison/
cp    tools/comparison/apidiff/*.html jenkins-results/apicomparison/
cp -R tools/comparison/generator-diff jenkins-results/generator-diff

printf "✅ [API Diff (from PR only)]($BUILD_URL/API_diff_(PR_only))\\n" >> $WORKSPACE/jenkins/pr-comments.md
printf "✅ [Generator Diff]($BUILD_URL/Generator_Diff)\\n" >> $WORKSPACE/jenkins/pr-comments.md
