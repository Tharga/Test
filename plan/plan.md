# Plan: GitHub Actions CI/CD migration

- [x] 1. Create feature branch `feature/ci-cd-github-actions` from `master`
- [x] 2. Create `plan/feature.md` and `plan/plan.md`
- [x] 3. Add `.github/workflows/build.yml` (Crawler-derived; MAJOR_MINOR=1.14, packs Tharga.Test.Toolkit)
- [x] 4. Delete `azure-pipelines.yml` and `buildnumber.yml`
- [x] 5. Update `Tharga.Test.sln` Solution Items section
- [x] 6. Verify local build + test still pass — `dotnet build -c Release` clean (0 warnings), `dotnet test -c Release` exits 0 with no test projects
- [~] 7. Commit changes
- [ ] 8. Push branch and open PR to `master`
- [ ] 9. Update Requests.md status to Done after PR is merged (separate session, post-merge)

## Last session
Migrated Tharga.Test from Azure DevOps to GitHub Actions:
- Workflow mirrors `Tharga.Crawler` reference, customized to MAJOR_MINOR `1.14` and the single pack target `Tharga.Test.Toolkit/Tharga.Test.Toolkit.csproj`. No `--filter` since there are no test projects in the repo.
- Removed obsolete `azure-pipelines.yml` + `buildnumber.yml` and cleaned them out of the `Solution Items` section in `Tharga.Test.sln`.
- Local build clean, `dotnet test` no-op succeeds.

Manual follow-ups required in GitHub / AzDo UI before the workflow can publish:
- Configure `NUGET_API_KEY` secret
- Configure `release` and `prerelease` environments
- Disable the old Azure DevOps pipeline
- Delete the `develop` branch after PR merge

README.md does not need changes — the workflow is infrastructure, not consumer-facing API.
