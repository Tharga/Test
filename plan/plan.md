# Plan: Icon, docs site, and integrated docs CI

- [x] 1. Create feature branch `feature/icon-and-docs` from `master`
- [x] 2. Create `plan/feature.md` and `plan/plan.md`
- [x] 3. Update `<PackageIconUrl>` in `Tharga.Test.Toolkit.csproj` → `https://thargelion.net/assets/component-test.png`
- [x] 4. Scaffold `docs/` (docfx.json, CNAME=test.tharga.net, index.md, toc.yml, articles/, templates/thg/public/main.css)
- [x] 5. `docs/api/` is generated, not committed — gitignored to match the actual Mcp pattern (initial plan was wrong; corrected after checking `git ls-files`)
- [x] 6. Update `README.md` with docs link
- [x] 7. Update `.gitignore` to exclude `docs/_site/`
- [x] 8. Add `pages: write` + `id-token: write` to `.github/workflows/build.yml` permissions
- [x] 9. Append `docs` (needs: release) and `docs-deploy` (needs: docs) jobs to `build.yml`
- [x] 10. `dotnet build -c Release` — 0 warnings, 0 errors
- [x] 11. `docfx docs/docfx.json` — 0 warnings, 0 errors (14 models built)
- [~] 12. Commit changes
- [ ] 13. Push branch and open PR to `master`
