# Infra Lessons

RULE: Never overwrite files in AgentSkills/ without verifying they are backed up; it is the single source of truth and tool-specific entry points must point to it, never duplicate it. #circular-redirect 2026-05-31
RULE: Use repo-relative paths in all agent and IDE instruction files; never write absolute local drive paths in shared documentation. #wrong-path 2026-06-06
RULE: When splitting a monolithic memory file into domain files, grep the whole repo for the old path and update every reference before deleting the original. #stale-reference 2026-06-07
