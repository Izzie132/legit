# Renovate

[Renovate](https://docs.renovatebot.com/) is a tool that automates dependency updates. It can be used to keep dependencies up-to-date in a project by automatically creating pull requests to update dependencies when new versions are released.

## Configuration

The `renovate.json` file at the root of the repository contains the base configuration for Renovate.

Relevant configuration files also live in the `/Tools/Renovate` directory (which is attached to this solution so should appear towards the bottom of the Solution Explorer).

Note that the default configuration assumes you are hosting and building your code using Azure DevOps. You may need to adjust the configuration to suit your own setup.

## Setup

The typical approach is to set up a separate pipeline to run Renovate on a schedule (every day at 3AM by default). When this runs, it will automatically raise PRs to update dependencies. To set up the pipeline, create a new pipeline in Azure DevOps, based on the `Tools/Renovate/renovate.yaml` pipeline definition file.

To enable GitHub integration (see below), create a secret variable called `gitHubPersonalAccessToken` on the newly created pipeline in Azure DevOps.

## GitHub integration

Renovate can integrate with GitHub to automatically provide patch notes for each dependency update. To enable this, a GitHub Personal Access Token is required. To avoid needing to issue and store PATs linked to an individual's GitHub account, the `GhystonRenovate` GitHub user has been created, and a suitable PAT for this user is stored in 1Password, in the relevant secret within the `Technical` vault.

If you need to set up a new PAT, follow the instructions from [this page](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-fine-grained-personal-access-token).
