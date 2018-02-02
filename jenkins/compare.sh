#!/bin/bash -e

cd $WORKSPACE
./tools/compare-commits.sh --base=origin/pr/$ghprbPullId/merge^1

cp -R tools/comparison/apidiff jenkins-results/apicomparison
cp -R tools/comparison/generator-diff jenkins-results/generator-diff
