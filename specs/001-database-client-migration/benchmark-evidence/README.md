# T172 Benchmark Evidence

This folder keeps the minimum tracked benchmark evidence needed for review of `T172`.

Contents:
- `sqlite-efcore-report.md`
- `sqlite-nhibernate-report.md`
- `mariadb-efcore-report.md`
- `mariadb-nhibernate-report.md`
- `sqlserver-efcore-report.md`
- `sqlserver-nhibernate-report.md`

Notes:
- These are copied from generated `BenchmarkDotNet` Markdown summaries.
- Raw `BenchmarkDotNet.Artifacts/` output is local/generated build output and should remain untracked.
- Accepted release interpretation is recorded in:
  - `specs/001-database-client-migration/tasks.md`
  - `specs/001-database-client-migration/implementation-state.md`
- `T172` is accepted release evidence.
- `T173` remains deferred manual validation and is not a release blocker for EF startup migration correctness.
