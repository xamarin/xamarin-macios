#!/bin/bash

# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

set -euo pipefail

repository=${REPOSITORY:-dotnet/macios}
remote=${REMOTE:-origin}
net10_branch=${NET10_BRANCH:-release/10.0.1xx}
net11_branch=${NET11_BRANCH:-release/11.0.1xx}

red=""
green=""
yellow=""
cyan=""
reset=""
if [[ -t 1 && -z ${NO_COLOR:-} && ${TERM:-} != "dumb" ]]; then
	red=$'\033[31m'
	green=$'\033[32m'
	yellow=$'\033[33m'
	cyan=$'\033[36m'
	reset=$'\033[0m'
fi

usage ()
{
	cat <<EOF
Usage: $(basename "$0")

Verify that every merged pull request with a backport label has a pull request
targeting the corresponding release branch, or a matching backport commit in
that branch's history. Open, closed, and merged backport pull requests all
count.

Environment variables:
  REPOSITORY    GitHub repository (default: $repository)
  REMOTE        Git remote to fetch release branches from (default: $remote)
  NET10_BRANCH  .NET 10 target branch (default: $net10_branch)
  NET11_BRANCH  .NET 11 target branch (default: $net11_branch)
  NO_COLOR      Disable colored output when set
EOF
}

if [[ $# -eq 1 && (${1:-} == "--help" || ${1:-} == "-h") ]]; then
	usage
	exit 0
fi

if [[ $# -ne 0 ]]; then
	usage >&2
	exit 2
fi

for command in gh git jq; do
	if ! command -v "$command" > /dev/null; then
		echo "error: '$command' is required." >&2
		exit 2
	fi
done

temporary_directory=$(mktemp -d)
trap 'rm -rf "$temporary_directory"' EXIT

missing=0

check_backports ()
{
	local label=$1
	local target_branch=$2
	local target_ref
	local source_issues_file="$temporary_directory/source-issues.json"
	local source_details_file="$temporary_directory/source-details.json"
	local sources_file="$temporary_directory/sources.json"
	local backports_file="$temporary_directory/backports.json"
	local history_file="$temporary_directory/history.json"
	local results_file="$temporary_directory/results.json"

	git fetch "$remote" "$target_branch" --quiet
	target_ref=FETCH_HEAD

	gh api \
		--paginate \
		--slurp \
		--method GET \
		"repos/$repository/issues" \
		-f state=closed \
		-f labels="$label" \
		-f per_page=100 > "$source_issues_file"

	: > "$source_details_file"
	while IFS= read -r pull_request_url; do
		gh api "$pull_request_url" >> "$source_details_file"
	done < <(
		jq -r 'add[] | select(.pull_request != null) | .pull_request.url' "$source_issues_file"
	)

	jq -s '
		map(
			select(.merged_at != null) |
			{
				number,
				title,
				url: .html_url,
				baseRefName: .base.ref
			}
		)
	' "$source_details_file" > "$sources_file"

	gh api \
		--paginate \
		--slurp \
		--method GET \
		"repos/$repository/pulls" \
		-f state=all \
		-f base="$target_branch" \
		-f per_page=100 |
		jq 'add | map({
			number,
			title,
			url: .html_url,
			state: (if .merged_at == null then .state | ascii_upcase else "MERGED" end),
			body
		})' > "$backports_file"

	git log --format='%H%x1f%B%x1e' "$target_ref" |
		jq -Rs '
			split("\u001e") |
			map(
				ltrimstr("\n") |
				select(length > 0) |
				split("\u001f") |
				{
					commit: .[0],
					message: (.[1:] | join("\u001f"))
				}
			)
		' > "$history_file"

	jq \
		--arg branch "$target_branch" \
		--slurpfile backports "$backports_file" \
		--slurpfile history "$history_file" \
		'
			def references_source($source):
				(.body // "") | test(
					"(?i)backport of (?:https://github\\.com/[^/\\s]+/[^/\\s]+/pull/)?#?"
					+ ($source.number | tostring)
					+ "(?:[^0-9]|$)"
				);

			def commit_references_source($source):
				(.message // "") | test(
					"(?i)backport of (?:https://github\\.com/[^/\\s]+/[^/\\s]+/pull/)?#?"
					+ ($source.number | tostring)
					+ "(?:[^0-9]|$)"
				);

			map(
				. as $source
				| if .baseRefName == $branch then
					. + { backport: ., result: "already-targets-branch" }
				else
					([ $backports [0][] | select(references_source($source)) ] | first) as $backport
					| ([ $history [0][] | select(commit_references_source($source)) ] | first) as $commit
					| . + {
						backport: $backport,
						commit: $commit,
						result: (
							if $backport != null then "found"
							elif $commit != null then "found-in-history"
							else "missing"
							end
						)
					}
				end
			)
		' "$sources_file" > "$results_file"

	printf '%s%s -> %s%s\n' "$cyan" "$label" "$target_branch" "$reset"
	if [[ $(jq length "$results_file") -eq 0 ]]; then
		printf '  %sNo merged pull requests have this label.%s\n' "$yellow" "$reset"
		return
	fi

	while IFS=$'\t' read -r result source_number source_url backport_number backport_state history_commit; do
		case "$result" in
		already-targets-branch)
			printf '  %s[OK]%s      #%s already targets %s\n' "$green" "$reset" "$source_number" "$target_branch"
			;;
		found)
			printf '  %s[OK]%s      #%s -> #%s (%s)\n' "$green" "$reset" "$source_number" "$backport_number" "$backport_state"
			;;
		found-in-history)
			printf '  %s[OK]%s      #%s -> commit %.12s (in %s history)\n' "$green" "$reset" "$source_number" "$history_commit" "$target_branch"
			;;
		missing)
			printf '  %s[MISSING]%s #%s has no backport PR or commit: %s\n' "$red" "$reset" "$source_number" "$source_url"
			missing=1
			;;
		esac
	done < <(
		jq -r '
			.[] | [
				.result,
				.number,
				.url,
				(.backport.number // "-"),
				(.backport.state // "-"),
				(.commit.commit // "-")
			] | @tsv
		' "$results_file"
	)
	echo
}

check_backports "backport-to-net10.0" "$net10_branch"
check_backports "backport-to-net11.0" "$net11_branch"

if [[ $missing -ne 0 ]]; then
	printf '%sOne or more backports are missing.%s\n' "$red" "$reset" >&2
	exit 1
fi

printf '%sAll labeled pull requests have a backport.%s\n' "$green" "$reset"
