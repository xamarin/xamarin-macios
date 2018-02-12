#!/bin/bash -e

cd $WORKSPACE
./tools/compare-commits.sh --base=origin/pr/$ghprbPullId/merge^1

mkdir -p jenkins-results/apicomparison

cp -R tools/comparison/apidiff/diff jenkins-results/apicomparison/
cp    tools/comparison/apidiff/*.html jenkins-results/apicomparison/
cp -R tools/comparison/generator-diff jenkins-results/generator-diff
