#!/bin/bash -e

report_error ()
{
	echo "🔥 [Provisioning failed]($BUILD_URL/console) 🔥" >> $WORKSPACE/jenkins/pr-comments.md
}
trap report_error ERR

cd $WORKSPACE
./system-dependencies.sh --provision-all

echo "✅ [Provisioning succeeded]($BUILD_URL/console)" >> $WORKSPACE/jenkins/pr-comments.md
