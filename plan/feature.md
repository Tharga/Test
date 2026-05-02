# Feature: Migrate Tharga.Test to GitHub Actions CI/CD

## Source
Request: `Tharga.Test` under "GitHub Actions CI/CD" in `$DOC_ROOT/Tharga/Requests.md` (Internal, 2026-04-06, Priority Medium, Status Pending).

## Goal
Replace the existing Azure DevOps pipeline with a GitHub Actions workflow that builds, tests, scans (CodeQL), packs, and publishes `Tharga.Test.Toolkit` to NuGet.org and creates GitHub Releases — matching the canonical Tharga.Crawler reference implementation.

## Scope
- Add `.github/workflows/build.yml` mirroring `c:\dev\tharga\Toolkit\Crawler\.github\workflows\build.yml`
- Customize per project:
  - `MAJOR_MINOR: '1.14'` (next stable: `1.14.8`)
  - Pack only `Tharga.Test.Toolkit/Tharga.Test.Toolkit.csproj`
  - .NET SDKs `8.0.x`, `9.0.x`, `10.0.x`
  - Default warning threshold (15)
  - No test filter (no test projects exist in this repo)
- Remove obsolete `azure-pipelines.yml` and `buildnumber.yml`
- Update `Tharga.Test.sln` to drop those entries from `Solution Items`

## Out of scope (manual follow-ups in GitHub / Azure DevOps UI)
- Configure `NUGET_API_KEY` secret
- Configure `release` and `prerelease` environments
- Disable the old Azure DevOps build pipeline
- Delete the `develop` branch after PR merge

## Acceptance criteria
- `feature/ci-cd-github-actions` branch contains the workflow, plan files, and removed AzDo files
- `dotnet build -c Release` and `dotnet test -c Release` still pass locally
- PR opened against `master`
- Workflow runs successfully on the PR (build + security + prerelease jobs)
- After merge: a stable release `1.14.8` is published to NuGet via the workflow

## Done condition
User has confirmed the workflow runs green on a PR and validates the migration.
