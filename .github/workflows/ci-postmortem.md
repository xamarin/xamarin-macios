---
on:
  workflow_dispatch:
  permissions: {}

# ###############################################################
# Select a PAT from the pool and override COPILOT_GITHUB_TOKEN.
# Run agentic jobs in the existing `gh-aw-environment` environment.
#
# When org-level billing is available, this will be removed.
# See `shared/pat_pool.README.md` for more information.
# ###############################################################
imports:
  - uses: shared/pat_pool.md
    with:
      environment: gh-aw-environment

permissions:
  contents: read
  issues: read
environment: gh-aw-environment
engine:
  id: copilot
  model: claude-sonnet-4.5
  env:
    COPILOT_GITHUB_TOKEN: |
      ${{ case(
        needs.pat_pool.outputs.pat_number == '0', secrets.COPILOT_PAT_0,
        needs.pat_pool.outputs.pat_number == '1', secrets.COPILOT_PAT_1,
        needs.pat_pool.outputs.pat_number == '2', secrets.COPILOT_PAT_2,
        needs.pat_pool.outputs.pat_number == '3', secrets.COPILOT_PAT_3,
        needs.pat_pool.outputs.pat_number == '4', secrets.COPILOT_PAT_4,
        needs.pat_pool.outputs.pat_number == '5', secrets.COPILOT_PAT_5,
        needs.pat_pool.outputs.pat_number == '6', secrets.COPILOT_PAT_6,
        needs.pat_pool.outputs.pat_number == '7', secrets.COPILOT_PAT_7,
        needs.pat_pool.outputs.pat_number == '8', secrets.COPILOT_PAT_8,
        needs.pat_pool.outputs.pat_number == '9', secrets.COPILOT_PAT_9,
        'NO COPILOT PAT AVAILABLE')
      }}
network:
  allowed:
    - defaults
    - dotnet
    - github
    - "aka.ms"
    - "dev.azure.com"
    - "devdiv.visualstudio.com"
    - "microsoft.com"
    - "vsassets.io"
tools:
  bash: ["cat", "ls", "find", "grep", "head", "tail", "wc", "az", "gh", "python3", "mkdir", "zip"]
  github:
    github-token: ${{ secrets.GITHUB_TOKEN }}
    toolsets: [issues, repos]
    min-integrity: none
safe-outputs:
  github-token: ${{ secrets.GITHUB_TOKEN }}
  create-issue:
    max: 20
  add-comment:
    max: 20
  update-issue:
    max: 20
---

# CI Post-Mortem Analysis

Perform a weekly post-mortem analysis of CI failures across recent PRs in dotnet/macios to identify flaky tests, infrastructure issues, and shared regressions that are not caused by any specific PR.

## Instructions

1. Read the skill definition from `.agents/skills/macios-ci-postmortem/SKILL.md` — this contains the full 4-phase workflow.
2. Read the Azure DevOps CLI reference from `.agents/skills/macios-ci-postmortem/references/azure-devops-cli.md`.
3. Execute all four phases of the workflow:
   - **Phase 1: Discovery** — collect all PR-validation builds from the last 7 days
   - **Phase 2: Extraction** — download TestSummary artifacts for triage, then HtmlReport artifacts only for jobs with test failures, and parse NUnit XML for individual test-level failures
   - **Phase 3: Classification** — categorize failures as flaky (cross-PR or rerun-recovered), infrastructure (bot-specific or cross-bot), or PR-specific (exclude these). Also exclude `AppSizeTest` failures.
   - **Phase 4: Issue Actions** — search for existing `ci-postmortem` issues, then file new issues or comment on existing ones
4. All issues must have the `ci-postmortem` and `copilot` labels.
5. File one issue per distinct test failure — do not group unrelated test failures together.
6. For infrastructure issues, check if failures are concentrated on specific bots by extracting `workerName` from build timelines.

## Constraints

- Only file issues for failures that appear across 2+ unrelated PRs, or that are confirmed flaky by rerun recovery (same commit, different outcome).
- Never file issues for PR-specific failures — those are the PR author's responsibility.
- Always search for existing `ci-postmortem` issues before creating new ones. Comment on existing issues if the failure is already tracked.
- Always exclude `AppSizeTest` failures — they are expected to fail across PRs.
